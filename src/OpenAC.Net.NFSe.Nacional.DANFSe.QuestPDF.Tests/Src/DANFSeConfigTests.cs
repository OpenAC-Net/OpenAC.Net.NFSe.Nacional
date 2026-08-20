using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Configuracao;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Tests.Src;

public class DANFSeConfigTests
{
    [Fact]
    public void Deve_Ter_Valores_Padrao_Corretos()
    {
        var seguranca = new DANFSeConfig();
        Assert.True(seguranca.PermitirImpressao);
        Assert.False(seguranca.PermitirModificacao);
        Assert.True(seguranca.PermitirCopiarConteudo);
        Assert.True(seguranca.PermitirAnotacoes);
        Assert.True(seguranca.PermitirPreenchimentoFormularios);
        Assert.False(seguranca.PermitirMontarDocumento);
    }

    [Fact]
    public void Deve_Indicar_Criptografia_Quando_Tiver_Senha_Usuario()
    {
        var seguranca = new DANFSeConfig
        {
            SenhaUsuario = "123456"
        };
        Assert.True(seguranca.TemCriptografia);
    }

    [Fact]
    public void Deve_Indicar_Criptografia_Quando_Tiver_Senha_Proprietario()
    {
        var seguranca = new DANFSeConfig
        {
            SenhaProprietario = "admin"
        };
        Assert.True(seguranca.TemCriptografia);
    }

    [Fact]
    public void Nao_Deve_Indicar_Criptografia_Quando_Nao_Tiver_Senha()
    {
        var seguranca = new DANFSeConfig();
        Assert.False(seguranca.TemCriptografia);
    }
}
