// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
// ***********************************************************************
// <copyright file="InfPedReg.cs" company="OpenAC .Net">
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

using System;
using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.DFe.Core.Serializer;

namespace OpenAC.Net.NFSe.Nacional.Common;

public sealed class InfPedReg
{
    #region Properties

    [DFeAttribute(TipoCampo.Str, "Id")] 
    public string Id { get; set; } = string.Empty;
    
    [DFeElement(TipoCampo.Enum, "tpAmb", Ocorrencia = Ocorrencia.Obrigatoria)]
    public DFeTipoAmbiente TipoAmbiente { get; set; } = DFeTipoAmbiente.Homologacao;
    
    [DFeElement(TipoCampo.Str, "verAplic", Min = 1, Max = 20, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string VersaoAplicacao { get; set; } = "OpenAC .Net NFSe Nacional";
    
    [DFeElement(TipoCampo.DatHorTz, "dhEvento", Ocorrencia = Ocorrencia.Obrigatoria)]
    public DateTimeOffset DhEvento { get; set; }
    
    [DFeElement(TipoCampo.StrNumber, "CNPJAutor", Min = 14, Max = 14, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? CNPJAutor { get; set; }
    
    [DFeElement(TipoCampo.StrNumber, "CPFAutor", Min = 11, Max = 11, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? CPFAutor { get; set; }

    [DFeElement(TipoCampo.Str, "chNFSe", Min = 50, Max = 50, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string ChNFSe { get; set; } = string.Empty;
    
    [DFeElement(TipoCampo.Int, "nPedRegEvento", Min = 1, Max = 3, Ocorrencia = Ocorrencia.Obrigatoria)]
    public int NumeroPedido { get; set; }

    [DFeItem(typeof(EventoCancelamento), "e101101")]
    [DFeItem(typeof(EventoCancelamentoPorSubstituicao), "e105102")]
    [DFeItem(typeof(EventoSolicitacaoCancelamento), "e101103")]
    [DFeItem(typeof(EventoCancelamentoDeferido), "e105104")]
    [DFeItem(typeof(EventoCancelamentoIndeferido), "e105105")]
    [DFeItem(typeof(EventoConfirmacaoPrestador), "e202201")]
    [DFeItem(typeof(EventoConfirmacaoTomador), "e203202")]
    [DFeItem(typeof(EventoConfirmacaoIntermediario), "e203203")]
    [DFeItem(typeof(EventoConfirmacaoTacita), "e203204")]
    [DFeItem(typeof(EventoRejeicaoPrestador), "e203205")]
    [DFeItem(typeof(EventoRejeicaoTomador), "e203206")]
    [DFeItem(typeof(EventoRejeicaoIntermediario), "e203207")]
    [DFeItem(typeof(EventoAnulacaoRejeicao), "e203208")]
    [DFeItem(typeof(EventoCancelamentoOficio), "e305101")]
    [DFeItem(typeof(EventoBloqueioOficio), "e305102")]
    [DFeItem(typeof(EventoDesbloqueioOficio), "e305103")]
    public IEventoNFSe Evento { get; set; } = null!;

    #endregion Properties
}