// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : lucasmoraes804
// Created          : 13-05-2025
//
// Last Modified By : lucasmoraes804
// Last Modified On : 13-05-2025
// ***********************************************************************
// <copyright file="RTCTotalCBSCredPres.cs" company="OpenAC .Net">
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
/// Totais de credito presumido para a CBS.
/// </summary>
public sealed class RTCTotalCBSCredPres
{
    /// <summary>
    /// Aliquota do credito presumido da CBS.
    /// </summary>
    [DFeElement(TipoCampo.De2, "pCredPresCBS", Min = 1, Max = 3, Ocorrencia = Ocorrencia.Obrigatoria)]
    public decimal PercentualCreditoPresumido { get; set; }

    /// <summary>
    /// Valor do credito presumido da CBS.
    /// </summary>
    [DFeElement(TipoCampo.De2, "vCredPresCBS", Min = 4, Max = 18, Ocorrencia = Ocorrencia.Obrigatoria)]
    public decimal ValorCreditoPresumido { get; set; }
}

