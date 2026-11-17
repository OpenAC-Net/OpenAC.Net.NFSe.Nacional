// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 04-11-2026
//
// Last Modified By : RFTD
// Last Modified On : 04-11-2026
// ***********************************************************************
// <copyright file="RTCTpAjusteBC.cs" company="OpenAC .Net">
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

using OpenAC.Net.DFe.Core.Attributes;

namespace OpenAC.Net.NFSe.Nacional.Common.Types;

/// <summary>
/// Tipo de Ajuste da Base de Cálculo (TSRTCTpAjusteBC).
/// </summary>
public enum RTCTpAjusteBC
{
    /// <summary>
    /// 2 - Materiais.
    /// </summary>
    [DFeEnum("2")]
    Materiais,

    /// <summary>
    /// 5 - Repasse consorciado.
    /// </summary>
    [DFeEnum("5")]
    RepasseConsorciado,

    /// <summary>
    /// 6 - Repasse plano de saúde.
    /// </summary>
    [DFeEnum("6")]
    RepassePlanoSaude,

    /// <summary>
    /// 7 - Serviços.
    /// </summary>
    [DFeEnum("7")]
    Servicos,

    /// <summary>
    /// 8 - Subempreitada de mão de obra.
    /// </summary>
    [DFeEnum("8")]
    SubempreitadaMaoObra,

    /// <summary>
    /// 9 - Profissional parceiro.
    /// </summary>
    [DFeEnum("9")]
    ProfissionalParceiro,

    /// <summary>
    /// 99 - Outras deduções exclusivas do ISSQN.
    /// </summary>
    [DFeEnum("99")]
    OutrasDeducoesISSQN,

    /// <summary>
    /// 101 - Repasse de remuneração por intermediação de imóveis a demais corretores envolvidos na operação.
    /// </summary>
    [DFeEnum("101")]
    RepasseIntermediacaoImoveis,

    /// <summary>
    /// 102 - Repasse de valores a fornecedor relativo a fornecimento intermediado por agência de turismo.
    /// </summary>
    [DFeEnum("102")]
    RepasseAgenciaTurismo,

    /// <summary>
    /// 103 - Reembolso ou ressarcimento recebido por agência de propaganda e publicidade por valores pagos relativos a serviços de produção externa por conta e ordem de terceiro.
    /// </summary>
    [DFeEnum("103")]
    ReembolsoPropagandaProducaoExterna,

    /// <summary>
    /// 104 - Reembolso ou ressarcimento recebido por agência de propaganda e publicidade por valores pagos relativos a serviços de mídia por conta e ordem de terceiro.
    /// </summary>
    [DFeEnum("104")]
    ReembolsoPropagandaMidia,

    /// <summary>
    /// 105 - Glosa de Serviços de Saúde.
    /// </summary>
    [DFeEnum("105")]
    GlosaServicosSaude,

    /// <summary>
    /// 199 - Outros reembolsos ou ressarcimentos recebidos por valores pagos relativos a operações por conta e ordem de terceiro.
    /// </summary>
    [DFeEnum("199")]
    OutrosReembolsosRessarcimentos
}
