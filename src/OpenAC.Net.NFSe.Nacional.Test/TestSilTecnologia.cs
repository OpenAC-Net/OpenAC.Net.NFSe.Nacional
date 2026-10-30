using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.Xml.Linq;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Collection;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.Common.Types;
using OpenAC.Net.NFSe.Nacional.Webservice;
using OpenAC.Net.NFSe.Nacional.Webservice.SilTecnologia;

namespace OpenAC.Net.NFSe.Nacional.Test;

/// <summary>
/// Testes offline do provedor SIL Tecnologia (Bauru/SP): montagem da NFS-e, assinatura, envelope
/// SOAP e leitura do retorno. O transporte HTTP é simulado; não há acesso à rede.
/// </summary>
public class TestSilTecnologia
{
    private const string CodBauru = "3506003";

    private const string CnpjPrestador = "12345678000195";

    private const string UrlHomologacao = "https://homologacao.siltecnologia.com.br/tbbauru/services/NotaFiscalNacional";

    private static string CaminhoSchemaNFSe => Path.Combine(AppContext.BaseDirectory, "Schemas", "1.01", "NFSe_v1.01.xsd");

    [Test]
    public async Task Montar_PreencheCamposFixosEEmitente()
    {
        var dps = MontarDps();

        var nfse = SilTecnologiaNFSe.Montar(dps, _ => "Bauru", DateTimeOffset.Now);
        var inf = nfse.Informacoes;

        await Assert.That(nfse.Versao).IsEqualTo("1.01");
        await Assert.That(inf.Id).IsEqualTo("NFS" + new string('0', 50));
        await Assert.That(inf.NumeroNFSe).IsEqualTo(0L);
        await Assert.That(inf.AmbienteGerador).IsEqualTo(AmbienteGerador.Prefeitura);
        await Assert.That(inf.TipoEmissao).IsEqualTo(TipoEmissao.ModeloMunicipio);
        await Assert.That(inf.SituacaoNFSe).IsEqualTo(StatusNFSe.Gerada);
        await Assert.That(inf.VersaoAplicacao).IsEqualTo(SilTecnologiaNFSe.VersaoAplicacao);
        await Assert.That(inf.NumeroDFSe).IsEqualTo(7);
        await Assert.That(inf.CodLocalIncidencia).IsEqualTo(CodBauru);
        await Assert.That(inf.LocalEmissao).IsEqualTo("Bauru");
        await Assert.That(inf.Emitente.RazaoSocial).IsEqualTo("Prestador de Teste Ltda");
        await Assert.That(inf.Emitente.Endereco.UF).IsEqualTo("SP");
        await Assert.That(inf.Emitente.Endereco.CEP).IsEqualTo("17010000");

        // Nome e endereço vão para emit e saem do prest da DPS embutida; a DPS original não muda.
        await Assert.That(inf.Dps.Informacoes.Prestador.Nome).IsNull();
        await Assert.That(inf.Dps.Informacoes.Prestador.Endereco).IsNull();
        await Assert.That(inf.Dps.Informacoes.VersaoAplicacao).IsEqualTo(SilTecnologiaNFSe.VersaoAplicacao);
        await Assert.That(dps.Informacoes.Prestador.Nome).IsEqualTo("Prestador de Teste Ltda");
    }

    [Test]
    public async Task Montar_SemNomeDoPrestador_LancaExcecao()
    {
        var dps = MontarDps();
        dps.Informacoes.Prestador.Nome = null;

        await Assert.That(() => SilTecnologiaNFSe.Montar(dps, _ => "Bauru", DateTimeOffset.Now))
            .Throws<InvalidOperationException>();
    }

    [Test]
    public async Task CalcularValores_SegueRegrasDaSil()
    {
        var valores = MontarDps().Informacoes.Valores;
        valores.ValoresDesconto = new ValoresDesconto { ValorIncodicional = 50, ValorCondicional = 10 };
        valores.Tributos.Municipal.Aliquota = 2.5m;
        valores.Tributos.Municipal.TipoRetencaoISSQN = TipoRetencaoISSQN.RetidoTomador;
        valores.Tributos.Federal = new TributoFederal { ValorIRRF = 15 };

        var resultado = SilTecnologiaNFSe.CalcularValores(valores);

        // vBC = 1000 - 50; vISSQN = 950 x 2,5%; vTotalRet = IRRF + ISS retido; vLiq = 1000 - 10 - 50 - vTotalRet.
        await Assert.That(resultado.ValorBc).IsEqualTo(950m);
        await Assert.That(resultado.Aliquota).IsEqualTo(2.5m);
        await Assert.That(resultado.ValorISSQN).IsEqualTo(23.75m);
        await Assert.That(resultado.TotalRetido).IsEqualTo(38.75m);
        await Assert.That(resultado.ValorLiquido).IsEqualTo(901.25m);
    }

