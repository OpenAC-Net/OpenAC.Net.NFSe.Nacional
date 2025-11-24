// ***********************************************************************
// Assembly     : OpenAC.Net.NFSe.Nacional
// Author       : RGG
// Created      : 15-11-2025
//
// Last Modified By : RGG
// Last Modified On : 15-11-2025
// ***********************************************************************
// <copyright file="TipoOperacaoIBSCBS.cs" company="OpenAC .Net">
// The MIT License (MIT)
// Copyright (c)2014-2023 Grupo OpenAC.Net
// </copyright>
// ***********************************************************************

using OpenAC.Net.DFe.Core.Attributes;

namespace OpenAC.Net.NFSe.Nacional.Common.Types;

/// <summary>
/// Tipo de Operação com Entes Governamentais ou outros serviços sobre bens imóveis:
/// 1 – Fornecimento com pagamento posterior;
/// 2 - Recebimento do pagamento com fornecimento já realizado;
/// 3 – Fornecimento com pagamento já realizado;
/// 4 – Recebimento do pagamento com fornecimento posterior;
/// 5 – Fornecimento e recebimento do pagamento concomitantes.
/// </summary>
public enum TipoOperacaoIBSCBS
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
    FornecimentoPagamentoJaRealizado,

    /// <summary>
    /// 4 – Recebimento do pagamento com fornecimento posterior
    /// </summary>
    [DFeEnum("4")]
    RecebimentoPagamentoFornecimentoPosterior,

    /// <summary>
    /// 5 – Fornecimento e recebimento do pagamento concomitantes
    /// </summary>
    [DFeEnum("5")]
    FornecimentoRecebimentoConcomitantes
}
