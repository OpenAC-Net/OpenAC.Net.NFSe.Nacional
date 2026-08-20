using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Comum;
using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Utilitarios;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Componentes;

/// <summary>
/// Componente responsável pela renderização da seção de tributação municipal (ISSQN) no DANFSe.
/// </summary>
internal sealed class TributacaoMunicipal : IComponent
{
    #region Fields    

    private readonly Common.Model.NotaFiscalServico _nfse;

    #endregion

    #region Constructors

    /// <summary>
    /// Inicializa uma nova instância do componente de tributação municipal.
    /// </summary>
    /// <param name="nfse">Dados da NFS-e contendo os tributos municipais.</param>
    public TributacaoMunicipal(Common.Model.NotaFiscalServico nfse)
    {
        _nfse = nfse;
    }

    #endregion Constructors

    #region Methods

    /// <summary>
    /// Renderiza a primeira linha de tributação municipal: tipo, município/UF/país de incidência.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarPrimeiraLinha(ColumnDescriptor coluna)
    {
        var tributos = _nfse.Informacoes.Dps.Informacoes.Valores.Tributos.Municipal;
        var codigoLocalIncidencia = _nfse.Informacoes.CodLocalIncidencia;
        var municipioTexto = $"{Formatador.Texto($"{_nfse.Informacoes.LocalIncidencia}/ {Formatador.FormatarUfPorCodigoMunipio(codigoLocalIncidencia)} / BR")}";
        coluna
            .Item()
            .Row(linha =>
            {
                SessaoFabrica.Criar(linha.RelativeItem(), DANFSeConstantes.Mensagens.TributacaoMunicipalIssqn);
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.TipoTributacaoIssqn, Formatador.FormatarTributoISSQN(tributos.ISSQN)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.MunicipioSiglaUfPaisIncidenciaIssqn, municipioTexto));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, "", ""));
            });
    }

    /// <summary>
    /// Renderiza a segunda linha de tributação municipal: regime especial, imunidade, suspensão e processo.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarSegundaLinha(ColumnDescriptor coluna)
    {
        var tributos = _nfse.Informacoes.Dps.Informacoes.Valores.Tributos.Municipal;
        coluna
            .Item()
            .Row(linha =>
            {
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.RegimeEspecialTributosIssqn, Formatador.FormatarRegimeEspecialISSQN(_nfse.Informacoes.Dps.Informacoes.Prestador.Regime.RegimeEspecial)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.TipoImunidadeIssqn, Formatador.FormatarTipoImunidadeISSQN(tributos.TipoImunidade)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.SuspensaoExigibiliddeIssqn, Formatador.FormatarSuspensaoExibilidadeISSQN(tributos.Suspensao?.TipoSuspensao)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.NumeroProcessoSuspensao, Formatador.Texto(tributos.Suspensao?.NumeroProcesso)));
            });
    }

    /// <summary>
    /// Renderiza a terceira linha de tributação municipal: benefício municipal, deduções/reduções e descontos.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarTerceiraLinha(ColumnDescriptor coluna)
    {
        var valores = _nfse.Informacoes.Valores;
        var valoresDps = _nfse.Informacoes.Dps.Informacoes.Valores;
        coluna
            .Item()
            .Row(linha =>
            {
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.BeneficioMunicipal, Formatador.FormatarTipoBeneficioMunicipal(valores.TipoBeneficioMunicipal)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.CalculoBeneficioMunicipal, Formatador.Moeda(valores.ValorBcBeneficioMunicipal)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.TotalDeducoesReducoes, Formatador.Moeda(valoresDps.ValoresDeducaoReducao?.Valor)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.DescontoIncondicionado, Formatador.Moeda(valoresDps.ValoresDesconto?.ValorIncodicional)));
            });
    }

    /// <summary>
    /// Renderiza a quarta linha de tributação municipal: base de cálculo, alíquota, retenção e ISSQN apurado.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarQuartaLinha(ColumnDescriptor coluna)
    {
        var dps = _nfse.Informacoes.Dps.Informacoes;
        var valores = _nfse.Informacoes.Valores;
        var valoresDps = dps.Valores;
        coluna
            .Item()
            .Row(linha =>
            {
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.BaseCalculoIssqn, Formatador.Moeda(valores.ValorBc)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.AliquotaAplicada, Formatador.Percentual(valores.Aliquota)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.RetencaoIssqn, Formatador.FormatarTipoRetencaoISSQN(valoresDps.Tributos.Municipal.TipoRetencaoISSQN)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.IssqnApurado, Formatador.Moeda(valores.ValorISSQN)));
            });
    }

    /// <summary>
    /// Compõe a seção de tributação municipal no container informado.
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
                CriarTerceiraLinha(coluna);
                CriarQuartaLinha(coluna);
            });
    }

    #endregion
}