// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
// ***********************************************************************
// <copyright file="InfNFSe.cs" company="OpenAC .Net">
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
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Representa as informações principais da NFSe Nacional.
/// </summary>
public sealed class InfNFSe
{
    #region Properties

    /// <summary>
    /// Identificador único da NFSe.
    /// </summary>
    [DFeAttribute(TipoCampo.Str, "Id")] 
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Local de emissão da NFSe.
    /// </summary>
    [DFeElement(TipoCampo.Str, "xLocEmi", Min = 1, Max = 150, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string LocalEmissao { get; set; } = string.Empty;
    
    /// <summary>
    /// Local de prestação do serviço.
    /// </summary>
    [DFeElement(TipoCampo.Str, "xLocPrestacao", Min = 1, Max = 150)]
    public string LocalPrestacao { get; set; } = string.Empty;
    
    /// <summary>
    /// Número da NFSe.
    /// </summary>
    [DFeElement(TipoCampo.Long, "nNFSe", Min = 1, Max = 13, Ocorrencia = Ocorrencia.Obrigatoria)]
    public long NumeroNFSe { get; set; } = 1;
    
    /// <summary>
    /// Código do local de incidência.
    /// </summary>
    [DFeElement(TipoCampo.StrNumber, "cLocIncid", Min = 0, Max = 7, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string CodLocalIncidencia { get; set; } = string.Empty;
    
    /// <summary>
    /// Descrição do local de incidência.
    /// </summary>
    [DFeElement(TipoCampo.Str, "xLocIncid", Min = 1, Max = 150, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string LocalIncidencia { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do tributo nacional.
    /// </summary>
    [DFeElement(TipoCampo.Str, "xTribNac", Min = 1, Max = 600, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string DescricaoTributoNacional { get; set; } = string.Empty;
    
    /// <summary>
    /// Descrição do tributo municipal.
    /// </summary>
    [DFeElement(TipoCampo.Str, "xTribMun", Min = 1, Max = 600, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string? DescricaoTributoMunicipal { get; set; }
    
    /// <summary>
    /// Descrição do código NBS.
    /// </summary>
    [DFeElement(TipoCampo.Str, "xNBS", Min = 1, Max = 600, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string? DescricaoCodNBS { get; set; }
    
    /// <summary>
    /// Versão da aplicação emissora.
    /// </summary>
    [DFeElement(TipoCampo.Str, "verAplic", Min = 1, Max = 20, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string VersaoAplicacao { get; set; } = "OpenAC .Net NFSe Nacional";
    
    /// <summary>
    /// Ambiente gerador da NFSe.
    /// </summary>
    [DFeElement(TipoCampo.Enum, "ambGer", Ocorrencia = Ocorrencia.Obrigatoria)]
    public AmbienteGerador AmbienteGerador { get; set; }

    /// <summary>
    /// Tipo de emissão da NFSe.
    /// </summary>
    [DFeElement(TipoCampo.Enum, "tpEmis", Ocorrencia = Ocorrencia.Obrigatoria)]
    public TipoEmissao TipoEmissao { get; set; } = TipoEmissao.ModeloNacional;
    
    /// <summary>
    /// Processo de emissão da NFSe.
    /// </summary>
    [DFeElement(TipoCampo.Enum, "procEmi", Ocorrencia = Ocorrencia.Obrigatoria)]
    public ProcessoEmissao ProcessoEmissao { get; set; } = ProcessoEmissao.AplicativoContribuinte;
    
    /// <summary>
    /// Situação da NFSe.
    /// </summary>
    [DFeElement(TipoCampo.Int, "cStat", Min = 1, Max = 13, Ocorrencia = Ocorrencia.Obrigatoria)]
    public StatusNFSe SituacaoNFSe { get; set; }
    
    /// <summary>
    /// Data e hora do processamento.
    /// </summary>
    [DFeElement(TipoCampo.DatHorTz, "dhProc", Ocorrencia = Ocorrencia.Obrigatoria)]
    public DateTimeOffset DhProcessamento { get; set; }
    
    /// <summary>
    /// Número do DFSe.
    /// </summary>
    [DFeElement(TipoCampo.Int, "nDFSe", Min = 1, Max = 13, Ocorrencia = Ocorrencia.Obrigatoria)]
    public int NumeroDFSe { get; set; }
    
    /// <summary>
    /// Dados do emitente da NFSe.
    /// </summary>
    [DFeElement("emit", Ocorrencia = Ocorrencia.Obrigatoria)]
    public EmitenteNFSe Emitente { get; set; } = new();
    
    /// <summary>
    /// Valores da NFSe.
    /// </summary>
    [DFeElement("valores", Ocorrencia = Ocorrencia.Obrigatoria)]
    public ValoresNFSe Valores { get; set; } = new();

    /// <summary>
    /// Grupo calculado do IBS/CBS.
    /// </summary>
    [DFeElement("IBSCBS", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public RTCIBSCBS? IBSCBS { get; set; }
    
    /// <summary>
    /// Dados do Documento de Prestação de Serviço (DPS).
    /// </summary>
    [DFeElement("DPS", Ocorrencia = Ocorrencia.Obrigatoria)]
    public Dps Dps { get; set; } = new();

    #endregion Properties
}