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

    [Test]
    public async Task DpsComCnpjAlfanumerico_PreservaLetrasNoXml()
    {
        var dps = MontarDps();
        dps.GerarId();

        var xml = dps.GetXml();

        await Assert.That(xml).Contains($"<CNPJ>{CnpjAlfanumericoPrestador}</CNPJ>");
        await Assert.That(xml).Contains($"<CNPJ>{CnpjAlfanumericoTomador}</CNPJ>");
    }

    [Test]
    public async Task DpsComCnpjAlfanumerico_ValidaContraSchemaV101()
    {
        var dps = MontarDps();
        dps.GerarId();

        var xml = dps.GetXml();

        await Assert.That(File.Exists(CaminhoSchemaDps)).IsTrue();

        var valido = XmlSchemaValidation.ValidarXml(xml, CaminhoSchemaDps, out var erros, out _);

        await Assert.That(valido).IsTrue();
    }

    [Test]
    public async Task DocDedRed_UsaNomesDeElementoDoTCDocDedRed()
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

        var xml = dps.GetXml();

        await Assert.That(xml).Contains("<chNFe>");
        await Assert.That(xml).Contains("<tpDedRed>");

        var valido = XmlSchemaValidation.ValidarXml(xml, CaminhoSchemaDps, out var erros, out _);

        await Assert.That(valido).IsTrue();
    }

    [Test]
    public async Task CnpjComFormatacao_EhNormalizadoParaAlfanumerico()
    {
        var pessoa = new InfoPessoaNFSe { CNPJ = "12.ABC.678/0001-99" };

        await Assert.That(pessoa.CNPJ).IsEqualTo(CnpjAlfanumericoPrestador);
    }

    [Test]
    public async Task GerarId_ComCnpj_MontaIdTipo2Com45Posicoes()
    {
        var dps = MontarDps();
        dps.Informacoes.Serie = "13";
        dps.Informacoes.NumeroDps = "7";

        dps.GerarId();

        var id = dps.Informacoes.Id;

        await Assert.That(id.Length).IsEqualTo(45);
        await Assert.That(id.Substring(0, 3)).IsEqualTo("DPS");
        await Assert.That(id.Substring(3, 7)).IsEqualTo(CodMunicipio);
        await Assert.That(id.Substring(10, 1)).IsEqualTo("2");
        await Assert.That(id.Substring(11, 14)).IsEqualTo(CnpjAlfanumericoPrestador);
        await Assert.That(id.Substring(25, 5)).IsEqualTo("00013");
        await Assert.That(id.Substring(30, 15)).IsEqualTo("000000000000007");
        await Assert.That(RegexIdDps.IsMatch(id)).IsTrue();
    }

    [Test]
    public async Task GerarId_ComCpf_MontaIdTipo1Com45Posicoes()
    {
        var dps = MontarDps();
        dps.Informacoes.Prestador.CNPJ = null;
        dps.Informacoes.Prestador.CPF = "12345678909";
        dps.Informacoes.Serie = "1";
        dps.Informacoes.NumeroDps = "1";

        dps.GerarId();

        var id = dps.Informacoes.Id;

        await Assert.That(id.Length).IsEqualTo(45);
        await Assert.That(id.Substring(10, 1)).IsEqualTo("1");
        await Assert.That(id.Substring(11, 14)).IsEqualTo("00012345678909");
        await Assert.That(id.Substring(25, 5)).IsEqualTo("00001");
        await Assert.That(id.Substring(30, 15)).IsEqualTo("000000000000001");
        await Assert.That(RegexIdDps.IsMatch(id)).IsTrue();
    }

    [Test]
    public async Task GerarId_ComIdJaInformado_NaoSobrescreve()
    {
        var dps = MontarDps();
        dps.Informacoes.Id = "DPS35503082" + CnpjAlfanumericoPrestador + "0001300000000000007";

        var original = dps.Informacoes.Id;
        dps.GerarId();

        await Assert.That(dps.Informacoes.Id).IsEqualTo(original);
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
