using System.Collections.Generic;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Resposta do envio de lote de DPS de forma síncrona
/// (operação <c>recepcionarLoteDpsSincrono</c> do provedor Fiorilli),
/// contendo, além do protocolo, as NFS-e geradas.
/// </summary>
public sealed class RespostaRecepcaoLoteSincrono : RespostaRecepcaoLote
{
    /// <summary>
    /// NFS-e geradas a partir do lote enviado.
    /// </summary>
    public List<NotaFiscalServico> NotasFiscais { get; set; } = new();
}
