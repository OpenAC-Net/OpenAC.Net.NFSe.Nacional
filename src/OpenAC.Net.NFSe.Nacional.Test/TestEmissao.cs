using OpenAC.Net.Core.Extensions;
using OpenAC.Net.NFSe.Nacional.Common;

namespace OpenAC.Net.NFSe.Nacional.Test
{
    [TestClass]
    public class TestEmissao
    {
        [TestMethod]
        public void TestarEmissaoNFSe()
        {
            var openNFSeNacional = new OpenNFSeNacional();
            SetupOpenNFSeNacional.SetSetup(openNFSeNacional);
            var numDPS = "1";
            var codMun = "3525300";
            var tipoInscricaoFederal = 1; //1 CNPJ; 2 CPF;
            var inscricaoFederal = "42250933000187";
            var serieDPS = "1";

            var prest = new PrestadorDps()
            {
                CNPJ = inscricaoFederal,
                Email = "teste@teste.com",
                Regime = new RegimeTributario()
                {
                    OptanteSimplesNacional = OptanteSimplesNacional.OptanteMEI,
                    RegimeEspecial = RegimeEspecial.Nenhum
                }
            };

            var toma = new InfoPessoaNFSe()
            {
                CNPJ = "52309133000148",
                Nome = "Lecom Automação de Processos",
                Endereco = new EnderecoNFSe
                {
                    Bairro = "Centro",
                    Logradouro = "Rua Conde do Pinhal",
                    Municipio = new MunicipioNacional
                    {
                        CEP = "17201040",
                        CodMunicipio = "3525300"
                    },
                    Numero = "375"
                }
            };

            var serv = new ServicoNFSe() 
            {
                Localidade = new LocalidadeNFSe()
                {
                    CodMunicipioPrestacao = "3525300"
                },
                Informacoes = new InformacoesServico
                {
                    CodTributacaoNacional = "080201",
                    Descricao = "Referente ao serviço prestado"
                }
            };

            var valores = new ValoresDps() 
            {
                ValoresServico = new ValoresServico
                {
                    Valor = 100
                },
                Tributos = new TributosNFSe
                {
                    Municipal = new TributoMunicipal
                    {
                        ISSQN = TributoISSQN.OperaçãoTributavel,
                        TipoRetencaoISSQN = TipoRetencaoISSQN.RetidoTomador,
                    },
                    Total = new TotalTributos
                    {
                        PorcentagemTotal = new PorcentagemTotalTributos()
                        {
                            TotalEstadual = 0,
                            TotalFederal = 0,
                            TotalMunicipal = 0,
                        }
                    }
                }
            };


            var dps = new Common.Dps();
            dps.Versao = "1.00";
            dps.Informacoes = new Common.InfDps()
            {
                Id = "DPS" + codMun + tipoInscricaoFederal + inscricaoFederal.PadLeft(14, '0') + serieDPS.PadLeft(5, '0') + numDPS.PadLeft(15, '0'),
                TipoAmbiente = DFe.Core.Common.DFeTipoAmbiente.Homologacao,
                DhEmissao = DateTime.Now,
                Serie = serieDPS,
                NumeroDps = numDPS.ToInt32(),
                Competencia = DateTime.Now,
                TipoEmitente = Common.EmitenteDps.Prestador,
                Prestador = prest,
                Tomador = toma,
                Servico = serv,
                Valores = valores
            };
            openNFSeNacional.EnviarAsync(dps);
        }
    }
}