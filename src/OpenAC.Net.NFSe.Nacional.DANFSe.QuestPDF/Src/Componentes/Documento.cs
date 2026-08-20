using System;
using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Comum;
using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Configuracao;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Componentes;

/// <summary>
/// Representa o documento PDF da NFS-e Padrão Nacional.
/// </summary>
/// <summary>
/// Representa o documento PDF completo do DANFSe Padrão Nacional,
/// agrupando todos os componentes visuais da NFS-e.
/// </summary>
internal sealed class Documento : IDocument
{
    #region Fields
    private readonly DANFSeConfiguracao _configuracao;

    private readonly Common.Model.NotaFiscalServico _nfse;

    #endregion

    #region Constructors

    /// <summary>
    /// Inicializa uma nova instância do documento PDF do DANFSe.
    /// </summary>
    /// <param name="configuracao">Configurações de impressão do DANFSe.</param>
    /// <param name="nfse">Dados da NFS-e a serem apresentados.</param>
    public Documento(
        DANFSeConfiguracao configuracao,
        Common.Model.NotaFiscalServico nfse)
    {
        _configuracao = configuracao ?? throw new ArgumentNullException(nameof(configuracao));
        _nfse = nfse ?? throw new ArgumentNullException(nameof(nfse));
    }

    #endregion Constructors

    #region Methods

    /// <summary>
    /// Configura o tamanho, margens e estilo de texto padrão da página.
    /// </summary>
    /// <param name="pagina">Descritor da página do QuestPDF.</param>
    private void ConfigurarPagina(PageDescriptor pagina)
    {
        pagina.Size(PageSizes.A4);
        pagina.MarginVertical((float)_configuracao.MargemVerticalMm);
        pagina.MarginHorizontal((float)_configuracao.MargemHorizontalMm);
        pagina.DefaultTextStyle(
            estilo => estilo
                .FontFamily("Arial")
                .FontSize(6));
    }

    /// <summary>
    /// Renderiza o canhoto de recebimento quando configurado para exibição.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarCanhoto(ColumnDescriptor coluna)
    {
        if (!_configuracao.ExibirCanhoto)
        {
            return;
        }
        coluna
            .Item()
            .Component(new CanhotoRecebimento(_nfse.Informacoes.NumeroNFSe, _nfse.Informacoes.Id));
    }

    /// <summary>
    /// Renderiza o cabeçalho do DANFSe.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarCabecalho(ColumnDescriptor coluna)
    {
        coluna
            .Item()
            .Component(new Cabecalho(_configuracao, _nfse));
    }

    /// <summary>
    /// Renderiza a seção de identificação da NFS-e.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarIdentificacao(ColumnDescriptor coluna)
    {
        coluna
            .Item()
            .Component(new Identificacao(_configuracao, _nfse));
    }

    /// <summary>
    /// Renderiza a seção de dados do prestador de serviços.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarPrestador(ColumnDescriptor coluna)
    {
        coluna
            .Item()
            .Component(new Prestador(_nfse));
    }

    /// <summary>
    /// Renderiza a seção de dados do tomador/adquirente.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarTomador(ColumnDescriptor coluna)
    {
        var tomador = _nfse.Informacoes.Dps.Informacoes.Tomador;
        coluna
            .Item()
            .Component(new Pessoa(DANFSeConstantes.Mensagens.TomadorAdiquirente, DANFSeConstantes.Mensagens.TomadorAdiquirenteOperacaoNaoIdentificado, tomador));
    }

    /// <summary>
    /// Renderiza a seção de dados do destinatário da operação.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarDestinatario(ColumnDescriptor coluna)
    {
        var destinatario = _nfse.Informacoes.Dps.Informacoes.IBSCBS?.Destinatario;
        coluna
            .Item()
            .Component(new Destinatario(destinatario));
    }

    /// <summary>
    /// Renderiza a seção de dados do intermediário da operação.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarIntermediario(ColumnDescriptor coluna)
    {
        var intermediario = _nfse.Informacoes.Dps.Informacoes.Intermediario;
        coluna
            .Item()
            .Component(new Pessoa(DANFSeConstantes.Mensagens.IntermediarioOperacao, DANFSeConstantes.Mensagens.IntermediarioOperacaoNaoIdentificado, intermediario));
    }

    /// <summary>
    /// Renderiza a seção de descrição do serviço prestado.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarServico(ColumnDescriptor coluna)
    {
        coluna
            .Item()
            .Component(new Servico(_nfse));
    }

    /// <summary>
    /// Renderiza a seção de tributação municipal (ISSQN).
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarTributacaoMunicipal(ColumnDescriptor coluna)
    {
        coluna
            .Item()
            .Component(new TributacaoMunicipal(_nfse));
    }

    /// <summary>
    /// Renderiza a seção de tributação federal (exceto IBS/CBS).
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarTributacaoFederal(ColumnDescriptor coluna)
    {
        coluna
            .Item()
            .Component(new TributacaoFederal(_nfse));
    }

    /// <summary>
    /// Renderiza a seção de tributação IBS/CBS quando houver informações.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarTributacaoIbsCbs(ColumnDescriptor coluna)
    {
        coluna
            .Item()
            .Component(new TributacaoIbsCbs(_nfse));
    }

    /// <summary>
    /// Renderiza a seção de totais da NFS-e.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarTotais(ColumnDescriptor coluna)
    {
        coluna
            .Item()
            .Component(new Totais(_nfse));
    }

    /// <summary>
    /// Renderiza a seção de informações complementares da NFS-e.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarInformacoesComplementares(ColumnDescriptor coluna)
    {
        coluna
            .Item()
            .Component(new InformacoesComplementares(_nfse));
    }

