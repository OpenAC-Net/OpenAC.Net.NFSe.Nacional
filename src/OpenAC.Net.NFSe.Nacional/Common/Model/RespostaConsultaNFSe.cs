using System.Collections.Generic;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Resposta da consulta de NFS-e
/// (operação <c>consultarNfse</c> do provedor Fiorilli).
/// </summary>
public sealed class RespostaConsultaNFSe : RespostaSoapBase
{
    /// <summary>
    /// NFS-e retornadas na consulta.
    /// </summary>
    public List<NotaFiscalServico> NotasFiscais { get; set; } = new();
}
