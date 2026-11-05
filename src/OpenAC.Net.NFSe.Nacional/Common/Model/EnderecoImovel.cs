// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 04-11-2026
//
// Last Modified By : RFTD
// Last Modified On : 04-11-2026
// ***********************************************************************
// <copyright file="EnderecoImovel.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014-2026 Grupo OpenAC.Net
//
//	 Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//	 The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//	 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// </copyright>
// <summary></summary>
// ***********************************************************************

using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Serializer;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Grupo de informações do endereço do imóvel (TCEnderImovel).
/// </summary>
public sealed partial class EnderecoImovel
{
    /// <summary>
    /// Código numérico do Endereçamento Postal nacional (CEP) do endereço do imóvel.
    /// </summary>
    [DFeElement(TipoCampo.StrNumberFill, "CEP", Min = 8, Max = 8, Ocorrencia = Ocorrencia.Obrigatoria, Ordem = 1)]
    public string CEP { get; set; } = string.Empty;

    /// <summary>
    /// Tipo e nome do logradouro do endereço do imóvel.
    /// </summary>
    [DFeElement(TipoCampo.Str, "xLgr", Min = 1, Max = 255, Ocorrencia = Ocorrencia.Obrigatoria, Ordem = 2)]
    public string Logradouro { get; set; } = string.Empty;

    /// <summary>
    /// Número no logradouro do endereço do imóvel.
    /// </summary>
    [DFeElement(TipoCampo.Str, "nro", Min = 1, Max = 60, Ocorrencia = Ocorrencia.Obrigatoria, Ordem = 3)]
    public string Numero { get; set; } = string.Empty;

    /// <summary>
    /// Complemento do endereço do imóvel.
    /// </summary>
    [DFeElement(TipoCampo.Str, "xCpl", Min = 1, Max = 156, Ocorrencia = Ocorrencia.NaoObrigatoria, Ordem = 4)]
    public string? Complemento { get; set; }

    /// <summary>
    /// Bairro do endereço do imóvel.
    /// </summary>
    [DFeElement(TipoCampo.Str, "xBairro", Min = 1, Max = 60, Ocorrencia = Ocorrencia.NaoObrigatoria, Ordem = 5)]
    public string? Bairro { get; set; }
}
