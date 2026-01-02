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
using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Representa as informações do pedido de registro de evento da NFSe Nacional.
/// </summary>
public sealed class InfPedReg
{
    #region Properties

    /// <summary>
    /// Identificador único do pedido.
    /// </summary>
    [DFeAttribute(TipoCampo.Str, "Id")] 
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Tipo de ambiente (Produção ou Homologação).
    /// </summary>
    [DFeElement(TipoCampo.Enum, "tpAmb", Ocorrencia = Ocorrencia.Obrigatoria)]
    public DFeTipoAmbiente TipoAmbiente { get; set; } = DFeTipoAmbiente.Homologacao;
    
    /// <summary>
    /// Versão do aplicativo emissor.
    /// </summary>
    [DFeElement(TipoCampo.Str, "verAplic", Min = 1, Max = 20, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string VersaoAplicacao { get; set; } = "OpenAC.NFSe.Nacional";

    /// <summary>
    /// Data e hora do evento.
    /// </summary>
    [DFeElement(TipoCampo.DatHorTz, "dhEvento", Ocorrencia = Ocorrencia.Obrigatoria)]
    public DateTimeOffset DhEvento { get; set; }
    
    /// <summary>
    /// CNPJ do autor do evento.
    /// </summary>
    [DFeElement(TipoCampo.StrNumber, "CNPJAutor", Min = 14, Max = 14, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? CNPJAutor { get; set; }
    
    /// <summary>
    /// CPF do autor do evento.
    /// </summary>
    [DFeElement(TipoCampo.StrNumber, "CPFAutor", Min = 11, Max = 11, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? CPFAutor { get; set; }

    /// <summary>
    /// Chave da NFSe relacionada ao evento.
    /// </summary>
    [DFeElement(TipoCampo.Str, "chNFSe", Min = 50, Max = 50, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string ChNFSe { get; set; } = string.Empty;
    
    /// <summary>
    /// Número do pedido de registro do evento.
    /// </summary>
    [DFeElement(TipoCampo.Int, "nPedRegEvento", Min = 0, Max = 3, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public int NumeroPedido { get; set; }

    /// <summary>
    /// Evento relacionado à NFSe.
    /// </summary>
    [DFeItem(typeof(EventoCancelamento), $"e{TipoEventoCod.Cancelamento}")]
    [DFeItem(typeof(EventoCancelamentoPorSubstituicao), $"e{TipoEventoCod.CancelamentoPorSubstituicao}")]
    [DFeItem(typeof(EventoSolicitacaoCancelamento), $"e{TipoEventoCod.SolicitacaoCancelamento}")]
    [DFeItem(typeof(EventoCancelamentoDeferido), $"e{TipoEventoCod.CancelamentoDeferido}")]
    [DFeItem(typeof(EventoCancelamentoIndeferido), $"e{TipoEventoCod.CancelamentoIndeferido}")]
    [DFeItem(typeof(EventoConfirmacaoPrestador), $"e{TipoEventoCod.ConfirmacaoPrestador}")]
    [DFeItem(typeof(EventoConfirmacaoTomador), $"e{TipoEventoCod.ConfirmacaoTomador}")]
    [DFeItem(typeof(EventoConfirmacaoIntermediario), $"e{TipoEventoCod.ConfirmacaoIntermediario}")]
    [DFeItem(typeof(EventoConfirmacaoTacita), $"e{TipoEventoCod.ConfirmacaoTacita}")]
    [DFeItem(typeof(EventoRejeicaoPrestador), $"e{TipoEventoCod.RejeicaoPrestador}")]
    [DFeItem(typeof(EventoRejeicaoTomador), $"e{TipoEventoCod.RejeicaoTomador}")]
    [DFeItem(typeof(EventoRejeicaoIntermediario), $"e{TipoEventoCod.RejeicaoIntermediario}")]
    [DFeItem(typeof(EventoAnulacaoRejeicao), $"e{TipoEventoCod.AnulacaoRejeicao}")]
    [DFeItem(typeof(EventoCancelamentoOficio), $"e{TipoEventoCod.CancelamentoOficio}")]
    [DFeItem(typeof(EventoBloqueioOficio), $"e{TipoEventoCod.BloqueioOficio}")]
    [DFeItem(typeof(EventoDesbloqueioOficio), $"e{TipoEventoCod.DesbloqueioOficio}")]
    public IEventoNFSe Evento { get; set; } = null!;

    #endregion Properties
}