    [Test]
    [Arguments("070201", true)]
    [Arguments("120101", true)]
    [Arguments("121301", false)]
    [Arguments("160101", true)]
    [Arguments("010101", false)]
    [Arguments("170101", false)]
    public async Task IncideNoLocalDaPrestacao_SegueArt3DaLC116(string codigo, bool esperado)
    {
        await Assert.That(SilTecnologiaNFSe.IncideNoLocalDaPrestacao(codigo)).IsEqualTo(esperado);
    }

    [Test]
    public async Task Enviar_AssinaInfNFSeEValidaContraSchema()
    {
        var (provedor, transporte, certificado) = CriarProvedor(RetornoSoap("<Retorno><Status>ERRO</Status>" +
            "<MensagemErro>Rejeição do Emissor Nacional: E1235 - Falha no esquema XML do DF-e.</MensagemErro></Retorno>"));

        await provedor.EnviarAsync(MontarDps());

        var envelope = XDocument.Parse(transporte.UltimoEnvio!);
        var operacao = envelope.Descendants().Single(x => x.Name.LocalName == "NotaFiscalNacionalGerar");
        var xml = operacao.Element("xml")!.Value;

        var documento = new XmlDocument { PreserveWhitespace = true };
        documento.LoadXml(xml);
        var assinatura = (XmlElement)documento.GetElementsByTagName("Signature", SignedXml.XmlDsigNamespaceUrl)[0]!;
        var signedXml = new SignedXml(documento);
        signedXml.LoadXml(assinatura);

        await Assert.That(transporte.UltimaUrl).IsEqualTo(UrlHomologacao);
        await Assert.That(documento.DocumentElement!.LocalName).IsEqualTo("NFSe");
        await Assert.That(assinatura.ParentNode!.LocalName).IsEqualTo("NFSe");
        await Assert.That(signedXml.SignedInfo!.CanonicalizationMethod).IsEqualTo(SignedXml.XmlDsigExcC14NTransformUrl);
        await Assert.That(((Reference)signedXml.SignedInfo.References[0]!).Uri).IsEqualTo("#NFS" + new string('0', 50));
        await Assert.That(signedXml.CheckSignature(certificado, true)).IsTrue();

        // O NFSe_v1.01.xsd exige nNFSe >= 1; a SIL exige 0. O restante da NFS-e deve seguir o schema.
        var xmlSchema = xml.Replace("<nNFSe>0</nNFSe>", "<nNFSe>1</nNFSe>");
        XmlSchemaValidation.ValidarXml(xmlSchema, CaminhoSchemaNFSe, out var erros, out _);
        await Assert.That(string.Join(" | ", erros)).IsEmpty();
    }

    [Test]
    public async Task Enviar_RetornoComErro_PreencheErros()
    {
        var (provedor, _, _) = CriarProvedor(RetornoSoap("<Retorno><Status>ERRO</Status>" +
            "<MensagemErro>Rejeição do Emissor Nacional: E1235 - Falha no esquema XML do DF-e.</MensagemErro></Retorno>"));

        var retorno = await provedor.EnviarAsync(MontarDps());

        await Assert.That(retorno.Sucesso).IsFalse();
        await Assert.That(retorno.Resultado!.Erros.Count).IsEqualTo(1);
        await Assert.That(retorno.Resultado.Erros[0].Codigo).IsEqualTo("E1235");
        await Assert.That(retorno.Resultado.Erros[0].Mensagem).StartsWith("Rejeição do Emissor Nacional");
    }

    [Test]
    public async Task Enviar_RetornoSemErro_LeChaveDeAcesso()
    {
        const string chave = "35060032212345678000195000000000000126104123456789";
        var (provedor, _, _) = CriarProvedor(RetornoSoap(
            $"<Retorno><Status>SUCESSO</Status><ChaveAcesso>{chave}</ChaveAcesso></Retorno>"));

        var retorno = await provedor.EnviarAsync(MontarDps());

        await Assert.That(retorno.Sucesso).IsTrue();
        await Assert.That(retorno.Resultado!.ChaveAcesso).IsEqualTo(chave);
        await Assert.That(retorno.Resultado.Erros).IsEmpty();
    }