    /// <summary>
    /// Renderiza a marca d'água do documento
    /// </summary>
    /// <param name="pagina">Descritor da página do QuestPDF.</param>
    /// <param name="texto">Texto que será renderizado.</param>
    /// <param name="cor">Cor do texto que será renderizado.</param>
    private static void RenderizarMarcaDagua(
        PageDescriptor pagina,
        string texto,
        string cor)
    {
        pagina
            .Foreground()
            .AlignCenter()
            .AlignMiddle()
            .Rotate(-40)
            .Text(texto)
            .FontSize(DANFSeConstantes.Fonte.TamanhoTituloGigante)
            .FontColor(cor)
            .Bold();
    }

    /// <summary>
    /// Renderiza a marca d'água do documento quando a nota estiver cancelada,
    /// substituída ou em ambiente de homologação.
    /// </summary>
    /// <param name="pagina">Descritor da página do QuestPDF.</param>
    private void CriarMarcaDagua(PageDescriptor pagina)
    {
        if (_configuracao.Cancelada)
        {
            RenderizarMarcaDagua(
                pagina,
                DANFSeConstantes.Mensagens.NfseCancelada,
                DANFSeConstantes.Fonte.Cores.TextoCancelada);
            return;
        }
        if (_configuracao.Substituida)
        {
            RenderizarMarcaDagua(
                pagina,
                DANFSeConstantes.Mensagens.NfseSubstituida,
                DANFSeConstantes.Fonte.Cores.TextoSubstituida);
            return;
        }
        if (_configuracao.Homologacao)
        {
            RenderizarMarcaDagua(
                pagina,
                DANFSeConstantes.Mensagens.NfseSemValidadeJuridica,
                DANFSeConstantes.Fonte.Cores.TextoSemValidadeJuridica);
        }
    }

    /// <summary>
    /// Compõe o conteúdo principal do DANFSe, incluindo todos os componentes
    /// e a marca d'água de cancelada, substituída ou homologação quando configurada.
    /// </summary>
    /// <param name="pagina">Descritor da página do QuestPDF.</param>
    private void CriarConteudo(PageDescriptor pagina)
    {
        pagina
            .Content()
            .Element(container =>
            {
                container
                    .Border(DANFSeConstantes.Sessao.TamanhoBordaCabecalho)
                    .Column(coluna =>
                    {
                        CriarCanhoto(coluna);
                        CriarCabecalho(coluna);
                        CriarIdentificacao(coluna);
                        CriarPrestador(coluna);
                        CriarTomador(coluna);
                        CriarDestinatario(coluna);
                        CriarIntermediario(coluna);
                        CriarServico(coluna);
                        CriarTributacaoMunicipal(coluna);
                        CriarTributacaoFederal(coluna);
                        CriarTributacaoIbsCbs(coluna);
                        CriarTotais(coluna);
                        CriarInformacoesComplementares(coluna);
                    });
            });
        CriarMarcaDagua(pagina);
    }

    /// <summary>
    /// Configura o cabeçalho da página do DANFSe.
    /// </summary>
    /// <param name="pagina">Descritor da página do QuestPDF.</param>
    private void CriarCabecalhoPagina(PageDescriptor pagina)
    {
        pagina
            .Header()
            .MinHeight(0);
    }

    /// <summary>
    /// Configura o rodapé da página do DANFSe com numeração de páginas.
    /// </summary>
    /// <param name="pagina">Descritor da página do QuestPDF.</param>
    private static void CriarRodapePagina(PageDescriptor pagina)
    {
        pagina
            .Footer()
            .AlignCenter()
            .Text(texto =>
            {
                texto.CurrentPageNumber()
                    .FontSize(5);
                texto.Span(" / ")
                    .FontSize(5);
                texto.TotalPages()
                    .FontSize(5);
            });
    }

    /// <summary>
    /// Cria o título do documento com base no número da NFS-e.
    /// </summary>
    /// <returns>Título a ser utilizado nos metadados do PDF.</returns>
    private string CriarTitulo()
    {
        var numero = _nfse.Informacoes?.NumeroNFSe;
        if (numero is null or 0)
        {
            return DANFSeConstantes.Mensagens.MetaDadoTitulo;
        }
        return $"{DANFSeConstantes.Mensagens.Nfse} {numero}";
    }

    /// <summary>
    /// Retorna os metadados do documento PDF.
    /// </summary>
    /// <returns>Metadados do documento.</returns>
    public DocumentMetadata GetMetadata()
    {
        return new DocumentMetadata
        {
            Title = CriarTitulo(),
            Author = "OpenAC.Net",
            Subject = DANFSeConstantes.Mensagens.MetaDadoTitulo,
            Keywords = "NFS-e, NFSe, NFS-e Nacional, PDF",
            Creator = "OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF"
        };
    }

    /// <summary>
    /// Retorna as configurações de geração do documento PDF.
    /// </summary>
    /// <returns>Configurações padrão do documento.</returns>
    public DocumentSettings GetSettings()
    {
        return DocumentSettings.Default;
    }

    /// <summary>
    /// Compõe a estrutura completa do documento PDF no container informado.
    /// </summary>
    /// <param name="container">Container do documento QuestPDF.</param>
    public void Compose(IDocumentContainer container)
    {
        container
            .Page(pagina =>
            {
                ConfigurarPagina(pagina);
                CriarConteudo(pagina);
                CriarCabecalhoPagina(pagina);
                CriarRodapePagina(pagina);
            });
    }

    #endregion
}
