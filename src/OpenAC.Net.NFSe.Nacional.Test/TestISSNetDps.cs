using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.DFe.Core.Extensions;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.Common.Types;
using OpenAC.Net.NFSe.Nacional.Webservice.ISSNet;

namespace OpenAC.Net.NFSe.Nacional.Test;

/// <summary>
/// Testes offline da adequação da DPS ao layout da ISSNet (<see cref="ISSNetDps"/>).
/// </summary>
public class TestISSNetDps
{
    private const string CodMunicipioPrestacao = "3513108";

    private static string CaminhoSchemaISSNet =>
        Path.Combine(AppContext.BaseDirectory, "Schemas", ISSNetDps.PastaSchema, ISSNetDps.VersaoSchema, ISSNetDps.ArquivoSchema);

    [Test]
    public async Task ObraComCep_UsaEndNacComMunicipioDePrestacao_EValidaNoSchemaISSNet()
    {
        var dps = MontarDps();
        var xml = ISSNetDps.Adequar(Xml(dps), dps, () => throw new InvalidOperationException());

        await Assert.That(xml).Contains($"<obra><end><endNac><cMun>{CodMunicipioPrestacao}</cMun><CEP>14140000</CEP></endNac><xLgr>");
        await Assert.That(ValidarSchemaISSNet(xml, out var erros)).IsTrue().Because(string.Join(Environment.NewLine, erros));
    }

    [Test]
    public async Task XmlOriginal_ComCepDireto_NaoValidaNoSchemaISSNet()
    {
        var dps = MontarDps();

        await Assert.That(ValidarSchemaISSNet(Xml(dps), out _)).IsFalse();
    }

    [Test]
    public async Task ObraComCodMunicipioInformado_PrevaleceSobreMunicipioDePrestacao()
    {
        var dps = MontarDps();
        dps.Informacoes.Servico.Obra!.Endereco!.CodMunicipio = "3543402";

        var xml = ISSNetDps.Adequar(Xml(dps), dps, () => throw new InvalidOperationException());

        await Assert.That(xml).Contains("<endNac><cMun>3543402</cMun><CEP>14140000</CEP></endNac>");
    }

    [Test]
    public async Task Obra_RemoveCObraEIncluiNProcessoObra()
    {
        var dps = MontarDps();
        dps.Informacoes.Servico.Obra!.InscricaoImobiliaria = "123";
        dps.Informacoes.Servico.Obra.CodObra = "CNO123";
        dps.Informacoes.Servico.Obra.NumeroProcesso = "2026000123";

        var nacional = Xml(dps);
        var xml = ISSNetDps.Adequar(nacional, dps, () => throw new InvalidOperationException());

        await Assert.That(nacional).Contains("<cObra>CNO123</cObra>");
        await Assert.That(nacional).DoesNotContain("nProcessoObra");
        await Assert.That(xml).DoesNotContain("cObra");
        await Assert.That(xml).Contains("<obra><inscImobFisc>123</inscImobFisc><nProcessoObra>2026000123</nProcessoObra><end>");
        await Assert.That(ValidarSchemaISSNet(xml, out var erros)).IsTrue().Because(string.Join(Environment.NewLine, erros));
    }

    [Test]
    public async Task ObraNoExterior_IncluiCPais()
    {
        var dps = MontarDps();
        var endereco = dps.Informacoes.Servico.Obra!.Endereco!;
        endereco.CEP = string.Empty;
        endereco.EnderecoExterior = new EnderecoExterior
        {
            CodPais = "US",
            EnderecoPostal = "10001",
            Cidade = "New York",
            EstadoProvincia = "NY"
        };

        var xml = ISSNetDps.Adequar(Xml(dps), dps, () => throw new InvalidOperationException());

        await Assert.That(xml).Contains("<end><endExt><cPais>US</cPais><cEndPost>10001</cEndPost>");
        await Assert.That(ValidarSchemaISSNet(xml, out var erros)).IsTrue().Because(string.Join(Environment.NewLine, erros));
    }

    [Test]
    public async Task EnderecoSimplesNoExterior_NaoSerializaCepNoLayoutNacional()
    {
        var dps = MontarDps();
        var endereco = dps.Informacoes.Servico.Obra!.Endereco!;
        endereco.EnderecoExterior = new EnderecoExterior
        {
            EnderecoPostal = "10001",
            Cidade = "New York",
            EstadoProvincia = "NY"
        };

        var xml = Xml(dps);

        await Assert.That(xml).Contains("<obra><end><endExt><cEndPost>10001</cEndPost>");
        await Assert.That(xml).DoesNotContain("<obra><end><CEP>");
    }

    [Test]
    public async Task ObraSemMunicipio_LancaExcecaoClara()
    {
        var dps = MontarDps();
        dps.Informacoes.Servico.Localidade.CodMunicipioPrestacao = null;
        dps.Informacoes.Servico.Localidade.CodPaisPrestacao = "US";

        await Assert.That(() => ISSNetDps.Adequar(Xml(dps), dps, () => throw new InvalidOperationException()))
            .Throws<InvalidOperationException>()
            .WithMessageContaining("cMun");
    }

    [Test]
    public async Task DpsSemObra_RetornaXmlInalterado()
    {
        var dps = MontarDps();
        dps.Informacoes.Servico.Obra = null;
        var nacional = Xml(dps);

        var xml = ISSNetDps.Adequar(nacional, dps, () => throw new InvalidOperationException());

        await Assert.That(ReferenceEquals(xml, nacional)).IsTrue();
    }

