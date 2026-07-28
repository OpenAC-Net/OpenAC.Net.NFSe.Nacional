using System.Collections.Generic;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Representa um lote de DPS a ser transmitido para provedores que suportam envio em lote
/// (ex.: Fiorilli - operações <c>recepcionarLoteDps</c> e <c>recepcionarLoteDpsSincrono</c>).
/// </summary>
public sealed class LoteDps
{
    #region Properties

    /// <summary>
    /// Identificador opcional do lote (atributo <c>Id</c> do elemento <c>LoteDps</c>).
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Número do lote.
    /// </summary>
    public long NumeroLote { get; set; }

    /// <summary>
    /// CNPJ do transmissor do lote. Informar CNPJ ou CPF.
    /// </summary>
    public string? CNPJ { get; set; }

    /// <summary>
    /// CPF do transmissor do lote. Informar CNPJ ou CPF.
    /// </summary>
    public string? CPF { get; set; }

    /// <summary>
    /// Inscrição municipal do transmissor do lote.
    /// </summary>
    public string? IM { get; set; }

    /// <summary>
    /// Documentos DPS que compõem o lote.
    /// </summary>
    public List<Dps> Documentos { get; set; } = new();

    /// <summary>
    /// Quantidade de DPS no lote (derivado de <see cref="Documentos"/>).
    /// </summary>
    public int QuantidadeDps => Documentos.Count;

    #endregion Properties
}
