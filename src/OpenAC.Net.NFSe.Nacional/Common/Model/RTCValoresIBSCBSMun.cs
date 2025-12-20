// ***********************************************************************
// Assembly      : OpenAC.Net.NFSe.Nacional
// Author       : RGG
// Created          : 19-12-2025
//
// Last Modified By : RGG
// Last Modified On : 19-12-2025
// ***********************************************************************
// <copyright file="RTCValoresIBSCBSMun.cs" company="OpenAC .Net">
//		     		The MIT License (MIT)
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
/// Grupo de Informações relativas aos valores do IBS Municipal
/// </summary>
public sealed class RTCValoresIBSCBSMun
{
    #region Properties

    /// <summary>
    /// Alíquota do Município para IBS da localidade de incidência parametrizada no sistema
    /// </summary>
    [DFeElement(TipoCampo.De2, "pIBSMun", Min = 1, Max = 5, Ocorrencia = Ocorrencia.Obrigatoria)]
    public decimal AliquotaIBSMunicipal { get; set; }

    /// <summary>
    /// Percentual de redução de alíquota municipal
  /// </summary>
    [DFeElement(TipoCampo.De2, "pRedAliqMun", Min = 1, Max = 6, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public decimal? PercentualReducaoAliquotaMunicipal { get; set; }

    /// <summary>
    /// pAliqEfetMun = pIBSMun x (1 - pRedAliqMun) x (1 - pRedutor)
    /// Se pRedAliqMun não for informado na DPS, então pAliqEfetMun é a própria pIBSMun
    /// </summary>
    [DFeElement(TipoCampo.De2, "pAliqEfetMun", Min = 1, Max = 5, Ocorrencia = Ocorrencia.Obrigatoria)]
    public decimal AliquotaEfetivaMunicipal { get; set; }

    #endregion Properties
}
