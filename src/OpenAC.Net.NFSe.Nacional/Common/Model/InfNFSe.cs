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

public sealed class InfNFSe
{
    #region Properties

    [DFeAttribute(TipoCampo.Str, "Id")] 
    public string Id { get; set; } = string.Empty;
    
    [DFeElement(TipoCampo.Str, "xLocEmi", Min = 1, Max = 150, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string LocalEmissao { get; set; } = string.Empty;
    
    [DFeElement(TipoCampo.Str, "xLocEmi", Min = 1, Max = 150, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string LocalPrestacao { get; set; } = string.Empty;
    
    [DFeElement(TipoCampo.Int, "nNFSe", Min = 1, Max = 13, Ocorrencia = Ocorrencia.Obrigatoria)]
    public int NumeroNFSe { get; set; } = 1;
    
    [DFeElement(TipoCampo.StrNumber, "cLocIncid", Min = 0, Max = 7, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string CodLocalIncidencia { get; set; } = string.Empty;
    
    [DFeElement(TipoCampo.Str, "xLocIncid", Min = 1, Max = 150, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string LocalIncidencia { get; set; } = string.Empty;

    [DFeElement(TipoCampo.Str, "xTribNac", Min = 1, Max = 600, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string DescricaoTributoNacional { get; set; } = string.Empty;
    
    [DFeElement(TipoCampo.Str, "xTribMun", Min = 1, Max = 600, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string? DescricaoTributoMunicipal { get; set; }
    
    [DFeElement(TipoCampo.Str, "xNBS", Min = 1, Max = 600, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string? DescricaoCodNBS { get; set; }
    
    [DFeElement(TipoCampo.Str, "verAplic", Min = 1, Max = 20, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string VersaoAplicacao { get; set; } = "OpenAC .Net NFSe Nacional";
    
    [DFeElement(TipoCampo.Enum, "ambGer", Ocorrencia = Ocorrencia.Obrigatoria)]
    public AmbienteGerador AmbienteGerador { get; set; }

    [DFeElement(TipoCampo.Enum, "tpEmis", Ocorrencia = Ocorrencia.Obrigatoria)]
    public TipoEmissao TipoEmissao { get; set; } = TipoEmissao.ModeloNacional;
    
    [DFeElement(TipoCampo.Enum, "procEmi", Ocorrencia = Ocorrencia.Obrigatoria)]
    public ProcessoEmissao ProcessoEmissao { get; set; } = ProcessoEmissao.AplicativoContribuinte;
    
    [DFeElement(TipoCampo.Int, "cStat", Min = 1, Max = 13, Ocorrencia = Ocorrencia.Obrigatoria)]
    public StatusNFSe SituacaoNFSe { get; set; }
    
    [DFeElement(TipoCampo.DatHorTz, "dhProc", Ocorrencia = Ocorrencia.Obrigatoria)]
    public DateTimeOffset DhProcessamento { get; set; }
    
    [DFeElement(TipoCampo.Int, "nDFSe", Min = 1, Max = 13, Ocorrencia = Ocorrencia.Obrigatoria)]
    public int NumeroDFSe { get; set; }
    
    [DFeElement("emit", Ocorrencia = Ocorrencia.Obrigatoria)]
    public EmitenteNFSe Emitente { get; set; } = new();
    
    [DFeElement("valores", Ocorrencia = Ocorrencia.Obrigatoria)]
    public ValoresNFSe Valores { get; set; } = new();
    
    [DFeElement("DPS", Ocorrencia = Ocorrencia.Obrigatoria)]
    public Dps Dps { get; set; } = new();

    #endregion Properties
}