// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
// ***********************************************************************
// <copyright file="IdDeducaoReducao.cs" company="OpenAC .Net">
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
/// Identificação da Dedução/Redução:
/// 1 – Alimentação e bebidas/frigobar;
/// 2 – Materiais;
/// 3 – Produção externa;
/// 4 – Reembolso de despesas;
/// 5 – Repasse consorciado;
/// 6 – Repasse plano de saúde;
/// 7 – Serviços;
/// 8 – Subempreitada de mão de obra;
/// 99 – Outras deduções;
/// </summary>
public enum TipoDeducaoReducao
{
    [DFeEnum("1")]
    AlimentacaoBebidasFrigobar,
    
    [DFeEnum("2")]
    Materiais,
    
    [DFeEnum("3")]
    ProducaoExterna,
    
    [DFeEnum("4")]
    ReembolsoDespesas,
    
    [DFeEnum("5")]
    RepasseConsorciado,
    
    [DFeEnum("6")]
    RepassePlanSaude,
    
    [DFeEnum("7")]
    Servicos,
    
    [DFeEnum("8")]
    SubempreitadaMaoObra,
    
    [DFeEnum("99")]
    Outras
}