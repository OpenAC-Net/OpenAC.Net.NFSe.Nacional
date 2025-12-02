// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
// ***********************************************************************
// <copyright file="TipoBeneficioMunicipal.cs" company="OpenAC .Net">
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
/// Enumera os tipos de benefício municipal.
///   Tipo Benefício Municipal (BM):
///   1) Isenção;
///   2) Redução da BC em 'ppBM' %;
///   3) Redução da BC em R$ 'vInfoBM';
///   4) Alíquota Diferenciada de 'aliqDifBM' %;
/// </summary>
public enum TipoBeneficioMunicipal
{
    /// <summary>
    /// Isenção;
    /// </summary>
    [DFeEnum("1")]
    Isencao,

    /// <summary>
    /// Redução da BC em 'ppBM' %;
    /// </summary>
    [DFeEnum("2")]
    ReducaoBCPerc,

    /// <summary>
    /// Redução da BC em R$ 'vInfoBM';
    /// </summary>
    [DFeEnum("3")]
    ReducaoBCValor,

    /// <summary>
    /// Alíquota Diferenciada de 'aliqDifBM' %;
    /// </summary>
    [DFeEnum("4")]
    AliquotaDiferenciada,

}