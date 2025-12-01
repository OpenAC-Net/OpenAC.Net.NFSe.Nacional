// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
// ***********************************************************************
// <copyright file="ValoresNFSe.cs" company="OpenAC .Net">
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
/// Representa os valores de uma NFSe, incluindo base de cálculo, alíquotas, valores de ISSQN, retenções e informações adicionais.
/// </summary>
public sealed class ValoresNFSe
{

    /// <summary>
    /// Valor da base de cálculo após redução/dedução.
    /// </summary>
    [DFeElement(TipoCampo.De2, "vCalcDR", Min = 4, Max = 18, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public decimal? ValorBcReducaoDeducao { get; set; }


    /// <summary>
    /// Tipo do benefício municipal Aplicado
    /// </summary>
    [DFeElement(TipoCampo.Enum, "tpBM", Ocorrencia = Ocorrencia.Obrigatoria)]
    public TipoBeneficioMunicipal TipoBeneficioMunicipal { get; set; }
    /// <summary>
    /// Valor da base de cálculo do benefício municipal.
    /// </summary>
    [DFeElement(TipoCampo.De2, "vCalcBM", Min = 4, Max = 18, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public decimal? ValorBcBeneficioMunicipal { get; set; }

    /// <summary>
    /// Valor da base de cálculo do ISSQN.
    /// </summary>
    [DFeElement(TipoCampo.De2, "vBC", Min = 4, Max = 18, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public decimal? ValorBc { get; set; }

    /// <summary>
    /// Alíquota aplicada.
    /// </summary>
    [DFeElement(TipoCampo.De2, "pAliqAplic", Min = 4, Max = 4, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public decimal? Aliquota { get; set; }

    /// <summary>
    /// Valor do ISSQN.
    /// </summary>
    [DFeElement(TipoCampo.De2, "vISSQN", Min = 4, Max = 18, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public decimal? ValorISSQN { get; set; }

    /// <summary>
    /// Valor total retido.
    /// </summary>
    [DFeElement(TipoCampo.De2, "vTotalRet", Min = 4, Max = 18, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public decimal? TotalRetido { get; set; }

    /// <summary>
    /// Valor líquido da NFSe.
    /// </summary>
    [DFeElement(TipoCampo.De2, "vLiq", Min = 4, Max = 18, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public decimal? ValorLiquido { get; set; }

    /// <summary>
    /// Outras informações relevantes.
    /// </summary>
    [DFeElement(TipoCampo.Str, "xOutInf", Min = 1, Max = 2000, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? OutrasInformacoes { get; set; }
}