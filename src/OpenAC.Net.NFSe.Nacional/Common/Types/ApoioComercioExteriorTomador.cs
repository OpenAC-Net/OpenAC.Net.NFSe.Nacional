// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
// ***********************************************************************
// <copyright file="ApoioComercioExteriorTomador.cs" company="OpenAC .Net">
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
/// Mecanismo de apoio/fomento ao Comércio Exterior utilizado pelo tomador do serviço:
/// </summary>
public enum ApoioComercioExteriorTomador
{
    /// <summary>
    /// 00 - Desconhecido (tipo não informado na nota de origem)
    /// </summary>
    [DFeEnum("00")]
    Desconhecido,
    
    /// <summary>
    /// 01 - Nenhum
    /// </summary>
    [DFeEnum("01")]
    Nenhum,
    
    /// <summary>
    /// 02 - Adm. Pública e Repr. Internacional
    /// </summary>
    [DFeEnum("02")]
    AdmPublicaReprInternacional,
    
    /// <summary>
    /// 03 - Alugueis e Arrend. Mercantil de maquinas, equip., embarc. e aeronaves
    /// </summary>
    [DFeEnum("03")]
    AlugueisArrendMercantil,
    
    /// <summary>
    /// 04 - Arrendamento Mercantil de aeronave para empresa de transporte aéreo público
    /// </summary>
    [DFeEnum("04")]
    ArrendamentoMercantilAeronaveTransportePublico,
    
    /// <summary>
    /// 05 - Comissão a agentes externos na exportação
    /// </summary>
    [DFeEnum("05")]
    ComissaoAgentesExternosExportacao,
    
    /// <summary>
    /// 06 - Despesas de armazenagem, mov. e transporte de carga no exterior
    /// </summary>
    [DFeEnum("06")]
    DespesasArmazenagemMovTransporteCargaExterior,
    
    /// <summary>
    /// 07 - Eventos FIFA (subsidiária)
    /// </summary>
    [DFeEnum("07")]
    EventosFIFASubsidiaria,
    
    /// <summary>
    /// 08 - Eventos FIFA
    /// </summary>
    [DFeEnum("08")]
    EventosFIFA,
    
    /// <summary>
    /// 09 - Fretes, arrendamentos de embarcações ou aeronaves e outros
    /// </summary>
    [DFeEnum("09")]
    FretesArrendamentosEmbarcacoesAeronavesOutros,
    
    /// <summary>
    /// 10 - Material Aeronáutico
    /// </summary>
    [DFeEnum("10")]
    MaterialAeronautico,
    
    /// <summary>
    /// 11 - Promoção de Bens no Exterior
    /// </summary>
    [DFeEnum("11")]
    PromocaoBensExterior,
    
    /// <summary>
    /// 12 - Promoção de Dest. Turísticos Brasileiros
    /// </summary>
    [DFeEnum("12")]
    PromocaoDestTuristicosBrasileiros,
    
    /// <summary>
    /// 13 - Promoção do Brasil no Exterior
    /// </summary>
    [DFeEnum("13")]
    PromocaoBrasilExterior,
    
    /// <summary>
    /// 14 - Promoção Serviços no Exterior
    /// </summary>
    [DFeEnum("14")]
    PromocaoServicosExterior,
    
    /// <summary>
    /// 15 - RECINE
    /// </summary>
    [DFeEnum("15")]
    RECINE,
    
    /// <summary>
    /// 16 - RECOPA
    /// </summary>
    [DFeEnum("16")]
    RECOPA,
    
    /// <summary>
    /// 17 - Registro e Manutenção de marcas, patentes e cultivares
    /// </summary>
    [DFeEnum("17")]
    RegistroManutencaoMarcasPatentesCultivares,
    
    /// <summary>
    /// 18 - REICOMP
    /// </summary>
    [DFeEnum("18")]
    REICOMP,
    
    /// <summary>
    /// 19 - REIDI
    /// </summary>
    [DFeEnum("19")]
    REIDI,
    
    /// <summary>
    /// 20 - REPENEC
    /// </summary>
    [DFeEnum("20")]
    REPENEC,
    
    /// <summary>
    /// 21 - REPES
    /// </summary>
    [DFeEnum("21")]
    REPES,
    
    /// <summary>
    /// 22 - RETAERO
    /// </summary>
    [DFeEnum("22")]
    RETAERO,
    
    /// <summary>
    /// 23 - RETID
    /// </summary>
    [DFeEnum("23")]
    RETID,
    
    /// <summary>
    /// 24 - Royalties, Assistência Técnica, Científica e Assemelhados
    /// </summary>
    [DFeEnum("24")]
    RoyaltiesAssistanciaTecnicaCientíficaAssemelhados,
    
    /// <summary>
    /// 25 - Serviços de avaliação da conformidade vinculados aos Acordos da OMC
    /// </summary>
    [DFeEnum("25")]
    ServiçosAvaliacaoConformidadeVinculadosAcordosOMC,
    
    /// <summary>
    /// 26 - ZPE
    /// </summary>
    [DFeEnum("26")]
    ZPE
}