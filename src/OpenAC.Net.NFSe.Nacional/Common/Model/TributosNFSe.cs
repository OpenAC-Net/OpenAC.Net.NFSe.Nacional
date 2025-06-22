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

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Representa os tributos de uma NFSe, incluindo municipal, federal e o total.
/// </summary>
public sealed class TributosNFSe
{
    /// <summary>
    /// Tributo municipal obrigatório.
    /// </summary>
    [DFeElement("tribMun", Ocorrencia = Ocorrencia.Obrigatoria)]
    public TributoMunicipal Municipal { get; set; } = new();

    /// <summary>
    /// Tributo federal opcional.
    /// </summary>
    [DFeElement("tribFed", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public TributoFederal? Federal { get; set; }

    /// <summary>
    /// Total dos tributos obrigatório.
    /// </summary>
    [DFeElement("totTrib", Ocorrencia = Ocorrencia.Obrigatoria)]
    public TotalTributos Total { get; set; } = new();
}