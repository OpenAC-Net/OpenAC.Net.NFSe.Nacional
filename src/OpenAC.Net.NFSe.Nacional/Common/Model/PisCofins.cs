// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
// ***********************************************************************
// <copyright file="PisCofins.cs" company="OpenAC .Net">
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
using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Representa os dados de PIS e COFINS para NFSe.
/// </summary>
public sealed class PisCofins
{
    /// <summary>
    /// Código de Situação Tributária do PIS/COFINS.
    /// </summary>
    [DFeElement(TipoCampo.Enum, "CST", Min = 2, Max = 2, Ocorrencia = Ocorrencia.Obrigatoria)]
    public TipoCST Cst { get; set; } = TipoCST.Nenhum;
    
    /// <summary>
    /// Valor da base de cálculo do PIS/COFINS.
    /// </summary>
    [DFeElement(TipoCampo.De2, "vBCPisCofins", Min = 4, Max = 18, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public decimal? ValorBcCofins { get; set; }
    
    /// <summary>
    /// Alíquota do PIS.
    /// </summary>
    [DFeElement(TipoCampo.De2, "pAliqPis", Min = 4, Max = 7, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public decimal? AliquotaPis{ get; set; }
    
    /// <summary>
    /// Alíquota do COFINS.
    /// </summary>
    [DFeElement(TipoCampo.De2, "pAliqCofins", Min = 4, Max = 7, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public decimal? AliquotaCofins{ get; set; }
    
    /// <summary>
    /// Valor do PIS.
    /// </summary>
    [DFeElement(TipoCampo.De2, "vPis", Min = 4, Max = 18, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public decimal? ValorPis { get; set; }
    
    /// <summary>
    /// Valor do COFINS.
    /// </summary>
    [DFeElement(TipoCampo.De2, "vCofins", Min = 4, Max = 18, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public decimal? ValorCofins { get; set; }
    
    /// <summary>
    /// Tipo de retenção do PIS/COFINS.
    /// </summary>
    [DFeElement(TipoCampo.Enum, "tpRetPisCofins", Min = 1, Max = 1, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public TipoRetencaoPisCofinsCsll? TipoRetencao { get; set; }
}