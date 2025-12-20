// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : lucasmoraes804
// Created          : 13-05-2025
//
// Last Modified By : lucasmoraes804
// Last Modified On : 13-05-2025
// ***********************************************************************
// <copyright file="RTCValoresIBSCBS.cs" company="OpenAC .Net">
// The MIT License (MIT)
// Copyright (c) 2014-2025 Grupo OpenAC.Net
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// </copyright>
// ***********************************************************************

using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Serializer;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Valores brutos calculados para IBS/CBS.
/// </summary>
public sealed class RTCValoresIBSCBS
{
    /// <summary>
    /// Base de calculo do IBS/CBS antes das reducoes.
    /// </summary>
    [DFeElement(TipoCampo.De2, "vBC", Min = 4, Max = 18, Ocorrencia = Ocorrencia.Obrigatoria)]
    public decimal ValorBaseCalculo { get; set; }

    /// <summary>
    /// Valor monetario relativo a reembolso, repasse ou ressarcimento.
    /// </summary>
    [DFeElement(TipoCampo.De2, "vCalcReeRepRes", Min = 4, Max = 18, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public decimal? ValorCalcReeRepRes { get; set; }

    /// <summary>
    /// Detalhamento dos valores do IBS estadual.
    /// </summary>
    [DFeElement("uf", Ocorrencia = Ocorrencia.Obrigatoria)]
    public RTCValoresIBSCBSUf Estado { get; set; } = new();

    /// <summary>
    /// Detalhamento dos valores do IBS municipal.
    /// </summary>
    [DFeElement("mun", Ocorrencia = Ocorrencia.Obrigatoria)]
    public RTCValoresIBSCBSMunicipio Municipio { get; set; } = new();

    /// <summary>
    /// Detalhamento dos valores da CBS.
    /// </summary>
    [DFeElement("fed", Ocorrencia = Ocorrencia.Obrigatoria)]
    public RTCValoresIBSCBSFederal Federal { get; set; } = new();
}

