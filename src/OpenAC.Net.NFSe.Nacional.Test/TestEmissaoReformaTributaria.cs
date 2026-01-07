using OpenAC.Net.Core.Extensions;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.Common.Types;
using OpenAC.Net.NFSe.Nacional.Test.Extensions;
using System.Diagnostics;

namespace OpenAC.Net.NFSe.Nacional.Test;

/// <summary>
/// Testes para a estrutura de impostos da Reforma Tributária (IBS/CBS) - v1.01
/// </summary>
[TestClass]
public class TestEmissaoReformaTributaria
{
    [TestMethod]
    public async Task EmissaoNFSeComIBSCBS_AliquotaReduzida()
    {
        var openNFSeNacional = new OpenNFSeNacional();
        SetupOpenNFSeNacional.ConfiguracaoModeloNovoCenario2(openNFSeNacional, "20", "1", "1");

        var prest = new PrestadorDps
        {
            CNPJ = SetupOpenNFSeNacional.InscricaoFederal,
            Email = "teste@teste.com",
            Regime = new RegimeTributario
            {
                OptanteSimplesNacional = OptanteSimplesNacional.NaoOptante,
                RegimeEspecial = RegimeEspecial.SociedadeProfissionais
            }
        };


        // Ou obtenha um tomador específico:
        var toma = SetupOpenNFSeNacional.ObterTomador("2"); // Para Tomador2

        var serv = new ServicoNFSe
        {
            Localidade = new LocalidadeNFSe
            {
                CodMunicipioPrestacao = SetupOpenNFSeNacional.CodMunIBGE
            },
            Informacoes = new InformacoesServico
            {
                CodNBS = "113011000",
                CodTributacaoNacional = "171401",
                //CodTributacaoMunicipio = "001",
                Descricao = "HHDIR - Honorários - HORA  DIRETOR - R$ 85000,00"
            }
        };


        var valores = new ValoresDps
        {
            ValoresServico = new ValoresServico
            {
                Valor = 85000
            },
            Tributos = new TributosNFSe
            {
                Municipal = new TributoMunicipal
                {
                    ISSQN = TributoISSQN.OperacaoTributavel,
                    TipoRetencaoISSQN = TipoRetencaoISSQN.NaoRetido,
                },
                Total = new TotalTributos
                {
                    PorcentagemTotal = new PorcentagemTotalTributos
                    {
                        TotalEstadual = 0,
                        TotalFederal = 13.45m,
                        TotalMunicipal = 4.07m,
                    }
                }
            }
        };

        var ibscbs = new RTCInfoIBSCBS
        {
            FinalidadeNFSe = RTCFinNFSe.Regular,
            IndicadorUsoFinal = RTCIndFinal.Nao,
            CodigoIndicadorOperacao = "100301",
            IndicadorDestinatario = RTCIndDest.ProprioTomador,
            Valores = new RTCInfoValoresIBSCBS
            {
                Tributos = new RTCInfoTributosIBSCBS
                {
                    GrupoIBSCBS = new RTCInfoTributosSitClas
                    {

                        CodigoSituacaoTributaria = "200",
                        CodigoClassificacaoTributaria = "200052"
                    }
                }
            }
        };

        var dps = new Dps
        {
            Versao = openNFSeNacional.Configuracoes.Geral.Versao, // Versão da reforma tributária
            Informacoes = new InfDps
            {
                Id = "DPS" + SetupOpenNFSeNacional.CodMunIBGE +
                SetupOpenNFSeNacional.TipoInscricaoFederal +
                SetupOpenNFSeNacional.InscricaoFederal.PadLeft(14, '0') +
                SetupOpenNFSeNacional.SerieDPS.PadLeft(5, '0') +
                SetupOpenNFSeNacional.NumDPS.PadLeft(15, '0'),
                TipoAmbiente = DFe.Core.Common.DFeTipoAmbiente.Homologacao,
                DhEmissao = DateTime.Now,
                LocalidadeEmitente = SetupOpenNFSeNacional.CodMunIBGE,
                Serie = SetupOpenNFSeNacional.SerieDPS,
                NumeroDps = SetupOpenNFSeNacional.NumDPS,
                Competencia =  DateTime.Now,
                TipoEmitente = EmitenteDps.Prestador,
                Prestador = prest,
                Tomador = toma,
                Servico = serv,
                Valores = valores,
                IBSCBS = ibscbs,
            }
        };
        var retorno = await openNFSeNacional.EnviarAsync(dps);

        Assert.IsTrue(retorno.Sucesso, "A emissão com Aliquota Reduzida deveria ter sucesso");

    }



