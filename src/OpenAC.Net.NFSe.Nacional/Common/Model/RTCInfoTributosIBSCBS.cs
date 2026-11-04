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
public sealed partial class RTCInfoTributosIBSCBS
{
    /// <summary>
    /// Código de Situação Tributária do IBS e da CBS. NT 009: filho direto de trib, antes de gIBSCBS.
    /// </summary>
    [DFeElement(TipoCampo.Str, "CST", Min = 3, Max = 3, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string CodigoSituacaoTributaria { get; set; } = string.Empty;

    /// <summary>
    /// Código de Classificação Tributária do IBS e da CBS. NT 009: filho direto de trib, antes de gIBSCBS.
    /// </summary>
    [DFeElement(TipoCampo.Str, "cClassTrib", Min = 6, Max = 6, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string CodigoClassificacaoTributaria { get; set; } = string.Empty;

    /// <summary>
    /// Grupo de informações relacionadas ao IBS e à CBS.
    /// </summary>
    [DFeElement("gIBSCBS", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public RTCInfoTributosSitClas? GrupoIBSCBS { get; set; }
}
