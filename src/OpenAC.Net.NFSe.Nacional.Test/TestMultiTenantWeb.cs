using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Web;

namespace OpenAC.Net.NFSe.Nacional.Test;

public class TestMultiTenantWeb
{
    [Test]
    public async Task Factory_Concorrente_MantemConfiguracaoECertificadoPorTenant()
    {
        FakeCertificateProvider.Calls.Clear();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IHostEnvironment>(new TestHostEnvironment());
        services.AddOpenNFSeNacionalWebMultiTenant<FakeConfigurationProvider, FakeCertificateProvider>();
        await using var serviceProvider = services.BuildServiceProvider(validateScopes: true);
        var factory = serviceProvider.GetRequiredService<IOpenNFSeNacionalClientFactory>();

        var tenantIds = Enumerable.Range(0, 20).Select(index => $"empresa-{index % 2}").ToArray();
        var clients = await Task.WhenAll(tenantIds.Select(async tenant =>
            await factory.CreateAsync(tenant)));

        await Assert.That(clients.Distinct().Count()).IsEqualTo(20);
        await Assert.That(FakeCertificateProvider.Calls["empresa-0"]).IsEqualTo(10);
        await Assert.That(FakeCertificateProvider.Calls["empresa-1"]).IsEqualTo(10);
    }

    private sealed class FakeConfigurationProvider : INFSeConfigurationProvider<ConfiguracaoNFSe>
    {
        public ValueTask<ConfiguracaoNFSe> GetConfigurationAsync(string tenantId,
            CancellationToken cancellationToken = default)
        {
            var configuration = new ConfiguracaoNFSe();
            configuration.WebServices.CodigoMunicipio = tenantId == "empresa-0" ? 5002704 : 3550308;
            return ValueTask.FromResult(configuration);
        }
    }

    private sealed class FakeCertificateProvider : INFSeCertificateProvider
    {
        private const string Password = "openac-test";
        private static readonly byte[] Certificate = CreateCertificate();
        internal static readonly ConcurrentDictionary<string, int> Calls = new();

        public ValueTask ConfigureAsync(string tenantId, NFSeCertificadoConfig certificateConfiguration,
            CancellationToken cancellationToken = default)
        {
            Calls.AddOrUpdate(tenantId, 1, (_, count) => count + 1);
            certificateConfiguration.CertificadoBytes = Certificate;
            certificateConfiguration.Senha = Password;
            return ValueTask.CompletedTask;
        }

        private static byte[] CreateCertificate()
        {
            using var rsa = RSA.Create(2048);
            var request = new CertificateRequest("CN=OpenAC Test", rsa, HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);
            using var certificate = request.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1),
                DateTimeOffset.Now.AddDays(30));
            return certificate.Export(X509ContentType.Pfx, Password);
        }
    }

    private sealed class TestHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Development;
        public string ApplicationName { get; set; } = "OpenAC.Net.NFSe.Nacional.Test";
        public string ContentRootPath { get; set; } = Path.GetTempPath();
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
