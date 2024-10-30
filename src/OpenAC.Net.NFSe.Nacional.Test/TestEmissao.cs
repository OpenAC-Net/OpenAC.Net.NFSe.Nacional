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
        SetupOpenNFSeNacional.Configuracao(openNFSeNacional);

        var prest = new PrestadorDps()
        {
            CNPJ = SetupOpenNFSeNacional.InscricaoFederal,
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
                    TipoRetencaoISSQN = TipoRetencaoISSQN.NaoRetido,
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


        var dps = new Dps
        {
            Versao = "1.00",
            Informacoes = new InfDps
            {
                Id = "DPS" + SetupOpenNFSeNacional.CodMun +
                     SetupOpenNFSeNacional.TipoInscricaoFederal +
                     SetupOpenNFSeNacional.InscricaoFederal.PadLeft(14, '0') +
                     SetupOpenNFSeNacional.SerieDPS.PadLeft(5, '0') +
                     SetupOpenNFSeNacional.NumDPS.PadLeft(15, '0'),
                TipoAmbiente = DFe.Core.Common.DFeTipoAmbiente.Homologacao,
                DhEmissao = DateTime.Now,
                LocalidadeEmitente = SetupOpenNFSeNacional.CodMun,
                Serie = SetupOpenNFSeNacional.SerieDPS,
                NumeroDps = SetupOpenNFSeNacional.NumDPS.ToInt32(),
                Competencia = DateTime.Now,
                TipoEmitente = EmitenteDps.Prestador,
                Prestador = prest,
                Tomador = toma,
                Servico = serv,
                Valores = valores
            }
        };
        var retorno = await openNFSeNacional.EnviarAsync(dps);

        Assert.IsTrue(retorno.Sucesso);
    }

    [TestMethod]
    public async Task CancelamentoNFSe()
    {
        var openNFSeNacional = new OpenNFSeNacional();
        SetupOpenNFSeNacional.Configuracao(openNFSeNacional);

        var chaveNFse = "35253002242250933000187000000000000324057909658427";

        var cancelamento = new EventoCancelamento()
        {
            CodMotivo = MotivoCancelamento.ErroEmissao,
            Motivo = "Dados inválidos"
        };

        var evento = new PedidoRegistroEvento();
        evento.Versao = "1.00";
        evento.Informacoes = new()
        {
            Id = "PRE" + chaveNFse + TipoEvento.Cancelamento + SetupOpenNFSeNacional.NumEvento.PadLeft(3, '0'),
            ChNFSe = chaveNFse,
            CNPJAutor = SetupOpenNFSeNacional.InscricaoFederal,
            DhEvento = DateTime.Now,
            TipoAmbiente = DFe.Core.Common.DFeTipoAmbiente.Homologacao,
            NumeroPedido = SetupOpenNFSeNacional.NumEvento.ToInt32(),
            Evento = cancelamento
        };

        var retorno = await openNFSeNacional.EnviarAsync(evento);

        Assert.IsTrue(retorno.Sucesso);
    }
    
    /*[TestMethod]
    public async Task CancelamentoNFSePorSubstituicao()
    {
        var openNFSeNacional = new OpenNFSeNacional();
        SetupOpenNFSeNacional.Configuracao(openNFSeNacional);

        var chaveNFse = "35253002242250933000187000000000000524050500187874";

        var cancelamentoPorSubstituicao = new EventoCancelamentoPorSubstituicao()
        {
            CodMotivo = JustificativaSubstituicao.Outros,
            Motivo = "Dados Inválidos",
            ChaveSubstituta = "35253002242250933000187000000000000624050073048925"
        };

        var evento = new PedidoRegistroEvento();
        evento.Versao = "1.00";
        evento.Informacoes = new()
        {
            Id = "PRE" + chaveNFse + TipoEvento.CancelamentoPorSubstituicao + SetupOpenNFSeNacional.NumEvento.PadLeft(3, '0'),
            ChNFSe = chaveNFse,
            CNPJAutor = SetupOpenNFSeNacional.InscricaoFederal,
            DhEvento = DateTime.Now,
            TipoAmbiente = DFe.Core.Common.DFeTipoAmbiente.Homologacao,
            NumeroPedido = SetupOpenNFSeNacional.NumEvento.ToInt32(),
            Evento = cancelamentoPorSubstituicao
        };

        var retorno = await openNFSeNacional.EnviarAsync(evento);

        Assert.IsTrue(retorno.Sucesso);
    }*/
    
    [TestMethod]
    public async Task SolicitacaoCancelamentoNFSe()
    {
        var openNFSeNacional = new OpenNFSeNacional();
        SetupOpenNFSeNacional.Configuracao(openNFSeNacional);

        var chaveNFse = "35253002242250933000187000000000000724054029982347";

        var solicitacaoCancelamento = new EventoSolicitacaoCancelamento()
        {
            CodMotivo = JustificativaAnalise.Outros,
            Motivo = "Dados Inválidos",
        };

        var evento = new PedidoRegistroEvento();
        evento.Versao = "1.00";
        evento.Informacoes = new()
        {
            Id = "PRE" + chaveNFse + TipoEvento.SolicitacaoCancelamento + SetupOpenNFSeNacional.NumEvento.PadLeft(3, '0'),
            ChNFSe = chaveNFse,
            CNPJAutor = SetupOpenNFSeNacional.InscricaoFederal,
            DhEvento = DateTime.Now,
            TipoAmbiente = DFe.Core.Common.DFeTipoAmbiente.Homologacao,
            NumeroPedido = SetupOpenNFSeNacional.NumEvento.ToInt32(),
            Evento = solicitacaoCancelamento
        };

        var retorno = await openNFSeNacional.EnviarAsync(evento);

        Assert.IsTrue(retorno.Sucesso);
    }
}