// ***********************************************************************
// Assembly     : OpenAC.Net.NFSe.Nacional
// Author       : RGG
// Created      : 15-11-2025
//
// Last Modified By : RGG
// Last Modified On : 15-11-2025
// ***********************************************************************
// <copyright file="RTCListaDocOutro.cs" company="OpenAC .Net">
//        The MIT License (MIT)
//  Copyright (c) 2014-2023 Grupo OpenAC.Net
// </copyright>
// ***********************************************************************

using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Serializer;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Grupo de informações de documento não fiscal.
/// </summary>
public sealed class RTCListaDocOutro
{
    /// <summary>
    /// Número do documento não fiscal.
    /// </summary>
    [DFeElement(TipoCampo.Str, "nDoc", Min = 1, Max = 255, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string NumeroDocumento { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do documento não fiscal.
    /// </summary>
    [DFeElement(TipoCampo.Str, "xDoc", Min = 1, Max = 255, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string DescricaoDocumento { get; set; } = string.Empty;
}
