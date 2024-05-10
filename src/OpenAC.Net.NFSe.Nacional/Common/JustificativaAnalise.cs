using OpenAC.Net.DFe.Core.Attributes;

namespace OpenAC.Net.NFSe.Nacional.Common;

/// <summary>
/// Código do motivo da solicitação de análise fiscal para cancelamento de NFS-e:
/// 1 - Erro na Emissão;
/// 2 - Serviço não Prestado;
/// 9 - Outros.
/// </summary>
public enum JustificativaAnalise
{
    [DFeEnum("1")]
    ErroEmissao,
    
    [DFeEnum("2")]
    ServicoNãoPrestado,
    
    [DFeEnum("9")]
    Outros
}