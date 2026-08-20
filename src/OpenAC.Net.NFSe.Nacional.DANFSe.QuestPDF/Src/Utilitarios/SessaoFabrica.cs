using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Comum;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Utilitarios;

/// <summary>
/// Fábrica responsável pela criação de sessões/títulos de agrupamento
/// do DANFSe, com fundo destacado.
/// </summary>
internal static class SessaoFabrica
{
    /// <summary>
    /// Aplica o estilo visual padrão de título de sessão no container.
    /// </summary>
    /// <param name="container">Container do QuestPDF a ser estilizado.</param>
    /// <returns>Container estilizado.</returns>
    private static IContainer Titulo(
        IContainer container)
    {
        return container
            .Background(DANFSeConstantes.Fonte.Cores.FundoTitulo)
            .PaddingHorizontal(DANFSeConstantes.Sessao.TamanhoEspacoConteudo)
            .PaddingVertical(DANFSeConstantes.Sessao.TamanhoEspacoConteudoPequeno);
    }

    /// <summary>
    /// Cria uma sessão com o título informado no container.
    /// </summary>
    /// <param name="container">Container do QuestPDF onde a sessão será renderizada.</param>
    /// <param name="titulo">Texto do título da sessão.</param>
    public static void Criar(IContainer container, string titulo)
    {
        container
            .Element(Titulo)
            .Text(titulo)
            .FontSize(DANFSeConstantes.Fonte.TamanhoTitulo)
            .Bold();
    }
}