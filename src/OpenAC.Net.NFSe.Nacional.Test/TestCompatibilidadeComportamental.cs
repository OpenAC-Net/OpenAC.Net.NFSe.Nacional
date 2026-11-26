using System.Net;
using System.Security.Cryptography.X509Certificates;
using OpenAC.Net.DFe.Core.Http;
using OpenAC.Net.DFe.Core.Storage;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Storage;
using OpenAC.Net.NFSe.Nacional.Webservice;
using OpenAC.Net.NFSe.Nacional.Webservice.Nacional;

namespace OpenAC.Net.NFSe.Nacional.Test;

public class TestCompatibilidadeComportamental
{
    [Test]
    [Arguments(NFSeCategorias.Dps)]
    [Arguments(NFSeCategorias.NFSe)]
    [Arguments(DFeCategorias.SolicitacaoTecnica)]
    [Arguments(DFeCategorias.RespostaTecnica)]
    public async Task Arquivos_MantemDiretoriosAnterioresComTodasAsSeparacoes(string categoria)
    {
        var root = Path.Combine(Path.GetTempPath(), $"openac-compatibilidade-{Guid.NewGuid():N}");
        var data = new DateTime(2024, 2, 29);
        const string cnpj = "12.345.678/0001-90";
        try
        {
            for (var flags = 0; flags < 64; flags++)
            {
                var caseRoot = Path.Combine(root, flags.ToString());
                var files = new NFSeArquivoConfig
                {
                    PathDps = Path.Combine(caseRoot, "Solicitacoes"),
                    PathNFSe = Path.Combine(caseRoot, "Autorizadas"),
                    PathEnvio = Path.Combine(caseRoot, "Comunicacao"),
                    SepararPorCNPJ = (flags & 1) != 0,
                    SepararPorModelo = (flags & 2) != 0,
                    SepararPorAno = (flags & 4) != 0,
                    SepararPorMes = (flags & 8) != 0,
                    SepararPorDia = (flags & 16) != 0,
                    AdicionarLiteral = (flags & 32) != 0
                };
                // Roteamento do DesktopNFSeDocumentStore anterior à migração para o Core.
                var diretorioAnterior = categoria switch
                {
                    NFSeCategorias.Dps => files.GetPathDps(data, cnpj),
                    NFSeCategorias.NFSe => files.GetPathNFSe(data, cnpj),
                    _ => files.GetPathEnvio(data, cnpj)
                };
                var document = new NFSeDocument(files, categoria, "ação", "documento.xml", cnpj, data);
                await DFeFileSystemDocumentStore.Instance.SaveAsync(document);
                var caminhoAnterior = Path.Combine(diretorioAnterior, "documento.xml");
                await Assert.That(File.Exists(caminhoAnterior)).IsTrue().Because($"categoria={categoria}, flags={flags}");
                await Assert.That(await File.ReadAllTextAsync(caminhoAnterior)).IsEqualTo("ação");
                await Assert.That(Directory.GetFiles(caseRoot, "*", SearchOption.AllDirectories).Length).IsEqualTo(1);
            }
        }
        finally
        {
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }

    [Test]
    public async Task Http_MantemMetodoUrlConteudoCabecalhosERespostaDeErro()
    {
        var transport = new RecordingTransport();
        var configuration = new ConfiguracaoNFSe { HttpTransport = transport };
        var provider = new TestProvider(configuration);
        using var response = await provider.Send();
        await Assert.That(transport.Method).IsEqualTo(HttpMethod.Post);
        await Assert.That(transport.Url).IsEqualTo("https://servico.test/operacao?ambiente=2");
        await Assert.That(transport.Body).IsEqualTo("<envio>ação</envio>");
        await Assert.That(transport.ContentType).IsEqualTo("application/xml; charset=utf-8");
        await Assert.That(transport.SoapAction).IsEqualTo("urn:EnviarDps");
        await Assert.That(transport.UserAgent).Contains("OpenAC.Net.NFSe.Nacional/");
        await Assert.That(transport.UserAgent).Contains("(+https://github.com/OpenAC-Net/OpenAC.Net.NFSe.Nacional)");
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
        await Assert.That(await response.Content.ReadAsStringAsync()).IsEqualTo("rejeição fiscal");
    }

    private sealed class TestProvider(ConfiguracaoNFSe config) : NacionalWebservice(config, new NFSeServiceInfo())
    {
        protected override X509Certificate2? ObterCertificado() => null;
        internal Task<HttpResponseMessage> Send() => SendAsync(
            new StringContent("<envio>ação</envio>", System.Text.Encoding.UTF8, "application/xml"),
            HttpMethod.Post, "https://servico.test/operacao?ambiente=2",
            [new KeyValuePair<string, string>("SOAPAction", "urn:EnviarDps")]);
    }

    private sealed class RecordingTransport : IDFeHttpTransport
    {
        internal HttpMethod? Method { get; private set; }
        internal string? Url { get; private set; }
        internal string? Body { get; private set; }
        internal string? ContentType { get; private set; }
        internal string? SoapAction { get; private set; }
        internal string UserAgent { get; private set; } = string.Empty;

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, DFeHttpTransportOptions options,
            CancellationToken cancellationToken = default)
        {
            Method = request.Method;
            Url = request.RequestUri?.AbsoluteUri;
            Body = request.Content == null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
            ContentType = request.Content?.Headers.ContentType?.ToString();
            SoapAction = request.Headers.GetValues("SOAPAction").Single();
            UserAgent = request.Headers.UserAgent.ToString();
            return new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("rejeição fiscal") };
        }
    }
}
