using System.Net;
using Microsoft.Extensions.DependencyInjection;
using OpenAC.Net.DFe.Core.Http;
using OpenAC.Net.NFSe.Nacional.Common;

namespace OpenAC.Net.NFSe.Nacional.Test;

public class TestCompatibilidadeHttpWeb
{
    [Test]
    public async Task TransporteWeb_BufferizaCorpoAntesDeEncerrarTimeout()
    {
        var content = new TrackingContent();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IHttpClientFactory>(new RecordingFactory(content));
        services.AddNFSeNacional(_ => { });
        await using var provider = services.BuildServiceProvider();
        var transport = provider.GetRequiredService<IDFeHttpTransport>();
        var configuration = new ConfiguracaoNFSe();
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://servico.test/");
        using var response = await transport.SendAsync(request, new DFeHttpTransportOptions(configuration.WebServices));
        await Assert.That(content.WasRead).IsTrue();
        await Assert.That(await response.Content.ReadAsStringAsync()).IsEqualTo("retorno");
        await Assert.That(content.WasRead).IsTrue();
    }

    private sealed class RecordingFactory(TrackingContent content) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new(new ResponseHandler(content));
    }

    private sealed class ResponseHandler(TrackingContent content) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content });
    }

    private sealed class TrackingContent : HttpContent
    {
        internal bool WasRead { get; private set; }
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
        {
            WasRead = true;
            return stream.WriteAsync(System.Text.Encoding.UTF8.GetBytes("retorno")).AsTask();
        }
        protected override bool TryComputeLength(out long length) { length = 7; return true; }
    }
}
