// ***********************************************************************
// Assembly     : OpenAC.Net.NFSe.Nacional
// Author       : RGG
// Created      : 15-11-2025
//
// Last Modified By : RGG
// Last Modified On : 15-11-2025
// ***********************************************************************
// <copyright file="RTCListaDocFornec.cs" company="OpenAC .Net">
//        The MIT License (MIT)
//  Copyright (c) 2014-2023 Grupo OpenAC.Net
// </copyright>
// ***********************************************************************

using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Grupo de informações do fornecedor do documento referenciado.
/// </summary>
public sealed class RTCListaDocFornec
{
    #region Choice
    /// <summary>
    /// CNPJ do Fornecedor do serviço.
    /// </summary>
    [DFeElement(TipoCampo.Str, "CNPJ", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? CNPJ { get; set; }
    /// <summary>
    /// CPF do Fornecedor do serviço.
    /// </summary>
    [DFeElement(TipoCampo.Str, "CPF", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? CPF { get; set; }
    /// <summary>
    /// NIF do Fornecedor do serviço.
    /// </summary>
    [DFeElement(TipoCampo.Str, "NIF", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? NIF { get; set; }
    /// <summary>
    /// Indicador de ausência de NIF do Fornecedor do serviço.
    /// </summary>
    [DFeElement(TipoCampo.Enum, "cNaoNIF", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public CodNaoNIF? CodigoNaoNIF { get; set; }

    #endregion

    /// <summary>
    /// Nome / Razão Social do Fornecedor do serviço.
    /// </summary>
    [DFeElement(TipoCampo.Str, "xNome", Min = 1, Max = 150, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string Nome { get; set; } = string.Empty;
}
