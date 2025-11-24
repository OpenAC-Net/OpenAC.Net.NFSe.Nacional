// ***********************************************************************
// Assembly     : OpenAC.Net.NFSe.Nacional
// Author  : RGG
// Created      : 15-11-2025
//
// Last Modified By : RGG
// Last Modified On : 15-11-2025
// ***********************************************************************
// <copyright file="RTCInfoValoresIBSCBS.cs" company="OpenAC .Net">
//        The MIT License (MIT)
//  Copyright (c) 2014-2023 Grupo OpenAC.Net
// </copyright>
// ***********************************************************************

using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Serializer;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Grupo de informações relativas aos valores do serviço prestado para IBS e CBS.
/// </summary>
public sealed class RTCInfoValoresIBSCBS
{
    /// <summary>
    /// Grupo de informações relativas a valores incluídos neste documento e recebidos por motivo de estarem relacionadas
    /// a operações de terceiros, objeto de reembolso, repasse ou ressarcimento pelo recebedor, já tributados e aqui referenciados.
    /// </summary>
    [DFeElement("gReeRepRes", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public RTCInfoReeRepRes? ReeRepRes { get; set; }

    /// <summary>
    /// Grupo de informações relacionados aos tributos IBS e CBS.
    /// </summary>
    [DFeElement("trib", Ocorrencia = Ocorrencia.Obrigatoria)]
    public RTCInfoTributosIBSCBS Tributos { get; set; } = new();
}
