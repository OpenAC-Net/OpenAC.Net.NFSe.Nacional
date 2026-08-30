using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional.Test
{
    public class TestConsulta
    {
        [Test]
        public async Task ConsultaEventoDeCancelamento()
        {
            var openNFSeNacional = new OpenNFSeNacional();
            SetupOpenNFSeNacional.ConfiguracaoModeloAtual(openNFSeNacional);

            var resposta = await openNFSeNacional.ConsultaEventoAsync("35260812345678000190550010000012341000012345", TipoEventoCod.Cancelamento, 1);

            await Assert.That(resposta).IsNotNull();
            await Assert.That(resposta?.Sucesso).IsTrue();
            await Assert.That(resposta?.Resultado).IsNotNull();
            await Assert.That(resposta?.Resultado?.Eventos).IsNotNull();
            await Assert.That(resposta?.Resultado?.Eventos?.Count).IsEqualTo(1);
            await Assert.That(resposta?.Resultado?.Eventos?.FirstOrDefault()?.ChaveAcesso).IsNotNullOrWhiteSpace();
            await Assert.That(resposta?.Resultado?.Eventos?.FirstOrDefault()?.TipoEvento).IsEqualTo(int.Parse(TipoEventoCod.Cancelamento));
            await Assert.That(resposta?.Resultado?.Eventos?.FirstOrDefault()?.NumeroPedidoRegistroEvento).IsNotDefault();
            await Assert.That(resposta?.Resultado?.Eventos?.FirstOrDefault()?.DataHoraRecebimento).IsNotDefault();
            await Assert.That(resposta?.Resultado?.Eventos?.FirstOrDefault()?.XmlEvento).IsNotNullOrWhiteSpace();
            await Assert.That(resposta?.Resultado?.Eventos?.FirstOrDefault()?.XmlEvento).StartsWith("<?xml");
        }
    }
}