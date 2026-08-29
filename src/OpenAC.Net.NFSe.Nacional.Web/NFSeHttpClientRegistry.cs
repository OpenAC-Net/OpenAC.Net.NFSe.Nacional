using System.Collections.Concurrent;
using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using OpenAC.Net.NFSe.Nacional.Common;
using System.Runtime.CompilerServices;

namespace OpenAC.Net.NFSe.Nacional.Web;

internal sealed record NFSeHttpClientRegistration(X509Certificate2 Certificate, SslProtocols Protocols,
    int Tentativas, TimeSpan IntervaloTentativas);

internal sealed class NFSeHttpClientRegistry : INFSeTenantContextAccessor
{
    internal const string Prefix = "OpenAC.Net.NFSe.Nacional:";
    private readonly ConcurrentDictionary<string, NFSeHttpClientRegistration> registrations = new();
    private readonly ConditionalWeakTable<ConfiguracaoNFSe, TenantIdentity> tenants = new();

    public void Bind(ConfiguracaoNFSe configuration, string tenantId) =>
        tenants.AddOrUpdate(configuration, new TenantIdentity(tenantId));

    public string GetTenant(ConfiguracaoNFSe configuration) =>
        tenants.TryGetValue(configuration, out var tenant) ? tenant.Value : NFSeTenant.Padrao;

    public string Register(ConfiguracaoNFSe configuration, X509Certificate2 certificate,
        SslProtocols protocols, Uri endpoint)
    {
        var identity = certificate.Thumbprint;
        if (string.IsNullOrWhiteSpace(identity))
            identity = Convert.ToHexString(SHA256.HashData(certificate.RawData));
        var authority = Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(endpoint.Authority)));
        var tenant = GetTenant(configuration);
        var tenantHash = Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(tenant)));
        var tentativas = Math.Max(1, configuration.WebServices.Tentativas);
        var intervaloTentativas = TimeSpan.FromMilliseconds(
            Math.Max(0, configuration.WebServices.IntervaloTentativas));
        var name = $"{Prefix}{tenantHash}:{identity}:{(int)protocols}:{authority}:{tentativas}:{intervaloTentativas.TotalMilliseconds}";
        registrations.TryAdd(name, new NFSeHttpClientRegistration(certificate, protocols, tentativas,
            intervaloTentativas));
        return name;
    }

    public bool TryGet(string name, out NFSeHttpClientRegistration registration) =>
        registrations.TryGetValue(name, out registration!);
}

internal sealed record TenantIdentity(string Value);

internal sealed class NFSeHttpClientOptions : IConfigureNamedOptions<HttpClientFactoryOptions>
{
    private readonly NFSeHttpClientRegistry registry;
    private readonly OpenNFSeNacionalWebOptions options;

    public NFSeHttpClientOptions(NFSeHttpClientRegistry registry, IOptions<OpenNFSeNacionalWebOptions> options)
    {
        this.registry = registry;
        this.options = options.Value;
    }

    public void Configure(HttpClientFactoryOptions value) { }

    public void Configure(string? name, HttpClientFactoryOptions value)
    {
        if (name is null || !registry.TryGet(name, out var registration)) return;
        value.HandlerLifetime = options.TempoVidaHandler;
        value.HttpClientActions.Add(client => client.Timeout = Timeout.InfiniteTimeSpan);
        value.HttpMessageHandlerBuilderActions.Add(builder =>
        {
            builder.PrimaryHandler = new SocketsHttpHandler
            {
                AutomaticDecompression = DecompressionMethods.All,
                PooledConnectionLifetime = options.TempoVidaConexaoPool,
                SslOptions = new SslClientAuthenticationOptions
                {
                    EnabledSslProtocols = registration.Protocols,
                    ClientCertificates = new X509CertificateCollection { registration.Certificate }
                }
            };
            var retry = new HttpRetryStrategyOptions
            {
                MaxRetryAttempts = Math.Max(0, registration.Tentativas - 1),
                Delay = registration.IntervaloTentativas,
                BackoffType = DelayBackoffType.Constant,
                UseJitter = false
            };
            retry.DisableForUnsafeHttpMethods();
            var pipelineBuilder = new ResiliencePipelineBuilder<HttpResponseMessage>()
                .AddRetry(retry)
                .AddTimeout(options.TimeoutTentativa);
            if (options.CircuitBreakerHabilitado)
                pipelineBuilder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions());
            builder.AdditionalHandlers.Add(new ResilienceHandler(pipelineBuilder.Build()));
        });
    }
}
