// ***********************************************************************
// Assembly     : OpenAC.Net.NFSe.Nacional
// Author       : RGG
// Created      : 15-11-2025
//
// Last Modified By : RGG
// Last Modified On : 15-11-2025
// ***********************************************************************
// <copyright file="RTCInfoTributosSitClas.cs" company="OpenAC .Net">
//        The MIT License (MIT)
//  Copyright (c) 2014-2023 Grupo OpenAC.Net
// </copyright>
// ***********************************************************************

using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Grupo de informações relacionadas ao IBS e à CBS.
/// </summary>
public sealed class RTCInfoTributosSitClas
{
    /// <summary>
    /// Código de Situação Tributária do IBS e da CBS.
    /// </summary>
    [DFeElement(TipoCampo.Str, "CST", Min = 3, Max = 3, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string CodigoSituacaoTributaria { get; set; } = string.Empty;

    /// <summary>
    /// Código de Classificação Tributária do IBS e da CBS.
    /// </summary>
    [DFeElement(TipoCampo.Str, "cClassTrib", Min = 6, Max = 6, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string CodigoClassificacaoTributaria { get; set; } = string.Empty;

    /// <summary>
    /// Código e Classificação do Crédito Presumido: IBS e CBS.
    /// </summary>
    [DFeElement(TipoCampo.Str, "cCredPres", Min = 2, Max = 2, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? CodigoCreditoPresumido { get; set; }

    /// <summary>
    /// Grupo de informações da Tributação Regular.
    /// </summary>
    [DFeElement("gTribRegular", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public RTCInfoTributosTribRegular? GrupoTributacaoRegular { get; set; }

    /// <summary>
    /// Grupo de informações relacionadas ao diferimento para IBS e CBS.
    /// </summary>
    [DFeElement("gDif", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public RTCInfoTributosDif? GrupoDiferimento { get; set; }
}
