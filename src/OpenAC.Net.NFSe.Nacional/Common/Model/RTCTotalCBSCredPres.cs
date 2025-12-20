// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RGG
// Created        : 19-12-2025
//
// Last Modified By : RGG
// Last Modified On : 19-12-2025
// ***********************************************************************
// <copyright file="RTCTotalCBSCredPres.cs" company="OpenAC .Net">
//		   		   The MIT License (MIT)
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
/// Grupo de valores referentes ao crédito presumido para CBS
/// </summary>
public sealed class RTCTotalCBSCredPres
{
    #region Properties

    /// <summary>
    /// Alíquota do crédito presumido para a CBS
    /// </summary>
    [DFeElement(TipoCampo.De2, "pCredPresCBS", Min = 1, Max = 5, Ocorrencia = Ocorrencia.Obrigatoria)]
    public decimal PercentualCreditoPresumidoCBS { get; set; }

  /// <summary>
    /// Valor do Crédito Presumido da CBS
    /// vCredPresCBS = vBC x pCredPresCBS
    /// </summary>
    [DFeElement(TipoCampo.De2, "vCredPresCBS", Min = 1, Max = 15, Ocorrencia = Ocorrencia.Obrigatoria)]
    public decimal ValorCreditoPresumidoCBS { get; set; }

    #endregion Properties
}
