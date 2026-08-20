using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Configuracao;
using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Extensoes;
using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Tests.Src.Comum;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Tests.Src;

public class NfsePdfTests
{
    private readonly ITestOutputHelper _output;

    public NfsePdfTests(ITestOutputHelper output)
    {
        _output = output;
    }    

    [Theory]
    [InlineData("nota.xml", true, true)]
    [InlineData("nota.xml", false, false)]
    public void Deve_Gerar_Pdf_A_Partir_Do_Xml(string nomeArquivo, bool comCanhoto, bool comQrCode)
    {
        var nfse = CarregadorNfse.Carregar(nomeArquivo);
        var configuracao = CriarConfiguracao(comCanhoto, comQrCode);
        var pdf = nfse.GerarPdf(configuracao);

        Assert.NotNull(pdf);
        Assert.True(pdf.Length > 0, "O PDF gerado está vazio.");

        var sufixo = $"{Path.GetFileNameWithoutExtension(nomeArquivo)}_canhoto_{comCanhoto}_qrcode_{comQrCode}";
        var caminho = SalvarPdf(sufixo, pdf);
        _output.WriteLine($"PDF gerado: {caminho}");
        _output.WriteLine($"Tamanho: {pdf.Length} bytes");
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void Deve_Gerar_Pdf_Com_Variacoes_De_Logo(bool comLogoNacional, bool comLogoPrestador)
    {
        var nfse = CarregadorNfse.Carregar("nota.xml");
        var configuracao = new DANFSeConfiguracao();
        if (comLogoNacional)
        {
            configuracao.LogoNacional = ObterLogo();
        }
        if (comLogoPrestador)
        {
            configuracao.LogoPrestador = ObterLogo();
        }
        var pdf = nfse.GerarPdf(configuracao);

        Assert.NotNull(pdf);
        Assert.True(pdf.Length > 100);

        var sufixo = $"nota_logo_nacional_{comLogoNacional}_prestador_{comLogoPrestador}";
        SalvarPdf(sufixo, pdf);
    }

    [Theory]
    [InlineData("nota.xml", true)]
    [InlineData("sem_tomador.xml", false)]
    public void Deve_Gerar_Pdf_Com_E_Sem_Tomador(string nomeArquivo, bool temTomador)
    {
        var nfse = CarregadorNfse.Carregar(nomeArquivo);
        var pdf = nfse.GerarPdf(new DANFSeConfiguracao());

        Assert.NotNull(pdf);
        Assert.True(pdf.Length > 100);

        var sufixo = $"{Path.GetFileNameWithoutExtension(nomeArquivo)}_tomador_{temTomador}";
        SalvarPdf(sufixo, pdf);

        if (temTomador)
        {
            Assert.False(string.IsNullOrWhiteSpace(nfse.Informacoes.Dps.Informacoes.Tomador?.CNPJ));
        }
        else
        {
            Assert.True(string.IsNullOrWhiteSpace(nfse.Informacoes.Dps.Informacoes.Tomador?.CNPJ));
        }
    }

    [Theory]
    [InlineData("nota.xml", false)]
    [InlineData("com_destinatario.xml", true)]
    public void Deve_Gerar_Pdf_Com_E_Sem_Destinatario(string nomeArquivo, bool temDestinatario)
    {
        var nfse = CarregadorNfse.Carregar(nomeArquivo);
        var pdf = nfse.GerarPdf(new DANFSeConfiguracao());

        Assert.NotNull(pdf);
        Assert.True(pdf.Length > 100);

        var sufixo = $"{Path.GetFileNameWithoutExtension(nomeArquivo)}_destinatario_{temDestinatario}";
        SalvarPdf(sufixo, pdf);

        if (temDestinatario)
        {
            Assert.NotNull(nfse.Informacoes.Dps.Informacoes.IBSCBS?.Destinatario);
        }
    }

    [Theory]
    [InlineData("nota.xml", false)]
    [InlineData("com_intermediario.xml", true)]
    public void Deve_Gerar_Pdf_Com_E_Sem_Intermediario(string nomeArquivo, bool temIntermediario)
    {
        var nfse = CarregadorNfse.Carregar(nomeArquivo);
        var pdf = nfse.GerarPdf(new DANFSeConfiguracao());

        Assert.NotNull(pdf);
        Assert.True(pdf.Length > 100);

        var sufixo = $"{Path.GetFileNameWithoutExtension(nomeArquivo)}_intermediario_{temIntermediario}";
        SalvarPdf(sufixo, pdf);

        if (temIntermediario)
        {
            Assert.NotNull(nfse.Informacoes.Dps.Informacoes.Intermediario);
        }
    }

    [Theory]
    [InlineData("nota.xml", true)]
    [InlineData("sem_complemento.xml", false)]
    public void Deve_Gerar_Pdf_Com_E_Sem_Complemento(string nomeArquivo, bool temComplemento)
    {
        var nfse = CarregadorNfse.Carregar(nomeArquivo);
        var pdf = nfse.GerarPdf(new DANFSeConfiguracao());

        Assert.NotNull(pdf);
        Assert.True(pdf.Length > 100);

        var sufixo = $"{Path.GetFileNameWithoutExtension(nomeArquivo)}_complemento_{temComplemento}";
        SalvarPdf(sufixo, pdf);

        if (temComplemento)
        {
            Assert.False(string.IsNullOrWhiteSpace(nfse.Informacoes.Dps.Informacoes.Servico.InformacoesComplementares?.Informacoes));
        }
        else
        {
            Assert.True(string.IsNullOrWhiteSpace(nfse.Informacoes.Dps.Informacoes.Servico.InformacoesComplementares?.Informacoes));
        }
    }

    [Fact]
    public void Deve_Gerar_Pdf_Com_Homologacao()
    {
        var nfse = CarregadorNfse.Carregar("nota.xml");
        var configuracao = new DANFSeConfiguracao
        {
            Homologacao = true
        };
        var pdf = nfse.GerarPdf(configuracao);

        Assert.NotNull(pdf);
        Assert.True(pdf.Length > 100);

        SalvarPdf("nota_homologacao", pdf);
    }

    [Fact]
    public void Deve_Gerar_Pdf_Sem_Ibs_Cbs()
    {
        var nfse = CarregadorNfse.Carregar("sem_ibs_cbs.xml");
        var pdf = nfse.GerarPdf(new DANFSeConfiguracao());

        Assert.NotNull(pdf);
        Assert.True(pdf.Length > 100);

        SalvarPdf("sem_ibs_cbs", pdf);
    }

    private static DANFSeConfiguracao CriarConfiguracao(bool comCanhoto, bool comQrCode)
    {
        var configuracao = new DANFSeConfiguracao
        {
            Homologacao = false,
            ExibirCanhoto = comCanhoto,
            ExibirQRCode = comQrCode
        };
        var logo = ObterLogo();
        if (logo is not null)
        {
            configuracao.LogoNacional = logo;
        }
        return configuracao;
    }

    private static byte[]? ObterLogo()
    {
        const string caminhoLogo = "Assets/logo-nacional.png";
        return File.Exists(caminhoLogo) ? File.ReadAllBytes(caminhoLogo) : null;
    }

    private static string SalvarPdf(string nome, byte[] pdf)
    {
        var nomeArquivo = $"{nome}.pdf";
        return ResultadosTesteHelper.SalvarPdf(nomeArquivo, pdf);
    }
}