    [Test]
    public async Task Bauru_EstaCadastradoComProvedorSilTecnologia()
    {
        var servico = NFSeServiceManager.Instance.Services[int.Parse(CodBauru)];

        await Assert.That(servico).IsNotNull();
        await Assert.That(servico!.Provider).IsEqualTo(NFSeProvider.SilTecnologia);
        await Assert.That(servico[DFeTipoAmbiente.Homologacao].Enderecos[TipoUrl.Enviar]).IsEqualTo(UrlHomologacao);
        await Assert.That(servico[DFeTipoAmbiente.Producao].Enderecos[TipoUrl.Enviar])
            .IsEqualTo("https://tributario.bauru.sp.gov.br/services/NotaFiscalNacional");
    }

    [Test]
    public async Task Montar_IssNaoRetido_UsaAliquotaDaDpsERetiraDaDpsEmbutida()
    {
        var dps = MontarDps();
        dps.Informacoes.Valores.Tributos.Municipal.Aliquota = 3m;

        var nfse = SilTecnologiaNFSe.Montar(dps, _ => "Bauru", DateTimeOffset.Now);

        await Assert.That(nfse.Informacoes.Valores.Aliquota).IsEqualTo(3m);
        await Assert.That(nfse.Informacoes.Valores.ValorISSQN).IsEqualTo(30m);
        await Assert.That(nfse.Informacoes.Dps.Informacoes.Valores.Tributos.Municipal.Aliquota).IsNull();
        await Assert.That(dps.Informacoes.Valores.Tributos.Municipal.Aliquota).IsEqualTo(3m);
    }

    [Test]
    public async Task Montar_IssRetido_MantemAliquotaNaDpsEmbutida()
    {
        var dps = MontarDps();
        dps.Informacoes.Valores.Tributos.Municipal.Aliquota = 3m;
        dps.Informacoes.Valores.Tributos.Municipal.TipoRetencaoISSQN = TipoRetencaoISSQN.RetidoTomador;

        var nfse = SilTecnologiaNFSe.Montar(dps, _ => "Bauru", DateTimeOffset.Now);

        await Assert.That(nfse.Informacoes.Valores.TotalRetido).IsEqualTo(30m);
        await Assert.That(nfse.Informacoes.Dps.Informacoes.Valores.Tributos.Municipal.Aliquota).IsEqualTo(3m);
    }

    [Test]
    public async Task Cancelamento_EnviaEventoAssinadoParaSil()
    {
        var (provedor, transporte, certificado) = CriarProvedor(RetornoSoap("<Retorno><Status>SUCESSO</Status></Retorno>"));

        var retorno = await provedor.EnviarEventoAsync(MontarPedido(new EventoCancelamento
        {
            CodMotivo = MotivoCancelamento.ErroEmissao,
            Motivo = "Erro na emissao da nota"
        }));

        var operacao = XDocument.Parse(transporte.UltimoEnvio!).Descendants()
            .Single(x => x.Name.LocalName == "NotaFiscalNacionalCancelar");
        var xml = operacao.Element("xml")!.Value;
        var documento = new XmlDocument { PreserveWhitespace = true };
        documento.LoadXml(xml);
        var assinaturas = documento.GetElementsByTagName("Signature", SignedXml.XmlDsigNamespaceUrl);
        var signedXml = new SignedXml(documento);
        signedXml.LoadXml((XmlElement)assinaturas[0]!);
        var infEvento = (XmlElement)documento.GetElementsByTagName("infEvento", "http://www.sped.fazenda.gov.br/nfse")[0]!;

        await Assert.That(retorno.Sucesso).IsTrue();
        await Assert.That(retorno.Resultado!.XmlEvento).IsEqualTo(xml);
        await Assert.That(transporte.UltimaUrl).IsEqualTo(UrlHomologacao);
        await Assert.That(documento.DocumentElement!.LocalName).IsEqualTo("evento");
        await Assert.That(infEvento.GetAttribute("Id")).IsEqualTo($"EVT{ChaveNFSe}101101001");
        await Assert.That(assinaturas.Count).IsEqualTo(1);
        await Assert.That(assinaturas[0]!.ParentNode!.LocalName).IsEqualTo("evento");
        await Assert.That(signedXml.CheckSignature(certificado, true)).IsTrue();

        XmlSchemaValidation.ValidarXml(xml, Path.Combine(AppContext.BaseDirectory, "Schemas", "1.01", "evento_v1.01.xsd"),
            out var erros, out _);
        await Assert.That(string.Join(" | ", erros)).IsEmpty();
    }

