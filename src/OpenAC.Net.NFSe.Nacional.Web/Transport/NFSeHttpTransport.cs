using Microsoft.Extensions.Logging;
using OpenAC.Net.DFe.Core.Http;
using OpenAC.Net.NFSe.Nacional.Web.Common;

namespace OpenAC.Net.NFSe.Nacional.Web.Transport;

/// <summary>Adapta o contrato HTTP do DFe.Core ao IHttpClientFactory do host.</summary>
internal sealed class NFSeHttpTransport(IHttpClientFactory httpClientFactory, NFSeHttpClientRegistry registry,
    ILogger<NFSeHttpTransport> logger) : DFeHttpTransportBase
{
    internal static readonly HttpRequestOptionsKey<string> TenantKey = new("OpenAC.Net.NFSe.Nacional.Tenant");

    protected override bool ManageRetries => false;

    protected override async Task<HttpResponseMessage> SendCoreAsync(HttpRequestMessage request, DFeHttpTransportOptions options,
        CancellationToken cancellationToken = default)
    {
        var endpoint = request.RequestUri ?? throw new InvalidOperationException("A requisição não possui endpoint.");
        logger.LogDebug("Enviando {Method} ao host {Host} pelo IHttpClientFactory", request.Method, endpoint.Authority);
        var tenant = request.Options.TryGetValue(TenantKey, out var identity) ? identity : NFSeTenant.Padrao;
        var name = registry.Register(tenant, options, endpoint);
        using var client = httpClientFactory.CreateClient(name);
        // A base do Core conclui a leitura do corpo dentro do timeout da operação.
        return await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);
    }
}

// A empresa acompanha a requisição, sem depender do certificado nem de um contexto global mutável.
internal sealed class NFSeTenantHttpTransport(IDFeHttpTransport inner, string tenantId) : IDFeHttpTransport
{
    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, DFeHttpTransportOptions options,
        CancellationToken cancellationToken = default)
    {
        request.Options.Set(NFSeHttpTransport.TenantKey, tenantId);
        return inner.SendAsync(request, options, cancellationToken);
    }
}
