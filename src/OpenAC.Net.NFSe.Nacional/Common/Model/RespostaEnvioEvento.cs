// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : Rafael Dias
// Created          : 09-09-2023
//
// Last Modified By : Rafael Dias
// Last Modified On : 30-10-2024
// ***********************************************************************
// <copyright file="RespostaEnvioEvento.cs" company="OpenAC .Net">
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

using OpenAC.Net.NFSe.Nacional.Common.Converter;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Representa a resposta do envio de evento, contendo o XML do evento em formato GZip e Base64.
/// </summary>
public sealed class RespostaEnvioEvento : RespostaBase
{
    /// <summary>
    /// Obtém ou define o XML do evento compactado em GZip e codificado em Base64.
    /// </summary>
    [JsonPropertyName("eventoXmlGZipB64")]
    [JsonConverter(typeof(XmlGzipJsonConverter))]
    public string XmlEvento { get; set; } = string.Empty;

    /// <summary>
    /// Propriedade alternativa para capturar "erro" do JSON quando a API retorna "erro" ao invés de "erros".
    /// Não deve ser usada diretamente. Use a propriedade <see cref="RespostaBase.Erros"/> para acessar os erros.
    /// </summary>
    [JsonPropertyName("erro")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<MensagemProcessamento>? ErroAlternativo
    {
        set
        {
            if (value != null)
                Erros = value;
        }
    }
}