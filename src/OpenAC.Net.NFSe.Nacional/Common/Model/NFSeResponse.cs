// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
// ***********************************************************************
// <copyright file="NFSeResponse.cs" company="OpenAC .Net">
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
using OpenAC.Net.Core.Logging;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Representa a resposta de uma operação NFSe, contendo os dados de envio, retorno e resultado desserializado.
/// </summary>
/// <typeparam name="T">Tipo do resultado esperado na resposta.</typeparam>
public sealed class NFSeResponse<T> : IOpenLog where T : class, new()
{
    #region Constructors

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="NFSeResponse{T}"/>.
    /// </summary>
    /// <param name="xmlEnvio">XML enviado na requisição.</param>
    /// <param name="envio">Dados de envio em formato JSON.</param>
    /// <param name="resposta">Resposta recebida em formato JSON.</param>
    /// <param name="sucesso">Indica se a operação foi bem-sucedida.</param>
    private NFSeResponse(string xmlEnvio, string envio, string resposta, bool sucesso, JsonSerializerOptions? options)
    {
        XmlEnvio = xmlEnvio;
        JsonEnvio = envio;
        JsonRetorno = resposta;
        Sucesso = sucesso;
        JsonOptions = options;

        try
        {
            Resultado = JsonSerializer.Deserialize<T>(resposta, options);
        }
        catch (Exception e)
        {
            this.Log().Error(e);
            Resultado = null;
        }
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Obtém o XML enviado na requisição.
    /// </summary>
    public string XmlEnvio { get; }

    /// <summary>
    /// Obtém os dados de envio em formato JSON.
    /// </summary>
    public string JsonEnvio { get; }

    /// <summary>
    /// Obtém a resposta recebida em formato JSON.
    /// </summary>
    public string JsonRetorno { get; }

    /// <summary>
    /// Indica se a operação foi bem-sucedida.
    /// </summary>
    public bool Sucesso { get; }

    /// <summary>
    /// Obtém o resultado desserializado da resposta.
    /// </summary>
    public T? Resultado { get; }

    /// <summary>
    /// Define as opções de serialização JSON utilizadas.
    /// </summary>
    public JsonSerializerOptions? JsonOptions { get; }

    #endregion Properties

    #region Methods

    /// <summary>
    /// Cria uma instância de <see cref="NFSeResponse{T}"/> contendo os dados de envio e retorno.
    /// </summary>
    /// <param name="xmlEnvio">XML enviado na requisição.</param>
    /// <param name="envio">Dados de envio em formato JSON.</param>
    /// <param name="resposta">Resposta recebida em formato JSON.</param>
    /// <param name="sucesso">Indica se a operação foi bem-sucedida.</param>
    /// <returns>Instância de <see cref="NFSeResponse{T}"/> com o resultado desserializado (ou <c>null</c> em caso de erro de desserialização).</returns>

    public static NFSeResponse<T> Create(string xmlEnvio, string envio, string resposta, bool sucesso, JsonSerializerOptions? jsonOptions = null)
    {
        return new NFSeResponse<T>(xmlEnvio, envio, resposta, sucesso, jsonOptions);
    }

    #endregion Methods
}