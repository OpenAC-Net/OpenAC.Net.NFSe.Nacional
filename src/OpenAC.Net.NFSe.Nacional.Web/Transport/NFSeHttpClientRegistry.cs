using System.Collections.Concurrent;
using System.Net;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.DFe.Core.Http;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Web.Accessor;
using OpenAC.Net.NFSe.Nacional.Web.Common;
using OpenAC.Net.NFSe.Nacional.Web.Configuration;
using Polly;

namespace OpenAC.Net.NFSe.Nacional.Web.Transport;

internal sealed record NFSeHttpClientRegistration(X509Certificate2? Certificate, SslProtocols Protocols,
    string Proxy, bool ValidateServerCertificate, bool UseCookies, int Tentativas, TimeSpan IntervaloTentativas);

internal sealed class NFSeHttpClientRegistry : INFSeTenantContextAccessor, IDisposable
{
    internal const string Prefix = "OpenAC.Net.NFSe.Nacional:";
    private readonly ConcurrentDictionary<string, NFSeHttpClientRegistration> registrations = new();
    private readonly ConditionalWeakTable<ConfiguracaoNFSe, TenantIdentity> tenants = new();
    private readonly ConditionalWeakTable<DFeArquivosConfigBase, TenantIdentity> files = new();

    public void Bind(ConfiguracaoNFSe configuration, string tenantId)
    {
        var identity = new TenantIdentity(tenantId);
        tenants.AddOrUpdate(configuration, identity);
        files.AddOrUpdate(configuration.Arquivos, identity);
    }

    public string GetTenant(ConfiguracaoNFSe configuration) =>
        tenants.TryGetValue(configuration, out var tenant) ? tenant.Value : NFSeTenant.Padrao;

    public string GetTenant(DFeArquivosConfigBase configuration) =>
        files.TryGetValue(configuration, out var tenant) ? tenant.Value : NFSeTenant.Padrao;

    public string Register(string tenant, DFeHttpTransportOptions options, Uri endpoint)
    {
        var identity = $"{tenant}|{options.Certificate?.Thumbprint}|{(int)options.Protocols}|{endpoint.GetLeftPart(UriPartial.Authority)}|{options.Proxy}|{options.ValidateServerCertificate}|{options.UseCookies}|{options.Attempts}|{options.RetryDelay.Ticks}";
        var name = Prefix + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(identity)));
        if (!registrations.ContainsKey(name))
        {
            var owned = options.Certificate == null ? null : new X509Certificate2(options.Certificate);
            if (!registrations.TryAdd(name, new(owned, options.Protocols, options.Proxy,
                    options.ValidateServerCertificate, options.UseCookies, options.Attempts, options.RetryDelay)))
                owned?.Dispose();
        }
        return name;
    }

    public bool TryGet(string name, out NFSeHttpClientRegistration registration) =>
        registrations.TryGetValue(name, out registration!);

    public void Dispose()
    {
        foreach (var entry in registrations.Values) entry.Certificate?.Dispose();
        registrations.Clear();
    }

    private sealed record TenantIdentity(string Value);
}

internal sealed class NFSeHttpClientOptions(NFSeHttpClientRegistry registry, IOptions<OpenNFSeNacionalWebOptions> options)
    : IConfigureNamedOptions<HttpClientFactoryOptions>
{
    public void Configure(HttpClientFactoryOptions value) { }

    public void Configure(string? name, HttpClientFactoryOptions value)
    {
        if (name is null || !registry.TryGet(name, out var registration)) return;
        value.HandlerLifetime = options.Value.TempoVidaHandler;
        value.HttpClientActions.Add(client => client.Timeout = Timeout.InfiniteTimeSpan);
        value.HttpMessageHandlerBuilderActions.Add(builder =>
        {
            var certificate = registration.Certificate == null ? null : new X509Certificate2(registration.Certificate);
            var ssl = new SslClientAuthenticationOptions { EnabledSslProtocols = registration.Protocols };
            if (certificate != null) ssl.ClientCertificates = new X509CertificateCollection { certificate };
            if (!registration.ValidateServerCertificate) ssl.RemoteCertificateValidationCallback = (_, _, _, _) => true;
            builder.PrimaryHandler = new CertificateHandler(certificate, new SocketsHttpHandler
            {
                UseCookies = registration.UseCookies,
                AutomaticDecompression = DecompressionMethods.All,
                PooledConnectionLifetime = options.Value.TempoVidaConexaoPool,
                Proxy = string.IsNullOrWhiteSpace(registration.Proxy) ? null : new WebProxy(registration.Proxy, true),
                SslOptions = ssl
            });
            var pipeline = new ResiliencePipelineBuilder<HttpResponseMessage>();
            if (registration.Tentativas > 1)
            {
                var retry = new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = registration.Tentativas - 1,
                    Delay = registration.IntervaloTentativas,
                    BackoffType = DelayBackoffType.Constant,
                    UseJitter = false,
                    OnRetry = args => { args.Outcome.Result?.Dispose(); return default; }
                };
                retry.DisableForUnsafeHttpMethods();
                pipeline.AddRetry(retry);
            }
            pipeline.AddTimeout(options.Value.TimeoutTentativa);
            if (options.Value.CircuitBreakerHabilitado)
                pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions());
            builder.AdditionalHandlers.Add(new ResilienceHandler(pipeline.Build()));
        });
    }

    private sealed class CertificateHandler(X509Certificate2? certificate, HttpMessageHandler inner) : DelegatingHandler(inner)
    {
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing) certificate?.Dispose();
        }
    }
}
