using System.Text;
using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Comum;
using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Utilitarios;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Componentes;

/// <summary>
/// Componente responsável pela renderização das informações complementares da NFS-e no DANFSe.
/// </summary>
internal sealed class InformacoesComplementares : IComponent
{
    #region Fields    

    private readonly Common.Model.NotaFiscalServico _nfse;

    #endregion

    #region Constructors

    /// <summary>
    /// Inicializa uma nova instância do componente de informações complementares.
    /// </summary>
    /// <param name="nfse">Dados da NFS-e contendo as informações complementares.</param>
    public InformacoesComplementares(Common.Model.NotaFiscalServico nfse)
    {
        _nfse = nfse;
    }

    #endregion Constructors

    #region Methods

    /// <summary>
    /// Renderiza o título da seção de informações complementares.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarPrimeiraLinha(ColumnDescriptor coluna)
    {
        coluna
            .Item()
            .Row(linha =>
            {
                SessaoFabrica.Criar(linha.RelativeItem(), DANFSeConstantes.Mensagens.InformacoesComplementares);
            });
    }

    /// <summary>
    /// Renderiza o texto das informações complementares e o valor total aproximado dos tributos.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarSegundaLinha(ColumnDescriptor coluna)
    {
        var complementares = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(_nfse.Informacoes.Valores.OutrasInformacoes))
        {
            complementares.AppendLine(_nfse.Informacoes.Valores.OutrasInformacoes);
        }
        if (!string.IsNullOrWhiteSpace(_nfse.Informacoes.Dps.Informacoes.Servico.InformacoesComplementares?.Informacoes))
        {
            complementares.AppendLine($"Inf. Cont: {_nfse.Informacoes.Dps.Informacoes.Servico.InformacoesComplementares?.Informacoes}");
        }        
        var valorTotalTributos = _nfse.Informacoes.Dps.Informacoes.Valores.Tributos.Total;
        if (valorTotalTributos is not null)
        {
            complementares.Append(string.Format(DANFSeConstantes.Mensagens.InformacoesComplementaresTotais, Formatador.Moeda(valorTotalTributos.ValorTotal?.TotalFederal), Formatador.Moeda(valorTotalTributos.ValorTotal?.TotalEstadual), Formatador.Moeda(valorTotalTributos.ValorTotal?.TotalMunicipal)));
        }
        coluna
            .Item()
            .Row(linha =>
            {
                linha
                    .RelativeItem()
                    .PaddingHorizontal(DANFSeConstantes.Sessao.TamanhoEspacoConteudoMedio)
                    .PaddingVertical(DANFSeConstantes.Sessao.TamanhoEspacoConteudoPequeno)
                    .Text(complementares.ToString())
                    .FontSize(DANFSeConstantes.Fonte.Campo.TamanhoValor);
            });
    }

    /// <summary>
    /// Compõe a seção de informações complementares no container informado.
    /// </summary>
    /// <param name="container">Container do QuestPDF onde a seção será renderizada.</param>
    public void Compose(IContainer container)
    {
        container
            .Column(coluna =>
            {
                CriarPrimeiraLinha(coluna);
                CriarSegundaLinha(coluna);
            });
    }

    #endregion
}