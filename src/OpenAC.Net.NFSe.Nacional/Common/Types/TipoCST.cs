// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RGG
// Last Modified On : 13-02-2026
// ***********************************************************************
// <copyright file="TipoCST.cs" company="OpenAC .Net">
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
///    Código de Situação Tributária do PIS/COFINS (CST):
///    00 - Nenhum;      
///    01 - Operação Tributável com Alíquota Básica;
///    02 - Operação Tributável com Alíquota Diferenciada;
///    03 - Operação Tributável com Alíquota por Unidade de Medida de Produto;
///    04 - Operação Tributável monofásica - Revenda a Alíquota Zero;
///    05 - Operação Tributável por Substituição Tributária;
///    06 - Operação Tributável a Alíquota Zero;
///    07 - Operação Isenta da Contribuição;
///    08 - Operação sem Incidência da Contribuição;
///    09 - Operação com Suspensão da Contribuição;
///    49 - Outras Operações de Saída;
///    50 - Operação com Direito a Crédito – Vinculada Exclusivamente a Receita Tributada no Mercado Interno;
///    51 - Operação com Direito a Crédito – Vinculada Exclusivamente a Receita Não-Tributada no Mercado Interno;
///    52 - Operação com Direito a Crédito – Vinculada Exclusivamente a Receita de Exportação;
///    53 - Operação com Direito a Crédito – Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno;
///    54 - Operação com Direito a Crédito – Vinculada a Receitas Tributadas no Mercado Interno e de Exportação;
///    55 - Operação com Direito a Crédito – Vinculada a Receitas Não Tributadas no Mercado Interno e de Exportação;
///    56 - Operação com Direito a Crédito – Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno e de Exportação;
///    60 - Crédito Presumido – Operação de Aquisição Vinculada Exclusivamente a Receita Tributada no Mercado Interno;
///    61 - Crédito Presumido – Operação de Aquisição Vinculada Exclusivamente a Receita Não-Tributada no Mercado Interno;
///    62 - Crédito Presumido – Operação de Aquisição Vinculada Exclusivamente a Receita de Exportação;
///    63 - Crédito Presumido – Operação de Aquisição Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno;
///    64 - Crédito Presumido – Operação de Aquisição Vinculada a Receitas Tributadas no Mercado Interno e de Exportação;
///    65 - Crédito Presumido – Operação de Aquisição Vinculada a Receitas Não-Tributadas no Mercado Interno e de Exportação;
///    66 - Crédito Presumido – Operação de Aquisição Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno e de Exportação;
///    67 - Crédito Presumido – Outras Operações;
///    70 - Operação de Aquisição sem Direito a Crédito;
///    71 - Operação de Aquisição com Isenção;
///    72 - Operação de Aquisição com Suspensão;
///    73 - Operação de Aquisição a Alíquota Zero;
///    74 - Operação de Aquisição sem Incidência da Contribuição;
///    75 - Operação de Aquisição por Substituição Tributária;
///    98 - Outras Operações de Entrada;
///    99 - Outras Operações;
/// </summary>
public enum TipoCST
{
    /// <summary>
    /// 00 - Nenhum
    /// </summary>
    [DFeEnum("00")]
    Nenhum,

    /// <summary>
    /// 01 - Operação Tributável com Alíquota Básica
    /// </summary>
    [DFeEnum("01")]
    AliquotaBasica,

    /// <summary>
    /// 02 - Operação Tributável com Alíquota Diferenciada
    /// </summary>
    [DFeEnum("02")]
    AliquotaDiferenciada,

    /// <summary>
    /// 03 - Operação Tributável com Alíquota por Unidade de Medida de Produto
    /// </summary>
    [DFeEnum("03")]
    AliquotaUnidadeMedidaProduto,

    /// <summary>
    /// 04 - Operação Tributável monofásica - Revenda a Alíquota Zero
    /// </summary>
    [DFeEnum("04")]
    RevendaAliquotaZero,

    /// <summary>
    /// 05 - Operação Tributável por Substituição Tributária
    /// </summary>
    [DFeEnum("05")]
    SubstituicaoTributaria,

    /// <summary>
    /// 06 - Operação Tributável a Alíquota Zero
    /// </summary>
    [DFeEnum("06")]
    AliquotaZero,

    /// <summary>
    /// 07 - Operação Isenta da Contribuição
    /// </summary>
    [DFeEnum("07")]
    IsentaContribuicao,

    /// <summary>
    /// 08 - Operação sem Incidência da Contribuição
    /// </summary>
    [DFeEnum("08")]
    SemIncidenciaContribuicao,

    /// <summary>
    /// 09 - Operação com Suspensão da Contribuição
    /// </summary>
    [DFeEnum("09")]
    ComSuspensaoContribuicao,

    /// <summary>
    /// 49 - Outras Operações de Saída
    /// </summary>
    [DFeEnum("49")]
    OutrasOperacoesSaida,

    /// <summary>
    /// 50 - Operação com Direito a Crédito – Vinculada Exclusivamente a Receita Tributada no Mercado Interno
    /// </summary>
    [DFeEnum("50")]
    DireitoCreditoReceitaTributadaMercadoInterno,

