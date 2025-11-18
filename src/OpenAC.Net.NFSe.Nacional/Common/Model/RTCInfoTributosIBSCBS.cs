// ***********************************************************************
// Assembly     : OpenAC.Net.NFSe.Nacional
// Author       : RGG
// Created      : 15-11-2025
//
// Last Modified By : RGG
// Last Modified On : 15-11-2025
// ***********************************************************************
// <copyright file="RTCInfoTributosIBSCBS.cs" company="OpenAC .Net">
//        The MIT License (MIT)
//  Copyright (c) 2014-2023 Grupo OpenAC.Net
// </copyright>
// ***********************************************************************

using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Serializer;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Grupo de informações relacionados aos tributos IBS e CBS.
/// </summary>
public sealed class RTCInfoTributosIBSCBS
{
    /// <summary>
    /// Grupo de informações relacionadas ao IBS e à CBS.
    /// </summary>
    [DFeElement("gIBSCBS", Ocorrencia = Ocorrencia.Obrigatoria)]
    public RTCInfoTributosSitClas GrupoIBSCBS { get; set; } = new();
}
