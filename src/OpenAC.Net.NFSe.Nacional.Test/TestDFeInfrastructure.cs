using System.Net;
using System.Security.Cryptography.X509Certificates;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.DFe.Core.Http;
using OpenAC.Net.DFe.Core.Storage;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Webservice;
using OpenAC.Net.NFSe.Nacional.Webservice.Nacional;

namespace OpenAC.Net.NFSe.Nacional.Test;

public class TestDFeInfrastructure
{
    [Test]
    public async Task Provider_UsaTransporteConfiguradoEPreservaRespostaDeErro()
    {
        var config = new ConfiguracaoNFSe();
        var transport = new RecordingTransport();
        config.HttpTransport = transport;
        config.WebServices.Tentativas = 1;
        config.WebServices.Proxy = "http://proxy.test:8080";
        config.WebServices.TimeOut = TimeSpan.FromMilliseconds(1750);
        var provider = new TestProvider(config);
        using var response = await provider.Send();
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
        await Assert.That(await response.Content.ReadAsStringAsync()).IsEqualTo("rejeição fiscal");
        await Assert.That(transport.Options!.Attempts).IsEqualTo(1);
        await Assert.That(transport.Options.Proxy).IsEqualTo("http://proxy.test:8080");
        await Assert.That(transport.Options.Timeout).IsEqualTo(TimeSpan.FromMilliseconds(1750));
        await Assert.That(transport.Calls).IsEqualTo(1);
    }

    [Test]
    public async Task Provider_RespeitaTokenDaConfiguracaoAntesDeEnviar()
    {
        var config = new ConfiguracaoNFSe();
        var transport = new RecordingTransport();
        config.HttpTransport = transport;
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        config.OperationToken.Value = cancellation.Token;
        var provider = new TestProvider(config);
        await Assert.That(async () => await provider.Send()).Throws<OperationCanceledException>();
        await Assert.That(transport.Calls).IsEqualTo(0);
    }

    [Test]
    public async Task Provider_UsaStoreConfiguradoECategoriasCompartilhadas()
    {
        var config = new ConfiguracaoNFSe();
        var store = new RecordingStore();
        config.DocumentStore = store;
        var provider = new TestProvider(config);
        await provider.SaveRequest();
        await provider.SaveFiscal();
        await provider.SaveTechnical();
        await Assert.That(store.Documents.Select(document => document.Tipo).ToArray()).IsEquivalentTo(new[]
        {
            DFeTipoDocumento.Solicitacao, DFeTipoDocumento.DocumentoFiscal, DFeTipoDocumento.RespostaTecnica
        });
        await Assert.That(store.Configurations.All(configuration => ReferenceEquals(configuration, config.Arquivos))).IsTrue();
    }

    private sealed class TestProvider(ConfiguracaoNFSe config) : NacionalWebservice(config, new NFSeServiceInfo())
    {
        protected override X509Certificate2? ObterCertificado() => null;
        internal Task<HttpResponseMessage> Send() => SendAsync(new StringContent("consulta"), HttpMethod.Post, "https://servico.test/");
        internal Task SaveRequest() => GravarDpsEmDiscoAsync("evento enviado", "evento.xml", "12345678000190", DateTime.Today);
        internal Task SaveFiscal() => GravarNFSeEmDiscoAsync("evento autorizado", "evento.xml", "12345678000190", DateTime.Today);
        internal Task SaveTechnical() => GravarArquivoEmDiscoAsync("resposta", "evento-resp.json", null);
    }

    private sealed class RecordingTransport : IDFeHttpTransport
    {
        internal int Calls { get; private set; }
        internal DFeHttpTransportOptions? Options { get; private set; }
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, DFeHttpTransportOptions options,
            CancellationToken cancellationToken = default)
        {
            Calls++;
            Options = options;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("rejeição fiscal") });
        }
    }

    private sealed class RecordingStore : IDFeDocumentStore
    {
        internal List<DFeDocument> Documents { get; } = new();
        internal List<DFeArquivosConfigBase> Configurations { get; } = new();
        public Task SaveAsync(DFeDocument document, DFeArquivosConfigBase configuration,
            CancellationToken cancellationToken = default)
        {
            Documents.Add(document);
            Configurations.Add(configuration);
            return Task.CompletedTask;
        }
    }
}
