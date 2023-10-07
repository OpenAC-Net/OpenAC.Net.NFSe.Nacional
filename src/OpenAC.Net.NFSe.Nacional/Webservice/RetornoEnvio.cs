// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
// ***********************************************************************
// <copyright file="RetornoEnvio.cs" company="OpenAC .Net">
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
using System.Collections.Generic;
using System.Text.Json.Serialization;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Nacional.Common;

namespace OpenAC.Net.NFSe.Nacional.Webservice;

public sealed class DpsEnvioResposta
{
    #region Fields

    private NotaFiscalServico? nota;

    #endregion Fields
    
    #region Properties

    [JsonPropertyName("tipoAmbiente")]
    [JsonConverter(typeof(TipoAmbienteJsonConverter))]
    public DFeTipoAmbiente Ambiente { get; set; }

    [JsonPropertyName("versaoAplicativo")]
    public string VersaoAplicativo { get; set; } = string.Empty;

    [JsonPropertyName("dataHoraProcessamento")]
    public DateTime DataHoraProcessamento { get; set; }
    
    [JsonPropertyName("idDps")]
    public string IdDps { get; set; } = string.Empty;
    
    [JsonPropertyName("chaveAcesso")]
    public string ChaveAcesso { get; set; } = string.Empty;
    
    [JsonPropertyName("nfseXmlGZipB64")]
    [JsonConverter(typeof(XmlGzipJsonConverter))]
    public string XmlNFSe { get; set; } = string.Empty;

    [JsonIgnore] 
    public NotaFiscalServico? NFSe => XmlNFSe.IsEmpty() ? null : nota ??= NotaFiscalServico.Load(XmlNFSe);
    
    [JsonPropertyName("alertas")] 
    public List<MensagemProcessamento> Alertas { get; set; } = new();
    
    [JsonPropertyName("erros")] 
    public List<MensagemProcessamento> Erros { get; set; } = new();
    
    #endregion Properties
}