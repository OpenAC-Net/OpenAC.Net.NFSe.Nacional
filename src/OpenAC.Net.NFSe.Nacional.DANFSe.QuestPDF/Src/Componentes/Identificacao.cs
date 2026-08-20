using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Comum;
using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Configuracao;
using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Utilitarios;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Componentes;

/// <summary>
/// Componente responsável pela renderização da seção de identificação da NFS-e,
/// incluindo chave de acesso, número, competência, datas e QR Code.
/// </summary>
internal sealed class Identificacao : IComponent
{
    #region Fields
    private readonly DANFSeConfiguracao _configuracao;

    private readonly Common.Model.NotaFiscalServico _nfse;

    #endregion

    #region Constructors

    /// <summary>
    /// Inicializa uma nova instância da identificação da NFS-e.
    /// </summary>
    /// <param name="configuracao">Configurações de impressão do DANFSe.</param>
    /// <param name="nfse">Dados da NFS-e a serem apresentados.</param>
    public Identificacao(
        DANFSeConfiguracao configuracao,
        Common.Model.NotaFiscalServico nfse)
    {
        _configuracao = configuracao;
        _nfse = nfse;
    }

    #endregion Constructors

    #region Methods

    /// <summary>
    /// Renderiza o campo da chave de acesso da NFS-e.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarChaveAcesso(ColumnDescriptor coluna)
    {
        CampoFabrica.Criar(coluna.Item(), DANFSeConstantes.Mensagens.ChaveAcesso, Formatador.Texto(_nfse.Informacoes.Id));
    }

    /// <summary>
    /// Renderiza a primeira linha de identificação: número, competência e data/hora de emissão.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarPrimeiraLinha(ColumnDescriptor coluna)
    {
        coluna
            .Item()
            .Row(linha =>
            {
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.NumeroNfse, Formatador.Texto(_nfse.Informacoes.NumeroNFSe.ToString())));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.CompetenciaNfse, Formatador.Competencia(_nfse.Informacoes.Dps.Informacoes.Competencia)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.DataHoraEmissao, Formatador.DataHora(_nfse.Informacoes.DhProcessamento)));
            });
    }

    /// <summary>
    /// Renderiza a segunda linha de identificação: número da DPS, série e data/hora de emissão da DPS.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    public void CriarSegundaLinha(ColumnDescriptor coluna)
    {
        coluna
            .Item()
            .Row(linha =>
            {
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.NumeroDps, _nfse.Informacoes.Dps.Informacoes.NumeroDps));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.SerieDps, _nfse.Informacoes.Dps.Informacoes.Serie));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.DataHoraEmissaoDps, Formatador.DataHora(_nfse.Informacoes.Dps.Informacoes.DhEmissao)));
            });
    }

    /// <summary>
    /// Renderiza a terceira linha de identificação: emitente, situação e finalidade.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarTerceiraLinha(ColumnDescriptor coluna)
    {
        coluna
            .Item()
            .Row(linha =>
            {
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.EmitenteNfse, Formatador.FormatarEmitente(_nfse.Informacoes.Dps.Informacoes.TipoEmitente)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.SituacaoNfse, Formatador.FormatarStatus(_nfse.Informacoes.SituacaoNFSe)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.Finalidade, Formatador.FormatarFinalidade(_nfse.Informacoes.Dps.Informacoes.IBSCBS?.FinalidadeNFSe)));
            });
    }

    /// <summary>
    /// Renderiza o QR Code de consulta pública da NFS-e quando configurado.
    /// </summary>
    /// <param name="linha">Descritor da linha do QuestPDF.</param>
    private void CriarQrCode(RowDescriptor linha)
    {
        if (!_configuracao.ExibirQRCode)
        {
            return;
        }
        var chaveAcesso = _nfse.Informacoes.Id;
        var urlQrCode = $"{DANFSeConstantes.UrlConsultaPublica}${chaveAcesso}";
        if (string.IsNullOrWhiteSpace(chaveAcesso))
        {
            linha
                .RelativeItem(1)
                .Padding(DANFSeConstantes.Sessao.TamanhoEspacoConteudo)
                .AlignCenter()
                .AlignMiddle()
                .Text(DANFSeConstantes.Mensagens.QrCodeNaoDisponivel)
                .FontSize(DANFSeConstantes.Fonte.TamanhoTituloPequeno)
                .AlignCenter();
        }
        var imagemQrCode = QrCode.Gerar(urlQrCode);
        linha
            .RelativeItem(1)
            .Padding(DANFSeConstantes.Sessao.TamanhoEspacoConteudo)
            .Column(coluna =>
            {
                coluna
                    .Item()
                    .AlignCenter()
                    .Width(DANFSeConstantes.Sessao.TamanhoQrCode)
                    .Image(imagemQrCode);
                coluna
                    .Item()
                    .PaddingTop(DANFSeConstantes.Sessao.EspacoAlturaDescricaoQrCode)
                    .Text(DANFSeConstantes.Mensagens.QrCodeDescricao)
                    .FontSize(DANFSeConstantes.Fonte.QrCode.TamanhoValor);
            });
    }

    /// <summary>
    /// Compõe a seção de identificação completa no container informado.
    /// </summary>
    /// <param name="container">Container do QuestPDF onde a identificação será renderizada.</param>
    public void Compose(IContainer container)
    {
        container
            .Border(DANFSeConstantes.Sessao.TamanhoBorda)
            .Row(linha =>
            {
                linha
                    .RelativeItem(DANFSeConstantes.Sessao.TamanhoEspacoConteudoMedio)
                    .Column(coluna =>
                    {
                        CriarChaveAcesso(coluna);
                        CriarPrimeiraLinha(coluna);
                        CriarSegundaLinha(coluna);
                        CriarTerceiraLinha(coluna);
                    });
                CriarQrCode(linha);
            });
    }

    #endregion
}
