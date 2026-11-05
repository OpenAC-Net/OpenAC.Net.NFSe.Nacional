// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 04-11-2026
//
// Last Modified By : RFTD
// Last Modified On : 04-11-2026
// ***********************************************************************
// <copyright file="RTCLocacaoImovel.cs" company="OpenAC .Net">
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

using System;
using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Serializer;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Grupo de informações relativas às operações de locação, cessão onerosa ou arrendamento de bens imóveis (TCRTCLocacaoImovel).
/// </summary>
public sealed partial class RTCLocacaoImovel
{
    /// <summary>
    /// Percentual de Copropriedade (pCopropriedade).
    /// </summary>
    [DFeElement(TipoCampo.De2, "pCopropriedade", Min = 4, Max = 6, Ocorrencia = Ocorrencia.Obrigatoria, Ordem = 1)]
    public decimal PercentualCopropriedade { get; set; }

    /// <summary>
    /// Valor total da operação com bem imóvel (vTotOper).
    /// </summary>
    [DFeElement(TipoCampo.De2, "vTotOper", Min = 4, Max = 18, Ocorrencia = Ocorrencia.Obrigatoria, Ordem = 2)]
    public decimal ValorTotalOperacao { get; set; }

    /// <summary>
    /// Valor total do Desconto Incondicionado (vDescIncondTot).
    /// </summary>
    [DFeElement(TipoCampo.De2, "vDescIncondTot", Min = 4, Max = 18, Ocorrencia = Ocorrencia.NaoObrigatoria, Ordem = 3)]
    public decimal? ValorDescontoIncondicionadoTotal { get; set; }

    /// <summary>
    /// Valor total do Desconto Condicionado (vDescCondTot).
    /// </summary>
    [DFeElement(TipoCampo.De2, "vDescCondTot", Min = 4, Max = 18, Ocorrencia = Ocorrencia.NaoObrigatoria, Ordem = 4)]
    public decimal? ValorDescontoCondicionadoTotal { get; set; }

    /// <summary>
    /// Data de vencimento original Ano, Mês e Dia (AAAA-MM-DD) (dVencOrig).
    /// </summary>
    [DFeElement(TipoCampo.Dat, "dVencOrig", Ocorrencia = Ocorrencia.NaoObrigatoria, Ordem = 5)]
    public DateTime? DataVencimentoOriginal { get; set; }
}
