using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional.Test;

/// <summary>
/// Testes de emissão simples de NFS-e pelo provedor Fiorilli (webservice SOAP).
/// Requerem as variáveis com prefixo "Fiorilli" no arquivo .env (código do município,
/// inscrições, certificado e, opcionalmente, o endpoint de homologação).
/// </summary>
public class TestEmissaoFiorilli
{
    /// <summary>
    /// Emissão simples com tomador (operação recepcionarDps).
    /// </summary>
    [Test]
    public async Task EmissaoNFSeFiorilli()
    {
        var openNFSeNacional = new OpenNFSeNacional();
        SetupOpenNFSeNacional.ConfigurarFiorilli(openNFSeNacional);

        var prest = new PrestadorDps
        {
            CNPJ = SetupOpenNFSeNacional.InscricaoFederal,
            InscricaoMunicipal = SetupOpenNFSeNacional.InscricaoMunicipal,
            Email = "teste@teste.com",
            Regime = new RegimeTributario
            {
                OptanteSimplesNacional = OptanteSimplesNacional.NaoOptante,
                RegimeEspecial = RegimeEspecial.Nenhum
            }
        };

        var toma = SetupOpenNFSeNacional.ObterTomador("2");

        var serv = new ServicoNFSe
        {
            Localidade = new LocalidadeNFSe
            {
                // Serviço prestado no município do provedor Fiorilli.
                CodMunicipioPrestacao = SetupOpenNFSeNacional.CodMunIBGE
            },
            Informacoes = new InformacoesServico
            {
                CodTributacaoNacional = "070201",
                //CodTributacaoMunicipio = "0000070200001",
                Descricao = "Referente ao servico prestado de Desenvolvimento"
            },
            Obra = new ObraNFSe()
            {
                Endereco = new EnderecoSimplesNFSe()
                {
                    CEP = "17250000",
                    Logradouro = "ESTRADA MUNICIPAL ENTRE BARIRI x ITAJU",
                    Numero = "s/n",
                    Bairro = "ESTRADA"
                }
            }
        };

        var valores = new ValoresDps
        {
            ValoresServico = new ValoresServico
            {
                Valor = 1
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
                IBSCBS = null,
            }
        };

        var retorno = await openNFSeNacional.EnviarAsync(dps);

        await Assert.That(retorno.Sucesso).IsTrue();
    }

    /// <summary>
    /// Emissão simples sem tomador (operação recepcionarDps).
    /// </summary>
    [Test]
    public async Task EmissaoNFSeFiorilliSemTomador()
    {
        var openNFSeNacional = new OpenNFSeNacional();
        SetupOpenNFSeNacional.ConfigurarFiorilli(openNFSeNacional, "2");

        var prest = new PrestadorDps
        {
            CNPJ = SetupOpenNFSeNacional.InscricaoFederal,
            InscricaoMunicipal = SetupOpenNFSeNacional.InscricaoMunicipal,
            Email = "teste@teste.com",
            Regime = new RegimeTributario
            {
                OptanteSimplesNacional = OptanteSimplesNacional.NaoOptante,
                RegimeEspecial = RegimeEspecial.Nenhum
            }
        };

        var serv = new ServicoNFSe
        {
            Localidade = new LocalidadeNFSe
            {
                CodMunicipioPrestacao = SetupOpenNFSeNacional.CodMunIBGE
            },
            Informacoes = new InformacoesServico
            {
                CodTributacaoNacional = "010101",
                //CodTributacaoMunicipio = "002",
                Descricao = "Servico sem tomador - emissao simples"
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
                Tomador = null,
                Servico = serv,
                Valores = valores,
                IBSCBS = null,
            }
        };

        var retorno = await openNFSeNacional.EnviarAsync(dps);

        await Assert.That(retorno.Sucesso).IsTrue();
    }
    
    [Test]
    public async Task CancelamentoNFSeFiorilli()
    {
        var openNFSeNacional = new OpenNFSeNacional();
        SetupOpenNFSeNacional.ConfigurarFiorilli(openNFSeNacional);

        var chaveNFse = "35052031206345083000137000000000000326070068624920";

        var cancelamento = new EventoCancelamento
        {
            CodMotivo = MotivoCancelamento.ErroEmissao,
            Motivo = "Dados inválidos"
        };

        var evento = new PedidoRegistroEvento();
        evento.Versao = openNFSeNacional.Configuracoes.Geral.Versao;
        evento.Informacoes = new()
        {
            Id = "PRE" + chaveNFse + TipoEventoCod.Cancelamento,
            ChNFSe = chaveNFse,
            CNPJAutor = SetupOpenNFSeNacional.InscricaoFederal,
            DhEvento = DateTime.Now,
            TipoAmbiente = DFe.Core.Common.DFeTipoAmbiente.Homologacao,
            Evento = cancelamento
        };

        var retorno = await openNFSeNacional.EnviarEventoAsync(evento);

        await Assert.That(retorno.Sucesso).IsTrue();
    }
}
