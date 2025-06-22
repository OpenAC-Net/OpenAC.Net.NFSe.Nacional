// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
// ***********************************************************************
// <copyright file="VinculoPrestador.cs" company="OpenAC .Net">
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

namespace OpenAC.Net.NFSe.Nacional.Common.Types;

/// <summary>
/// Vínculo entre as partes no negócio.
/// </summary>
public enum VinculoPrestador
{
    /// <summary>
    /// Sem vínculo com o tomador/Prestador.
    /// </summary>
    [DFeEnum("0")]
    SemVinculo = 0,

    /// <summary>
    /// Controlada.
    /// </summary>
    [DFeEnum("1")]
    Controlada = 1,

    /// <summary>
    /// Controladora.
    /// </summary>
    [DFeEnum("2")]
    Controladora = 2,

    /// <summary>
    /// Coligada.
    /// </summary>
    [DFeEnum("3")]
    Coligada = 3,

    /// <summary>
    /// Matriz.
    /// </summary>
    [DFeEnum("4")]
    Matriz = 4,

    /// <summary>
    /// Filial ou sucursal.
    /// </summary>
    [DFeEnum("5")]
    Filial = 5,

    /// <summary>
    /// Outro vínculo.
    /// </summary>
    [DFeEnum("6")]
    Outro = 6
}