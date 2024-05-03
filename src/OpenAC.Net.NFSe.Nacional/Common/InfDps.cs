// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
// ***********************************************************************
// <copyright file="InfDps.cs" company="OpenAC .Net">
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

public sealed class InfDps
{
    #region Properties

    [DFeAttribute(TipoCampo.Str, "Id")] 
    public string Id { get; set; } = string.Empty;
    
    [DFeElement(TipoCampo.Enum, "tpAmb", Ocorrencia = Ocorrencia.Obrigatoria)]
    public DFeTipoAmbiente TipoAmbiente { get; set; } = DFeTipoAmbiente.Homologacao;

    [DFeElement(TipoCampo.DatHorTz, "dhEmi", Ocorrencia = Ocorrencia.Obrigatoria)]
    public DateTimeOffset DhEmissao { get; set; }

    [DFeElement(TipoCampo.Str, "verAplic", Min = 1, Max = 20, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string VersaoAplicacao { get; set; } = "OpenAC .Net NFSe Nacional";

    [DFeElement(TipoCampo.Str, "serie", Min = 1, Max = 5, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string Serie { get; set; } = string.Empty;

    [DFeElement(TipoCampo.Int, "nDPS", Min = 1, Max = 13, Ocorrencia = Ocorrencia.Obrigatoria)]
    public int NumeroDps { get; set; } = 1;

    [DFeElement(TipoCampo.Dat, "dCompet", Ocorrencia = Ocorrencia.Obrigatoria)]
    public DateTime Competencia { get; set; }

    [DFeElement(TipoCampo.Enum, "tpEmit", Ocorrencia = Ocorrencia.Obrigatoria)]
    public EmitenteDps TipoEmitente { get; set; } = EmitenteDps.Prestador;

    [DFeElement(TipoCampo.Str, "cLocEmi", Min = 0, Max = 7, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string LocalidadeEmitente { get; set; } = string.Empty;

    [DFeElement("subst", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public DpsSubstituida? Substituida { get; set; }

    [DFeElement("prest", Ocorrencia = Ocorrencia.Obrigatoria)]
    public PrestadorDps Prestador { get; set; } = new();
    
    [DFeElement("toma", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public InfoPessoaNFSe? Tomador { get; set; }
    
    [DFeElement("interm", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public InfoPessoaNFSe? Intermediario { get; set; }
    
    [DFeElement("serv", Ocorrencia = Ocorrencia.Obrigatoria)]
    public ServicoNFSe Servico { get; set; } = new();
    
    [DFeElement("valores", Ocorrencia = Ocorrencia.Obrigatoria)]
    public ValoresDps Valores { get; set; } = new();

    #endregion Properties
}