    [TestMethod]
    public async Task EmissaoNFSeComIBSCBS_ComReembolso()
    {
        var openNFSeNacional = new OpenNFSeNacional();
        SetupOpenNFSeNacional.ConfiguracaoModeloNovoCenario1(openNFSeNacional);

        var prest = new PrestadorDps
        {
            CNPJ = SetupOpenNFSeNacional.InscricaoFederal,
            Email = "teste@teste.com",
            Regime = new RegimeTributario
            {
                OptanteSimplesNacional = OptanteSimplesNacional.NaoOptante,
                RegimeEspecial = RegimeEspecial.Nenhum
            }
        };

        // Usa Tomador2
        var toma = SetupOpenNFSeNacional.ObterTomador("2");

        var serv = new ServicoNFSe
        {
            Localidade = new LocalidadeNFSe
            {
                CodMunicipioPrestacao = SetupOpenNFSeNacional.CodMunIBGE
            },
            Informacoes = new InformacoesServico
            {
                CodTributacaoNacional = "010101",
                CodTributacaoMunicipio = "002",
                Descricao = "Serviço com reembolso de despesas"
            }
        };

        var valores = new ValoresDps
        {
            ValoresServico = new ValoresServico
            {
                Valor = 5000.00m
            },
            Tributos = new TributosNFSe
            {
                Municipal = new TributoMunicipal
                {
                    ISSQN = TributoISSQN.OperacaoTributavel,
                    TipoRetencaoISSQN = TipoRetencaoISSQN.NaoRetido,
                },
                Total = new TotalTributos
                {
                    PorcentagemTotal = new PorcentagemTotalTributos
                    {
                        TotalEstadual = 0,
                        TotalFederal = 0,
                        TotalMunicipal = 0,
                    }
                }
            }
        };

        var dps = new Dps
        {
            Versao = openNFSeNacional.Configuracoes.Geral.Versao,
            Informacoes = new InfDps
            {
                Id = "DPS" + SetupOpenNFSeNacional.CodMunIBGE +
               SetupOpenNFSeNacional.TipoInscricaoFederal +
                SetupOpenNFSeNacional.InscricaoFederal.PadLeft(14, '0') +
          SetupOpenNFSeNacional.SerieDPS.PadLeft(5, '0') +
                       SetupOpenNFSeNacional.NumDPS.PadLeft(15, '0'),
                TipoAmbiente = DFe.Core.Common.DFeTipoAmbiente.Homologacao,
                DhEmissao = DateTime.Now,
                LocalidadeEmitente = SetupOpenNFSeNacional.CodMunIBGE,
                Serie = SetupOpenNFSeNacional.SerieDPS,
                NumeroDps = SetupOpenNFSeNacional.NumDPS,
                Competencia = DateTime.Now,
                TipoEmitente = EmitenteDps.Prestador,
                Prestador = prest,
                Tomador = toma,
                Servico = serv,
                Valores = valores,
                IBSCBS = new RTCInfoIBSCBS
                {
                    FinalidadeNFSe = RTCFinNFSe.Regular,
                    IndicadorUsoFinal = RTCIndFinal.Nao,
                    CodigoIndicadorOperacao = "000001",
                    IndicadorDestinatario = RTCIndDest.ProprioTomador,
                    Valores = new RTCInfoValoresIBSCBS
                    {
                        Tributos = new RTCInfoTributosIBSCBS
                        {
                            GrupoIBSCBS = new RTCInfoTributosSitClas
                            {
                                CodigoSituacaoTributaria = "101",
                                CodigoClassificacaoTributaria = "000001"
                            }
                        }
                    }
                },
            }
        };

        var retorno = await openNFSeNacional.EnviarAsync(dps);

        Assert.IsTrue(retorno.Sucesso, "A emissão com reembolso deveria ter sucesso");
    }

