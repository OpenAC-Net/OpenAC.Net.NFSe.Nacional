using OpenAC.Net.DFe.Core.Attributes;

namespace OpenAC.Net.NFSe.Nacional.Common.Types;

/// <summary>
/// Motivo da Emissão da DPS pelo Tomador/Intermediário
/// </summary>
public enum MotivoEmissaoTomadoIntermediario
{
    /// <summary>
    /// Importação de Serviço
    /// </summary>
    [DFeEnum("1")]
    ImportacaoServico = 1,

    /// <summary>
    /// Tomador/Intermediário obrigado a emitir NFS-e por legislação municipal
    /// </summary>
    [DFeEnum("2")]
    ObrigadoPorLegislacao = 2,

    /// <summary>
    /// Tomador/Intermediário emitindo NFS-e por recusa de emissão pelo prestador
    /// </summary>
    [DFeEnum("3")]
    RecusaEmissaoPrestador = 3,

    /// <summary>
    /// Tomador/Intermediário emitindo por rejeitar a NFS-e emitida pelo prestador
    /// </summary>
    [DFeEnum("4")]
    EmissaoPorRejeitarNFSePrestador = 4,
}