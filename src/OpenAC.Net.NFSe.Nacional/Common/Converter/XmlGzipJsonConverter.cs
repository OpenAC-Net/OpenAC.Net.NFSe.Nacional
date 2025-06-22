// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
// ***********************************************************************
// <copyright file="XmlGzipJsonConverter.cs" company="OpenAC .Net">
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
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenAC.Net.Core.Extensions;

namespace OpenAC.Net.NFSe.Nacional.Common.Converter;

/// <summary>
/// Conversor JSON para strings XML compactadas em GZip e codificadas em Base64.
/// </summary>
public class XmlGzipJsonConverter : JsonConverter<string>
{
    /// <summary>
    /// Lê uma string codificada em Base64 e compactada em GZip de JSON, descompacta e retorna o XML como string.
    /// </summary>
    /// <param name="reader">Leitor JSON.</param>
    /// <param name="typeToConvert">Tipo a ser convertido.</param>
    /// <param name="options">Opções do serializador JSON.</param>
    /// <returns>String XML descompactada.</returns>
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dados = reader.GetString();
        if (dados!.IsEmpty()) return dados;

        using var memoryStream = new MemoryStream(Convert.FromBase64String(dados!));
        using var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
        using var memoryStreamOutput = new MemoryStream();
        gZipStream.CopyTo(memoryStreamOutput);
        
        var outputBytes = memoryStreamOutput.ToArray();

        return Encoding.UTF8.GetString(outputBytes);
    }

    /// <summary>
    /// Escreve uma string XML como Base64 compactado em GZip no JSON.
    /// </summary>
    /// <param name="writer">Gravador JSON.</param>
    /// <param name="value">String XML a ser compactada e escrita.</param>
    /// <param name="options">Opções do serializador JSON.</param>
    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        if (value.IsEmpty())
        {
            writer.WriteStringValue(string.Empty);
            return;
        }

        var dados = Encoding.UTF8.GetBytes(value);
        using var memoryStream = new MemoryStream();
        using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress))
            gZipStream.Write(dados, 0, dados.Length);

        var compressedBytes = memoryStream.ToArray();
        writer.WriteBase64StringValue(compressedBytes);
    }
}