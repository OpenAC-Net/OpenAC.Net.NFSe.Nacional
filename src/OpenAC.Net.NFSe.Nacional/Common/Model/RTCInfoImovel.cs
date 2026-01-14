// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author     : RGG
// Created     : 15-11-2025
//
// Last Modified By : RGG
// Last Modified On : 15-11-2025
// ***********************************************************************
// <copyright file="RTCInfoImovel.cs" company="OpenAC .Net">
//        The MIT License (MIT)
//  Copyright (c) 2014-2023 Grupo OpenAC.Net
// </copyright>
// ***********************************************************************

using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Serializer;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Grupo de informações de operações relacionadas a bens imóveis, exceto obras.
/// </summary>
public sealed class RTCInfoImovel
{
    /// <summary>
    /// Inscrição imobiliária fiscal.
    /// </summary>
    [DFeElement(TipoCampo.Str, "inscImobFisc", Min = 1, Max = 30, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? InscricaoImobiliariaFiscal { get; set; }

    #region Choice
    /// <summary>
    /// Código CIB (Código do Imóvel na Prefeitura).
    /// </summary>
    [DFeElement(TipoCampo.Str, "cCIB", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? CodigoCIB { get; set; }
    /// <summary>
    /// Endereço do imóvel.
    /// </summary>
    [DFeElement("end", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public EnderecoSimplesNFSe? Endereco { get; set; }

    #endregion
}