    [Test]
    public async Task Substituicao_UsaOperacaoSubstituir()
    {
        var (provedor, transporte, _) = CriarProvedor(RetornoSoap("<Retorno><Status>SUCESSO</Status></Retorno>"));

        await provedor.EnviarEventoAsync(MontarPedido(new EventoCancelamentoPorSubstituicao
        {
            CodMotivo = JustificativaSubstituicao.Outros,
            Motivo = "Substituida por outra nota",
            ChaveSubstituta = "35060032212345678000195000000000000226104123456789"
        }));

        await Assert.That(transporte.UltimoEnvio).Contains("NotaFiscalNacionalSubstituir");
        await Assert.That(transporte.UltimoEnvio).Contains($"EVT{ChaveNFSe}105102001");
    }

    [Test]
    public async Task EventoDeConfirmacao_VaiParaSefinNacional()
    {
        var (provedor, transporte, _) = CriarProvedor("{}");

        try
        {
            await provedor.EnviarEventoAsync(MontarPedido(new EventoConfirmacaoPrestador()));
        }
        catch (Exception)
        {
            // O retorno simulado não é um retorno válido da Sefin; interessa só o destino da requisição.
        }

        await Assert.That(transporte.UltimaUrl).Contains("sefin");
    }

    [Test]
    public async Task ConsultaNsu_VaiParaAdn()
    {
        var (provedor, transporte, _) = CriarProvedor("{}");

        try
        {
            await provedor.ConsultaNsuAsync(1);
        }
        catch (Exception)
        {
            // O retorno simulado não é um retorno válido do ADN; interessa só o destino da requisição.
        }

        await Assert.That(transporte.UltimaUrl).Contains("adn");
    }

    [Test]
    public async Task ConsultaChaveDps_ConsultaNaSilPeloIdDaDps()
    {
        var (provedor, transporte, _) = CriarProvedor(RetornoSoap(
            $"<Retorno><Status>SUCESSO</Status><ChaveAcesso>{ChaveNFSe}</ChaveAcesso></Retorno>"));
        var idDps = MontarDps().Informacoes.Id;

        var retorno = await provedor.ConsultaChaveDpsAsync(idDps);

        var xml = XDocument.Parse(transporte.UltimoEnvio!).Descendants()
            .Single(x => x.Name.LocalName == "NotaFiscalNacionalConsultar").Element("xml")!.Value;
        var consulta = XElement.Parse(xml);

        await Assert.That(retorno.Sucesso).IsTrue();
        await Assert.That(retorno.Resultado!.Chave).IsEqualTo(ChaveNFSe);
        await Assert.That(consulta.Element("consul")!.Element("CNPJ")!.Value).IsEqualTo(CnpjPrestador);
        await Assert.That(consulta.Element("DPS")!.Value).IsEqualTo(idDps);
        await Assert.That(xml).Contains("<Signature");
    }

    private const string ChaveNFSe = "35060032212345678000195000000000000126104123456789";

    private static PedidoRegistroEvento MontarPedido(IEventoNFSe evento) => new()
    {
        Versao = VersaoNFSe.Ve101,
        Informacoes = new InfPedReg
        {
            Id = "PRE" + ChaveNFSe + evento switch
            {
                EventoCancelamento => TipoEventoCod.Cancelamento,
                EventoCancelamentoPorSubstituicao => TipoEventoCod.CancelamentoPorSubstituicao,
                _ => TipoEventoCod.ConfirmacaoPrestador
            },
            TipoAmbiente = DFeTipoAmbiente.Homologacao,
            DhEvento = DateTimeOffset.Now,
            CNPJAutor = CnpjPrestador,
            ChNFSe = ChaveNFSe,
            Evento = evento
        }
    };

