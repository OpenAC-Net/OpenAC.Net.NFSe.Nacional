using OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Utilitarios;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Tests.Src;

public class FormatadorTests
{
    [Theory]
    [InlineData(1234.56, "R$ 1.234,56")]
    [InlineData(0, "R$ 0,00")]
    [InlineData(-10, "-R$ 10,00")]
    public void Deve_Formatar_Valor_Como_Moeda(decimal valor, string esperado)
    {
        Assert.Equal(esperado, Formatador.Moeda(valor));
    }

    [Fact]
    public void Deve_Retornar_Hifen_Quando_Moeda_For_Nula()
    {
        Assert.Equal("-", Formatador.Moeda(null));
    }

    [Theory]
    [InlineData(12.3456, "12,35%")]
    [InlineData(0, "0,00%")]
    [InlineData(100, "100,00%")]
    public void Deve_Formatar_Percentual(decimal valor, string esperado)
    {
        Assert.Equal(esperado, Formatador.Percentual(valor));
    }

    [Fact]
    public void Deve_Retornar_Hifen_Quando_Percentual_For_Nulo()
    {
        Assert.Equal("-", Formatador.Percentual(null));
    }

    [Fact]
    public void Deve_Formatar_Data()
    {
        var data = new DateTime(2025, 6, 15);
        Assert.Equal("15/06/2025", Formatador.Data(data));
    }

    [Fact]
    public void Deve_Retornar_Hifen_Quando_Data_For_Nula()
    {
        Assert.Equal("-", Formatador.Data(null));
    }

    [Fact]
    public void Deve_Formatar_Data_Hora()
    {
        var dataHora = new DateTimeOffset(2025, 6, 15, 14, 30, 45, TimeSpan.FromHours(-3));
        Assert.Equal("15/06/2025 14:30:45", Formatador.DataHora(dataHora));
    }

    [Fact]
    public void Deve_Retornar_Hifen_Quando_Data_Hora_For_Nula()
    {
        Assert.Equal("-", Formatador.DataHora(null));
    }

    [Theory]
    [InlineData("  texto  ", "texto")]
    [InlineData(null, "-")]
    [InlineData("   ", "-")]
    [InlineData("", "-")]
    public void Deve_Formatar_Texto(string? valor, string esperado)
    {
        Assert.Equal(esperado, Formatador.Texto(valor));
    }

    [Theory]
    [InlineData(new[] { "", "valor", null }, "valor")]
    [InlineData(new[] { "", null, "   " }, "-")]
    [InlineData(new string[] { }, "-")]
    public void Deve_Retornar_Primeiro_Texto_Nao_Vazio(string?[] valores, string esperado)
    {
        Assert.Equal(esperado, Formatador.Texto(valores));
    }

    [Theory]
    [InlineData(new[] { "a", "", "b" }, "a, b")]
    [InlineData(new string[] { }, "-")]
    [InlineData(new[] { "", "   ", null }, "-")]
    public void Deve_Concatenar_Textos(string?[] valores, string esperado)
    {
        Assert.Equal(esperado, Formatador.Concatenar(valores));
    }

    [Theory]
    [InlineData("abc123", "123")]
    [InlineData(null, "")]
    [InlineData("!@#", "")]
    public void Deve_Retornar_Somente_Numeros(string? valor, string esperado)
    {
        Assert.Equal(esperado, Formatador.SomenteNumeros(valor));
    }

    [Theory]
    [InlineData(123.456, "123,46")]
    [InlineData(123.4, "123,4")]
    [InlineData(123, "123")]
    public void Deve_Formatar_Decimal(decimal valor, string esperado)
    {
        Assert.Equal(esperado, Formatador.Decimal(valor));
    }

    [Fact]
    public void Deve_Retornar_Hifen_Quando_Decimal_For_Nulo()
    {
        Assert.Equal("-", Formatador.Decimal(null));
    }

    [Theory]
    [InlineData(1234, "1234")]
    [InlineData(0, "0")]
    public void Deve_Formatar_Inteiro(int valor, string esperado)
    {
        Assert.Equal(esperado, Formatador.Inteiro(valor));
    }

    [Fact]
    public void Deve_Retornar_Hifen_Quando_Inteiro_For_Nulo()
    {
        Assert.Equal("-", Formatador.Inteiro(null));
    }

    [Theory]
    [InlineData("12345678901", "123.456.789-01")]
    [InlineData("12345678000195", "12.345.678/0001-95")]
    [InlineData("999", "999")]
    [InlineData(null, "-")]
    public void Deve_Formatar_Documento(string? valor, string esperado)
    {
        Assert.Equal(esperado, Formatador.Documento(valor));
    }

    [Theory]
    [InlineData("12345678", "12.345-678")]
    [InlineData("123", "123")]
    [InlineData(null, "-")]
    public void Deve_Formatar_Cep(string? valor, string esperado)
    {
        Assert.Equal(esperado, Formatador.Cep(valor));
    }

    [Theory]
    [InlineData("1123456789", "(11) 2345-6789")]
    [InlineData("11987654321", "(11) 98765-4321")]
    [InlineData("123", "123")]
    [InlineData(null, "-")]
    public void Deve_Formatar_Telefone(string? valor, string esperado)
    {
        Assert.Equal(esperado, Formatador.Telefone(valor));
    }

    [Fact]
    public void Deve_Formatar_Competencia()
    {
        var data = new DateTime(2025, 6, 1);
        Assert.Equal("06/2025", Formatador.Competencia(data));
    }

    [Fact]
    public void Deve_Retornar_Hifen_Quando_Competencia_For_Nula()
    {
        Assert.Equal("-", Formatador.Competencia(null));
    }
}
