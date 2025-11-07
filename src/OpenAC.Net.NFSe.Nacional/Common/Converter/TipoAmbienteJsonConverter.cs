// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
// ***********************************************************************
// <copyright file="TipoAmbienteJsonConverter.cs" company="OpenAC .Net">
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
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenAC.Net.DFe.Core.Common;

namespace OpenAC.Net.NFSe.Nacional.Common.Converter;

/// <summary>
/// Conversor JSON para o enum <see cref="DFeTipoAmbiente"/>.
/// </summary>
public class TipoAmbienteJsonConverter : JsonConverter<DFeTipoAmbiente>
{
    /// <summary>
    /// Lê e converte o valor JSON para <see cref="DFeTipoAmbiente"/>.
    /// </summary>
    /// <param name="reader">Leitor JSON.</param>
    /// <param name="typeToConvert">Tipo a ser convertido.</param>
    /// <param name="options">Opções de serialização.</param>
    /// <returns>Valor convertido para <see cref="DFeTipoAmbiente"/>.</returns>
    public override DFeTipoAmbiente Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var valor = reader.GetInt32();
        return (DFeTipoAmbiente) valor;
    }

    /// <summary>
    /// Escreve o valor de <see cref="DFeTipoAmbiente"/> como número no JSON.
    /// </summary>
    /// <param name="writer">Gravador JSON.</param>
    /// <param name="value">Valor a ser escrito.</param>
    /// <param name="options">Opções de serialização.</param>
    public override void Write(Utf8JsonWriter writer, DFeTipoAmbiente value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue((int)value);
    }
}