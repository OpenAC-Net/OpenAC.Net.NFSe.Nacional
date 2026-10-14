using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using OpenAC.Net.NFSe.Nacional.Common;

namespace OpenAC.Net.NFSe.Nacional.Webservice;

/// <summary>
/// Abstrai o transporte HTTP utilizado pelos provedores de NFS-e.
/// </summary>
public interface INFSeHttpTransport
{
    /// <summary>
    /// Envia uma requisição utilizando a configuração e o certificado da operação atual.
    /// </summary>
    /// <param name="request">Requisição HTTP pronta para envio.</param>
    /// <param name="configuracao">Configuração fiscal e de certificado da operação.</param>
    /// <param name="cancellationToken">Token para cancelar o envio.</param>
    /// <returns>A resposta HTTP. O chamador é responsável por descartá-la.</returns>
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, ConfiguracaoNFSe configuracao,
        CancellationToken cancellationToken = default);
}
