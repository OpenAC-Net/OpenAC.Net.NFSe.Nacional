// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : lucasmoraes804
// Created          : 13-05-2025
//
// Last Modified By : lucasmoraes804
// Last Modified On : 13-05-2025
// ***********************************************************************
// <copyright file="RTCIBSCBS.cs" company="OpenAC .Net">
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
/// Grupo de informacoes geradas pelo sistema referentes ao IBS e CBS.
/// </summary>
public sealed class RTCIBSCBS
{
    /// <summary>
    /// Codigo IBGE da localidade de incidencia do IBS/CBS.
    /// </summary>
    [DFeElement(TipoCampo.StrNumber, "cLocalidadeIncid", Min = 0, Max = 7, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string CodigoLocalidadeIncidencia { get; set; } = string.Empty;

    /// <summary>
    /// Nome da localidade de incidencia do IBS/CBS.
    /// </summary>
    [DFeElement(TipoCampo.Str, "xLocalidadeIncid", Min = 1, Max = 600, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string DescricaoLocalidadeIncidencia { get; set; } = string.Empty;

    /// <summary>
    /// Percentual de redutor aplicado em compra governamental.
    /// </summary>
    [DFeElement(TipoCampo.De2, "pRedutor", Min = 1, Max = 3, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public decimal PercentualRedutor { get; set; }
    
    /// <summary>
    /// Valores brutos referentes ao IBS/CBS.
    /// </summary>
    [DFeElement("valores", Ocorrencia = Ocorrencia.Obrigatoria)]
    public RTCValoresIBSCBS Valores { get; set; } = new();
    
    /// <summary>
    /// Grupo de totalizadores do IBS/CBS.
    /// </summary>
    [DFeElement("totCIBS", Ocorrencia = Ocorrencia.Obrigatoria)]
    public RTCTotalCIBS Totais { get; set; } = new();
}

