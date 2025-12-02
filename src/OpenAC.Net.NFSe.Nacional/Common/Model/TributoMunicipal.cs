// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
// ***********************************************************************
// <copyright file="TributosNFSe.cs" company="OpenAC .Net">
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
/// Representa os tributos municipais relacionados ao ISSQN.
/// </summary>
public sealed class TributoMunicipal
{
    /// <summary>
    /// Informações do tributo ISSQN.
    /// </summary>
    [DFeElement(TipoCampo.Enum, "tribISSQN", Ocorrencia = Ocorrencia.Obrigatoria)]
    public TributoISSQN ISSQN { get; set; }

    /// <summary>
    /// Código do país de resultado (opcional, 2 caracteres).
    /// </summary>
    [DFeElement(TipoCampo.Str, "cPaisResult", Min = 2, Max = 2, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? CodPais { get; set; }

    /// <summary>
    /// Benefício municipal (opcional).
    /// </summary>
    [DFeElement("BM", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public BeneficioMunicipal? Beneficio { get; set; }

    /// <summary>
    /// Suspensão de exigibilidade (opcional).
    /// </summary>
    [DFeElement("tribMun", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public ExigibilidadeSuspensa? Suspensao { get; set; }

    /// <summary>
    /// Tipo de imunidade (opcional).
    /// </summary>
    [DFeElement(TipoCampo.Enum, "tpImunidade", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public TipoImunidade? TipoImunidade { get; set; }


    /// <summary>
    /// Tipo de retenção do ISSQN (opcional).
    /// </summary>
    [DFeElement(TipoCampo.Enum, "tpRetISSQN", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public TipoRetencaoISSQN? TipoRetencaoISSQN { get; set; }

    /// <summary>
    /// Alíquota do tributo (opcional, 4 a 7 dígitos).
    /// </summary>
    [DFeElement(TipoCampo.De2, "pAliq", Min = 4, Max = 7, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public decimal? Aliquota { get; set; }

}