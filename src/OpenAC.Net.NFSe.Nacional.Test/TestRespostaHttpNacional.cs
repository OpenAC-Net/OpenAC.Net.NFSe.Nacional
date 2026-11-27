using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using OpenAC.Net.DFe.Core.Collection;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.DFe.Core.Http;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.Common.Types;
using OpenAC.Net.NFSe.Nacional.Webservice;
using OpenAC.Net.NFSe.Nacional.Webservice.Nacional;

namespace OpenAC.Net.NFSe.Nacional.Test;

/// <summary>
/// Testes offline do provedor Nacional com um transporte HTTP falso: não exigem <c>.env</c>,
/// certificado real nem acesso ao Sefin Nacional.
/// </summary>
public class TestRespostaHttpNacional
{
    private const string CnpjPrestador = "12345678000195";
    private const string CodMunicipio = "3550308";
    private const string UrlHomologacao = "https://sefin.producaorestrita.nfse.test/SefinNacional";

    private const string RetornoRejeicao =
        """{"tipoAmbiente":2,"versaoAplicativo":"1.0","dataHoraProcessamento":"2026-01-01T10:00:00-03:00","erros":[{"codigo":"E0310","descricao":"Código de tributação inválido"}]}""";

    [Test]
    [Arguments(HttpStatusCode.BadRequest)]
    [Arguments(HttpStatusCode.UnprocessableEntity)]
    [Arguments(HttpStatusCode.InternalServerError)]
    [Arguments(HttpStatusCode.BadGateway)]
    [Arguments(HttpStatusCode.ServiceUnavailable)]
    public async Task EnviarDps_RespostaDeErro_ExpoeStatusHttp(HttpStatusCode status)
    {
        var (provedor, _, _) = CriarProvedor(status, RetornoRejeicao);

        var resposta = await provedor.EnviarAsync(MontarDps());

        await Assert.That(resposta.Sucesso).IsFalse();
        await Assert.That(resposta.StatusHttp).IsEqualTo(status);
        await Assert.That(resposta.JsonRetorno).IsEqualTo(RetornoRejeicao);
    }

    [Test]
    public async Task EnviarDps_FalhaSemCorpoJson_AindaExpoeStatusHttp()
    {
        var (provedor, _, _) = CriarProvedor(HttpStatusCode.BadGateway, "<html><body>502 Bad Gateway</body></html>");

        var resposta = await provedor.EnviarAsync(MontarDps());

        await Assert.That(resposta.Sucesso).IsFalse();
        await Assert.That(resposta.Resultado).IsNull();
        await Assert.That(resposta.StatusHttp).IsEqualTo(HttpStatusCode.BadGateway);
    }

    [Test]
    public async Task EnviarDps_Sucesso_ExpoeStatusHttp()
    {
        var (provedor, _, _) = CriarProvedor(HttpStatusCode.Created,
            """{"tipoAmbiente":2,"chaveAcesso":"35503082212345678000195000000000000726010000000001"}""");

        var resposta = await provedor.EnviarAsync(MontarDps());

        await Assert.That(resposta.Sucesso).IsTrue();
        await Assert.That(resposta.StatusHttp).IsEqualTo(HttpStatusCode.Created);
    }

