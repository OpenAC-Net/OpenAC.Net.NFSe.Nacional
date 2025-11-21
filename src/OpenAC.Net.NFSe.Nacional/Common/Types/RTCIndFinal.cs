// ***********************************************************************
// Assembly   : OpenAC.Net.NFSe.Nacional
// Author       : RGG
// Created    : 15-11-2025
//
// Last Modified By : RGG
// Last Modified On : 15-11-2025
// ***********************************************************************
// <copyright file="RTCIndFinal.cs" company="OpenAC .Net">
//       The MIT License (MIT)
//  Copyright (c)2014-2023 Grupo OpenAC.Net
// </copyright>
// ***********************************************************************

using OpenAC.Net.DFe.Core.Attributes;

namespace OpenAC.Net.NFSe.Nacional.Common.Types;

/// <summary>
/// Indica operação de uso ou consumo pessoal (art.57):
///0 - Não;
///1 - Sim.
/// </summary>
public enum RTCIndFinal
{
    /// <summary>
    ///0 - Não
    /// </summary>
    [DFeEnum("0")]
    Nao,

    /// <summary>
    ///1 - Sim
    /// </summary>
    [DFeEnum("1")]
    Sim
}
