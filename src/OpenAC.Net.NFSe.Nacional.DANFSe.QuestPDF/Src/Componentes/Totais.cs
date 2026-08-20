using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Comum;
using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Utilitarios;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Componentes;

/// <summary>
/// Componente responsável pela renderização da seção de totais da NFS-e no DANFSe.
/// </summary>
internal sealed class Totais : IComponent
{
    #region Fields    

    private readonly Common.Model.NotaFiscalServico _nfse;

    #endregion

    #region Constructors

    /// <summary>
    /// Inicializa uma nova instância do componente de totais.
    /// </summary>
    /// <param name="nfse">Dados da NFS-e contendo os valores totais.</param>
    public Totais(Common.Model.NotaFiscalServico nfse)
    {
        _nfse = nfse;
    }

    #endregion Constructors

    #region Methods

    /// <summary>
    /// Renderiza a primeira linha de totais: valor total, operação/serviço e descontos.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarPrimeiraLinha(ColumnDescriptor coluna)
    {
        var valoresDps = _nfse.Informacoes.Dps.Informacoes.Valores;
        var valoresServico = valoresDps.ValoresServico;
        var valoresDesconto = valoresDps.ValoresDesconto;
        coluna
            .Item()
            .Row(linha =>
            {
                SessaoFabrica.Criar(linha.RelativeItem(), DANFSeConstantes.Mensagens.ValorTotalNfse);
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.ValorOperacaoServico, Formatador.Moeda(valoresServico.Valor)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.DescontoIncondicionado, Formatador.Moeda(valoresDesconto?.ValorIncodicional)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.DescontoCondicionado, Formatador.Moeda(valoresDesconto?.ValorCondicional)));
            });
    }

    /// <summary>
    /// Renderiza a segunda linha de totais: retenções federais, valor líquido e totais IBS/CBS.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarSegundaLinha(ColumnDescriptor coluna)
    {
        var valroesDps = _nfse.Informacoes.Dps.Informacoes.Valores;
        var valorServicos = valroesDps.ValoresServico.Valor;
        var decontoIncondicionado = valroesDps.ValoresDesconto?.ValorIncodicional;
        var valorRetencoes = _nfse.Informacoes.Valores.TotalRetido;
        var valorLiquido = _nfse.Informacoes.Valores.ValorLiquido ?? (valorServicos - decontoIncondicionado - valorRetencoes);
        var valorTotalIbsCbs = (_nfse.Informacoes.IBSCBS?.Totais.TotalIBS.ValorTotalIBS ?? 0) + (_nfse.Informacoes.IBSCBS?.Totais.TotalCBS.ValorCBS ?? 0);
        coluna
            .Item()
            .Row(linha =>
            {
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.TotalRetencaoFederal, Formatador.Moeda(_nfse.Informacoes.Valores.TotalRetido)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.ValorLiquidoNfse, Formatador.Moeda(valorLiquido), true));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.TotalIbsCbs, Formatador.Moeda(valorTotalIbsCbs)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.ValorLiquidoSomaIbsCbs, Formatador.Moeda(valorLiquido + valorTotalIbsCbs)));
            });
    }


    /// <summary>
    /// Compõe a seção de totais no container informado.
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