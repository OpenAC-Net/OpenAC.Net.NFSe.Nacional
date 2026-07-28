namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Filtro para consulta de lote de DPS (operação <c>consultarLoteDps</c> do provedor Fiorilli).
/// </summary>
public sealed class ConsultaLoteFiltro
{
    #region Properties

    /// <summary>
    /// Protocolo retornado no envio assíncrono do lote.
    /// </summary>
    public string Protocolo { get; set; } = string.Empty;

    /// <summary>
    /// CNPJ do transmissor do lote.
    /// </summary>
    public string? CNPJ { get; set; }

    /// <summary>
    /// CPF do transmissor do lote.
    /// </summary>
    public string? CPF { get; set; }

    /// <summary>
    /// Inscrição municipal do transmissor do lote.
    /// </summary>
    public string? IM { get; set; }

    #endregion Properties
}
