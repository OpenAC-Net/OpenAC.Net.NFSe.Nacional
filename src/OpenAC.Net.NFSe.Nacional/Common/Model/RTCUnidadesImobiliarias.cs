// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 04-11-2026
//
// Last Modified By : RFTD
// Last Modified On : 04-11-2026
// ***********************************************************************
// <copyright file="RTCUnidadesImobiliarias.cs" company="OpenAC .Net">
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

using System.Collections.Generic;
using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Serializer;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Grupo de informações relativas às unidades imobiliárias objeto da operação (TCRTCUnidadesImobiliarias).
/// </summary>
public sealed partial class RTCUnidadesImobiliarias
{
    /// <summary>
    /// Inscrição imobiliária fiscal (inscImobFisc).
    /// </summary>
    [DFeElement(TipoCampo.Str, "inscImobFisc", Min = 1, Max = 30, Ocorrencia = Ocorrencia.NaoObrigatoria, Ordem = 1)]
    public string? InscricaoImobiliariaFiscal { get; set; }

    #region Choice

    /// <summary>
    /// Código do Cadastro Imobiliário Brasileiro - CIB (cCIB).
    /// </summary>
    [DFeElement(TipoCampo.Str, "cCIB", Min = 8, Max = 8, Ocorrencia = Ocorrencia.NaoObrigatoria, Ordem = 2)]
    public string? CodigoCIB { get; set; }

    /// <summary>
    /// Grupo de informações do endereço do imóvel (end).
    /// </summary>
    [DFeElement("end", Ocorrencia = Ocorrencia.NaoObrigatoria, Ordem = 2)]
    public EnderecoImovel? Endereco { get; set; }

    #endregion Choice

    /// <summary>
    /// Grupo de informações relativas a valores de ajuste (dedução/redução) da Base de Cálculo do IBS e da CBS
    /// referentes às operações de locação, cessão onerosa ou arrendamento de bens imóveis (gAjusteBCLocImoveis).
    /// </summary>
    [DFeCollection("gAjusteBCLocImoveis", MinSize = 0, MaxSize = 1000, Ocorrencia = Ocorrencia.NaoObrigatoria, Ordem = 3)]
    public List<RTCValoresBCImoveis> AjustesBCLocImoveis { get; set; } = new();

    /// <summary>
    /// Indica se o código CIB deve ser serializado.
    /// </summary>
    public bool ShouldSerializeCodigoCIB() => Endereco == null && !string.IsNullOrEmpty(CodigoCIB);

    /// <summary>
    /// Indica se o endereço deve ser serializado.
    /// </summary>
    public bool ShouldSerializeEndereco() => Endereco != null;

    /// <summary>
    /// Indica se a lista de ajustes de BC de locação de imóveis deve ser serializada.
    /// </summary>
    public bool ShouldSerializeAjustesBCLocImoveis() => AjustesBCLocImoveis is { Count: > 0 };
}
