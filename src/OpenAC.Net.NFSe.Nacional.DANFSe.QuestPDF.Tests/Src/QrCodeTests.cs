using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Utilitarios;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Tests.Src;

public class QrCodeTests
{
    [Fact]
    public void Deve_Gerar_Qr_Code_Com_Conteudo_Valido()
    {
        var imagem = QrCode.Gerar("https://www.nfse.gov.br/ConsultaPublica/?chave=123");
        Assert.NotNull(imagem);
        Assert.NotEmpty(imagem);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Deve_Lancar_Excecao_Quando_Conteudo_For_Vazio(string? conteudo)
    {
        var excecao = Assert.Throws<ArgumentException>(() => QrCode.Gerar(conteudo!));
        Assert.Equal("conteudo", excecao.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Deve_Lancar_Excecao_Quando_Tamanho_Modulo_For_Invalido(int tamanho)
    {
        var excecao = Assert.Throws<ArgumentOutOfRangeException>(() => QrCode.Gerar("conteudo", tamanho));
        Assert.Equal("tamanhoModulo", excecao.ParamName);
    }
}
