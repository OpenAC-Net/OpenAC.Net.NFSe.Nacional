using System;
using QRCoder;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.QuestPDF.Src.Utilitarios;

/// <summary>
/// Responsável pela geração de códigos QR.
/// </summary>
internal static class QrCode
{
    /// <summary>
    /// Gera um QR Code em formato PNG.
    /// </summary>
    /// <param name="conteudo">
    /// Conteúdo que será codificado no QR Code.
    /// </param>
    /// <param name="tamanhoModulo">
    /// Tamanho de cada módulo do QR Code.
    /// </param>
    /// <returns>
    /// Imagem do QR Code em formato PNG.
    /// </returns>
    public static byte[] Gerar(string conteudo, int tamanhoModulo = 5)
    {
        if (string.IsNullOrWhiteSpace(conteudo))
        {
            throw new ArgumentException("O conteúdo do QR Code deve ser informado.", nameof(conteudo));
        }
        if (tamanhoModulo <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(tamanhoModulo), tamanhoModulo, "O tamanho do módulo deve ser maior que zero.");
        }
        using var gerador = new QRCodeGenerator();
        using var dados = gerador.CreateQrCode(conteudo, QRCodeGenerator.ECCLevel.M);
        using var qrCode = new PngByteQRCode(dados);
        return qrCode.GetGraphic(tamanhoModulo);
    }
}