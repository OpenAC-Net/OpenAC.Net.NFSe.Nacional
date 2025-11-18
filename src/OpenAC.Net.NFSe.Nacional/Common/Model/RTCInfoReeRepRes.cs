// ***********************************************************************
// Assembly     : OpenAC.Net.NFSe.Nacional
// Author       : RGG
// Created      : 15-11-2025
//
// Last Modified By : RGG
// Last Modified On : 15-11-2025
// ***********************************************************************
// <copyright file="RTCInfoReeRepRes.cs" company="OpenAC .Net">
//        The MIT License (MIT)
//  Copyright (c) 2014-2023 Grupo OpenAC.Net
// </copyright>
// ***********************************************************************

using System.Collections.Generic;
using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Serializer;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Grupo de informações relativas a valores incluídos neste documento e recebidos por motivo de estarem relacionadas
/// a operações de terceiros, objeto de reembolso, repasse ou ressarcimento pelo recebedor, já tributados e aqui referenciados.
/// </summary>
public sealed class RTCInfoReeRepRes
{
    /// <summary>
    /// Grupo relativo aos documentos referenciados nos casos de reembolso, repasse e ressarcimento que serão
    /// considerados na base de cálculo do ISSQN, do IBS e da CBS.
    /// </summary>
    [DFeCollection("documentos", MinSize = 1, MaxSize = 1000)]
    [DFeItem(typeof(RTCListaDoc), "documentos")]
    public List<RTCListaDoc> Documentos { get; set; } = new();
}
