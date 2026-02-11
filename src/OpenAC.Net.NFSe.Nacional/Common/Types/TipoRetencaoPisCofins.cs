// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
// ***********************************************************************
// <copyright file="TipoRetencaoPisCofins.cs" company="OpenAC .Net">
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
/// Tipo de retenção do PIS/COFINS/CSLL.
/// </summary>
public enum TipoRetencaoPisCofinsCsll
{
    /// <summary>
    /// 0 - PIS/COFINS/CSLL Não Retidos
    /// </summary>
    [DFeEnum("0")]
    PisCofinsCsllNaoRetidos,
    /// <summary>
    /// 1 - PIS/COFINS Retidos
    /// </summary>
    [DFeEnum("1")]
    PisCofinsRetidos,

    /// <summary>
    /// 2 - PIS/COFINS Não Retidos
    /// </summary>
    [DFeEnum("2")]
    PisCofinsNaoRetidos,
    
    /// <summary>
    /// 3 - PIS/COFINS/CSLL Retidos
    /// </summary>
    [DFeEnum("3")]
    PisCofinsCsllRetidos,
    
    /// <summary>
    /// 4 - PIS/COFINS Retidos, CSLL Não Retido
    /// </summary>
    [DFeEnum("4")]
    PisCofinsRetidosCsllNaoRetido,
    
    /// <summary>
    /// 5 - PIS Retido, COFINS/CSLL Não Retidos;
    /// </summary>
    [DFeEnum("5")]
    PisRetidoCofinsCsllNaoRetidos,
    
    /// <summary>
    /// 6 - COFINS Retido, PIS/CSLL Não Retidos
    /// </summary>
    [DFeEnum("6")]
    CofinsRetidoPisCSllNaoRetidos,
    
    /// <summary>
    /// 7 - PIS Não Retido, COFINS/CSLL Retidos
    /// </summary>
    [DFeEnum("7")]
    PisNaoRetidoCofinsCsllRetidos,
    
    /// <summary>
    /// 8 - PIS/COFINS Não Retidos, CSLL Retido
    /// </summary>
    [DFeEnum("8")]
    PisCofinsNaoRetidosCsllRetido,
    
    /// <summary>
    /// 9 - COFINS Não Retido, PIS/CSLL Retidos
    /// </summary>
    [DFeEnum("9")]
    CofinsNaoRetidoPisCSllRetidos,
}