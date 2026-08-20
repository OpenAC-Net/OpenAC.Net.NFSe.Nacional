using System;
using System.IO;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Componentes;
using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Configuracao;
using PacoteQuest = QuestPDF;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using static QuestPDF.Fluent.DocumentOperation;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Extensoes;

/// <summary>
/// Extensões para geração do PDF da NFS-e Padrão Nacional utilizando QuestPDF.
/// </summary>
/// <remarks>
/// <para>
/// A criptografia do PDF é aplicada via <see cref="DocumentOperation"/> do QuestPDF,
/// que atualmente aceita apenas caminhos de arquivo (<see cref="string"/>)
/// e não suporta <see cref="MemoryStream"/> ou <c>byte[]</c> diretamente.
/// Portanto, quando a configuração de segurança (<see cref="DANFSeConfig"/>) exigir senha,
/// o fluxo utiliza arquivos temporários no disco como intermediários.
/// </para>
/// </remarks>
public static class NotaFiscalServicoExtensao
{
    private static Documento CriarDocumento(
        DANFSeConfiguracao configuracoes,
        NotaFiscalServico nfse)
    {
        return new Documento(configuracoes, nfse);
    }

    /// <summary>
    /// Gera o PDF da NFS-e em memória e retorna como array de bytes.
    /// </summary>
    /// <param name="nfse">
    /// Instância da <see cref="NotaFiscalServico"/> contendo os dados da nota.
    /// </param>
    /// <param name="configuracoes">
    /// Configurações do DANFSe, incluindo layout, fontes e opções de segurança/criptografia.
    /// </param>
    /// <returns>
    /// Array de bytes representando o documento PDF. Se <see cref="DANFSeConfiguracao.Seguranca"/>
    /// possuir senha configurada, o PDF retornado estará criptografado com <see cref="Encryption256Bit"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Lançada quando <paramref name="nfse"/> ou <paramref name="configuracoes"/> é <c>null</c>.
    /// </exception>
    /// <remarks>
    /// <para>
    /// Fluxo sem senha (caminho rápido):
    /// <c>NotaFiscalServico → IDocument → GeneratePdf() → byte[]</c>
    /// </para>
    /// <para>
    /// Fluxo com senha:
    /// <c>NotaFiscalServico → IDocument → GeneratePdf(temp) → DocumentOperation.Encrypt() → byte[]</c>
    /// </para>
    /// <para>
    /// Arquivos temporários são criados em <see cref="Path.GetTempPath()"/> e removidos
    /// imediatamente após a leitura, mesmo em caso de exceção.
    /// </para>
    /// </remarks>
    public static byte[] GerarPdf(
        this NotaFiscalServico nfse,
        DANFSeConfiguracao configuracoes)
    {
        if (nfse == null)
        {
            throw new ArgumentNullException(nameof(nfse));
        }
        if (configuracoes == null)
        {
            throw new ArgumentNullException(nameof(configuracoes));
        }
        PacoteQuest.Settings.License = LicenseType.Community;
        var documento = CriarDocumento(configuracoes, nfse);
        if (!configuracoes.Seguranca.TemCriptografia)
        {
            return documento.GeneratePdf();
        }
        var pdfTemporario = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.pdf");
        try
        {
            documento.GeneratePdf(pdfTemporario);
            var seguranca = configuracoes.Seguranca;
            var pdfSeguroTemporario = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.pdf");
            LoadFile(pdfTemporario)
                .Encrypt(new Encryption256Bit
                {
                    UserPassword = seguranca.SenhaUsuario,
                    OwnerPassword = seguranca.SenhaProprietario ?? string.Empty,
                    AllowPrinting = seguranca.PermitirImpressao,
                    AllowContentExtraction = seguranca.PermitirCopiarConteudo,
                    AllowModification = seguranca.PermitirModificacao,
                    AllowAnnotation = seguranca.PermitirAnotacoes,
                    AllowFillingForms = seguranca.PermitirPreenchimentoFormularios,
                    AllowAssembly = seguranca.PermitirMontarDocumento,
                    EncryptMetadata = true
                })
                .Save(pdfSeguroTemporario);
            var pdfCriptografado = File.ReadAllBytes(pdfSeguroTemporario);
            File.Delete(pdfSeguroTemporario);
            return pdfCriptografado;
        }
        finally
        {
            if (File.Exists(pdfTemporario))
            {
                File.Delete(pdfTemporario);
            }
        }
    }

    /// <summary>
    /// Gera o PDF da NFS-e e salva diretamente no caminho especificado.
    /// </summary>
    /// <param name="nfse">
    /// Instância da <see cref="NotaFiscalServico"/> contendo os dados da nota.
    /// </param>
    /// <param name="configuracoes">
    /// Configurações do DANFSe, incluindo layout, fontes e opções de segurança/criptografia.
    /// </param>
    /// <param name="nomeArquivoComDiretorio">
    /// Caminho absoluto ou relativo onde o arquivo PDF será salvo.
    /// Exemplo: <c>@"C:\NFS-e\nota-123.pdf"</c>
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Lançada quando <paramref name="nfse"/> ou <paramref name="configuracoes"/> é <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Lançada quando <paramref name="nomeArquivoComDiretorio"/> é nulo, vazio ou contém apenas espaços.
    /// </exception>
    /// <remarks>
    /// Este método é um wrapper conveniente sobre <see cref="GerarPdf(NotaFiscalServico, DANFSeConfiguracao)"/>
    /// que persiste o resultado em disco. A criptografia, quando configurada, é aplicada antes da escrita do arquivo.
    /// </remarks>
    public static void GerarPdf(
        this NotaFiscalServico nfse,
        DANFSeConfiguracao configuracoes,
        string nomeArquivoComDiretorio)
    {
        if (string.IsNullOrWhiteSpace(nomeArquivoComDiretorio))
        {
            throw new ArgumentException(
                "O caminho do arquivo PDF deve ser informado.",
                nameof(nomeArquivoComDiretorio));
        }
        var pdf = GerarPdf(nfse, configuracoes);
        File.WriteAllBytes(nomeArquivoComDiretorio, pdf);
    }
}