// ***********************************************************************
// Assembly     : OpenAC.Net.NFSe.Nacional
// Author       : RGG
// Created      : 15-11-2025
//
// Last Modified By : RGG
// Last Modified On : 15-11-2025
// ***********************************************************************
// <copyright file="RTCInfoTributosDif.cs" company="OpenAC .Net">
//        The MIT License (MIT)
//  Copyright (c) 2014-2023 Grupo OpenAC.Net
// </copyright>
// ***********************************************************************

using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Serializer;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Grupo de informações relacionadas ao diferimento para IBS e CBS.
/// </summary>
public sealed class RTCInfoTributosDif
{
    /// <summary>
    /// Percentual de diferimento para o IBS estadual.
    /// </summary>
    [DFeElement(TipoCampo.De2, "pDifUF", Min = 1, Max = 3, Ocorrencia = Ocorrencia.Obrigatoria)]
    public decimal PercentualDiferimentoUF { get; set; }

    /// <summary>
    /// Percentual de diferimento para o IBS municipal.
    /// </summary>
    [DFeElement(TipoCampo.De2, "pDifMun", Min = 1, Max = 3, Ocorrencia = Ocorrencia.Obrigatoria)]
    public decimal PercentualDiferimentoMunicipal { get; set; }

    /// <summary>
    /// Percentual de diferimento para a CBS.
    /// </summary>
    [DFeElement(TipoCampo.De2, "pDifCBS", Min = 1, Max = 3, Ocorrencia = Ocorrencia.Obrigatoria)]
    public decimal PercentualDiferimentoCBS { get; set; }
}
