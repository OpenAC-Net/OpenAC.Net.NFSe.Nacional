using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Web;
using OpenAC.Net.NFSe.Nacional.Web.Client;
using OpenAC.Net.NFSe.Nacional.Web.Provider;

namespace OpenAC.Net.NFSe.Nacional.Test;

public class TestMultiTenantWeb
{
    [Test]
    public async Task Factory_Concorrente_MantemConfiguracaoECertificadoPorTenant()
    {
        FakeCertificateProvider.Calls.Clear();
        using var directory = new TemporaryDirectory();
        var services = new ServiceCollection();
        services.AddLogging();
        var httpClients = new RecordingHttpClientFactory();
        services.AddSingleton<IHttpClientFactory>(httpClients);
        services.AddSingleton<IHostEnvironment>(new TestHostEnvironment { ContentRootPath = directory.Path });
        services.AddOpenNFSeNacionalWebMultiTenant<FakeConfigurationProvider, FakeCertificateProvider>(
            options => options.PersistenciaHabilitada = true);
        await using var serviceProvider = services.BuildServiceProvider(validateScopes: true);
        var factory = serviceProvider.GetRequiredService<IOpenNFSeNacionalClientFactory>();

        var tenantIds = Enumerable.Range(0, 20).Select(index => $"empresa-{index % 2}").ToArray();
        var clients = await Task.WhenAll(tenantIds.Select(async tenant =>
            await factory.CreateAsync(tenant)));

        await Assert.That(clients.Distinct().Count()).IsEqualTo(20);
        var results = await Task.WhenAll(clients.Select(client => client.ConsultaExisteDpsAsync("DPS123")));
        await Assert.That(results.All(result => result)).IsTrue();
        // O mesmo certificado em duas empresas deve produzir dois clientes HTTP nomeados distintos.
        await Assert.That(httpClients.Names.Distinct().Count()).IsEqualTo(2);
        await Assert.That(httpClients.Names.Count).IsEqualTo(20);
        var handlers = serviceProvider.GetRequiredService<IHttpMessageHandlerFactory>();
        var cookieContainers = new List<System.Net.CookieContainer>();
        foreach (var name in httpClients.Names.Distinct())
        {
            var handler = handlers.CreateHandler(name);
            while (handler is DelegatingHandler delegating) handler = delegating.InnerHandler!;
            await Assert.That(handler is SocketsHttpHandler).IsTrue();
            var sockets = (SocketsHttpHandler)handler;
            await Assert.That(sockets.UseCookies).IsTrue();
            cookieContainers.Add(sockets.CookieContainer);
            await Assert.That(sockets.SslOptions.ClientCertificates!.Count).IsEqualTo(1);
            await Assert.That(((X509Certificate2)sockets.SslOptions.ClientCertificates[0]).HasPrivateKey).IsTrue();
            await Assert.That(((System.Net.WebProxy)sockets.Proxy!).Address!.Host).IsEqualTo("proxy.test");
        }
        await Assert.That(cookieContainers.Distinct().Count()).IsEqualTo(2);
        foreach (var tenant in new[] { "empresa-0", "empresa-1" })
        {
            var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(tenant)));
            var tenantDirectory = Path.Combine(directory.Path, "App_Data/NFSe", "Tenants", hash, "Technical");
            await Assert.That(Directory.GetFiles(tenantDirectory, "*.json").Length).IsEqualTo(10);
        }
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
            configuration.WebServices.Tentativas = 1;
            configuration.WebServices.Proxy = "http://proxy.test:8080";
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

    private sealed class RecordingHttpClientFactory : IHttpClientFactory
    {
        internal ConcurrentBag<string> Names { get; } = new();
        public HttpClient CreateClient(string name)
        {
            Names.Add(name);
            return new HttpClient(new SuccessHandler());
        }

        private sealed class SuccessHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken) =>
                Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(string.Empty)
                });
        }
    }

    private sealed class TemporaryDirectory : IDisposable
    {
        internal string Path { get; } = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"nfse-tenant-{Guid.NewGuid():N}");
        public void Dispose() { if (Directory.Exists(Path)) Directory.Delete(Path, true); }
    }

    private sealed class TestHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Development;
        public string ApplicationName { get; set; } = "OpenAC.Net.NFSe.Nacional.Test";
        public string ContentRootPath { get; set; } = Path.GetTempPath();
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
