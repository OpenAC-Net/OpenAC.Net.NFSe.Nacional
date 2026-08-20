using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Comum;
using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Utilitarios;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Componentes;

/// <summary>
/// Componente responsável pela renderização da seção de tributação federal no DANFSe.
/// </summary>
internal sealed class TributacaoFederal : IComponent
{
    #region Fields    

    private readonly Common.Model.NotaFiscalServico _nfse;

    #endregion

    #region Constructors

    /// <summary>
    /// Inicializa uma nova instância do componente de tributação federal.
    /// </summary>
    /// <param name="nfse">Dados da NFS-e contendo os tributos federais.</param>
    public TributacaoFederal(Common.Model.NotaFiscalServico nfse)
    {
        _nfse = nfse;
    }

    #endregion Constructors

    #region Methods

    /// <summary>
    /// Renderiza a primeira linha de tributação federal: IRRF, contribuição previdenciária e CSLL.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarPrimeiraLinha(ColumnDescriptor coluna)
    {
        var tributos = _nfse.Informacoes.Dps.Informacoes.Valores.Tributos.Federal;
        coluna
            .Item()
            .Row(linha =>
            {
                SessaoFabrica.Criar(linha.RelativeItem(), DANFSeConstantes.Mensagens.TributacaoFederalExcetoCbs);
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.Irrf, Formatador.Moeda(tributos?.ValorIRRF)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.ContribuicaoPrevidenciariaRetida, Formatador.Moeda(tributos?.ValorCP)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.ContribuicoesSocialRetida, Formatador.Moeda(tributos?.ValorCSLL)));
            });
    }

    /// <summary>
    /// Renderiza a segunda linha de tributação federal: PIS, COFINS e tipo de retenção.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarSegundaLinha(ColumnDescriptor coluna)
    {
        var tributos = _nfse.Informacoes.Dps.Informacoes.Valores.Tributos.Federal;
        coluna
            .Item()
            .Row(linha =>
            {
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.PisDebitoApuracaoPropria, Formatador.Moeda(tributos?.PisCofins?.ValorPis)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.CofinsDebitoApuracaoPropria, Formatador.Moeda(tributos?.PisCofins?.ValorCofins)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.DescricaoContribuicaoSocialRetida, Formatador.FormatarTipoRetencaoPisCofinsCsll(tributos?.PisCofins?.TipoRetencao)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, "", ""));
            });
    }


    /// <summary>
    /// Compõe a seção de tributação federal no container informado.
    /// </summary>
    /// <param name="container">Container do QuestPDF onde a seção será renderizada.</param>
    public void Compose(IContainer container)
    {
        container
            .Border(DANFSeConstantes.Sessao.TamanhoBorda)
            .Column(coluna =>
            {
                CriarPrimeiraLinha(coluna);
                CriarSegundaLinha(coluna);
            });
    }

    #endregion
}