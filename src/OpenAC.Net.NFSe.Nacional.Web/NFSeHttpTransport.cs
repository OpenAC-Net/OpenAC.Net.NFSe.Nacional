using Microsoft.Extensions.Logging;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Webservice;

namespace OpenAC.Net.NFSe.Nacional.Web;

/// <summary>
/// Transporte web apoiado integralmente pelo <see cref="IHttpClientFactory"/>.
/// </summary>
internal sealed class NFSeHttpTransport : INFSeHttpTransport
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly NFSeHttpClientRegistry registry;
    private readonly ILogger<NFSeHttpTransport> logger;

    public NFSeHttpTransport(IHttpClientFactory httpClientFactory, NFSeHttpClientRegistry registry,
        ILogger<NFSeHttpTransport> logger)
    {
        this.httpClientFactory = httpClientFactory;
        this.registry = registry;
        this.logger = logger;
    }

    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, ConfiguracaoNFSe configuracao,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Enviando {Method} para {Uri} pelo IHttpClientFactory", request.Method, request.RequestUri);
        var certificate = NFSeWebCertificateResolver.Resolve(configuracao.Certificados);
        var endpoint = request.RequestUri ?? throw new InvalidOperationException("A requisição não possui endpoint.");
        var name = registry.Register(configuracao, certificate,
            (System.Security.Authentication.SslProtocols)configuracao.WebServices.Protocolos, endpoint);
        var client = httpClientFactory.CreateClient(name);
        return client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
    }
}
