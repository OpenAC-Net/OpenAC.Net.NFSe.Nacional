// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
// ***********************************************************************
// <copyright file="TipoCST.cs" company="OpenAC .Net">
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

namespace OpenAC.Net.NFSe.Nacional.Common;

/// <summary>
/// Código de Situação Tributária do PIS/COFINS (CST):
/// 00 - Nenhum;
/// 01 - Operação Tributável com Alíquota Básica;
/// 02 - Operação Tributável com Alíquota Diferenciada;
/// 03 - Operação Tributável com Alíquota por Unidade de Medida de Produto;
/// 04 - Operação Tributável monofásica - Revenda a Alíquota Zero;
/// 05 - Operação Tributável por Substituição Tributária;
/// 06 - Operação Tributável a Alíquota Zero;
/// 07 - Operação Tributável da Contribuição;
/// 08 - Operação sem Incidência da Contribuição;
/// 09 - Operação com Suspensão da Contribuição;
/// </summary>
public enum TipoCST
{
    [DFeEnum("00")]
    Nenhum,
    
    [DFeEnum("01")]
    AliquotaBasica,
    
    [DFeEnum("02")]
    AliquotaDiferenciada,
    
    [DFeEnum("03")]
    AliquotaUnidadeMedidaProduto,
    
    [DFeEnum("04")]
    RevendaAliquotaZero,
    
    [DFeEnum("05")]
    SubstituicaoTributaria,
    
    [DFeEnum("06")]
    AliquotaZero,
    
    [DFeEnum("07")]
    Contribuicao,
    
    [DFeEnum("08")]
    SemIncidenciaContribuicao,
    
    [DFeEnum("09")]
    ComSuspensaoContribuicao
}