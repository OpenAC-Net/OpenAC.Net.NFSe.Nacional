using System.IO;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Comum;
using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Configuracao;
using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Utilitarios;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Componentes;

/// <summary>
/// Componente responsável pela renderização do cabeçalho do DANFSe,
/// contendo logotipo, título e informações do município/ambiente.
/// </summary>
internal sealed class Cabecalho : IComponent
{
    #region Fields
    private readonly DANFSeConfiguracao _configuracao;

    private readonly NotaFiscalServico _nfse;

    #endregion

    #region Constructors

    /// <summary>
    /// Inicializa uma nova instância do cabeçalho do DANFSe.
    /// </summary>
    /// <param name="configuracao">Configurações de impressão do DANFSe.</param>
    /// <param name="nfse">Dados da NFS-e a serem apresentados.</param>
    public Cabecalho(
        DANFSeConfiguracao configuracao,
        NotaFiscalServico nfse)
    {
        _configuracao = configuracao;
        _nfse = nfse;
    }

    #endregion Constructors

    #region Methods

    /// <summary>
    /// Renderiza a coluna do logotipo no cabeçalho.
    /// </summary>
    /// <param name="linha">Descritor da linha do QuestPDF.</param>
    private void CriarLogo(RowDescriptor linha)
    {
        linha
            .RelativeItem(DANFSeConstantes.Sessao.TamanhoEspacoLogo)
            .AlignMiddle()
            .AlignCenter()
            .Column(coluna =>
            {
                byte[]? logo = default;
                if (!string.IsNullOrWhiteSpace(_configuracao.LogoPrestadorPath))
                {
                    logo = File.ReadAllBytes(_configuracao.LogoPrestadorPath);
                }
                logo ??= _configuracao.LogoPrestador ?? _configuracao.LogoNacional;
                if (logo is not null)
                {
                    coluna
                        .Item()
                        .Height(DANFSeConstantes.Sessao.TamanhoLogo)
                        .Image(logo)
                        .FitArea();
                    return;
                }
                coluna
                    .Item()
                    .Text(DANFSeConstantes.Mensagens.Nfse)
                    .FontSize(DANFSeConstantes.Fonte.TamanhoTituloGrande)
                    .Bold();
            });
    }

    /// <summary>
    /// Renderiza a coluna do título do documento no cabeçalho.
    /// </summary>
    /// <param name="linha">Descritor da linha do QuestPDF.</param>
    private void CriarTitulo(RowDescriptor linha)
    {
        linha
            .RelativeItem(DANFSeConstantes.Sessao.TamanhoEspacoTituloCabecalho)
            .AlignCenter()
            .AlignMiddle()
            .Column(coluna =>
            {
                coluna
                    .Item()
                    .Text(DANFSeConstantes.Mensagens.NfseVersao)
                    .AlignCenter()
                    .FontSize(DANFSeConstantes.Fonte.TamanhoTituloMedio)
                    .Bold();
                coluna
                    .Item()
                    .Text(DANFSeConstantes.Mensagens.DocumentoAuxiliar)
                    .FontSize(DANFSeConstantes.Fonte.TamanhoTitulo)
                    .Bold();
                if (_configuracao.Homologacao)
                {
                    coluna
                        .Item()
                        .Text(DANFSeConstantes.Mensagens.NfseSemValidadeJuridica)
                        .FontSize(DANFSeConstantes.Fonte.TamanhoTitulo)
                        .FontColor(DANFSeConstantes.Fonte.Cores.TextoHomologacao)
                        .Bold();
                }
            });
    }

    /// <summary>
    /// Renderiza a coluna de informações do município, ambiente gerador
    /// e tipo de emissão no cabeçalho.
    /// </summary>
    /// <param name="linha">Descritor da linha do QuestPDF.</param>
    private void CriarInformacoes(RowDescriptor linha)
    {
        var informacoes = _nfse.Informacoes;
        var municipio = informacoes.LocalEmissao;
        if (!string.IsNullOrWhiteSpace(_configuracao.CabecalhoMunicipio))
        {
            municipio = _configuracao.CabecalhoMunicipio;
        }
        linha
            .RelativeItem(DANFSeConstantes.Sessao.TamanhoEspacoDescricaoCabecalho)
            .AlignMiddle()
            .Column(coluna =>
            {
                coluna
                    .Item()
                    .Text(string.Format(DANFSeConstantes.Mensagens.Municipio, municipio))
                    .FontSize(DANFSeConstantes.Fonte.TamanhoTitulo);
                coluna
                    .Item()
                    .Text(string.Format(DANFSeConstantes.Mensagens.AmbienteGerador, Formatador.FormatarAmbiente(informacoes.AmbienteGerador)))
                    .FontSize(DANFSeConstantes.Fonte.TamanhoTituloPequeno);
                coluna.Item()
                    .Text(string.Format(DANFSeConstantes.Mensagens.TipoEmissao, Formatador.FormatarTipoEmissao(informacoes.TipoEmissao)))
                    .FontSize(DANFSeConstantes.Fonte.TamanhoTituloPequeno);
            });
    }

    /// <summary>
    /// Compõe o cabeçalho completo do DANFSe no container informado.
    /// </summary>
    /// <param name="container">Container do QuestPDF onde o cabeçalho será renderizado.</param>
    public void Compose(IContainer container)
    {
        container
            .Background(DANFSeConstantes.Sessao.CorFundoCabecalho)
            .BorderBottom(DANFSeConstantes.Sessao.TamanhoBordaCabecalho)
            .Padding(DANFSeConstantes.Sessao.TamanhoEspacoConteudo)
            .Row(linha =>
            {
                CriarLogo(linha);
                CriarTitulo(linha);
                CriarInformacoes(linha);
            });
    }

    #endregion
}
