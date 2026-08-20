using System.Xml.Linq;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Tests.Src.Comum;

public static class GeradorXmlNfse
{
    private const string NomeArquivoBase = "nota.xml";

    private static readonly XNamespace NsNfse = "http://www.sped.fazenda.gov.br/nfse";

    public static string GerarVariacoes()
    {
        var diretorioDadosTeste = LocalizadorDadosTeste.ObterDadosTeste()
            ?? throw new DirectoryNotFoundException("Diretório de dados de teste não encontrado.");
        var diretorioVariacoes = LocalizadorDadosTeste.ObterDiretorioVariacoes();
        LimparVariacoes(diretorioVariacoes);
        var documentoBase = CarregarDocumentoBase(diretorioDadosTeste);
        GerarSemTomador(documentoBase, diretorioVariacoes);
        GerarSemComplemento(documentoBase, diretorioVariacoes);
        GerarSemIbsCbs(documentoBase, diretorioVariacoes);
        GerarComDestinatario(documentoBase, diretorioVariacoes);
        GerarComIntermediario(documentoBase, diretorioVariacoes);
        return diretorioVariacoes;
    }

    private static void LimparVariacoes(string diretorioVariacoes)
    {
        if (!Directory.Exists(diretorioVariacoes))
        {
            Directory.CreateDirectory(diretorioVariacoes);
            return;
        }
        foreach (var arquivo in Directory.EnumerateFiles(diretorioVariacoes, "*.xml"))
        {
            File.Delete(arquivo);
        }
    }

    private static XDocument CarregarDocumentoBase(string diretorioDadosTeste)
    {
        var caminhoBase = Path.Combine(diretorioDadosTeste, NomeArquivoBase);
        return XDocument.Load(caminhoBase);
    }

    private static void GerarSemTomador(XDocument documentoBase, string diretorioDadosTeste)
    {
        var documento = new XDocument(documentoBase);
        var toma = documento.Descendants(NsNfse + "toma").FirstOrDefault();
        toma?.Remove();
        documento.Save(Path.Combine(diretorioDadosTeste, "sem_tomador.xml"));
    }

    private static void GerarSemComplemento(XDocument documentoBase, string diretorioDadosTeste)
    {
        var documento = new XDocument(documentoBase);
        var infoCompl = documento.Descendants(NsNfse + "infoCompl").FirstOrDefault();
        infoCompl?.Remove();
        documento.Save(Path.Combine(diretorioDadosTeste, "sem_complemento.xml"));
    }

    private static void GerarSemIbsCbs(XDocument documentoBase, string diretorioDadosTeste)
    {
        var documento = new XDocument(documentoBase);
        var ibsCbs = documento.Root?
            .Element(NsNfse + "infNFSe")?
            .Element(NsNfse + "IBSCBS");
        ibsCbs?.Remove();
        documento.Save(Path.Combine(diretorioDadosTeste, "sem_ibs_cbs.xml"));
    }

    private static void GerarComDestinatario(XDocument documentoBase, string diretorioDadosTeste)
    {
        var documento = new XDocument(documentoBase);
        var ibsCbs = documento.Root?
            .Element(NsNfse + "infNFSe")?
            .Element(NsNfse + "DPS")?
            .Element(NsNfse + "infDPS")?
            .Element(NsNfse + "IBSCBS");
        if (ibsCbs is null)
        {
            return;
        }
        var dest = new XElement(NsNfse + "dest",
            new XElement(NsNfse + "CNPJ", "12345678000195"),
            new XElement(NsNfse + "xNome", "DESTINATARIO EXEMPLO LTDA"),
            new XElement(NsNfse + "end",
                new XElement(NsNfse + "endNac",
                    new XElement(NsNfse + "cMun", "4106902"),
                    new XElement(NsNfse + "CEP", "80010010")),
                new XElement(NsNfse + "xLgr", "RUA EXEMPLO"),
                new XElement(NsNfse + "nro", "100"),
                new XElement(NsNfse + "xBairro", "CENTRO")),
            new XElement(NsNfse + "email", "destinatario@exemplo.com.br"));
        ibsCbs.Add(dest);
        documento.Save(Path.Combine(diretorioDadosTeste, "com_destinatario.xml"));
    }

    private static void GerarComIntermediario(XDocument documentoBase, string diretorioDadosTeste)
    {
        var documento = new XDocument(documentoBase);
        var infDps = documento.Root?
            .Element(NsNfse + "infNFSe")?
            .Element(NsNfse + "DPS")?
            .Element(NsNfse + "infDPS");
        if (infDps is null)
        {
            return;
        }
        var intermed = new XElement(NsNfse + "interm",
            new XElement(NsNfse + "CNPJ", "98765432000198"),
            new XElement(NsNfse + "xNome", "INTERMEDIARIO EXEMPLO LTDA"),
            new XElement(NsNfse + "end",
                new XElement(NsNfse + "endNac",
                    new XElement(NsNfse + "cMun", "4106902"),
                    new XElement(NsNfse + "CEP", "80020020")),
                new XElement(NsNfse + "xLgr", "AVENIDA INTERMEDIARIA"),
                new XElement(NsNfse + "nro", "200"),
                new XElement(NsNfse + "xBairro", "PORTAO")),
            new XElement(NsNfse + "email", "intermediario@exemplo.com.br"));
        infDps.Add(intermed);
        documento.Save(Path.Combine(diretorioDadosTeste, "com_intermediario.xml"));
    }
}
