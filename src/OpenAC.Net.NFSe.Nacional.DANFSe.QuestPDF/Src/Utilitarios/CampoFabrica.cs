using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Comum;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Utilitarios;

/// <summary>
/// Fábrica responsável pela criação de campos visuais padrão do DANFSe,
/// compostos por título e valor.
/// </summary>
internal static class CampoFabrica
{
    /// <summary>
    /// Cria um campo visual com título e valor no container informado.
    /// </summary>
    /// <param name="container">Container do QuestPDF onde o campo será renderizado.</param>
    /// <param name="titulo">Texto do título do campo.</param>
    /// <param name="valor">Texto do valor do campo.</param>
    /// <param name="valorEmNegrito">Indica se o valor deve ser renderizado em negrito.</param>
    public static void Criar(
        IContainer container,
        string titulo,
        string valor,
        bool valorEmNegrito = false)
    {
        container
            .PaddingHorizontal(DANFSeConstantes.Sessao.TamanhoEspacoConteudoMedio)
            .PaddingVertical(DANFSeConstantes.Sessao.TamanhoEspacoConteudoPequeno)
            .Column(coluna =>
            {
                coluna
                    .Item()
                    .Text(titulo)
                    .FontSize(DANFSeConstantes.Fonte.Campo.TamanhoTitulo)
                    .Bold();
                var textoValor = coluna
                    .Item()
                    .Text(valor)
                    .FontSize(DANFSeConstantes.Fonte.Campo.TamanhoValor);
                if (valorEmNegrito)
                {
                    textoValor
                        .Bold();
                }
            });
    }
}