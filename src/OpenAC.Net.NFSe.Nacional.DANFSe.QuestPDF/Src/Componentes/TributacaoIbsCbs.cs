using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Comum;
using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Utilitarios;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Componentes;

/// <summary>
/// Componente responsável pela renderização da seção de tributação IBS/CBS no DANFSe.
/// </summary>
internal sealed class TributacaoIbsCbs : IComponent
{
    #region Fields    

    private readonly Common.Model.NotaFiscalServico _nfse;

    #endregion

    #region Constructors

    /// <summary>
    /// Inicializa uma nova instância do componente de tributação IBS/CBS.
    /// </summary>
    /// <param name="nfse">Dados da NFS-e contendo as informações de IBS/CBS.</param>
    public TributacaoIbsCbs(Common.Model.NotaFiscalServico nfse)
    {
        _nfse = nfse;
    }

    #endregion Constructors

    #region Methods

    /// <summary>
    /// Renderiza a primeira linha de IBS/CBS: CST/cClassTrib, indicação de operação e localidade.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarPrimeiraLinha(ColumnDescriptor coluna)
    {
        var ibsCbs = _nfse.Informacoes.IBSCBS;
        var ibsCbsDps = _nfse.Informacoes.Dps.Informacoes.IBSCBS;
        var grupoIbsCbs = _nfse.Informacoes.Dps.Informacoes.IBSCBS?.Valores.Tributos.GrupoIBSCBS;
        var cst = $"{Formatador.Texto(grupoIbsCbs?.CodigoSituacaoTributaria)} / {Formatador.Texto(grupoIbsCbs?.CodigoClassificacaoTributaria)}";
        var incidencia = $" {Formatador.Texto(ibsCbsDps?.CodigoIndicadorOperacao)} / {Formatador.Texto(ibsCbs?.CodigoLocalidadeIncidencia)} / {Formatador.Texto(ibsCbs?.DescricaoLocalidadeIncidencia)} / {Formatador.FormatarUfPorCodigoMunipio(ibsCbs?.CodigoLocalidadeIncidencia)}";
        coluna
            .Item()
            .Row(linha =>
            {
                SessaoFabrica.Criar(linha.RelativeItem(), DANFSeConstantes.Mensagens.TributacaoIbsCbs);
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.CstCclassTrib, cst));
                linha
                    .RelativeItem(DANFSeConstantes.Sessao.TamanhoEspacoConteudoPequeno)
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.IndicadorOperacaoCodigoIbgeIncidenciaMunicipioIncidenciaSiglaUf, incidencia));
            });
    }

    /// <summary>
    /// Renderiza a segunda linha de IBS/CBS: exclusões/reduções, base de cálculo e alíquotas.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarSegundaLinha(ColumnDescriptor coluna)
    {
        var ibsCbs = _nfse.Informacoes.IBSCBS;
        coluna
            .Item()
            .Row(linha =>
            {
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.ExclusaoReducaoBaseCalculo, Formatador.Moeda(_nfse.Informacoes.Valores.ValorISSQN)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.BaseCalculoAposExclusaoReducao, Formatador.Moeda(ibsCbs?.Valores.ValorBaseCalculo)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.ReducaoAliquotaIbsCbs, $"{Formatador.Percentual(ibsCbs?.PercentualRedutor)}"));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.AliquotaIbsUfMunicipio, $"{Formatador.Percentual(ibsCbs?.Valores.Estado.PercentualIBSUf)} / {Formatador.Percentual(ibsCbs?.Valores.Municipio.PercentualIBSMunicipal)}"));
            });
    }

    /// <summary>
    /// Renderiza a terceira linha de IBS/CBS: alíquotas efetivas e valores apurados municipais/estaduais.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarTerceiraLinha(ColumnDescriptor coluna)
    {
        var ibsCbs = _nfse.Informacoes.IBSCBS;
        coluna
            .Item()
            .Row(linha =>
            {
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.AliquotaEfetivaMunicipalIbs, Formatador.Percentual(ibsCbs?.Valores.Municipio.PercentualAliquotaEfetivaMunicipal)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.ValorApuradoMunicipalIbs, Formatador.Moeda(ibsCbs?.Totais.TotalIBS.TotalMunicipal.ValorIBSMunicipal)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.AliquotaEfetivaEstadualIbs, Formatador.Percentual(ibsCbs?.Valores.Estado.PercentualAliquotaEfetivaUf)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.ValorApuradoEstadualIbs, Formatador.Moeda(ibsCbs?.Totais.TotalIBS.TotalEstadual.ValorIBSUF)));
            });
    }

    /// <summary>
    /// Renderiza a quarta linha de IBS/CBS: totais apurados e alíquotas da CBS.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarQuartaLinha(ColumnDescriptor coluna)
    {
        var ibsCbs = _nfse.Informacoes.IBSCBS;
        coluna
            .Item()
            .Row(linha =>
            {
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.ValorTotalApuradoIbs, Formatador.Moeda(ibsCbs?.Totais.TotalIBS.ValorTotalIBS)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.AliquotaCbs, Formatador.Percentual(ibsCbs?.Valores.Federal.PercentualCBS)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.AliquotaEfetivaCbs, Formatador.Percentual(ibsCbs?.Valores.Federal.PercentualAliquotaEfetivaCBS)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.ValorTotalApuradoCbs, Formatador.Moeda(ibsCbs?.Totais.TotalCBS.ValorCBS)));
            });
    }

    /// <summary>
    /// Compõe a seção de tributação IBS/CBS no container informado.
    /// </summary>
    /// <param name="container">Container do QuestPDF onde a seção será renderizada.</param>
    public void Compose(IContainer container)
    {
        if (_nfse.Informacoes?.IBSCBS is null || string.IsNullOrWhiteSpace(_nfse.Informacoes?.IBSCBS.CodigoLocalidadeIncidencia))
        {
            return;
        }
        container
            .Border(DANFSeConstantes.Sessao.TamanhoBorda)
            .Column(coluna =>
            {
                CriarPrimeiraLinha(coluna);
                CriarSegundaLinha(coluna);
                CriarTerceiraLinha(coluna);
                CriarQuartaLinha(coluna);
            });
    }

    #endregion
}