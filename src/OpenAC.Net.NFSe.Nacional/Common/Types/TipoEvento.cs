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

/// <summary>
/// Define os tipos de eventos possíveis para NFSe.
/// </summary>
public enum TipoEvento
{
    /// <summary>
    /// Cancelamento da NFSe.
    /// </summary>
    CANCELAMENTO,

    /// <summary>
    /// Solicitação de cancelamento em análise fiscal.
    /// </summary>
    SOLICITACAO_CANCELAMENTO_ANALISE_FISCAL,

    /// <summary>
    /// Cancelamento por substituição.
    /// </summary>
    CANCELAMENTO_POR_SUBSTITUICAO,

    /// <summary>
    /// Cancelamento deferido após análise fiscal.
    /// </summary>
    CANCELAMENTO_DEFERIDO_ANALISE_FISCAL,

    /// <summary>
    /// Cancelamento indeferido após análise fiscal.
    /// </summary>
    CANCELAMENTO_INDEFERIDO_ANALISE_FISCAL,

    /// <summary>
    /// Confirmação pelo prestador.
    /// </summary>
    CONFIRMACAO_PRESTADOR,

    /// <summary>
    /// Rejeição pelo prestador.
    /// </summary>
    REJEICAO_PRESTADOR,

    /// <summary>
    /// Confirmação pelo tomador.
    /// </summary>
    CONFIRMACAO_TOMADOR,

    /// <summary>
    /// Rejeição pelo tomador.
    /// </summary>
    REJEICAO_TOMADOR,

    /// <summary>
    /// Confirmação pelo intermediário.
    /// </summary>
    CONFIRMACAO_INTERMEDIARIO,

    /// <summary>
    /// Rejeição pelo intermediário.
    /// </summary>
    REJEICAO_INTERMEDIARIO,

    /// <summary>
    /// Confirmação tácita.
    /// </summary>
    CONFIRMACAO_TACITA,

    /// <summary>
    /// Anulação de rejeição.
    /// </summary>
    ANULACAO_REJEICAO,

    /// <summary>
    /// Cancelamento por ofício.
    /// </summary>
    CANCELAMENTO_POR_OFICIO,

    /// <summary>
    /// Bloqueio por ofício.
    /// </summary>
    BLOQUEIO_POR_OFICIO,

    /// <summary>
    /// Desbloqueio por ofício.
    /// </summary>
    DESBLOQUEIO_POR_OFICIO,

    /// <summary>
    /// Inclusão de NFSe DAN.
    /// </summary>
    INCLUSAO_NFSE_DAN,

    /// <summary>
    /// Tributos da NFSe recolhidos.
    /// </summary>
    TRIBUTOS_NFSE_RECOLHIDOS
}