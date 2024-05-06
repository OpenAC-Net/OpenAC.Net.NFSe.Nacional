// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
// ***********************************************************************
// <copyright file="TotalTributos.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
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

namespace OpenAC.Net.NFSe.Nacional.Common;

/// <summary>
/// TIPO COMPLEXO PARA INFORMAÇÕES DE TRIBUTAÇÃO ESPECÍFICA PARA TOTAL DOS TRIBUTOS
/// So pode usar 1 dos tipos abaixo.
/// </summary>
public sealed class TotalTributos
{
    [DFeElement("vTotTrib", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public ValorTotalTributos? ValorTotal { get; set; }
    
    [DFeElement("pTotTrib", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public PorcentagemTotalTributos? PorcentagemTotal { get; set; }

    /// <summary>
    /// Indicador de informação de valor total de tributos. Possui valor fixo igual a zero (indTotTrib=0).
    /// Não informar nenhum valor estimado para os Tributos (Decreto 8.264/2014).
    /// 0 - Não;
    /// </summary>
    [DFeElement(TipoCampo.Int, "indTotTrib", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public int? IndicadorTotal { get; set; }
    
    /// <summary>
    /// Valor percentual aproximado do total dos tributos da alíquota do Simples Nacional (%)
    /// </summary>
    [DFeElement(TipoCampo.De2, "pTotTribSN", Min = 4, Max = 5, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public decimal? PercetualSimples { get; set; }
}