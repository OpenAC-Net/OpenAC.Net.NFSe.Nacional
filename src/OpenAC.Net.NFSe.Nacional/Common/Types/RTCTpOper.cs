// ***********************************************************************
// Assembly : OpenAC.Net.NFSe.Nacional
// Author : RGG
// Created : 15-11-2025
//
// Last Modified By : RGG
// Last Modified On : 15-11-2025
// ***********************************************************************
// <copyright file="RTCTpOper.cs" company="OpenAC .Net">
// The MIT License (MIT)
// Copyright (c) 2014-2023 Grupo OpenAC.Net
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
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
/// Tipo de Operação com Entes Governamentais ou outros serviços sobre bens imóveis.
/// </summary>
public enum RTCTpOper
{
    /// <summary>
    /// 1 – Fornecimento com pagamento posterior
    /// </summary>
    [DFeEnum("1")]
    FornecimentoPagamentoPosterior,

    /// <summary>
    /// 2 - Recebimento do pagamento com fornecimento já realizado
    /// </summary>
    [DFeEnum("2")]
    RecebimentoPagamentoFornecimentoRealizado,

    /// <summary>
    /// 3 – Fornecimento com pagamento já realizado
    /// </summary>
    [DFeEnum("3")]
    FornecimentoPagamentoRealizado,

    /// <summary>
    /// 4 – Recebimento do pagamento com fornecimento posterior
    /// </summary>
    [DFeEnum("4")]
    RecebimentoPagamentoFornecimentoPosterior,

    /// <summary>
    /// 5 – Fornecimento e recebimento do pagamento concomitantes
    /// </summary>
    [DFeEnum("5")]
    FornecimentoRecebimentoConcomitante
}
