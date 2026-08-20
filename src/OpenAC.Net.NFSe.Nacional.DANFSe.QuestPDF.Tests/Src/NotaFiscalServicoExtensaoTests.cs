using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Configuracao;
using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Extensoes;
using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Tests.Src.Comum;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Tests.Src;

public class NotaFiscalServicoExtensaoTests : IDisposable
{
    private readonly string _caminhoPdfTemporario;

    public NotaFiscalServicoExtensaoTests()
    {
        _caminhoPdfTemporario = Path.Combine(
            Path.GetTempPath(),
            $"teste-{Guid.NewGuid()}.pdf");
    }

    public void Dispose()
    {
        if (File.Exists(_caminhoPdfTemporario))
        {
            File.Delete(_caminhoPdfTemporario);
        }
    }

    [Fact]
    public void Deve_Lancar_Excecao_Quando_Nfse_For_Nula()
    {
        NotaFiscalServico? nfse = null;
        var configuracao = new DANFSeConfiguracao();

#pragma warning disable CS8604 // Possível argumento de referência nula.
        var excecao = Assert.Throws<ArgumentNullException>(() => nfse.GerarPdf(configuracao));
#pragma warning restore CS8604
        Assert.Equal("nfse", excecao.ParamName);
    }

    [Fact]
    public void Deve_Lancar_Excecao_Quando_Configuracao_For_Nula()
    {
        var nfse = CarregadorNfse.Carregar("nota.xml");

        var excecao = Assert.Throws<ArgumentNullException>(() => nfse.GerarPdf(null!));
        Assert.Equal("configuracoes", excecao.ParamName);
    }

    [Fact]
    public void Deve_Lancar_Excecao_Quando_Caminho_Do_Arquivo_For_Invalido()
    {
        var nfse = CarregadorNfse.Carregar("nota.xml");
        var configuracao = new DANFSeConfiguracao();
        var excecao = Assert.Throws<ArgumentException>(() => nfse.GerarPdf(configuracao, "   "));
        Assert.Equal("nomeArquivoComDiretorio", excecao.ParamName);
    }

    [Fact]
    public void Deve_Gerar_Pdf_Em_Memoria_Sem_Seguranca()
    {
        var nfse = CarregadorNfse.Carregar("nota.xml");
        var configuracao = new DANFSeConfiguracao();
        var pdf = nfse.GerarPdf(configuracao);
        Assert.NotNull(pdf);
        Assert.NotEmpty(pdf);
        Assert.True(pdf.Length > 100);
    }

    [Fact]
    public void Deve_Gerar_Pdf_Criptografado_Em_Memoria()
    {
        var nfse = CarregadorNfse.Carregar("nota.xml");
        var configuracao = new DANFSeConfiguracao
        {
            Seguranca = new DANFSeConfig
            {
                SenhaUsuario = "123456",
                SenhaProprietario = "admin"
            }
        };
        var pdf = nfse.GerarPdf(configuracao);
        Assert.NotNull(pdf);
        Assert.NotEmpty(pdf);
    }

    [Fact]
    public void Deve_Gerar_Pdf_E_Salvar_Em_Arquivo()
    {
        var nfse = CarregadorNfse.Carregar("nota.xml");
        var configuracao = new DANFSeConfiguracao();
        nfse.GerarPdf(configuracao, _caminhoPdfTemporario);
        Assert.True(File.Exists(_caminhoPdfTemporario));
        var info = new FileInfo(_caminhoPdfTemporario);
        Assert.True(info.Length > 0);
    }
}
