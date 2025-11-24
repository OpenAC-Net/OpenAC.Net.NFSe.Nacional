// ***********************************************************************
// Assembly     : OpenAC.Net.NFSe.Nacional
// Author       : RGG
// Created      : 15-11-2025
//
// Last Modified By : RGG
// Last Modified On : 15-11-2025
// ***********************************************************************
// <copyright file="RTCInfoDest.cs" company="OpenAC .Net">
//        The MIT License (MIT)
//  Copyright (c) 2014-2023 Grupo OpenAC.Net
// </copyright>
// ***********************************************************************

using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Grupo de informações relativas ao Destinatário.
/// </summary>
public sealed class RTCInfoDest
{
    #region Choice
    
    /// <summary>
    /// Número do CNPJ do Destinatário do serviço.
    /// </summary>
    public string? CNPJ { get; set; }
    /// <summary>
    ///  CPF (Cadastro de Pessoas Físicas).
    /// </summary>
    [DFeElement(TipoCampo.Str, "CPF", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? CPF { get; set; }
    /// <summary>
    /// NIF (Número de Identificação Fiscal).
    /// </summary>
    [DFeElement(TipoCampo.Str, "NIF", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? NIF { get; set; }
    /// <summary>
    ///  Motivo para não informação do NIF.
    /// </summary>
    [DFeElement(TipoCampo.Enum, "cNaoNIF", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public CodNaoNIF? CodigoNaoNIF { get; set; }
    
    #endregion
    
    /// <summary>
    /// Nome / Nome Empresarial do do Destinatário do serviço.
    /// </summary>
    [DFeElement(TipoCampo.Str, "xNome", Min = 1, Max = 150, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Grupo de informações do endereço do Destinatário do serviço.
    /// </summary>
    [DFeElement("end", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public EnderecoNFSe? Endereco { get; set; }

    /// <summary>
    /// Número do telefone do Destinatário do serviço.
    /// </summary>
    [DFeElement(TipoCampo.Str, "fone", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? Telefone { get; set; }

    /// <summary>
    /// E-mail do Destinatário do serviço.
    /// </summary>
    [DFeElement(TipoCampo.Str, "email", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? Email { get; set; }
}