    [TestMethod]
    public async Task EmissaoNFSeComIBSCBS_ComDestinatarioDiferente()
    {
        var openNFSeNacional = new OpenNFSeNacional();
        SetupOpenNFSeNacional.ConfiguracaoModeloNovoCenario1(openNFSeNacional);

        var prest = new PrestadorDps
        {
            CNPJ = SetupOpenNFSeNacional.InscricaoFederal,
            Email = "teste@teste.com",
            Regime = new RegimeTributario
            {
                OptanteSimplesNacional = OptanteSimplesNacional.NaoOptante,
                RegimeEspecial = RegimeEspecial.Nenhum
            }
        };

        // Usa Tomador2
        var toma = SetupOpenNFSeNacional.ObterTomador("2");

        var serv = new ServicoNFSe
        {
            Localidade = new LocalidadeNFSe
            {
                CodMunicipioPrestacao = "3304557"
            },
            Informacoes = new InformacoesServico
            {
                CodTributacaoNacional = "010101",
                CodTributacaoMunicipio = "002",
                Descricao = "Serviço com destinatário diferente do tomador"
            }
        };

        var valores = new ValoresDps
        {
            ValoresServico = new ValoresServico
            {
                Valor = 2000.00m
            },
            Tributos = new TributosNFSe
            {
                Municipal = new TributoMunicipal
                {
                    ISSQN = TributoISSQN.OperacaoTributavel,
                    TipoRetencaoISSQN = TipoRetencaoISSQN.NaoRetido,
                },
                Total = new TotalTributos
                {
                    PorcentagemTotal = new PorcentagemTotalTributos
                    {
                        TotalEstadual = 0,
                        TotalFederal = 0,
                        TotalMunicipal = 0,
                    }
                }
            }
        };

        var dps = new Dps
        {
            Versao = openNFSeNacional.Configuracoes.Geral.Versao,
            Informacoes = new InfDps
            {
                Id = "DPS" + SetupOpenNFSeNacional.CodMunIBGE +
                SetupOpenNFSeNacional.TipoInscricaoFederal +
                SetupOpenNFSeNacional.InscricaoFederal.PadLeft(14, '0') +
                SetupOpenNFSeNacional.SerieDPS.PadLeft(5, '0') +
                SetupOpenNFSeNacional.NumDPS.PadLeft(15, '0'),
                TipoAmbiente = DFe.Core.Common.DFeTipoAmbiente.Homologacao,
                DhEmissao = DateTime.Now,
                LocalidadeEmitente = SetupOpenNFSeNacional.CodMunIBGE,
                Serie = SetupOpenNFSeNacional.SerieDPS,
                NumeroDps = SetupOpenNFSeNacional.NumDPS,
                Competencia = DateTime.Now,
                TipoEmitente = EmitenteDps.Prestador,
                Prestador = prest,
                Tomador = toma,
                Servico = serv,
                Valores = valores,
                IBSCBS = new RTCInfoIBSCBS
                {
                    FinalidadeNFSe = RTCFinNFSe.Regular,
                    IndicadorUsoFinal = RTCIndFinal.Nao,
                    CodigoIndicadorOperacao = "000001",
                    IndicadorDestinatario = RTCIndDest.Outro,
                    // Usa Tomador3 como destinatário diferente
                    Destinatario = SetupOpenNFSeNacional.ObterTomador("3").ToRTCInfoDest(),
                    Valores = new RTCInfoValoresIBSCBS
                    {
                        Tributos = new RTCInfoTributosIBSCBS
                        {
                            GrupoIBSCBS = new RTCInfoTributosSitClas
                            {
                                CodigoSituacaoTributaria = "101",
                                CodigoClassificacaoTributaria = "000001"
                            }
                        }
                    }
                },
            }
        };

        var retorno = await openNFSeNacional.EnviarAsync(dps);

        Assert.IsTrue(retorno.Sucesso, "A emissão com destinatário diferente deveria ter sucesso");
    }

    private void OnEnvioSucesso(dynamic retorno)
    {
        Debug.WriteLine("Envio realizado com sucesso!");

        // Validações esperadas baseadas no JSON de output
        Assert.IsTrue(retorno.Sucesso, "A emissão deveria ter sucesso");

        // TODO: Validar valores calculados pelo sistema:
        // - IBS UF: R$ 0,70 (base R$ 1000 * 0,10% * redução 30% = 0,07%)
        // - IBS Municipal: R$ 0,00 (alíquota 0%)
        // - CBS: R$ 6,30 (base R$ 1000 * 0,90% * redução 30% = 0,63%)
        // - Total IBS: R$ 0,70
    }

}
