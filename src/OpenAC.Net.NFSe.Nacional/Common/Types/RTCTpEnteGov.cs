// ***********************************************************************
// Assembly    : OpenAC.Net.NFSe.Nacional
// Author      : RGG
// Created     : 15-11-2025
//
// Last Modified By : RGG
// Last Modified On : 15-11-2025
// ***********************************************************************
// <copyright file="RTCTpEnteGov.cs" company="OpenAC .Net">
//  The MIT License (MIT)
//        Copyright (c)2014-2023 Grupo OpenAC.Net
// </copyright>
// ***********************************************************************

using OpenAC.Net.DFe.Core.Attributes;

namespace OpenAC.Net.NFSe.Nacional.Common.Types;

/// <summary>
/// Tipo de ente governamental:
///1 - União;
///2 - Estado;
///3 - Distrito Federal;
///4 - Município.
/// </summary>
public enum RTCTpEnteGov
{
    /// <summary>
    ///1 - União
    /// </summary>
    [DFeEnum("1")]
    Uniao,

    /// <summary>
    ///2 - Estado
    /// </summary>
    [DFeEnum("2")]
    Estado,

    /// <summary>
    ///3 - Distrito Federal
    /// </summary>
    [DFeEnum("3")]
    DistritoFederal,

    /// <summary>
    ///4 - Município
    /// </summary>
    [DFeEnum("4")]
    Municipio
}
