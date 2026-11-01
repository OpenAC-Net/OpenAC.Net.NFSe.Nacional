using OpenAC.Net.DFe.Core.Attributes;

namespace OpenAC.Net.NFSe.Nacional.Common.Types;

/// <summary>
/// Indicador da finalidade da emissão de NFS-e
/// </summary>
public enum FinalidadeEmissaoNFSe
{
    /// <summary>
    /// 0 - NFS-e regular
    /// </summary>
    [DFeEnum("0")]
    NFSeRegular = 0,
}