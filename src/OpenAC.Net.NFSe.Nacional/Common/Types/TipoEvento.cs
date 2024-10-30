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

public struct TipoEvento
{
    public const string Cancelamento = "101101";
    public const string CancelamentoPorSubstituicao = "105102";
    public const string SolicitacaoCancelamento = "101103";
    public const string CancelamentoDeferido = "105104";
    public const string CancelamentoIndeferido = "105105";
    public const string ConfirmacaoPrestador = "202201";
    public const string ConfirmacaoTomador = "203202";
    public const string ConfirmacaoIntermediario = "203203";
    public const string ConfirmacaoTacita = "203204";
    public const string RejeicaoPrestador = "203205";
    public const string RejeicaoTomador = "203206";
    public const string RejeicaoIntermediario = "203207";
    public const string AnulacaoRejeicao = "203208";
    public const string CancelamentoOficio = "305101";
    public const string BloqueioOficio = "305102";
    public const string DesbloqueioOficio = "305103";
}
