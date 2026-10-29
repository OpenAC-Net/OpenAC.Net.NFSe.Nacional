using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.DFe.Core.Storage;

namespace OpenAC.Net.NFSe.Nacional.Test;

public class TestDocumentStore
{
    [Test]
    public async Task EscritasConcorrentes_NaoSobrescrevemDocumentos()
    {
        var root = Path.Combine(Path.GetTempPath(), $"openac-nfse-{Guid.NewGuid():N}");
        try
        {
            var configuration = new ConfiguracaoNFSe
            {
                Arquivos =
                {
                    PathEnvio = root
                }
            };
            var document = new DFeDocument(DFeTipoDocumento.SolicitacaoTecnica, "payload", "envio.json",
                null, DateTime.Today);

            await Task.WhenAll(Enumerable.Range(0, 20).Select(_ =>
                configuration.DocumentStore.SaveAsync(document, configuration.Arquivos)));

            await Assert.That(Directory.GetFiles(root, "envio*.json", SearchOption.AllDirectories).Length)
                .IsEqualTo(20);
        }
        finally
        {
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }
    [Test]
    [Arguments(DFeTipoDocumento.Solicitacao, "DPS")]
    [Arguments(DFeTipoDocumento.DocumentoFiscal, "NFSe")]
    [Arguments(DFeTipoDocumento.SolicitacaoTecnica, "Technical")]
    [Arguments(DFeTipoDocumento.RespostaTecnica, "Technical")]
    public async Task DiretoriosDesktop_PreservamDestinoPorCategoria(DFeTipoDocumento type, string category)
    {
        var root = Path.Combine(Path.GetTempPath(), $"openac-path-{Guid.NewGuid():N}");
        try
        {
            var configuration = new ConfiguracaoNFSe();
            configuration.Arquivos.PathDps = Path.Combine(root, "DPS");
            configuration.Arquivos.PathNFSe = Path.Combine(root, "NFSe");
            configuration.Arquivos.PathEnvio = Path.Combine(root, "Technical");
            var document = new DFeDocument(type, "conteúdo", "evento.xml", null, DateTime.Today);
            await configuration.DocumentStore.SaveAsync(document, configuration.Arquivos);
            await Assert.That(File.Exists(Path.Combine(root, category, "evento.xml"))).IsTrue();
        }
        finally
        {
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }

    [Test]
    public async Task StoreWeb_PreservaDestinosECaminhosDaConfiguracao()
    {
        var root = Path.Combine(Path.GetTempPath(), $"openac-web-store-{Guid.NewGuid():N}");
        try
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddSingleton<IHostEnvironment>(new TestHostEnvironment(root));
            services.AddOpenNFSeNacionalWeb(_ => { }, options => options.PersistenciaHabilitada = true);
            await using var provider = services.BuildServiceProvider(validateScopes: true);
            var store = provider.GetRequiredService<IDFeDocumentStore>();
            var tenants = provider.GetRequiredService<OpenAC.Net.NFSe.Nacional.Web.Accessor.INFSeTenantContextAccessor>();
            await using var firstScope = provider.CreateAsyncScope();
            await using var secondScope = provider.CreateAsyncScope();
            var configurations = new[]
            {
                firstScope.ServiceProvider.GetRequiredService<ConfiguracaoNFSe>(),
                secondScope.ServiceProvider.GetRequiredService<ConfiguracaoNFSe>()
            };
            // As configurações padrão têm a mesma empresa; ainda assim, sua identidade de arquivos deve estar vinculada.
            foreach (var configuration in configurations)
            {
                await Assert.That(tenants.GetTenant(configuration.Arquivos)).IsEqualTo(tenants.GetTenant(configuration));
                var originalPaths = (configuration.Arquivos.PathDps, configuration.Arquivos.PathNFSe, configuration.Arquivos.PathEnvio);
                await Task.WhenAll(new[] { DFeTipoDocumento.Solicitacao, DFeTipoDocumento.DocumentoFiscal,
                    DFeTipoDocumento.RespostaTecnica }.Select(type => store.SaveAsync(
                        new DFeDocument(type, "conteúdo", "evento.xml", null, DateTime.Today), configuration.Arquivos)));
                await Assert.That((configuration.Arquivos.PathDps, configuration.Arquivos.PathNFSe, configuration.Arquivos.PathEnvio))
                    .IsEqualTo(originalPaths);
            }
            var tenantHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(tenants.GetTenant(configurations[0]))));
            var tenantRoot = Path.Combine(root, "App_Data/NFSe", "Tenants", tenantHash);
            foreach (var category in new[] { "Fiscal/DPS", "Fiscal/NFSe", "Technical" })
                await Assert.That(Directory.GetFiles(Path.Combine(tenantRoot, category), "evento*.xml").Length).IsEqualTo(2);
        }
        finally
        {
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }

    private sealed class TestHostEnvironment(string root) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Development;
        public string ApplicationName { get; set; } = "OpenAC.Net.NFSe.Nacional.Test";
        public string ContentRootPath { get; set; } = root;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
