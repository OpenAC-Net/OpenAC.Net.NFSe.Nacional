using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Comum;
using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Utilitarios;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Componentes;

/// <summary>
/// Componente responsável pela renderização do canhoto de recebimento da NFS-e.
/// </summary>
internal sealed class CanhotoRecebimento : IComponent
{
    #region Fields
    private readonly long _numeroNfse;

    private readonly string? _chaveNfse;

    #endregion

    #region Constructors

    /// <summary>
    /// Inicializa uma nova instância do canhoto de recebimento.
    /// </summary>
    /// <param name="numeroNfse">Número da NFS-e.</param>
    /// <param name="chaveNfse">Chave de acesso da NFS-e.</param>
    public CanhotoRecebimento(
        long numeroNfse, 
        string? chaveNfse)
    {
        _numeroNfse = numeroNfse;
        _chaveNfse = chaveNfse?.Substring(3);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Renderiza a primeira linha do canhoto com o título da seção.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarPrimeiraLinha(ColumnDescriptor coluna)
    {
        coluna
            .Item()
            .Border(DANFSeConstantes.Sessao.TamanhoBorda)
            .Row(linha =>
            {
                SessaoFabrica.Criar(linha.RelativeItem(), DANFSeConstantes.Mensagens.CanhotoRecebimento);
            });
    }

    /// <summary>
    /// Renderiza a segunda linha do canhoto com data de cientificação,
    /// identificação do recebedor e número/chave da NFS-e.
    /// </summary>
    /// <param name="coluna">Descritor da coluna do QuestPDF.</param>
    private void CriarSegundaLinha(ColumnDescriptor coluna)
    {
        coluna
            .Item()
            .Row(linha =>
            {
                linha
                    .RelativeItem()
                    .Border(DANFSeConstantes.Sessao.TamanhoBorda)
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.DataCientificacao, "-"));
                linha
                    .RelativeItem()
                    .Border(DANFSeConstantes.Sessao.TamanhoBorda)
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.IdentificacaoAssinaturaRecebedor, "-"));
                linha
                    .RelativeItem()
                    .Border(DANFSeConstantes.Sessao.TamanhoBorda)
                    .Element(c => CampoFabrica.Criar(c, DANFSeConstantes.Mensagens.NumeroNfseChave, $"{Formatador.Texto(_numeroNfse.ToString())} / {Formatador.Texto(_chaveNfse)}"));
            });
    }

    /// <summary>
    /// Compõe o canhoto de recebimento completo no container informado.
    /// </summary>
    /// <param name="container">Container do QuestPDF onde o canhoto será renderizado.</param>
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