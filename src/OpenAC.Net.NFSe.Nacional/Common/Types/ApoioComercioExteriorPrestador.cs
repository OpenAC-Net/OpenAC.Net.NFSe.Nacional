// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
// ***********************************************************************
// <copyright file="MecanismoApoioComercioExterior.cs" company="OpenAC .Net">
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
/// Mecanismo de apoio/fomento ao Comércio Exterior utilizado pelo prestador do serviço:
/// 00 - Desconhecido (tipo não informado na nota de origem);
/// 01 - Nenhum;
/// 02 - ACC - Adiantamento sobre Contrato de Câmbio – Redução a Zero do IR e do IOF;
/// 03 - ACE – Adiantamento sobre Cambiais Entregues - Redução a Zero do IR e do IOF;
/// 04 - BNDES-Exim Pós-Embarque – Serviços;
/// 05 - BNDES-Exim Pré-Embarque - Serviços;
/// 06 - FGE - Fundo de Garantia à Exportação;
/// 07 - PROEX - EQUALIZAÇÃO
/// 08 - PROEX - Financiamento;
/// </summary>
public enum ApoioComercioExteriorPrestador
{
    [DFeEnum("00")]
    Desconhecido = 0,
    
    [DFeEnum("01")]
    Nenhum = 1,
    
    [DFeEnum("02")]
    ACC = 2,
    
    [DFeEnum("03")]
    ACE = 3,
    
    [DFeEnum("04")]
    BNDESEximPosEmbarque = 4,
    
    [DFeEnum("05")]
    BNDESEximPreEmbarque = 5,
    
    [DFeEnum("06")]
    FGE = 6,
    
    [DFeEnum("07")]
    PROEXEqualizacao = 7,
    
    [DFeEnum("08")]
    PROEXFinanciamento = 8
}