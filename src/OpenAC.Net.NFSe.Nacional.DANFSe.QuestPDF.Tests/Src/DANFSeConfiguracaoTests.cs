using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Configuracao;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Tests.Src;

public class DANFSeConfiguracaoTests : IDisposable
{
    private readonly string _caminhoLogoTemporario;

    public DANFSeConfiguracaoTests()
    {
        _caminhoLogoTemporario = Path.Combine(Path.GetTempPath(), $"logo-{Guid.NewGuid()}.png");
        File.WriteAllBytes(_caminhoLogoTemporario, [0x89, 0x50, 0x4E, 0x47]);
    }

    public void Dispose()
    {
        if (File.Exists(_caminhoLogoTemporario))
        {
            File.Delete(_caminhoLogoTemporario);
        }
    }

    [Fact]
    public void Deve_Ter_Valores_Padrao_Corretos()
    {
        var configuracao = new DANFSeConfiguracao();
        Assert.Null(configuracao.LogoNacional);
        Assert.Null(configuracao.LogoPrestador);
        Assert.Empty(configuracao.CabecalhoMunicipio);
        Assert.False(configuracao.ExibirCanhoto);
        Assert.True(configuracao.ExibirQRCode);
        Assert.False(configuracao.Homologacao);
        Assert.False(configuracao.Cancelada);
        Assert.False(configuracao.Substituida);
        Assert.Equal(3.0, configuracao.MargemVerticalMm);
        Assert.Equal(3.0, configuracao.MargemHorizontalMm);
        Assert.NotNull(configuracao.Seguranca);
    }

    [Fact]
    public void Deve_Carregar_Logo_Prestador_Pelo_Caminho_Quando_Arquivo_Existir()
    {
        var configuracao = new DANFSeConfiguracao
        {
            LogoPrestadorPath = _caminhoLogoTemporario
        };
        Assert.NotNull(configuracao.LogoPrestador);
        Assert.Equal(4, configuracao.LogoPrestador.Length);
    }

    [Fact]
    public void Nao_Deve_Alterar_Logo_Prestador_Quando_Caminho_Nao_Existir()
    {
        var configuracao = new DANFSeConfiguracao
        {
            LogoPrestadorPath = Path.Combine(Path.GetTempPath(), $"logo-inexistente-{Guid.NewGuid()}.png")
        };
        Assert.Null(configuracao.LogoPrestador);
    }

    [Fact]
    public void Deve_Ignorar_Caminho_Vazio_Para_Logo_Prestador()
    {
        var configuracao = new DANFSeConfiguracao
        {
            LogoPrestadorPath = string.Empty
        };
        Assert.Null(configuracao.LogoPrestador);
    }
}
