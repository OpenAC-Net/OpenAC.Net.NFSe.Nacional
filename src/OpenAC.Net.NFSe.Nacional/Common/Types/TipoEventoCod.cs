// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : LUCASMORAES804
// Created          : 09-09-2023
//
// Last Modified By : LUCASMORAES804
// Last Modified On : 06-05-2024
// ***********************************************************************
// <copyright file="EventoRejeicaoTomador.cs" company="OpenAC .Net">
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

namespace OpenAC.Net.NFSe.Nacional.Common.Types;

/// <summary>
/// Códigos dos tipos de eventos utilizados na NFSe Nacional.
/// </summary>
public struct TipoEventoCod
{
    /// <summary>
    /// Cancelamento do documento.
    /// </summary>
    public const string Cancelamento = "101101";

    /// <summary>
    /// Cancelamento por substituição.
    /// </summary>
    public const string CancelamentoPorSubstituicao = "105102";

    /// <summary>
    /// Solicitação de cancelamento.
    /// </summary>
    public const string SolicitacaoCancelamento = "101103";

    /// <summary>
    /// Cancelamento deferido.
    /// </summary>
    public const string CancelamentoDeferido = "105104";

    /// <summary>
    /// Cancelamento indeferido.
    /// </summary>
    public const string CancelamentoIndeferido = "105105";

    /// <summary>
    /// Confirmação pelo prestador.
    /// </summary>
    public const string ConfirmacaoPrestador = "202201";

    /// <summary>
    /// Confirmação pelo tomador.
    /// </summary>
    public const string ConfirmacaoTomador = "203202";

    /// <summary>
    /// Confirmação pelo intermediário.
    /// </summary>
    public const string ConfirmacaoIntermediario = "204203";

    /// <summary>
    /// Confirmação tácita.
    /// </summary>
    public const string ConfirmacaoTacita = "203204";

    /// <summary>
    /// Rejeição pelo prestador.
    /// </summary>
    public const string RejeicaoPrestador = "203205";

    /// <summary>
    /// Rejeição pelo tomador.
    /// </summary>
    public const string RejeicaoTomador = "203206";

    /// <summary>
    /// Rejeição pelo intermediário.
    /// </summary>
    public const string RejeicaoIntermediario = "203207";

    /// <summary>
    /// Anulação da rejeição.
    /// </summary>
    public const string AnulacaoRejeicao = "203208";

    /// <summary>
    /// Cancelamento de ofício.
    /// </summary>
    public const string CancelamentoOficio = "305101";

    /// <summary>
    /// Bloqueio de ofício.
    /// </summary>
    public const string BloqueioOficio = "305102";

    /// <summary>
    /// Desbloqueio de ofício.
    /// </summary>
    public const string DesbloqueioOficio = "305103";
}