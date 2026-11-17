// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 04-11-2026
//
// Last Modified By : RFTD
// Last Modified On : 04-11-2026
// ***********************************************************************
// <copyright file="DocumentoAjusteBC.cs" company="OpenAC .Net">
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

using System;
using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Grupo de informações de documento utilizado para ajuste dos valores de base de cálculo (ISSQN e/ou IBS/CBS) (TCDocAjusteBC).
/// </summary>
public sealed partial class DocumentoAjusteBC
{
    /// <summary>
    /// Tipo de Ajuste (tpAjusteBC).
    /// </summary>
    [DFeElement(TipoCampo.Enum, "tpAjusteBC", Ocorrencia = Ocorrencia.Obrigatoria, Ordem = 1)]
    public RTCTpAjusteBC TipoAjusteBC { get; set; }

    /// <summary>
    /// Descrição do ajuste quando o tipo informado exige detalhamento (xTpAjusteBC).
    /// </summary>
    [DFeElement(TipoCampo.Str, "xTpAjusteBC", Min = 1, Max = 150, Ocorrencia = Ocorrencia.NaoObrigatoria, Ordem = 2)]
    public string? DescricaoTipoAjusteBC { get; set; }

    /// <summary>
    /// Valor monetário total do documento informado (R$) (vTotDoc).
    /// </summary>
    [DFeElement(TipoCampo.De2, "vTotDoc", Min = 4, Max = 18, Ocorrencia = Ocorrencia.Obrigatoria, Ordem = 3)]
    public decimal ValorTotalDocumento { get; set; }

    /// <summary>
    /// Valor monetário efetivamente utilizado para ajuste de base(s) de cálculo do valor do serviço da NFS-e que está sendo emitida (R$) (vAjusteAplic).
    /// </summary>
    [DFeElement(TipoCampo.De2, "vAjusteAplic", Min = 4, Max = 18, Ocorrencia = Ocorrencia.Obrigatoria, Ordem = 4)]
    public decimal ValorAjusteAplicado { get; set; }

    /// <summary>
    /// Data da emissão do documento dedutível. Ano, mês e dia (AAAA-MM-DD) (dtEmiDoc).
    /// </summary>
    [DFeElement(TipoCampo.Dat, "dtEmiDoc", Ocorrencia = Ocorrencia.NaoObrigatoria, Ordem = 5)]
    public DateTime? DataEmissaoDocumento { get; set; }

    /// <summary>
    /// Data da competência do documento dedutível. Ano, mês e dia (AAAA-MM-DD) (dtCompDoc).
    /// </summary>
    [DFeElement(TipoCampo.Dat, "dtCompDoc", Ocorrencia = Ocorrencia.NaoObrigatoria, Ordem = 6)]
    public DateTime? DataCompetenciaDocumento { get; set; }

    #region Choice

    /// <summary>
    /// Grupo de informações de documentos fiscais eletrônicos que se encontram no repositório nacional (dFeNacional).
    /// </summary>
    [DFeElement("dFeNacional", Ocorrencia = Ocorrencia.NaoObrigatoria, Ordem = 7)]
    public RTCListaDocDFe? DocumentoFeNacional { get; set; }

    /// <summary>
    /// Grupo de informações de documentos fiscais, eletrônicos ou não, que não se encontram no repositório nacional (docFiscalOutro).
    /// </summary>
    [DFeElement("docFiscalOutro", Ocorrencia = Ocorrencia.NaoObrigatoria, Ordem = 7)]
    public RTCListaDocFiscalOutro? DocumentoFiscalOutro { get; set; }

    /// <summary>
    /// Grupo de informações de documento não fiscal (docOutro).
    /// </summary>
    [DFeElement("docOutro", Ocorrencia = Ocorrencia.NaoObrigatoria, Ordem = 7)]
    public RTCListaDocOutro? DocumentoOutro { get; set; }

    #endregion Choice

    /// <summary>
    /// Grupo de informações do fornecedor do serviço prestado (fornec).
    /// </summary>
    [DFeElement("fornec", Ocorrencia = Ocorrencia.NaoObrigatoria, Ordem = 8)]
    public InfoPessoaNFSe? Fornecedor { get; set; }
}
