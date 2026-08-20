using OpenAC.Net.NFSe.Nacional.Common.Model;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Tests.Src.Comum;

public static class ModeloDados
{
    private static NotaFiscalServico? _nfse;

    private static NotaFiscalServico Nfse => _nfse ??= CarregadorNfse.Carregar("nota.xml");

    public static InfoPessoaNFSe Prestador => Nfse.Informacoes.Dps.Informacoes.Prestador;

    public static InfoPessoaNFSe Tomador => Nfse.Informacoes.Dps.Informacoes.Tomador!;

    public static EnderecoNFSe EnderecoTomador => Tomador.Endereco!;

    public static EnderecoEmitente EnderecoEmitente => Nfse.Informacoes.Emitente.Endereco!;

    public static RTCInfoDest? Destinatario => Nfse.Informacoes.Dps.Informacoes.IBSCBS?.Destinatario;
}
