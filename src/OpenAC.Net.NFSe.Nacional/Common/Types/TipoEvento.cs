// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : Rafael Dias
// Created          : 30-10-2024
//
// Last Modified By : Rafael Dias
// Last Modified On : 30-10-2024
// ***********************************************************************
// <copyright file="TipoEvento.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014-2024 Grupo OpenAC.Net
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

namespace OpenAC.Net.NFSe.Nacional.Common.Types;

public enum TipoEvento
{
    CANCELAMENTO,
    SOLICITACAO_CANCELAMENTO_ANALISE_FISCAL,
    CANCELAMENTO_POR_SUBSTITUICAO,
    CANCELAMENTO_DEFERIDO_ANALISE_FISCAL,
    CANCELAMENTO_INDEFERIDO_ANALISE_FISCAL,
    CONFIRMACAO_PRESTADOR,
    REJEICAO_PRESTADOR,
    CONFIRMACAO_TOMADOR,
    REJEICAO_TOMADOR,
    CONFIRMACAO_INTERMEDIARIO,
    REJEICAO_INTERMEDIARIO,
    CONFIRMACAO_TACITA,
    ANULACAO_REJEICAO,
    CANCELAMENTO_POR_OFICIO,
    BLOQUEIO_POR_OFICIO,
    DESBLOQUEIO_POR_OFICIO,
    INCLUSAO_NFSE_DAN,
    TRIBUTOS_NFSE_RECOLHIDOS
}