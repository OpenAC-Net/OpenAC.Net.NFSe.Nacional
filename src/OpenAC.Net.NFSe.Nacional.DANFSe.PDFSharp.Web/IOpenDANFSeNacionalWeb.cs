using Microsoft.AspNetCore.Http;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.Web;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp.Web;

/// <summary>Gera DANFSe em memória e respostas de arquivo para aplicações ASP.NET Core.</summary>
public interface IOpenDANFSeNacionalWeb
{
    /// <summary>Gera o PDF de uma NFS-e.</summary>
    /// <param name="nota">NFS-e que será representada no DANFSe.</param>
    /// <param name="tenantId">Identificador da empresa ou <see cref="NFSeTenant.Padrao"/>.</param>
    /// <param name="cancellationToken">Token para cancelar a obtenção da configuração.</param>
    /// <returns>Documento PDF gerado.</returns>
    ValueTask<DANFSeWebDocument> GerarAsync(NotaFiscalServico nota,
        string tenantId = NFSeTenant.Padrao, CancellationToken cancellationToken = default);

    /// <summary>Gera um único PDF contendo várias NFS-e.</summary>
    /// <param name="notas">NFS-e que serão representadas no DANFSe.</param>
    /// <param name="tenantId">Identificador da empresa ou <see cref="NFSeTenant.Padrao"/>.</param>
    /// <param name="cancellationToken">Token para cancelar a obtenção da configuração.</param>
    /// <returns>Documento PDF gerado.</returns>
    ValueTask<DANFSeWebDocument> GerarAsync(NotaFiscalServico[] notas,
        string tenantId = NFSeTenant.Padrao, CancellationToken cancellationToken = default);

    /// <summary>Gera uma resposta HTTP contendo o PDF de uma NFS-e.</summary>
    /// <param name="nota">NFS-e que será representada no DANFSe.</param>
    /// <param name="tenantId">Identificador da empresa ou <see cref="NFSeTenant.Padrao"/>.</param>
    /// <param name="cancellationToken">Token para cancelar a obtenção da configuração.</param>
    /// <returns>Resultado HTTP pronto para Minimal APIs.</returns>
    ValueTask<IResult> GerarResultadoAsync(NotaFiscalServico nota,
        string tenantId = NFSeTenant.Padrao, CancellationToken cancellationToken = default);

    /// <summary>Gera uma resposta HTTP contendo um único PDF com várias NFS-e.</summary>
    /// <param name="notas">NFS-e que serão representadas no DANFSe.</param>
    /// <param name="tenantId">Identificador da empresa ou <see cref="NFSeTenant.Padrao"/>.</param>
    /// <param name="cancellationToken">Token para cancelar a obtenção da configuração.</param>
    /// <returns>Resultado HTTP pronto para Minimal APIs.</returns>
    ValueTask<IResult> GerarResultadoAsync(NotaFiscalServico[] notas,
        string tenantId = NFSeTenant.Padrao, CancellationToken cancellationToken = default);
}
