// ***********************************************************************
// Assembly     : OpenAC.Net.NFSe.Nacional
// Author       : RGG
// Created      : 15-11-2025
//
// Last Modified By : RGG
// Last Modified On : 15-11-2025
// ***********************************************************************
// <copyright file="RTCTipoChaveDFe.cs" company="OpenAC .Net">
//        The MIT License (MIT)
//  Copyright (c) 2014-2023 Grupo OpenAC.Net
// </copyright>
// ***********************************************************************

using OpenAC.Net.DFe.Core.Attributes;

namespace OpenAC.Net.NFSe.Nacional.Common.Types;

/// <summary>
/// Documento fiscal a que se refere a chaveDfe que seja um dos documentos do Repositório Nacional.
/// </summary>
public enum RTCTipoChaveDFe
{
    /// <summary>
    /// 1 - NFS-e
    /// </summary>
    [DFeEnum("1")]
    NFSe,

    /// <summary>
    /// 2 - NF-e
    /// </summary>
    [DFeEnum("2")]
    NFe,

    /// <summary>
    /// 3 - CT-e
    /// </summary>
    [DFeEnum("3")]
    CTe,

    /// <summary>
    /// 9 - Outro
    /// </summary>
    [DFeEnum("9")]
    Outro
}
