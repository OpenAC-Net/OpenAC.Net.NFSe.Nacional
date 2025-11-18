// ***********************************************************************
// Assembly     : OpenAC.Net.NFSe.Nacional
// Author  : RGG
// Created      : 15-11-2025
//
// Last Modified By : RGG
// Last Modified On : 15-11-2025
// ***********************************************************************
// <copyright file="RTCIndDest.cs" company="OpenAC .Net">
//  The MIT License (MIT)
//        Copyright (c)2014-2023 Grupo OpenAC.Net
// </copyright>
// ***********************************************************************

using OpenAC.Net.DFe.Core.Attributes;

namespace OpenAC.Net.NFSe.Nacional.Common.Types;

/// <summary>
/// A respeito do Destinatário dos serviços.
/// </summary>
public enum RTCIndDest
{
    /// <summary>
    /// 0 - O destinatário é o próprio tomador/adquirente
    /// </summary>
    [DFeEnum("0")]
    ProprioTomador,

    /// <summary>
    /// 1 - O destinatário não é o próprio adquirente
    /// </summary>
    [DFeEnum("1")]
    Outro
}
