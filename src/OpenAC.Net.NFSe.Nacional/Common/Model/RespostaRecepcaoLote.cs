using System;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Resposta do envio de lote de DPS de forma assíncrona
/// (operação <c>recepcionarLoteDps</c> do provedor Fiorilli).
/// </summary>
public class RespostaRecepcaoLote : RespostaSoapBase
{
    /// <summary>
    /// Número do lote recebido.
    /// </summary>
    public long NumeroLote { get; set; }

    /// <summary>
    /// Data e hora do recebimento do lote.
    /// </summary>
    public DateTimeOffset DataRecebimento { get; set; }

    /// <summary>
    /// Protocolo gerado para acompanhamento do processamento do lote.
    /// </summary>
    public string Protocolo { get; set; } = string.Empty;
}
