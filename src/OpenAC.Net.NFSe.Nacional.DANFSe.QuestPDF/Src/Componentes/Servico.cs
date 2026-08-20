using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Comum;
using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Utilitarios;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Componentes;

/// <summary>
/// Componente responsável pela renderização dos dados do serviço prestado no DANFSe.
/// </summary>
internal sealed class Servico : IComponent
{
    #region Fields    

    private readonly Common.Model.NotaFiscalServico _nfse;

    #endregion

    #region Constructors

    /// <summary>
    /// Inicializa uma nova instância do componente de dados do serviço.
    /// </summary>
    /// <param name="nfse">Dados da NFS-e contendo as informações do serviço.</param>
    public Servico(Common.Model.NotaFiscalServico nfse)
    {
        _nfse = nfse;
    }

    #endregion Constructors

    #region Methods

    /// <summary>
    /// Renderiza a primeira linha com título, códigos de tributação e local da prestação.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarPrimeiraLinha(ColumnDescriptor coluna)
    {
        var servico = _nfse.Informacoes.Dps.Informacoes.Servico;
        var informacoes = servico.Informacoes;
        var codigoTributacaoNacionalMunicipal = $"{Formatador.FormatarCodigoTributacaoNacional(informacoes.CodTributacaoNacional)} / {Formatador.Texto(informacoes.CodTributacaoMunicipio)}";
        var codigoMunicipio = _nfse.Informacoes.Dps.Informacoes.Servico.Localidade.CodMunicipioPrestacao;
        var localPrestacao = $"{Formatador.Texto(_nfse.Informacoes.LocalPrestacao)} / {Formatador.FormatarUfPorCodigoMunipio(codigoMunicipio)} / BR";
        coluna
            .Item()
            .Row(linha =>
            {
                SessaoFabrica.Criar(linha.RelativeItem(), DANFSeConstantes.Mensagens.ServicoPrestado);
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.CodigoTributacaoNacionalMunicipal, codigoTributacaoNacionalMunicipal));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.CodigoNbs, Formatador.FormatarCodigoNbs(informacoes.CodNBS)));
                linha
                    .RelativeItem()
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.LocalPrestacaoSiglaUfPais, localPrestacao));
            });
    }

    /// <summary>
    /// Renderiza a descrição do tributo nacional.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarSegundaLinha(ColumnDescriptor coluna)
    {
        var servico = _nfse.Informacoes.Dps.Informacoes.Servico;
        coluna
            .Item()
            .Row(linha =>
            {
                linha
                    .RelativeItem()
                    .PaddingHorizontal(DANFSeConstantes.Sessao.TamanhoEspacoConteudoMedio)
                    .PaddingVertical(DANFSeConstantes.Sessao.TamanhoEspacoConteudoPequeno)
                    .Text(_nfse.Informacoes.DescricaoTributoNacional)
                    .FontSize(DANFSeConstantes.Fonte.Campo.TamanhoValor);
            });
    }

    /// <summary>
    /// Renderiza a descrição detalhada do serviço prestado.
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
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.DescricaoServico, Formatador.Texto(_nfse.Informacoes.Dps.Informacoes.Servico.Informacoes.Descricao)));
            });
    }

    /// <summary>
    /// Compõe a seção de dados do serviço no container informado.
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
            });
    }

    #endregion
}