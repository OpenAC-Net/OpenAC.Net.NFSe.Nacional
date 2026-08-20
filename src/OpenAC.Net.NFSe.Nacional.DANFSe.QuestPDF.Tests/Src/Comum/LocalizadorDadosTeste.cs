namespace OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Tests.Src.Comum;

/// <summary>
/// Localiza os diretórios de dados de teste (arquivos base do projeto) e de variações
/// temporárias geradas durante a execução dos testes.
/// </summary>
public static class LocalizadorDadosTeste
{
    private const string NomeDiretorioDadosTeste = "DadosTeste";

    /// <summary>
    /// Retorna o diretório de dados de teste do projeto (onde o nota.xml base está).
    /// </summary>
    public static string? ObterDadosTeste()
    {
        var baseDir = AppContext.BaseDirectory;
        var diretorioProjeto = LocalizarDiretorioProjeto(baseDir);
        if (diretorioProjeto is null)
        {
            // Fallback para o diretório de trabalho atual.
            var caminhoAtual = Path.Combine(Directory.GetCurrentDirectory(), NomeDiretorioDadosTeste);
            return Directory.Exists(caminhoAtual) ? caminhoAtual : null;
        }
        return Path.Combine(diretorioProjeto, NomeDiretorioDadosTeste);
    }

    /// <summary>
    /// Retorna o diretório temporário onde as variações dos XMLs serão geradas.
    /// Os arquivos desta pasta podem ser descartados após os testes.
    /// </summary>
    public static string ObterDiretorioVariacoes()
    {
        var caminho = Path.Combine(Path.GetTempPath(), "OpenAC.NFSe.Nacional.QuestPDF.Tests", "Variacoes");
        if (!Directory.Exists(caminho))
        {
            Directory.CreateDirectory(caminho);
        }
        return caminho;
    }

    private static string? LocalizarDiretorioProjeto(string baseDir)
    {
        var diretorio = new DirectoryInfo(baseDir);
        string? ultimoEncontrado = null;
        while (diretorio is not null)
        {
            var caminhoDadosTeste = Path.Combine(diretorio.FullName, NomeDiretorioDadosTeste);
            if (Directory.Exists(caminhoDadosTeste))
            {
                ultimoEncontrado = diretorio.FullName;
            }
            diretorio = diretorio.Parent;
        }
        return ultimoEncontrado;
    }
}
