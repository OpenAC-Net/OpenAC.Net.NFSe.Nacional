using System.Text.RegularExpressions;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.DFe.Core.Extensions;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional.Test;

/// <summary>
/// Testes de conformidade com o schema v1.01 que rodam offline: não exigem <c>.env</c>,
/// certificado digital nem acesso aos webservices.
/// </summary>
/// <remarks>
/// <c>TCDPS</c> declara <c>ds:Signature</c> com <c>minOccurs="0"</c>, então uma DPS não assinada
/// valida contra <c>DPS_v1.01.xsd</c>. Isso permite cobrir a serialização sem depender de
/// infraestrutura externa.
/// </remarks>
[TestClass]
public class TestSchemaV101
{
    /// <summary>CNPJ alfanumérico, formato admitido pelo TSCNPJ a partir do layout v1.01.</summary>
    private const string CnpjAlfanumericoPrestador = "12ABC678000199";

    private const string CnpjAlfanumericoTomador = "98XYZ432000188";

    private const string CodMunicipio = "3550308";

    /// <summary>Padrão <c>TSIdDPS</c> do <c>tiposSimples_v1.01.xsd</c>.</summary>
    private static readonly Regex RegexIdDps =
        new(@"^DPS[0-9]{7}(1[0-9]{14}|2[0-9A-Z]{14})[0-9]{20}$", RegexOptions.CultureInvariant);

    private static string CaminhoSchemaDps =>
        Path.Combine(AppContext.BaseDirectory, "Schemas", VersaoNFSe.Ve101.GetDFeValue(), "DPS_v1.01.xsd");

    [TestMethod]
    public void DpsComCnpjAlfanumerico_PreservaLetrasNoXml()
    {
        var dps = MontarDps();
        dps.GerarId();

        var xml = dps.GetXml(DFeSaveOptions.DisableFormatting);

        StringAssert.Contains(xml, $"<CNPJ>{CnpjAlfanumericoPrestador}</CNPJ>",
            "O CNPJ do prestador perdeu as letras na serialização.");
        StringAssert.Contains(xml, $"<CNPJ>{CnpjAlfanumericoTomador}</CNPJ>",
            "O CNPJ do tomador perdeu as letras na serialização.");
    }

    [TestMethod]
    public void DpsComCnpjAlfanumerico_ValidaContraSchemaV101()
    {
        var dps = MontarDps();
        dps.GerarId();

        var xml = dps.GetXml(DFeSaveOptions.DisableFormatting);

        Assert.IsTrue(File.Exists(CaminhoSchemaDps), $"Schema não encontrado em {CaminhoSchemaDps}.");

        var valido = XmlSchemaValidation.ValidarXml(xml, CaminhoSchemaDps, out var erros, out _);

        Assert.IsTrue(valido, "XML reprovado no schema v1.01:" + Environment.NewLine +
                              string.Join(Environment.NewLine, erros));
    }

    [TestMethod]
    public void DocDedRed_UsaNomesDeElementoDoTCDocDedRed()
    {
        var dps = MontarDps();
        // TCInfoDedRed é um xs:choice: pDR, vDR ou documentos - preencher apenas um.
        dps.Informacoes.Valores.ValoresDeducaoReducao = new ValoresDeducaoReducao
        {
            Documentos =
            {
                new DocumentoDeducaoReducao
                {
                    ChaveNFe = new string('1', 6) + CnpjAlfanumericoTomador + new string('2', 24),
                    TipoDeducaoReducao = TipoDeducaoReducao.Materiais,
                    DataEmissao = DateTime.Today,
                    ValorDedutivelRedutivel = 10,
                    ValorReducaoDeducao = 10
                }
            }
        };
        dps.GerarId();

        var xml = dps.GetXml(DFeSaveOptions.DisableFormatting);

        StringAssert.Contains(xml, "<chNFe>", "A chave da NF-e deve ser gravada em <chNFe>, não em <chNFSe>.");
        StringAssert.Contains(xml, "<tpDedRed>", "O tipo de dedução/redução deve ser gravado em <tpDedRed>, não em <nDoc>.");

        var valido = XmlSchemaValidation.ValidarXml(xml, CaminhoSchemaDps, out var erros, out _);

        Assert.IsTrue(valido, "XML de dedução/redução reprovado no schema v1.01:" + Environment.NewLine +
                              string.Join(Environment.NewLine, erros));
    }

    [TestMethod]
    public void CnpjComFormatacao_EhNormalizadoParaAlfanumerico()
    {
        var pessoa = new InfoPessoaNFSe { CNPJ = "12.ABC.678/0001-99" };

        Assert.AreEqual(CnpjAlfanumericoPrestador, pessoa.CNPJ);
    }

