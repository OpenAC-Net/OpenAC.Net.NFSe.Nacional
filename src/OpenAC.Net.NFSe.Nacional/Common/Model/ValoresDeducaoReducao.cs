// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
// ***********************************************************************
// <copyright file="ValoresDeducaoReducao.cs" company="OpenAC .Net">
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

using System.Collections.Generic;
using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Serializer;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Representa os valores de dedução ou redução, incluindo porcentagem, valor e documentos relacionados.
/// </summary>
public sealed class ValoresDeducaoReducao
{
    /// <summary>
    /// Porcentagem de dedução ou redução.
    /// </summary>
    [DFeElement(TipoCampo.De2, "pDR", Min = 4, Max = 6, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public decimal? Porcentagem { get; set; }

    /// <summary>
    /// Valor de dedução ou redução.
    /// </summary>
    [DFeElement(TipoCampo.De2, "vDR", Min = 4, Max = 18, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public decimal? Valor { get; set; }

    /// <summary>
    /// Lista de documentos de dedução ou redução.
    /// </summary>
    [DFeCollection("documentos")]
    [DFeItem(typeof(DocumentoDeducaoReducao), "docDedRed")]
    public List<DocumentoDeducaoReducao> Documentos { get; set; } = new();
}