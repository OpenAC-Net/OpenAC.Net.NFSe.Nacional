using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAC.Net.NFSe.Nacional.Storage;
using OpenAC.Net.NFSe.Nacional.Webservice;

namespace OpenAC.Net.NFSe.Nacional.Web;

internal sealed class OpenNFSeNacionalClientFactory(
    INFSeConfigurationProvider<Common.ConfiguracaoNFSe> configurationProvider,
    INFSeCertificateProvider certificateProvider,
    INFSeHttpTransport transport,
    INFSeDocumentStore documentStore,
    ILoggerFactory loggerFactory,
    IOptions<OpenNFSeNacionalWebOptions> options,
    NFSeHttpClientRegistry registry)
    : IOpenNFSeNacionalClientFactory
{
    public async ValueTask<IOpenNFSeNacionalClient> CreateAsync(string tenantId = NFSeTenant.Padrao,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tenantId);
        var configuration = await configurationProvider.GetConfigurationAsync(tenantId, cancellationToken)
            .ConfigureAwait(false);
        await certificateProvider.ConfigureAsync(tenantId, configuration.Certificados, cancellationToken)
            .ConfigureAwait(false);
        var certificate = NFSeWebCertificateResolver.Resolve(configuration.Certificados);
        if (certificate.NotAfter <= DateTime.Now.Add(options.Value.MargemExpiracaoCertificado))
            throw new InvalidOperationException($"O certificado da empresa '{tenantId}' está expirado ou próximo do vencimento.");
        registry.Bind(configuration, tenantId);
        return new OpenNFSeNacionalWebClient(configuration, transport, documentStore,
            loggerFactory.CreateLogger<OpenNFSeNacionalWebClient>(), options, tenantId);
    }
}
