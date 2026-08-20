namespace OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Tests.Src.Comum;

/// <summary>
/// Helper para salvar artefatos gerados durante os testes (PDFs, etc.)
/// em uma pasta do projeto, permitindo inspeção visual após a execução.
/// </summary>
public static class ResultadosTesteHelper
{
    private const string NomeDiretorio = "Resultados";

    /// <summary>
    /// Retorna o caminho da pasta de resultados dentro do projeto de testes,
    /// criando-a se necessário.
    /// </summary>
    public static string ObterDiretorioResultados()
    {
        var diretorioProjeto = LocalizarDiretorioProjeto(AppContext.BaseDirectory);
        if (diretorioProjeto is null)
        {
            diretorioProjeto = Directory.GetCurrentDirectory();
        }

        var caminhoResultados = Path.Combine(diretorioProjeto, NomeDiretorio);
        if (!Directory.Exists(caminhoResultados))
        {
            Directory.CreateDirectory(caminhoResultados);
        }

        return caminhoResultados;
    }

    /// <summary>
    /// Salva os bytes de um PDF na pasta de resultados e retorna o caminho completo.
    /// </summary>
    public static string SalvarPdf(string nomeArquivo, byte[] pdf)
    {
        var diretorio = ObterDiretorioResultados();
        var caminho = Path.Combine(diretorio, nomeArquivo);
        File.WriteAllBytes(caminho, pdf);
        return caminho;
    }

    private static string? LocalizarDiretorioProjeto(string baseDir)
    {
        var diretorio = new DirectoryInfo(baseDir);
        string? ultimoEncontrado = null;
        while (diretorio is not null)
        {
            // Considera o diretório do projeto quando encontra o arquivo .csproj.
            if (diretorio.EnumerateFiles("*.csproj").Any())
            {
                ultimoEncontrado = diretorio.FullName;
            }
            diretorio = diretorio.Parent;
        }
        return ultimoEncontrado;
    }
}
