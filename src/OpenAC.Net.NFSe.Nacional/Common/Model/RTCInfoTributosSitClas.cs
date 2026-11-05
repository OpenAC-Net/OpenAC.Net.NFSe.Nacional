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
using OpenAC.Net.DFe.Core.Collection;
using OpenAC.Net.DFe.Core.Serializer;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Grupo de informações relacionadas ao IBS e à CBS.
/// </summary>
public sealed partial class RTCInfoTributosSitClas : DFeParentItem<RTCInfoTributosSitClas, InfDps>
{
    /// <summary>
    /// Código de Situação Tributária do IBS e da CBS (layout anterior a 01/12/2026 em Produção).
    /// </summary>
    [DFeElement(TipoCampo.Str, "CST", Min = 3, Max = 3, Ocorrencia = Ocorrencia.Obrigatoria, Ordem = 1)]
    public string CodigoSituacaoTributaria { get; set; } = string.Empty;

    /// <summary>
    /// Código de Classificação Tributária do IBS e da CBS (layout anterior a 01/12/2026 em Produção).
    /// </summary>
    [DFeElement(TipoCampo.Str, "cClassTrib", Min = 6, Max = 6, Ocorrencia = Ocorrencia.Obrigatoria, Ordem = 2)]
    public string CodigoClassificacaoTributaria { get; set; } = string.Empty;

    /// <summary>
    /// Código e Classificação do Crédito Presumido: IBS e CBS.
    /// </summary>
    [DFeElement(TipoCampo.Str, "cCredPres", Min = 2, Max = 2, Ocorrencia = Ocorrencia.NaoObrigatoria, Ordem = 3)]
    public string? CodigoCreditoPresumido { get; set; }

    /// <summary>
    /// Grupo de informações da Tributação Regular.
    /// </summary>
    [DFeElement("gTribRegular", Ocorrencia = Ocorrencia.NaoObrigatoria, Ordem = 4)]
    public RTCInfoTributosTribRegular? GrupoTributacaoRegular { get; set; }

    /// <summary>
    /// Grupo de informações relacionadas ao diferimento para IBS e CBS.
    /// </summary>
    [DFeElement("gDif", Ocorrencia = Ocorrencia.NaoObrigatoria, Ordem = 5)]
    public RTCInfoTributosDif? GrupoDiferimento { get; set; }

    internal bool ShouldSerializeCodigoSituacaoTributaria() => !(Parent?.UsarLayoutNovo101 ?? true);

    internal bool ShouldSerializeCodigoClassificacaoTributaria() => !(Parent?.UsarLayoutNovo101 ?? true);
}
