// ***********************************************************************
// Assembly     : OpenAC.Net.NFSe.Nacional
// Author       : RGG
// Created      : 15-11-2025
//
// Last Modified By : RGG
// Last Modified On : 15-11-2025
// ***********************************************************************
// <copyright file="RTCInfoTributosTribRegular.cs" company="OpenAC .Net">
//        The MIT License (MIT)
//  Copyright (c) 2014-2023 Grupo OpenAC.Net
// </copyright>
// ***********************************************************************

using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Serializer;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Grupo de informações da Tributação Regular.
/// </summary>
public sealed class RTCInfoTributosTribRegular
{
    /// <summary>
    /// Código de Situação Tributária do IBS e da CBS de tributação regular.
    /// </summary>
    [DFeElement(TipoCampo.Str, "CSTReg", Min = 3, Max = 3, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string CodigoSituacaoTributariaRegular { get; set; } = string.Empty;

    /// <summary>
    /// Código da Classificação Tributária do IBS e da CBS de tributação regular.
    /// </summary>
    [DFeElement(TipoCampo.Str, "cClassTribReg", Min = 6, Max = 6, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string CodigoClassificacaoTributariaRegular { get; set; } = string.Empty;
}
