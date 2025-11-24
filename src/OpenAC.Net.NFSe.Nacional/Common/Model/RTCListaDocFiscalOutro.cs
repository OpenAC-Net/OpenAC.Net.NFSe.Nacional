// ***********************************************************************
// Assembly     : OpenAC.Net.NFSe.Nacional
// Author       : RGG
// Created      : 15-11-2025
//
// Last Modified By : RGG
// Last Modified On : 15-11-2025
// ***********************************************************************
// <copyright file="RTCListaDocFiscalOutro.cs" company="OpenAC .Net">
//        The MIT License (MIT)
//  Copyright (c) 2014-2023 Grupo OpenAC.Net
// </copyright>
// ***********************************************************************

using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Serializer;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Grupo de informações de documento fiscais, eletrônicos ou não, que não se encontram no repositório nacional.
/// </summary>
public sealed class RTCListaDocFiscalOutro
{
    /// <summary>
    /// Código do município emissor do documento fiscal que não se encontra no repositório nacional.
    /// </summary>
    [DFeElement(TipoCampo.Int, "cMunDocFiscal", Min = 7, Max = 7, Ocorrencia = Ocorrencia.Obrigatoria)]
    public int CodigoMunicipioDocumentoFiscal { get; set; }

    /// <summary>
    /// Número do documento fiscal que não se encontra no repositório nacional.
    /// </summary>
    [DFeElement(TipoCampo.Str, "nDocFiscal", Min = 1, Max = 255, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string NumeroDocumentoFiscal { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do documento fiscal.
    /// </summary>
    [DFeElement(TipoCampo.Str, "xDocFiscal", Min = 1, Max = 255, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string DescricaoDocumentoFiscal { get; set; } = string.Empty;
}