    [Test]
    public async Task ConsultaChaveDps_NaoEncontrada_ExpoeStatusHttp()
    {
        var (provedor, transporte, _) = CriarProvedor(HttpStatusCode.NotFound, """{"erro":{"codigo":"E2220","descricao":"DPS não encontrada"}}""");

        var resposta = await provedor.ConsultaChaveDpsAsync("DPS355030821234567800019500001000000000000007");

        await Assert.That(transporte.UltimaUrl).IsEqualTo($"{UrlHomologacao}/dps/DPS355030821234567800019500001000000000000007");
        await Assert.That(resposta.Sucesso).IsFalse();
        await Assert.That(resposta.StatusHttp).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task NFSeResponse_CriadaSemStatus_MantemStatusHttpNulo()
    {
        var json = NFSeResponse<RespostaEnvioDps>.Create("", "", "{}", false);
        var xml = NFSeResponse<RespostaEnvioDps>.Create("", "", false, new RespostaEnvioDps());

        await Assert.That(json.StatusHttp).IsNull();
        await Assert.That(xml.StatusHttp).IsNull();
    }

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task EnviarDps_NaoEscreveNoConsole(bool assinarXml)
    {
        var (provedor, _, _) = CriarProvedor(HttpStatusCode.BadRequest, RetornoRejeicao, assinarXml);
        var dps = MontarDps();
        if (!assinarXml)
        {
            // Com AssinarXml desligado o chamador entrega a DPS já assinada.
            var (_, _, configuracao) = CriarProvedor(HttpStatusCode.OK, "{}");
            dps.Assinar(configuracao);
        }

        await provedor.EnviarAsync(dps);

        await Assert.That(TestContext.Current!.Output.GetStandardOutput()).IsEmpty();
    }

    private static (NacionalWebservice provedor, TransporteFalso transporte, ConfiguracaoNFSe configuracao) CriarProvedor(HttpStatusCode status,
        string corpo, bool assinarXml = true)
    {
        using var rsa = RSA.Create(2048);
        var requisicao = new CertificateRequest($"CN=PRESTADOR DE TESTE:{CnpjPrestador}", rsa,
            HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        using var certificado = requisicao.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddDays(1));

        var configuracao = new ConfiguracaoNFSe();
        configuracao.Geral.Versao = VersaoNFSe.Ve101;
        configuracao.Geral.Salvar = false;
        configuracao.Geral.AssinarXml = assinarXml;
        configuracao.Arquivos.Salvar = false;
        configuracao.Arquivos.PathSchemas = Path.Combine(AppContext.BaseDirectory, "Schemas", "1.01", "Homologacao");
        configuracao.Certificados.CertificadoBytes = certificado.Export(X509ContentType.Pfx, "1234");
        configuracao.Certificados.Senha = "1234";
        configuracao.WebServices.Ambiente = DFeTipoAmbiente.Homologacao;

        var servico = new NFSeServiceInfo
        {
            Codigo = int.Parse(CodMunicipio),
            Provider = NFSeProvider.Nacional,
            Nome = "São Paulo",
            UF = DFeSiglaUF.SP,
            Ambientes = new DFeCollection<NFSeEnvironment>
            {
                new NFSeEnvironment
                {
                    Ambiente = DFeTipoAmbiente.Homologacao,
                    Enderecos = new Dictionary<TipoUrl, string>
                    {
                        { TipoUrl.Enviar, UrlHomologacao },
                        { TipoUrl.ConsultarChaveDps, UrlHomologacao }
                    }
                }
            }
        };

        var transporte = new TransporteFalso(status, corpo);
        return (new NacionalWebservice(configuracao, servico, transporte), transporte, configuracao);
    }

    private static Dps MontarDps()
    {
        var dps = new Dps
        {
            Versao = VersaoNFSe.Ve101,
            Informacoes = new InfDps
            {
                TipoAmbiente = DFeTipoAmbiente.Homologacao,
                DhEmissao = DateTimeOffset.Now,
                LocalidadeEmitente = CodMunicipio,
                Serie = "00001",
                NumeroDps = "7",
                Competencia = DateTime.Today,
                TipoEmitente = EmitenteDps.Prestador,
                Prestador = new PrestadorDps
                {
                    CNPJ = CnpjPrestador,
                    Email = "prestador@teste.com",
                    Regime = new RegimeTributario
                    {
                        OptanteSimplesNacional = OptanteSimplesNacional.NaoOptante,
                        RegimeEspecial = RegimeEspecial.Nenhum
                    }
                },
                Tomador = new InfoPessoaNFSe
                {
                    CNPJ = "98765432000198",
                    Nome = "Tomador de Teste Ltda",
                    Endereco = new EnderecoNFSe
                    {
                        Logradouro = "Avenida Paulista",
                        Numero = "1000",
                        Bairro = "Bela Vista",
                        Municipio = new MunicipioNacional { CodMunicipio = CodMunicipio, CEP = "01310100" }
                    }
                },
                Servico = new ServicoNFSe
                {
                    Localidade = new LocalidadeNFSe { CodMunicipioPrestacao = CodMunicipio },
                    Informacoes = new InformacoesServico
                    {
                        CodTributacaoNacional = "010101",
                        Descricao = "Servico de desenvolvimento de software"
                    }
                },
                Valores = new ValoresDps
                {
                    ValoresServico = new ValoresServico { Valor = 100 },
                    Tributos = new TributosNFSe
                    {
                        Municipal = new TributoMunicipal
                        {
                            ISSQN = TributoISSQN.OperacaoTributavel,
                            TipoRetencaoISSQN = TipoRetencaoISSQN.NaoRetido
                        },
                        Total = new TotalTributos
                        {
                            PorcentagemTotal = new PorcentagemTotalTributos
                            {
                                TotalEstadual = 0,
                                TotalFederal = 0,
                                TotalMunicipal = 0
                            }
                        }
                    }
                }
            }
        };
        dps.GerarId();
        return dps;
    }

    /// <summary>Responde toda requisição com o status e o corpo informados.</summary>
    private sealed class TransporteFalso(HttpStatusCode status, string corpo) : IDFeHttpTransport
    {
        public string? UltimaUrl { get; private set; }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, DFeHttpTransportOptions options,
            CancellationToken cancellationToken = default)
        {
            UltimaUrl = request.RequestUri?.ToString();
            return Task.FromResult(new HttpResponseMessage(status) { Content = new StringContent(corpo) });
        }
    }
}
