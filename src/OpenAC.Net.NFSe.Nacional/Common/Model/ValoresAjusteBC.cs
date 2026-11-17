// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 04-11-2026
//
// Last Modified By : RFTD
// Last Modified On : 04-11-2026
// ***********************************************************************
// <copyright file="ValoresAjusteBC.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014-2026 Grupo OpenAC.Net
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
/// Grupo de informações relativas aos valores para ajuste dos valores de base de cálculo (TCInfoAjusteBC).
/// </summary>
public sealed partial class ValoresAjusteBC
{
    /// <summary>
    /// Valor percentual padrão para ajuste dos valores de base de cálculo do ISSQN (pAjusteBCISSQN).
    /// </summary>
    [DFeElement(TipoCampo.De2, "pAjusteBCISSQN", Min = 4, Max = 6, Ocorrencia = Ocorrencia.NaoObrigatoria, Ordem = 1)]
    public decimal? PercentualAjusteBCISSQN { get; set; }

    /// <summary>
    /// Valor monetário padrão para ajuste dos valores de base de cálculo do ISSQN (vAjusteBCISSQN).
    /// </summary>
    [DFeElement(TipoCampo.De2, "vAjusteBCISSQN", Min = 4, Max = 18, Ocorrencia = Ocorrencia.NaoObrigatoria, Ordem = 2)]
    public decimal? ValorAjusteBCISSQN { get; set; }

    /// <summary>
    /// Grupo de informações de documento utilizado para ajuste dos valores de base de cálculo (ISSQN e/ou IBS/CBS) (documentos).
    /// </summary>
    [DFeCollection("documentos", Ocorrencia = Ocorrencia.NaoObrigatoria, Ordem = 3)]
    [DFeItem(typeof(DocumentoAjusteBC), "docAjusteBC")]
    public List<DocumentoAjusteBC>? Documentos { get; set; }

    /// <summary>
    /// Indica se a lista de documentos deve ser serializada.
    /// </summary>
    public bool ShouldSerializeDocumentos() => Documentos is { Count: > 0 };
}
