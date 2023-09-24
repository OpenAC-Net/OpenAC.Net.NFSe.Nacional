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

namespace OpenAC.Net.NFSe.Nacional.Common;

/// <summary>
/// Mecanismo de apoio/fomento ao Comércio Exterior utilizado pelo tomador do serviço:
/// 00 - Desconhecido (tipo não informado na nota de origem);
/// 01 - Nenhum;
/// 02 - Adm. Pública e Repr. Internacional;
/// 03 - Alugueis e Arrend. Mercantil de maquinas, equip., embarc. e aeronaves;
/// 04 - Arrendamento Mercantil de aeronave para empresa de transporte aéreo público;
/// 05 - Comissão a agentes externos na exportação;
/// 06 - Despesas de armazenagem, mov. e transporte de carga no exterior;
/// 07 - Eventos FIFA (subsidiária);
/// 08 - Eventos FIFA;
/// 09 - Fretes, arrendamentos de embarcações ou aeronaves e outros;
/// 10 - Material Aeronáutico;
/// 11 - Promoção de Bens no Exterior;
/// 12 - Promoção de Dest. Turísticos Brasileiros;
/// 13 - Promoção do Brasil no Exterior;
/// 14 - Promoção Serviços no Exterior;
/// 15 - RECINE;
/// 16 - RECOPA;
/// 17 - Registro e Manutenção de marcas, patentes e cultivares;
/// 18 - REICOMP;
/// 19 - REIDI;
/// 20 - REPENEC;
/// 21 - REPES;
/// 22 - RETAERO;
/// 23 - RETID;
/// 24 - Royalties, Assistência Técnica, Científica e Assemelhados;
/// 25 - Serviços de avaliação da conformidade vinculados aos Acordos da OMC;
/// 26 - ZPE;
/// </summary>
public enum ApoioComercioExteriorTomador
{
    [DFeEnum("00")]
    Desconhecido,
    
    [DFeEnum("01")]
    Nenhum,
    
    [DFeEnum("02")]
    AdmPublicaReprInternacional,
    
    [DFeEnum("03")]
    AlugueisArrendMercantil,
    
    [DFeEnum("04")]
    ArrendamentoMercantilAeronaveTransportePublico,
    
    [DFeEnum("05")]
    ComissaoAgentesExternosExportacao,
    
    [DFeEnum("06")]
    DespesasArmazenagemMovTransporteCargaExterior,
    
    [DFeEnum("07")]
    EventosFIFASubsidiaria,
    
    [DFeEnum("08")]
    EventosFIFA,
    
    [DFeEnum("09")]
    FretesArrendamentosEmbarcacoesAeronavesOutros,
    
    [DFeEnum("10")]
    MaterialAeronautico,
    
    [DFeEnum("11")]
    PromocaoBensExterior,
    
    [DFeEnum("12")]
    PromocaoDestTuristicosBrasileiros,
    
    [DFeEnum("13")]
    PromocaoBrasilExterior,
    
    [DFeEnum("14")]
    PromocaoServicosExterior,
    
    [DFeEnum("15")]
    RECINE,
    
    [DFeEnum("16")]
    RECOPA,
    
    [DFeEnum("17")]
    RegistroManutencaoMarcasPatentesCultivares,
    
    [DFeEnum("18")]
    REICOMP,
    
    [DFeEnum("19")]
    REIDI,
    
    [DFeEnum("20")]
    REPENEC,
    
    [DFeEnum("21")]
    REPES,
    
    [DFeEnum("22")]
    RETAERO,
    
    [DFeEnum("23")]
    RETID,
    
    [DFeEnum("24")]
    RoyaltiesAssistanciaTecnicaCientíficaAssemelhados,
    
    [DFeEnum("25")]
    ServiçosAvaliacaoConformidadeVinculadosAcordosOMC,
    
    [DFeEnum("26")]
    ZPE
}