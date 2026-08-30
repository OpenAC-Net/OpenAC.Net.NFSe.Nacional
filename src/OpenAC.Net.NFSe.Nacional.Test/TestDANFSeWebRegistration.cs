using Microsoft.Extensions.DependencyInjection;
using OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp.Web.Configuration;
using OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp.Web.Service;
using OpenAC.Net.NFSe.Nacional.Web.Provider;

namespace OpenAC.Net.NFSe.Nacional.Test;

public sealed class TestDANFSeWebRegistration
{
    [Test]
    public async Task DeveRegistrarServicoEAplicarConfiguracoes()
    {
        var services = new ServiceCollection();
        services.AddOpenDANFSeNacionalWeb(options =>
        {
            options.ForcarDownload = true;
            options.NomeArquivoLote = "Notas.pdf";
            options.Configuracoes.ExibirCanhoto = true;
        });

        await using var provider = services.BuildServiceProvider();
        var primeiro = provider.GetRequiredService<IOpenDANFSeNacionalWeb>();
        var segundo = provider.GetRequiredService<IOpenDANFSeNacionalWeb>();
        var configurationProvider = provider
            .GetRequiredService<INFSeConfigurationProvider<DANFSeNacionalWebOptions>>();
        var options = await configurationProvider.GetConfigurationAsync();

        await Assert.That(primeiro).IsSameReferenceAs(segundo);
        await Assert.That(options.ForcarDownload).IsTrue();
        await Assert.That(options.NomeArquivoLote).IsEqualTo("Notas.pdf");
        await Assert.That(options.Configuracoes.ExibirCanhoto).IsTrue();
    }
}
