// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
// ***********************************************************************
// <copyright file="ValoresDps.cs" company="OpenAC .Net">
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

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Representa os valores de uma DPS (Documento de Prestação de Serviço).
/// </summary>
public sealed class ValoresDps
{
    /// <summary>
    /// Valores do serviço prestado.
    /// </summary>
    [DFeElement("vServPrest", Ocorrencia = Ocorrencia.Obrigatoria)]
    public ValoresServico ValoresServico { get; set; } = new();

    /// <summary>
    /// Valores de desconto, condicionados ou incondicionados.
    /// </summary>
    [DFeElement("vDescCondIncond", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public ValoresDesconto? ValoresDesconto { get; set; }

    /// <summary>
    /// Valores de dedução ou redução.
    /// </summary>
    [DFeElement("vDedRed", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public ValoresDeducaoReducao? ValoresDeducaoReducao { get; set; }

    /// <summary>
    /// Tributos incidentes na operação.
    /// </summary>
    [DFeElement("trib", Ocorrencia = Ocorrencia.Obrigatoria)]
    public TributosNFSe Tributos { get; set; } = new();
}