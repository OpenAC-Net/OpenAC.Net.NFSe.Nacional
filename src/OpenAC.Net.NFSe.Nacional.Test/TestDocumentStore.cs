using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.DFe.Core.Storage;
using OpenAC.Net.NFSe.Nacional.Storage;

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
            var document = new NFSeDocument(configuration.Arquivos, DFeCategorias.SolicitacaoTecnica, "payload",
                "envio.json",
                null, DateTime.Today);

            await Task.WhenAll(Enumerable.Range(0, 20).Select(_ =>
                configuration.DocumentStore.SaveAsync(document)));

            await Assert.That(Directory.GetFiles(root, "envio*.json", SearchOption.AllDirectories).Length)
                .IsEqualTo(20);
        }
        finally
        {
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }

    [Test]
    [Arguments(NFSeCategorias.Dps, "DPS")]
    [Arguments(NFSeCategorias.NFSe, "NFSe")]
    [Arguments(DFeCategorias.SolicitacaoTecnica, "Technical")]
    [Arguments(DFeCategorias.RespostaTecnica, "Technical")]
    public async Task DiretoriosDesktop_PreservamDestinoPorCategoria(string type, string category)
    {
        var root = Path.Combine(Path.GetTempPath(), $"openac-path-{Guid.NewGuid():N}");
        try
        {
            var configuration = new ConfiguracaoNFSe();
            configuration.Arquivos.PathDps = Path.Combine(root, "DPS");
            configuration.Arquivos.PathNFSe = Path.Combine(root, "NFSe");
            configuration.Arquivos.PathEnvio = Path.Combine(root, "Technical");
            var document = new NFSeDocument(configuration.Arquivos, type, "conteúdo", "evento.xml", null,
                DateTime.Today);
            await configuration.DocumentStore.SaveAsync(document);
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
            services.AddNFSeNacional(config =>
            {
                config.Arquivos.Salvar = config.WebServices.Salvar = true;
                config.Arquivos.PathDps = Path.Combine(root, "Fiscal/DPS");
                config.Arquivos.PathNFSe = Path.Combine(root, "Fiscal/NFSe");
                config.Arquivos.PathEnvio = Path.Combine(root, "Technical");
            });
            await using var provider = services.BuildServiceProvider(validateScopes: true);
            var store = provider.GetRequiredService<IDFeDocumentStore>();
            var tenants =
                provider.GetRequiredService<OpenAC.Net.NFSe.Nacional.Web.Accessor.INFSeTenantContextAccessor>();
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
                await Assert.That(tenants.GetTenant(configuration.Arquivos))
                    .IsEqualTo(tenants.GetTenant(configuration));
                var originalPaths = (configuration.Arquivos.PathDps, configuration.Arquivos.PathNFSe,
                    configuration.Arquivos.PathEnvio);
                await Task.WhenAll(new[]
                {
                    NFSeCategorias.Dps, NFSeCategorias.NFSe,
                    DFeCategorias.RespostaTecnica
                }.Select(type => store.SaveAsync(
                    new NFSeDocument(configuration.Arquivos, type, "conteúdo", "evento.xml", null, DateTime.Today))));
                await Assert.That((configuration.Arquivos.PathDps, configuration.Arquivos.PathNFSe,
                        configuration.Arquivos.PathEnvio))
                    .IsEqualTo(originalPaths);
            }

            var tenantHash =
                Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(tenants.GetTenant(configurations[0]))));
            foreach (var category in new[] { "Fiscal/DPS", "Fiscal/NFSe", "Technical" })
                await Assert.That(Directory.GetFiles(Path.Combine(root, category, tenantHash), "evento*.xml").Length)
                    .IsEqualTo(2);
        }
        finally
        {
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }

    [Test]
    [Arguments(DFeArquivoExiste.AdicionarSufixo)]
    [Arguments(DFeArquivoExiste.LancarErro)]
    [Arguments(DFeArquivoExiste.Sobrescrever)]
    public async Task StoreWeb_RespeitaComportamentoDeArquivoExistente(DFeArquivoExiste comportamento)
    {
        var root = Path.Combine(Path.GetTempPath(), $"openac-web-colisao-{Guid.NewGuid():N}");
        try
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddSingleton<IHostEnvironment>(new TestHostEnvironment(root));
            services.AddNFSeNacional(config => config.Arquivos.PathNFSe = root);
            await using var provider = services.BuildServiceProvider(validateScopes: true);
            await using var scope = provider.CreateAsyncScope();
            var configuration = scope.ServiceProvider.GetRequiredService<ConfiguracaoNFSe>();
            var store = provider.GetRequiredService<IDFeDocumentStore>();
            await store.SaveAsync(new NFSeDocument(configuration.Arquivos, NFSeCategorias.NFSe,
                "original", "nota.xml", null, DateTime.Today));
            var document = new NFSeDocument(configuration.Arquivos, NFSeCategorias.NFSe,
                "novo", "nota.xml", null, DateTime.Today, comportamento);
            if (comportamento == DFeArquivoExiste.LancarErro)
                await Assert.That(async () => await store.SaveAsync(document)).Throws<IOException>();
            else
                await store.SaveAsync(document);

            var files = Directory.GetFiles(root, "nota*.xml", SearchOption.AllDirectories);
            await Assert.That(files.Length).IsEqualTo(comportamento == DFeArquivoExiste.AdicionarSufixo ? 2 : 1);
            var original = files.Single(file => Path.GetFileName(file) == "nota.xml");
            await Assert.That(await File.ReadAllTextAsync(original))
                .IsEqualTo(comportamento == DFeArquivoExiste.Sobrescrever ? "novo" : "original");
        }
        finally
        {
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }

    [Test]
    public async Task StoreWeb_DesabilitadoNaoResolveNemCriaDiretoriosDesktop()
    {
        var root = Path.Combine(Path.GetTempPath(), $"openac-web-desabilitado-{Guid.NewGuid():N}");
        try
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddSingleton<IHostEnvironment>(new TestHostEnvironment(root));
            services.AddNFSeNacional(_ => { });
            await using var provider = services.BuildServiceProvider(validateScopes: true);
            var configuration = new ConfiguracaoNFSe();
            configuration.Arquivos.PathDps = Path.Combine(root, "desktop");
            configuration.Arquivos.Salvar = false;
            configuration.DocumentStore = provider.GetRequiredService<IDFeDocumentStore>();
            var fiscal = new PersistenceProbe(configuration);
            await fiscal.SaveAsync();
            await Assert.That(Directory.Exists(root)).IsFalse();
        }
        finally
        {
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }

    [Test]
    public async Task Documento_RejeitaNomeComDiretoriosAntesDeResolverCaminho()
    {
        var configuration = new NFSeArquivoConfig();
        await Assert.That(() => new NFSeDocument(configuration, NFSeCategorias.Dps,
            "xml", "../dps.xml", null, DateTime.Today)).Throws<ArgumentException>();
    }

    [Test]
    public async Task StoreWeb_RejeitaDocumentoSemIdentificacaoDaEmpresa()
    {
        var root = Path.Combine(Path.GetTempPath(), $"openac-web-tipo-{Guid.NewGuid():N}");
        try
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddSingleton<IHostEnvironment>(new TestHostEnvironment(root));
            services.AddNFSeNacional(_ => { });
            await using var provider = services.BuildServiceProvider(validateScopes: true);
            var document = new DFeDocument(Path.Combine(root, "fora-da-empresa.xml"), "xml");
            await Assert.That(async () => await provider.GetRequiredService<IDFeDocumentStore>().SaveAsync(document))
                .Throws<ArgumentException>();
            await Assert.That(Directory.Exists(root)).IsFalse();
        }
        finally
        {
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }

    [Test]
    public async Task DocumentoCancelado_NaoCriaDiretorios()
    {
        var root = Path.Combine(Path.GetTempPath(), $"openac-cancelado-{Guid.NewGuid():N}");
        try
        {
            var files = new NFSeArquivoConfig { PathDps = root };
            var document = new NFSeDocument(files, NFSeCategorias.Dps, "xml", "nota.xml", null, DateTime.Today);
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();
            await Assert.That(async () =>
                    await DFeFileSystemDocumentStore.Instance.SaveAsync(document, cancellation.Token))
                .Throws<OperationCanceledException>();
            await Assert.That(Directory.Exists(root)).IsFalse();
        }
        finally
        {
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }

    private sealed class PersistenceProbe(ConfiguracaoNFSe configuration)
        : OpenAC.Net.NFSe.Nacional.Webservice.Nacional.NacionalWebservice(configuration,
            new OpenAC.Net.NFSe.Nacional.Webservice.NFSeServiceInfo())
    {
        internal Task SaveAsync() => GravarDpsEmDiscoAsync("xml", "dps.xml", null, DateTime.Today);
    }

    private sealed class TestHostEnvironment(string root) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Development;
        public string ApplicationName { get; set; } = "OpenAC.Net.NFSe.Nacional.Test";
        public string ContentRootPath { get; set; } = root;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}