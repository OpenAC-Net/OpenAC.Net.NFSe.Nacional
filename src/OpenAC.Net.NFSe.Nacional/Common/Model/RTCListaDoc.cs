// ***********************************************************************
// Assembly     : OpenAC.Net.NFSe.Nacional
// Author       : RGG
// Created      : 15-11-2025
//
// Last Modified By : RGG
// Last Modified On : 15-11-2025
// ***********************************************************************
// <copyright file="RTCListaDoc.cs" company="OpenAC .Net">
//        The MIT License (MIT)
//  Copyright (c) 2014-2023 Grupo OpenAC.Net
// </copyright>
// ***********************************************************************

using System;
using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Grupo relativo aos documentos referenciados nos casos de reembolso, repasse e ressarcimento.
/// </summary>
public sealed class RTCListaDoc
{
    #region Choice
    
    [DFeElement("dFeNacional", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public RTCListaDocDFe? DocumentoFeNacional { get; set; }
    
    [DFeElement("docFiscalOutro", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public RTCListaDocFiscalOutro? DocumentoFiscalOutro { get; set; }
    
    [DFeElement("docOutro", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public RTCListaDocOutro? DocumentoOutro { get; set; }
    
    #endregion
    
    /// <summary>
    /// Grupo de informações do fornecedor do documento referenciado.
    /// </summary>
    [DFeElement("fornec", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public RTCListaDocFornec? Fornecedor { get; set; }

    /// <summary>
    /// Data da emissão do documento dedutível.
    /// </summary>
    [DFeElement(TipoCampo.Dat, "dtEmiDoc", Ocorrencia = Ocorrencia.Obrigatoria)]
    public DateTime DataEmissaoDocumento { get; set; }

    /// <summary>
    /// Data da competência do documento dedutível.
    /// </summary>
    [DFeElement(TipoCampo.Dat, "dtCompDoc", Ocorrencia = Ocorrencia.Obrigatoria)]
    public DateTime DataCompetenciaDocumento { get; set; }

    /// <summary>
    /// Tipo de valor incluído neste documento, recebido por motivo de estarem relacionadas a operações de terceiros,
    /// objeto de reembolso, repasse ou ressarcimento pelo recebedor, já tributados e aqui referenciados.
    /// </summary>
    [DFeElement(TipoCampo.Enum, "tpReeRepRes", Ocorrencia = Ocorrencia.Obrigatoria)]
    public RTCTpReeRepRes TipoReembolsoRepasseRessarcimento { get; set; }

    /// <summary>
    /// Descrição do reembolso ou ressarcimento quando a opção é "99 – Outros reembolsos ou ressarcimentos...".
    /// </summary>
    [DFeElement(TipoCampo.Str, "xTpReeRepRes", Min = 1, Max = 150, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? DescricaoTipoReembolsoRepasseRessarcimento { get; set; }

    /// <summary>
    /// Valor monetário (total ou parcial, conforme documento informado) utilizado para não inclusão na base de cálculo
    /// do ISS e do IBS e da CBS da NFS-e que está sendo emitida (R$).
    /// </summary>
    [DFeElement(TipoCampo.De2, "vlrReeRepRes", Min = 1, Max = 15, Ocorrencia = Ocorrencia.Obrigatoria)]
    public decimal ValorReembolsoRepasseRessarcimento { get; set; }
}
