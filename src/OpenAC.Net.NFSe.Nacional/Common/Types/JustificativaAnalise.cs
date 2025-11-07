using OpenAC.Net.DFe.Core.Attributes;

namespace OpenAC.Net.NFSe.Nacional.Common.Types;

/// <summary>
/// Código do motivo da solicitação de análise fiscal para cancelamento de NFS-e.
/// </summary>
/// <remarks>
/// 1 - Erro na Emissão;<br/>
/// 2 - Serviço não Prestado;<br/>
/// 9 - Outros.
/// </remarks>
public enum JustificativaAnalise
{
    /// <summary>
    ///  1 - Erro na Emissão
    /// </summary>
    [DFeEnum("1")]
    ErroEmissao,
    
    /// <summary>
    /// 2 - Serviço não Prestado
    /// </summary>
    [DFeEnum("2")]
    ServicoNãoPrestado,
    
    /// <summary>
    /// 9 - Outros
    /// </summary>
    [DFeEnum("9")]
    Outros
}