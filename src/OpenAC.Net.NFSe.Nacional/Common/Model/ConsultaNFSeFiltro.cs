namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Filtro para consulta de NFS-e (operação <c>consultarNfse</c> do provedor Fiorilli).
/// Informe ao menos um critério de identificação (chave, Id do DPS ou número/série do DPS)
/// juntamente com a identificação do prestador quando aplicável.
/// </summary>
public sealed class ConsultaNFSeFiltro
{
    #region Properties

    /// <summary>
    /// CNPJ do prestador.
    /// </summary>
    public string? CNPJ { get; set; }

    /// <summary>
    /// CPF do prestador.
    /// </summary>
    public string? CPF { get; set; }

    /// <summary>
    /// Inscrição municipal do prestador.
    /// </summary>
    public string? IM { get; set; }

    /// <summary>
    /// Chave de acesso da NFS-e.
    /// </summary>
    public string? ChaveNFSe { get; set; }

    /// <summary>
    /// Identificador do DPS que gerou a NFS-e.
    /// </summary>
    public string? IdDPS { get; set; }

    /// <summary>
    /// Número do DPS.
    /// </summary>
    public string? NumeroDPS { get; set; }

    /// <summary>
    /// Série do DPS.
    /// </summary>
    public string? SerieDPS { get; set; }

    #endregion Properties
}
