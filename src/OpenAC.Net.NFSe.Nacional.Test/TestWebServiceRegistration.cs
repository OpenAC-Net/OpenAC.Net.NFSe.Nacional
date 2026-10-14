using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Web.Policy;

namespace OpenAC.Net.NFSe.Nacional.Test;

public class TestWebServiceRegistration
{
    [Test]
    public async Task RegistroPadrao_CriaClienteEConfiguracaoPorEscopo()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IHostEnvironment>(new TestHostEnvironment());
        services.AddOpenNFSeNacionalWeb(config => config.WebServices.CodigoMunicipio = 5002704);
        await using var provider = services.BuildServiceProvider(validateScopes: true);

        await using var firstScope = provider.CreateAsyncScope();
        await using var secondScope = provider.CreateAsyncScope();

        var firstClient = firstScope.ServiceProvider.GetRequiredService<IOpenNFSeNacionalClient>();
        var httpClientFactory = firstScope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        var firstConfiguration = firstScope.ServiceProvider.GetRequiredService<ConfiguracaoNFSe>();
        var secondConfiguration = secondScope.ServiceProvider.GetRequiredService<ConfiguracaoNFSe>();

        await Assert.That(firstClient).IsNotNull();
        await Assert.That(httpClientFactory).IsNotNull();
        await Assert.That(firstConfiguration.WebServices.CodigoMunicipio).IsEqualTo(5002704);
        await Assert.That(ReferenceEquals(firstConfiguration, secondConfiguration)).IsFalse();
    }

    [Test]
    public async Task MatrizIdempotencia_NaoRepeteOperacoesDeEmissao()
    {
        await Assert.That(NFSeOperationPolicies.IsIdempotent(nameof(IOpenNFSeNacionalClient.ConsultaNsuAsync)))
            .IsTrue();
        await Assert.That(NFSeOperationPolicies.IsIdempotent(nameof(IOpenNFSeNacionalClient.EnviarAsync)))
            .IsFalse();
        await Assert.That(NFSeOperationPolicies.IsIdempotent(nameof(IOpenNFSeNacionalClient.EnviarEventoAsync)))
            .IsFalse();
    }

    private sealed class TestHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Development;
        public string ApplicationName { get; set; } = "OpenAC.Net.NFSe.Nacional.Test";
        public string ContentRootPath { get; set; } = Path.GetTempPath();
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
