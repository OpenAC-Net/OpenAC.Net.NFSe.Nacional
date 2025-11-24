// ***********************************************************************
// Assembly     : OpenAC.Net.NFSe.Nacional
// Author       : RGG
// Created      : 15-11-2025
//
// Last Modified By : RGG
// Last Modified On : 15-11-2025
// ***********************************************************************
// <copyright file="RTCListaDocDFe.cs" company="OpenAC .Net">
//        The MIT License (MIT)
//  Copyright (c) 2014-2023 Grupo OpenAC.Net
// </copyright>
// ***********************************************************************

using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Grupo de informações de documentos fiscais eletrônicos que se encontram no repositório nacional.
/// </summary>
public sealed class RTCListaDocDFe
{
    /// <summary>
    /// Documento fiscal a que se refere a chaveDfe que seja um dos documentos do Repositório Nacional.
    /// </summary>
    [DFeElement(TipoCampo.Enum, "tipoChaveDFe", Ocorrencia = Ocorrencia.Obrigatoria)]
    public RTCTipoChaveDFe TipoChaveDFe { get; set; }

    /// <summary>
    /// Descrição da DF-e a que se refere a chaveDfe. Preencher apenas quando "tipoChaveDFe = 9 (Outro)".
    /// </summary>
    [DFeElement(TipoCampo.Str, "xTipoChaveDFe", Min = 1, Max = 255, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? DescricaoTipoChaveDFe { get; set; }

    /// <summary>
    /// Chave do Documento Fiscal eletrônico do repositório nacional referenciado.
    /// </summary>
    [DFeElement(TipoCampo.Str, "chaveDFe", Min = 1, Max = 50, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string ChaveDFe { get; set; } = string.Empty;
}
