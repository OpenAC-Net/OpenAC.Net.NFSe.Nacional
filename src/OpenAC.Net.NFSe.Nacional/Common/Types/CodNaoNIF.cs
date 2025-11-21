// ***********************************************************************
// Assembly     : OpenAC.Net.NFSe.Nacional
// Author       : RGG
// Created      : 15-11-2025
//
// Last Modified By : RGG
// Last Modified On : 15-11-2025
// ***********************************************************************
// <copyright file="CodNaoNIF.cs" company="OpenAC .Net">
//        The MIT License (MIT)
//  Copyright (c) 2014-2023 Grupo OpenAC.Net
// </copyright>
// ***********************************************************************

using OpenAC.Net.DFe.Core.Attributes;

namespace OpenAC.Net.NFSe.Nacional.Common.Types;

/// <summary>
/// Motivo para não informação do NIF.
/// </summary>
public enum CodNaoNIF
{
    /// <summary>
    /// 0 - Não informado na nota de origem
    /// </summary>
    [DFeEnum("0")]
    NaoInformado,

    /// <summary>
    /// 1 - Dispensado do NIF
    /// </summary>
    [DFeEnum("1")]
    Dispensado,

    /// <summary>
    /// 2 - Não exigência do NIF
    /// </summary>
    [DFeEnum("2")]
    NaoExigido
}
