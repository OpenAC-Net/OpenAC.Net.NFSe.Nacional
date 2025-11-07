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
using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Representa as informações principais da DPS (Documento de Prestação de Serviço).
/// </summary>
public sealed class InfDps
{
    #region Properties

    /// <summary>
    /// Identificador único da DPS.
    /// </summary>
    [DFeAttribute(TipoCampo.Str, "Id")] 
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Tipo de ambiente (Produção ou Homologação).
    /// </summary>
    [DFeElement(TipoCampo.Enum, "tpAmb", Ocorrencia = Ocorrencia.Obrigatoria)]
    public DFeTipoAmbiente TipoAmbiente { get; set; } = DFeTipoAmbiente.Homologacao;

    /// <summary>
    /// Data e hora de emissão da DPS.
    /// </summary>
    [DFeElement(TipoCampo.DatHorTz, "dhEmi", Ocorrencia = Ocorrencia.Obrigatoria)]
    public DateTimeOffset DhEmissao { get; set; }

    /// <summary>
    /// Versão do aplicativo emissor.
    /// </summary>
    [DFeElement(TipoCampo.Str, "verAplic", Min = 1, Max = 20, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string VersaoAplicacao { get; set; } = "OpenAC.NFSe.Nacional";

    /// <summary>
    /// Série da DPS.
    /// </summary>
    [DFeElement(TipoCampo.Str, "serie", Min = 1, Max = 5, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string Serie { get; set; } = string.Empty;

    /// <summary>
    /// Número da DPS.
    /// </summary>
    [DFeElement(TipoCampo.Int, "nDPS", Min = 1, Max = 13, Ocorrencia = Ocorrencia.Obrigatoria)]
    public int NumeroDps { get; set; } = 1;

    /// <summary>
    /// Data de competência.
    /// </summary>
    [DFeElement(TipoCampo.Dat, "dCompet", Ocorrencia = Ocorrencia.Obrigatoria)]
    public DateTime Competencia { get; set; }

    /// <summary>
    /// Tipo de emitente da DPS.
    /// </summary>
    [DFeElement(TipoCampo.Enum, "tpEmit", Ocorrencia = Ocorrencia.Obrigatoria)]
    public EmitenteDps TipoEmitente { get; set; } = EmitenteDps.Prestador;

    /// <summary>
    /// Código da localidade do emitente.
    /// </summary>
    [DFeElement(TipoCampo.Str, "cLocEmi", Min = 0, Max = 7, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string LocalidadeEmitente { get; set; } = string.Empty;

    /// <summary>
    /// Informações da DPS substituída, se houver.
    /// </summary>
    [DFeElement("subst", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public DpsSubstituida? Substituida { get; set; }

    /// <summary>
    /// Informações do prestador de serviço.
    /// </summary>
    [DFeElement("prest", Ocorrencia = Ocorrencia.Obrigatoria)]
    public PrestadorDps Prestador { get; set; } = new();
    
    /// <summary>
    /// Informações do tomador de serviço, se houver.
    /// </summary>
    [DFeElement("toma", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public InfoPessoaNFSe? Tomador { get; set; }
    
    /// <summary>
    /// Informações do intermediário, se houver.
    /// </summary>
    [DFeElement("interm", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public InfoPessoaNFSe? Intermediario { get; set; }
    
    /// <summary>
    /// Informações do serviço prestado.
    /// </summary>
    [DFeElement("serv", Ocorrencia = Ocorrencia.Obrigatoria)]
    public ServicoNFSe Servico { get; set; } = new();
    
    /// <summary>
    /// Valores da DPS.
    /// </summary>
    [DFeElement("valores", Ocorrencia = Ocorrencia.Obrigatoria)]
    public ValoresDps Valores { get; set; } = new();

    #endregion Properties
}