    [TestMethod]
    public void GerarId_ComCnpj_MontaIdTipo2Com45Posicoes()
    {
        var dps = MontarDps();
        dps.Informacoes.Serie = "13";
        dps.Informacoes.NumeroDps = "7";

        dps.GerarId();

        var id = dps.Informacoes.Id;

        Assert.AreEqual(45, id.Length, $"Id gerado com tamanho inválido: '{id}'.");
        Assert.AreEqual("DPS", id.Substring(0, 3), "Literal inicial.");
        Assert.AreEqual(CodMunicipio, id.Substring(3, 7), "Código do município.");
        Assert.AreEqual("2", id.Substring(10, 1), "Tipo de inscrição federal (2 = CNPJ).");
        Assert.AreEqual(CnpjAlfanumericoPrestador, id.Substring(11, 14), "Inscrição federal.");
        Assert.AreEqual("00013", id.Substring(25, 5), "Série com 5 posições.");
        Assert.AreEqual("000000000000007", id.Substring(30, 15), "Número da DPS com 15 posições.");
        Assert.IsTrue(RegexIdDps.IsMatch(id), $"Id não casa com o padrão TSIdDPS: '{id}'.");
    }

    [TestMethod]
    public void GerarId_ComCpf_MontaIdTipo1Com45Posicoes()
    {
        var dps = MontarDps();
        dps.Informacoes.Prestador.CNPJ = null;
        dps.Informacoes.Prestador.CPF = "12345678909";
        dps.Informacoes.Serie = "1";
        dps.Informacoes.NumeroDps = "1";

        dps.GerarId();

        var id = dps.Informacoes.Id;

        Assert.AreEqual(45, id.Length, $"Id gerado com tamanho inválido: '{id}'.");
        Assert.AreEqual("1", id.Substring(10, 1), "Tipo de inscrição federal (1 = CPF).");
        Assert.AreEqual("00012345678909", id.Substring(11, 14), "CPF com 000 à esquerda.");
        Assert.AreEqual("00001", id.Substring(25, 5), "Série com 5 posições.");
        Assert.AreEqual("000000000000001", id.Substring(30, 15), "Número da DPS com 15 posições.");
        Assert.IsTrue(RegexIdDps.IsMatch(id), $"Id não casa com o padrão TSIdDPS: '{id}'.");
    }

    [TestMethod]
    public void GerarId_ComIdJaInformado_NaoSobrescreve()
    {
        var dps = MontarDps();
        dps.Informacoes.Id = "DPS35503082" + CnpjAlfanumericoPrestador + "0001300000000000007";

        var original = dps.Informacoes.Id;
        dps.GerarId();

        Assert.AreEqual(original, dps.Informacoes.Id);
    }

    /// <summary>
    /// Monta uma DPS v1.01 mínima e completa o suficiente para passar no schema.
    /// </summary>
    private static Dps MontarDps() => new()
    {
        Versao = VersaoNFSe.Ve101,
        Informacoes = new InfDps
        {
            TipoAmbiente = DFeTipoAmbiente.Homologacao,
            DhEmissao = DateTime.Now,
            LocalidadeEmitente = CodMunicipio,
            Serie = "13",
            NumeroDps = "7",
            Competencia = DateTime.Now,
            TipoEmitente = EmitenteDps.Prestador,
            Prestador = new PrestadorDps
            {
                CNPJ = CnpjAlfanumericoPrestador,
                Email = "prestador@teste.com",
                Regime = new RegimeTributario
                {
                    OptanteSimplesNacional = OptanteSimplesNacional.NaoOptante,
                    RegimeEspecial = RegimeEspecial.Nenhum
                }
            },
            Tomador = new InfoPessoaNFSe
            {
                CNPJ = CnpjAlfanumericoTomador,
                Nome = "Tomador de Teste Ltda",
                Endereco = new EnderecoNFSe
                {
                    Logradouro = "Avenida Paulista",
                    Numero = "1000",
                    Bairro = "Bela Vista",
                    Municipio = new MunicipioNacional
                    {
                        CEP = "01310100",
                        CodMunicipio = CodMunicipio
                    }
                }
            },
            Servico = new ServicoNFSe
            {
                Localidade = new LocalidadeNFSe
                {
                    CodMunicipioPrestacao = CodMunicipio
                },
                Informacoes = new InformacoesServico
                {
                    CodTributacaoNacional = "010101",
                    CodTributacaoMunicipio = "002",
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
            },
            IBSCBS = null
        }
    };
}
