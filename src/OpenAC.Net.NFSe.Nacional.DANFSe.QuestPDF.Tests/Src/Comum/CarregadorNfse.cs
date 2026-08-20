using System.Text;
using OpenAC.Net.NFSe.Nacional.Common.Model;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Tests.Src.Comum;

public static class CarregadorNfse
{
    private static readonly string DiretorioVariacoes;

    static CarregadorNfse()
    {
        DiretorioVariacoes = GeradorXmlNfse.GerarVariacoes();
    }

    public static NotaFiscalServico Carregar(string nomeArquivo)
    {
        var caminho = LocalizarArquivo(nomeArquivo);
        if (caminho is null)
        {
            throw new FileNotFoundException(
                $"Arquivo de fixture não encontrado: {nomeArquivo}",
                nomeArquivo);
        }
        var xml = File.ReadAllText(caminho, Encoding.UTF8);
        return NotaFiscalServico.Load(xml, Encoding.UTF8);
    }

    private static string? LocalizarArquivo(string nomeArquivo)
    {
        var caminhoVariacoes = Path.Combine(DiretorioVariacoes, nomeArquivo);
        if (File.Exists(caminhoVariacoes))
        {
            return caminhoVariacoes;
        }
        var diretorioDadosTeste = LocalizadorDadosTeste.ObterDadosTeste();
        if (diretorioDadosTeste is not null)
        {
            var caminhoDadosTeste = Path.Combine(diretorioDadosTeste, nomeArquivo);
            if (File.Exists(caminhoDadosTeste))
            {
                return caminhoDadosTeste;
            }
        }
        return null;
    }
}
