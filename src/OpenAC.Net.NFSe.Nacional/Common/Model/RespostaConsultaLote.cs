using System.Collections.Generic;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Resposta da consulta de lote de DPS
/// (operação <c>consultarLoteDps</c> do provedor Fiorilli).
/// </summary>
public sealed class RespostaConsultaLote : RespostaSoapBase
{
    /// <summary>
    /// Situação do processamento do lote (campo <c>Situacao</c> do WSDL).
    /// </summary>
    public int Situacao { get; set; }

    /// <summary>
    /// NFS-e processadas no lote consultado.
    /// </summary>
    public List<NotaFiscalServico> NotasFiscais { get; set; } = new();
}
