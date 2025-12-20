// ***********************************************************************
// Assembly   : OpenAC.Net.NFSe.Nacional
// Author       : RGG
// Created  : 19-12-2025
//
// Last Modified By : RGG
// Last Modified On : 19-12-2025
// ***********************************************************************
// <copyright file="RTCTotalTribCompraGov.cs" company="OpenAC .Net">
//		     		   The MIT License (MIT)
//	     		    Copyright (c) 2014-2023 Grupo OpenAC.Net
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
/// Grupo de informações da composição do valor do IBS e da CBS em compras governamentais
/// </summary>
public sealed class RTCTotalTribCompraGov
{
    #region Properties

    /// <summary>
/// Alíquota do IBS de competência do Estado
    /// </summary>
    [DFeElement(TipoCampo.De2, "pIBSUF", Min = 1, Max = 5, Ocorrencia = Ocorrencia.Obrigatoria)]
    public decimal AliquotaIBSUF { get; set; }

/// <summary>
    /// Valor do Tributo do IBS da UF calculado
    /// </summary>
    [DFeElement(TipoCampo.De2, "vIBSUF", Min = 1, Max = 15, Ocorrencia = Ocorrencia.Obrigatoria)]
    public decimal ValorIBSUF { get; set; }

    /// <summary>
  /// Alíquota do IBS de competência do Município
    /// </summary>
  [DFeElement(TipoCampo.De2, "pIBSMun", Min = 1, Max = 5, Ocorrencia = Ocorrencia.Obrigatoria)]
    public decimal AliquotaIBSMunicipal { get; set; }

    /// <summary>
    /// Valor do Tributo do IBS do Município calculado
    /// </summary>
    [DFeElement(TipoCampo.De2, "vIBSMun", Min = 1, Max = 15, Ocorrencia = Ocorrencia.Obrigatoria)]
    public decimal ValorIBSMunicipal { get; set; }

    /// <summary>
    /// Alíquota da CBS
    /// </summary>
    [DFeElement(TipoCampo.De2, "pCBS", Min = 1, Max = 5, Ocorrencia = Ocorrencia.Obrigatoria)]
    public decimal AliquotaCBS { get; set; }

    /// <summary>
    /// Valor do Tributo da CBS calculado
    /// </summary>
    [DFeElement(TipoCampo.De2, "vCBS", Min = 1, Max = 15, Ocorrencia = Ocorrencia.Obrigatoria)]
    public decimal ValorCBS { get; set; }

    #endregion Properties
}