    /// <summary>
    /// 51 - Operação com Direito a Crédito – Vinculada Exclusivamente a Receita Não-Tributada no Mercado Interno
    /// </summary>
    [DFeEnum("51")]
    DireitoCreditoReceitaNaoTributadaMercadoInterno,

    /// <summary>
    /// 52 - Operação com Direito a Crédito – Vinculada Exclusivamente a Receita de Exportação
    /// </summary>
    [DFeEnum("52")]
    DireitoCreditoReceitaExportacao,

    /// <summary>
    /// 53 - Operação com Direito a Crédito – Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno
    /// </summary>
    [DFeEnum("53")]
    DireitoCreditoReceitasTributadasNaoTributadasMercadoInterno,

    /// <summary>
    /// 54 - Operação com Direito a Crédito – Vinculada a Receitas Tributadas no Mercado Interno e de Exportação
    /// </summary>
    [DFeEnum("54")]
    DireitoCreditoReceitasTributadasMercadoInternoExportacao,

    /// <summary>
    /// 55 - Operação com Direito a Crédito – Vinculada a Receitas Não Tributadas no Mercado Interno e de Exportação
    /// </summary>
    [DFeEnum("55")]
    DireitoCreditoReceitasNaoTributadasMercadoInternoExportacao,

    /// <summary>
    /// 56 - Operação com Direito a Crédito – Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno e de Exportação
    /// </summary>
    [DFeEnum("56")]
    DireitoCreditoReceitasTributadasNaoTributadasMercadoInternoExportacao,

    /// <summary>
    /// 60 - Crédito Presumido – Operação de Aquisição Vinculada Exclusivamente a Receita Tributada no Mercado Interno
    /// </summary>
    [DFeEnum("60")]
    CreditoPresumidoAquisicaoReceitaTributadaMercadoInterno,

    /// <summary>
    /// 61 - Crédito Presumido – Operação de Aquisição Vinculada Exclusivamente a Receita Não-Tributada no Mercado Interno
    /// </summary>
    [DFeEnum("61")]
    CreditoPresumidoAquisicaoReceitaNaoTributadaMercadoInterno,

    /// <summary>
    /// 62 - Crédito Presumido – Operação de Aquisição Vinculada Exclusivamente a Receita de Exportação
    /// </summary>
    [DFeEnum("62")]
    CreditoPresumidoAquisicaoReceitaExportacao,

    /// <summary>
    /// 63 - Crédito Presumido – Operação de Aquisição Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno
    /// </summary>
    [DFeEnum("63")]
    CreditoPresumidoAquisicaoReceitasTributadasNaoTributadasMercadoInterno,

    /// <summary>
    /// 64 - Crédito Presumido – Operação de Aquisição Vinculada a Receitas Tributadas no Mercado Interno e de Exportação
    /// </summary>
    [DFeEnum("64")]
    CreditoPresumidoAquisicaoReceitasTributadasMercadoInternoExportacao,

    /// <summary>
    /// 65 - Crédito Presumido – Operação de Aquisição Vinculada a Receitas Não-Tributadas no Mercado Interno e de Exportação
    /// </summary>
    [DFeEnum("65")]
    CreditoPresumidoAquisicaoReceitasNaoTributadasMercadoInternoExportacao,

    /// <summary>
    /// 66 - Crédito Presumido – Operação de Aquisição Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno e de Exportação
    /// </summary>
    [DFeEnum("66")]
    CreditoPresumidoAquisicaoReceitasTributadasNaoTributadasMercadoInternoExportacao,

    /// <summary>
    /// 67 - Crédito Presumido – Outras Operações
    /// </summary>
    [DFeEnum("67")]
    CreditoPresumidoOutrasOperacoes,

    /// <summary>
    /// 70 - Operação de Aquisição sem Direito a Crédito
    /// </summary>
    [DFeEnum("70")]
    AquisicaoSemDireitoCredito,

    /// <summary>
    /// 71 - Operação de Aquisição com Isenção
    /// </summary>
    [DFeEnum("71")]
    AquisicaoComIsencao,

    /// <summary>
    /// 72 - Operação de Aquisição com Suspensão
    /// </summary>
    [DFeEnum("72")]
    AquisicaoComSuspensao,

    /// <summary>
    /// 73 - Operação de Aquisição a Alíquota Zero
    /// </summary>
    [DFeEnum("73")]
    AquisicaoAliquotaZero,

    /// <summary>
    /// 74 - Operação de Aquisição sem Incidência da Contribuição
    /// </summary>
    [DFeEnum("74")]
    AquisicaoSemIncidenciaContribuicao,

    /// <summary>
    /// 75 - Operação de Aquisição por Substituição Tributária
    /// </summary>
    [DFeEnum("75")]
    AquisicaoSubstituicaoTributaria,

    /// <summary>
    /// 98 - Outras Operações de Entrada
    /// </summary>
    [DFeEnum("98")]
    OutrasOperacoesEntrada,

    /// <summary>
    /// 99 - Outras Operações
    /// </summary>
    [DFeEnum("99")]
    OutrasOperacoes
}