    [Test]
    public async Task DpsAssinada_EReassinadaComAssinaturaValida()
    {
        using var certificado = CriarCertificado();
        var dps = MontarDps();
        dps.GerarId();
        var assinado = XmlSigning.AssinarXml(Xml(dps), "DPS", "infDPS", certificado, false, false, false, SignDigest.SHA1);

        var xml = ISSNetDps.Adequar(assinado, dps, () => certificado);

        var documento = new XmlDocument { PreserveWhitespace = true };
        documento.LoadXml(xml);

        await Assert.That(xml).IsNotEqualTo(assinado);
        await Assert.That(documento.GetElementsByTagName("Signature", "http://www.w3.org/2000/09/xmldsig#").Count).IsEqualTo(1);
        await Assert.That(xml).Contains("<endNac>");
        await Assert.That(XmlSigning.ValidarAssinatura(documento)).IsTrue();
        await Assert.That(ValidarSchemaISSNet(xml, out var erros)).IsTrue().Because(string.Join(Environment.NewLine, erros));
    }

    /// <summary>Serializa a DPS com as mesmas opções usadas no envio para a ISSNet.</summary>
    private static string Xml(Dps dps)
    {
        dps.GerarId();
        return dps.GetXml(DFeSaveOptions.DisableFormatting | DFeSaveOptions.OmitDeclaration);
    }

    /// <summary>
    /// Valida a DPS contra o schema da ISSNet. A DPS não assinada é serializada com um
    /// <c>Signature</c> vazio, que é removido para validar apenas o conteúdo.
    /// </summary>
    private static bool ValidarSchemaISSNet(string xmlDps, out string[] erros)
    {
        var documento = new XmlDocument { PreserveWhitespace = true };
        documento.LoadXml(xmlDps);
        var assinatura = documento.GetElementsByTagName("Signature", "http://www.w3.org/2000/09/xmldsig#").OfType<XmlElement>().SingleOrDefault();
        if (assinatura != null && string.IsNullOrEmpty(assinatura["SignatureValue", assinatura.NamespaceURI]?.InnerText))
            assinatura.ParentNode!.RemoveChild(assinatura);

        var valido = XmlSchemaValidation.ValidarXml(ISSNetDps.EnvelopeValidacao(documento.OuterXml), CaminhoSchemaISSNet, out var errosSchema, out _);
        erros = errosSchema.ToArray();
        return valido;
    }

    private static X509Certificate2 CriarCertificado()
    {
        using var rsa = RSA.Create(2048);
        var requisicao = new CertificateRequest("CN=TESTE ISSNET:35229661000178", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        using var certificado = requisicao.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddDays(1));
        return X509CertificateLoader.LoadPkcs12(certificado.Export(X509ContentType.Pfx), null, X509KeyStorageFlags.Exportable);
    }

    /// <summary>
    /// Monta uma DPS v1.01 com o grupo obra, nos moldes da DPS rejeitada pela ISSNet (cTribNac 07.02.01).
    /// </summary>
    private static Dps MontarDps() => new()
    {
        Versao = VersaoNFSe.Ve101,
        Informacoes = new InfDps
        {
            TipoAmbiente = DFeTipoAmbiente.Homologacao,
            DhEmissao = DateTime.Now,
            LocalidadeEmitente = "3543402",
            Serie = "1",
            NumeroDps = "608",
            Competencia = DateTime.Now,
            TipoEmitente = EmitenteDps.Prestador,
            Prestador = new PrestadorDps
            {
                CNPJ = "35229661000178",
                InscricaoMunicipal = "20061727",
                Regime = new RegimeTributario
                {
                    OptanteSimplesNacional = OptanteSimplesNacional.NaoOptante,
                    RegimeEspecial = RegimeEspecial.Nenhum
                }
            },
            Tomador = new InfoPessoaNFSe
            {
                CNPJ = "28845640000111",
                Nome = "Tomador de Teste Ltda",
                Endereco = new EnderecoNFSe
                {
                    Logradouro = "Avenida Ricardo Vianna Borelli",
                    Numero = "10",
                    Bairro = "Setor Industrial",
                    Municipio = new MunicipioNacional
                    {
                        CEP = "14140000",
                        CodMunicipio = CodMunicipioPrestacao
                    }
                }
            },
            Servico = new ServicoNFSe
            {
                Localidade = new LocalidadeNFSe
                {
                    CodMunicipioPrestacao = CodMunicipioPrestacao
                },
                Informacoes = new InformacoesServico
                {
                    CodTributacaoNacional = "070201",
                    CodTributacaoMunicipio = "70226",
                    Descricao = "Contrato de manutencao preventiva"
                },
                Obra = new ObraNFSe
                {
                    InscricaoImobiliaria = null,
                    CodObra = null,
                    Endereco = new EnderecoSimplesNFSe
                    {
                        CEP = "14140000",
                        Logradouro = "Avenida Ricardo Vianna Borelli",
                        Numero = "10",
                        Bairro = "Setor Industrial"
                    }
                }
            },
            Valores = new ValoresDps
            {
                ValoresServico = new ValoresServico { Valor = 6950 },
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
            },
            IBSCBS = null
        }
    };
}
