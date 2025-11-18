using OpenAC.Net.Core.Extensions;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional.Test;

[TestClass]
public class TestEmissao
{
    [TestMethod]
    public async Task EmissaoNFSe()
    {
        var openNFSeNacional = new OpenNFSeNacional();
        SetupOpenNFSeNacional.ConfiguracaoModeloAtual(openNFSeNacional, "1", "11", "1");

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

        // Obtém Tomador2 do .env
        var toma = SetupOpenNFSeNacional.ObterTomador("2");

        var serv = new ServicoNFSe
        {
            Localidade = new LocalidadeNFSe
            {
                // Usa o município do tomador
                CodMunicipioPrestacao = (toma?.Endereco?.Municipio is MunicipioNacional mn ? mn.CodMunicipio : null) ??
                                        SetupOpenNFSeNacional.CodMunIBGE
            },
            Informacoes = new InformacoesServico
            {
                CodTributacaoNacional = "010101",
                CodTributacaoMunicipio = "002",
                Descricao = "Referente ao serviço prestado de Desenvolvimento"
            }
        };

        var valores = new ValoresDps
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
            Versao = "1.00",
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
                NumeroDps = SetupOpenNFSeNacional.NumDPS.ToInt32(),
                Competencia = DateTime.Now,
                TipoEmitente = EmitenteDps.Prestador,
                Prestador = prest,
                Tomador = toma,
                Servico = serv,
                Valores = valores,
                IBSCBS = null,
            }
        };

        var retorno = await openNFSeNacional.EnviarAsync(dps);

        Assert.IsTrue(retorno.Sucesso);
    }

    [TestMethod]
    public async Task EmissaoNFSeOutroTomadorCenario2()
    {
        var openNFSeNacional = new OpenNFSeNacional();
        SetupOpenNFSeNacional.ConfiguracaoModeloAtual(openNFSeNacional, "1", "1", "1");

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

        var toma = SetupOpenNFSeNacional.ObterTomador("2");

        var serv = new ServicoNFSe
        {
            Localidade = new LocalidadeNFSe
            {
                // Usa o município do tomador
                CodMunicipioPrestacao = (toma?.Endereco?.Municipio is MunicipioNacional mn ? mn.CodMunicipio : null) ??
                                        SetupOpenNFSeNacional.CodMunIBGE
            },
            Informacoes = new InformacoesServico
            {
                CodTributacaoNacional = "171401",
                CodTributacaoMunicipio = "001",
                Descricao = "HHDIR - Honorários - HORA – DIRETOR - R$ 85000,00"
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
                Federal = new TributoFederal
                {
                    PisCofins = new PisCofins()
                    {
                        Cst = TipoCST.AliquotaBasica,
                        ValorBcCofins = 85000,
                        AliquotaCofins = 7.6m,
                        AliquotaPis = 1.65m,
                        ValorCofins = 6460,
                        ValorPis = 1402.5m,
                    }
                },
                Municipal = new TributoMunicipal
                {
                    ISSQN = TributoISSQN.OperaçãoTributavel,
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

        var dps = new Dps
        {
            Versao = "1.00",
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
                NumeroDps = SetupOpenNFSeNacional.NumDPS.ToInt32(),
                Competencia = DateTime.Now,
                TipoEmitente = EmitenteDps.Prestador,
                Prestador = prest,
                Tomador = toma,
                Servico = serv,
                Valores = valores,
                IBSCBS = null,
            }
        };

        var retorno = await openNFSeNacional.EnviarAsync(dps);

        Assert.IsTrue(retorno.Sucesso);
    }

    [TestMethod]
    public async Task CancelamentoNFSe()
    {
        var openNFSeNacional = new OpenNFSeNacional();
        SetupOpenNFSeNacional.ConfiguracaoModeloAtual(openNFSeNacional);

        var chaveNFse = "35253002242250933000187000000000000324057909658427";

        var cancelamento = new EventoCancelamento
        {
            CodMotivo = MotivoCancelamento.ErroEmissao,
            Motivo = "Dados inválidos"
        };

        var evento = new PedidoRegistroEvento();
        evento.Versao = "1.00";
        evento.Informacoes = new()
        {
            Id = "PRE" + chaveNFse + TipoEventoCod.Cancelamento + SetupOpenNFSeNacional.NumEvento.PadLeft(3, '0'),
            ChNFSe = chaveNFse,
            CNPJAutor = SetupOpenNFSeNacional.InscricaoFederal,
            DhEvento = DateTime.Now,
            TipoAmbiente = DFe.Core.Common.DFeTipoAmbiente.Homologacao,
            NumeroPedido = SetupOpenNFSeNacional.NumEvento.ToInt32(),
            Evento = cancelamento
        };

        var retorno = await openNFSeNacional.EnviarEventoAsync(evento);

        Assert.IsTrue(retorno.Sucesso);
    }

    [TestMethod]
    public async Task SolicitacaoCancelamentoNFSe()
    {
        var openNFSeNacional = new OpenNFSeNacional();
        SetupOpenNFSeNacional.ConfiguracaoModeloAtual(openNFSeNacional);

        var chaveNFse = "35253002242250933000187000000000000724054029982347";

        var solicitacaoCancelamento = new EventoSolicitacaoCancelamento
        {
            CodMotivo = JustificativaAnalise.Outros,
            Motivo = "Dados Inválidos",
        };

        var evento = new PedidoRegistroEvento();
        evento.Versao = "1.00";
        evento.Informacoes = new()
        {
            Id = "PRE" + chaveNFse + TipoEventoCod.SolicitacaoCancelamento + SetupOpenNFSeNacional.NumEvento.PadLeft(3, '0'),
            ChNFSe = chaveNFse,
            CNPJAutor = SetupOpenNFSeNacional.InscricaoFederal,
            DhEvento = DateTime.Now,
            TipoAmbiente = DFe.Core.Common.DFeTipoAmbiente.Homologacao,
            NumeroPedido = SetupOpenNFSeNacional.NumEvento.ToInt32(),
            Evento = solicitacaoCancelamento
        };

        var retorno = await openNFSeNacional.EnviarEventoAsync(evento);

        Assert.IsTrue(retorno.Sucesso);
    }
}