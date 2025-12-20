// ***********************************************************************
// Assembly: OpenAC.Net.NFSe.Nacional
// Author           : RGG
// Created          : 19-12-2025
//
// Last Modified By : RGG
// Last Modified On : 19-12-2025
// ***********************************************************************
// <copyright file="RTCTotalCIBS.cs" company="OpenAC .Net">
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
/// Grupo de Totalizadores
/// </summary>
public sealed class RTCTotalCIBS
{
    #region Properties

    /// <summary>
 /// Valor Total da NF considerando os impostos por fora: IBS e CBS
    /// O IBS e a CBS são por fora, por isso seus valores devem ser adicionados ao valor total da NF
    /// vTotNF = vLiq (em 2026)
    /// vTotNF = vLiq + vCBS + vIBSTot (a partir de 2027)
    /// </summary>
    [DFeElement(TipoCampo.De2, "vTotNF", Min = 1, Max = 15, Ocorrencia = Ocorrencia.Obrigatoria)]
 public decimal ValorTotalNF { get; set; }

    /// <summary>
    /// Grupo de valores referentes ao IBS
 /// </summary>
    [DFeElement("gIBS", Ocorrencia = Ocorrencia.Obrigatoria)]
    public RTCTotalIBS TotalIBS { get; set; } = new();

    /// <summary>
    /// Grupo de valores referentes à CBS
    /// </summary>
    [DFeElement("gCBS", Ocorrencia = Ocorrencia.Obrigatoria)]
    public RTCTotalCBS TotalCBS { get; set; } = new();

    /// <summary>
    /// Grupo de informações de tributação regular
  /// </summary>
    [DFeElement("gTribRegular", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public RTCTotalTribRegular? TributacaoRegular { get; set; }

    /// <summary>
    /// Grupo de informações da composição do valor do IBS e da CBS em compras governamentais
    /// </summary>
    [DFeElement("gTribCompraGov", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public RTCTotalTribCompraGov? TributacaoCompraGovernamental { get; set; }

    #endregion Properties
}
