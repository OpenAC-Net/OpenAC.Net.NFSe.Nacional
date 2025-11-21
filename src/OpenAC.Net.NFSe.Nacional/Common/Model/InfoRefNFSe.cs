// ***********************************************************************
// Assembly    : OpenAC.Net.NFSe.Nacional
// Author: RGG
// Created     : 15-11-2025
//
// Last Modified By : RGG
// Last Modified On : 15-11-2025
// ***********************************************************************
// <copyright file="InfoRefNFSe.cs" company="OpenAC .Net">
//  The MIT License (MIT)
//       Copyright (c) 2014-2023 Grupo OpenAC.Net
// </copyright>
// ***********************************************************************

using System.Collections.Generic;
using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Serializer;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Grupo de NFS-e referenciadas.
/// </summary>
public sealed class InfoRefNFSe
{
    /// <summary>
    /// Chave da NFS-e referenciada.
    /// </summary>
    [DFeCollection("refNFSe", MinSize = 1, MaxSize = 99)]
    [DFeItem(typeof(string), "refNFSe")]
    public List<string> RefNFSe { get; set; } = new();
}
