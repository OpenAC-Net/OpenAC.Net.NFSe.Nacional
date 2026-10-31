using System.Net;
using System.Security.Cryptography.X509Certificates;
using OpenAC.Net.DFe.Core.Http;
using OpenAC.Net.DFe.Core.Storage;
using OpenAC.Net.NFSe.Nacional.Storage;
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
        await Assert.That(transport.Options.UseCookies).IsTrue();
        await Assert.That(transport.Options.AutomaticDecompression).IsEqualTo(DecompressionMethods.None);
        await Assert.That(transport.Options.TimeoutPerAttempt).IsFalse();
        await Assert.That(transport.Options.BufferRequestContent).IsTrue();
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
        config.WebServices.Salvar = true;
        var provider = new TestProvider(config);
        await provider.SaveRequest();
        await provider.SaveFiscal();
        await provider.SaveTechnical();
        await Assert.That(store.Documents.Cast<NFSeDocument>().Select(document => document.Categoria).ToArray())
            .IsEquivalentTo(new[]
            {
                NFSeCategorias.Dps, NFSeCategorias.NFSe, DFeCategorias.RespostaTecnica
            });
        await Assert.That(store.Documents.Cast<NFSeDocument>()
            .All(document => ReferenceEquals(document.Arquivos, config.Arquivos))).IsTrue();
    }

    [Test]
    [Arguments(DFeArquivoExiste.AdicionarSufixo)]
    [Arguments(DFeArquivoExiste.LancarErro)]
    [Arguments(DFeArquivoExiste.Sobrescrever)]
    public async Task Provider_EncaminhaComportamentoDeColisaoAoStore(DFeArquivoExiste comportamento)
    {
        var config = new ConfiguracaoNFSe();
        var store = new RecordingStore();
        config.DocumentStore = store;
        var provider = new TestProvider(config);
        await provider.SaveRequest(comportamento);
        await Assert.That(store.Documents.Single().ArquivoExiste).IsEqualTo(comportamento);
    }

    [Test]
    [Arguments(false)]
    [Arguments(true)]
    public async Task Provider_UsaDecodificacaoCompartilhadaParaCharsetIncorretoOuAusente(bool latin1)
    {
        var config = new ConfiguracaoNFSe();
        config.HttpTransport = new RecordingTransport
        {
            ContentFactory = () =>
            {
                var bytes =
                    (latin1 ? System.Text.Encoding.Latin1 : System.Text.Encoding.UTF8).GetBytes("ação e emissão");
                var content = new ByteArrayContent(bytes);
                if (!latin1) content.Headers.TryAddWithoutValidation("Content-Type", "text/xml; charset=ISO-8859-1");
                return content;
            }
        };
        var provider = new TestProvider(config);
        await Assert.That(await provider.ReadText()).IsEqualTo("ação e emissão");
    }

    private sealed class TestProvider(ConfiguracaoNFSe config) : NacionalWebservice(config, new NFSeServiceInfo())
    {
        protected override X509Certificate2? ObterCertificado() => null;

        internal async Task<string> ReadText()
        {
            using var response = await Send();
            return await LerRespostaAsync(response);
        }

        internal Task<HttpResponseMessage> Send() =>
            SendAsync(new StringContent("consulta"), HttpMethod.Post, "https://servico.test/");

        internal Task SaveRequest(DFeArquivoExiste comportamento = DFeArquivoExiste.AdicionarSufixo) =>
            GravarDpsEmDiscoAsync("evento enviado", "evento.xml", "12345678000190", DateTime.Today, comportamento);

        internal Task SaveFiscal() =>
            GravarNFSeEmDiscoAsync("evento autorizado", "evento.xml", "12345678000190", DateTime.Today);

        internal Task SaveTechnical() => GravarArquivoEmDiscoAsync("resposta", "evento-resp.json", null);
    }

    private sealed class RecordingTransport : IDFeHttpTransport
    {
        internal Func<HttpContent> ContentFactory { get; init; } = () => new StringContent("rejeição fiscal");
        internal int Calls { get; private set; }
        internal DFeHttpTransportOptions? Options { get; private set; }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, DFeHttpTransportOptions options,
            CancellationToken cancellationToken = default)
        {
            Calls++;
            Options = options;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = ContentFactory() });
        }
    }

    private sealed class RecordingStore : IDFeDocumentStore
    {
        internal List<IDFeDocument> Documents { get; } = new();

        public Task SaveAsync(IDFeDocument document,
            CancellationToken cancellationToken = default)
        {
            Documents.Add(document);
            return Task.CompletedTask;
        }
    }
}