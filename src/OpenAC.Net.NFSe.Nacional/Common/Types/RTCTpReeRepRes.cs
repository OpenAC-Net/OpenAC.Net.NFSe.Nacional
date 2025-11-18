// ***********************************************************************
// Assembly     : OpenAC.Net.NFSe.Nacional
// Author       : RGG
// Created      : 15-11-2025
//
// Last Modified By : RGG
// Last Modified On : 15-11-2025
// ***********************************************************************
// <copyright file="RTCTpReeRepRes.cs" company="OpenAC .Net">
//        The MIT License (MIT)
//  Copyright (c) 2014-2023 Grupo OpenAC.Net
// </copyright>
// ***********************************************************************

using OpenAC.Net.DFe.Core.Attributes;

namespace OpenAC.Net.NFSe.Nacional.Common.Types;

/// <summary>
/// Tipo de valor incluído neste documento, recebido por motivo de estarem relacionadas a operações de terceiros,
/// objeto de reembolso, repasse ou ressarcimento pelo recebedor, já tributados e aqui referenciados.
/// </summary>
public enum RTCTpReeRepRes
{
    /// <summary>
    /// 01 - Repasse de remuneração por intermediação de imóveis a demais corretores envolvidos na operação
    /// </summary>
    [DFeEnum("01")]
    RepasseIntermediacaoImoveis,

    /// <summary>
    /// 02 - Repasse de valores a fornecedor relativo a fornecimento intermediado por agência de turismo
    /// </summary>
    [DFeEnum("02")]
    RepasseAgenciaTurismo,

    /// <summary>
    /// 03 - Reembolso ou ressarcimento recebido por agência de propaganda e publicidade por valores pagos relativos
    /// a serviços de produção externa por conta e ordem de terceiro
    /// </summary>
    [DFeEnum("03")]
    ReembolsoAgenciaPropagandaProducao,

    /// <summary>
    /// 04 - Reembolso ou ressarcimento recebido por agência de propaganda e publicidade por valores pagos relativos
    /// a serviços de mídia por conta e ordem de terceiro
    /// </summary>
    [DFeEnum("04")]
    ReembolsoAgenciaPropagandaMidia,

    /// <summary>
    /// 99 - Outros reembolsos ou ressarcimentos recebidos por valores pagos relativos a operações por conta e ordem de terceiro
    /// </summary>
    [DFeEnum("99")]
    Outros
}
