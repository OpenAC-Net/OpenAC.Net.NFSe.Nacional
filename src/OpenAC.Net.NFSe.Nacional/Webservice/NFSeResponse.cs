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

using System.Text.Json;

namespace OpenAC.Net.NFSe.Nacional.Webservice;

public sealed class NFSeResponse<T> where T : class, new()
{
    #region Constructors

    internal NFSeResponse(string xmlEnvio, string envio, string resposta, bool sucesso)
    {
        XmlEnvio = xmlEnvio;
        JsonEnvio = envio;
        JsonRetorno = resposta;
        Sucesso = sucesso;
        Resultado = JsonSerializer.Deserialize<T>(resposta) ?? new T();
    }

    #endregion Constructors
    
    #region Properties

    public string XmlEnvio { get; }

    public string JsonEnvio { get; }

    public string JsonRetorno { get; }
    
    public bool Sucesso { get; }
    
    public T Resultado { get; }

    #endregion Properties
}