    private static (SilTecnologiaWebService provedor, TransporteFalso transporte, X509Certificate2 certificado)
        CriarProvedor(string resposta)
    {
        using var rsa = RSA.Create(2048);
        var requisicao = new CertificateRequest($"CN=PRESTADOR DE TESTE:{CnpjPrestador}", rsa,
            HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        using var certificadoGerado = requisicao.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddDays(1));
        var pfx = certificadoGerado.Export(X509ContentType.Pfx, "1234");

        var configuracao = new ConfiguracaoNFSe();
        configuracao.Geral.Versao = VersaoNFSe.Ve101;
        configuracao.Geral.Salvar = false;
        configuracao.Arquivos.Salvar = false;
        configuracao.Arquivos.PathSchemas = Path.Combine(AppContext.BaseDirectory, "Schemas", "1.01");
        configuracao.Certificados.CertificadoBytes = pfx;
        configuracao.Certificados.Senha = "1234";
        configuracao.WebServices.Ambiente = DFeTipoAmbiente.Homologacao;

        var servico = new NFSeServiceInfo
        {
            Codigo = int.Parse(CodBauru),
            Provider = NFSeProvider.SilTecnologia,
            Nome = "Bauru",
            UF = DFeSiglaUF.SP,
            Ambientes = new DFeCollection<NFSeEnvironment>
            {
                new NFSeEnvironment
                {
                    Ambiente = DFeTipoAmbiente.Homologacao,
                    Enderecos = new Dictionary<TipoUrl, string> { { TipoUrl.Enviar, UrlHomologacao } }
                }
            }
        };

        var transporte = new TransporteFalso(resposta);
        var certificado = X509CertificateLoader.LoadPkcs12(pfx, "1234");
        return (new SilTecnologiaWebService(configuracao, servico, transporte), transporte, certificado);
    }

    private static string RetornoSoap(string retorno) =>
        "<S:Envelope xmlns:S=\"http://schemas.xmlsoap.org/soap/envelope/\"><S:Body>" +
        "<ns2:NotaFiscalNacionalGerarResponse xmlns:ns2=\"http://webservices.sil.com/\"><return>" +
        System.Security.SecurityElement.Escape("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" + retorno) +
        "</return></ns2:NotaFiscalNacionalGerarResponse></S:Body></S:Envelope>";

    private static Dps MontarDps()
    {
        var dps = new Dps
        {
            Versao = VersaoNFSe.Ve101,
            Informacoes = new InfDps
            {
                TipoAmbiente = DFeTipoAmbiente.Homologacao,
                DhEmissao = DateTimeOffset.Now,
                LocalidadeEmitente = CodBauru,
                Serie = "00001",
                NumeroDps = "7",
                Competencia = DateTime.Today,
                TipoEmitente = EmitenteDps.Prestador,
                Prestador = new PrestadorDps
                {
                    CNPJ = CnpjPrestador,
                    InscricaoMunicipal = "123456",
                    Nome = "Prestador de Teste Ltda",
                    Endereco = new EnderecoNFSe
                    {
                        Logradouro = "Rua Primeiro de Agosto",
                        Numero = "100",
                        Bairro = "Centro",
                        Municipio = new MunicipioNacional { CodMunicipio = CodBauru, CEP = "17010000" }
                    },
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
                        Logradouro = "Avenida Getulio Vargas",
                        Numero = "200",
                        Bairro = "Jardim Europa",
                        Municipio = new MunicipioNacional { CodMunicipio = CodBauru, CEP = "17017000" }
                    }
                },
                Servico = new ServicoNFSe
                {
                    Localidade = new LocalidadeNFSe { CodMunicipioPrestacao = CodBauru },
                    Informacoes = new InformacoesServico
                    {
                        CodTributacaoNacional = "010101",
                        Descricao = "Analise e desenvolvimento de sistemas"
                    }
                },
                Valores = new ValoresDps
                {
                    ValoresServico = new ValoresServico { Valor = 1000 },
                    Tributos = new TributosNFSe
                    {
                        Municipal = new TributoMunicipal
                        {
                            ISSQN = TributoISSQN.OperacaoTributavel,
                            TipoRetencaoISSQN = TipoRetencaoISSQN.NaoRetido,
                            Aliquota = 2
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

    private sealed class TransporteFalso(string resposta) : INFSeHttpTransport
    {
        public string? UltimoEnvio { get; private set; }

        public string? UltimaUrl { get; private set; }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, ConfiguracaoNFSe configuracao,
            CancellationToken cancellationToken = default)
        {
            UltimaUrl = request.RequestUri?.ToString();
            UltimoEnvio = request.Content == null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(resposta) };
        }
    }
}
