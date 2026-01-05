// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
// ***********************************************************************
// <copyright file="NFSeWebserviceConfig.cs" company="OpenAC .Net">
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
// <summary>
//     Defines the configuration settings for the NFSe web service.
// </summary>
// ***********************************************************************

using System;
using OpenAC.Net.Core;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Nacional.Webservice;

namespace OpenAC.Net.NFSe.Nacional.Common;

/// <summary>
/// Representa as configurações para os webservices da NFSe.
/// </summary>
public sealed class NFSeWebserviceConfig : DFeWebserviceConfigBase
{
    /// <summary>
    /// Uf do webservice em uso
    /// </summary>
    /// <value>The uf.</value>
    public string? Municipio { get; private set; }

    /// <summary>
    /// Provedor do webservice NFSe em uso.
    /// </summary>
    /// <value>Provedor atual. Padrão: NFSeProvider.Nacional.</value>
    public NFSeProvider Provedor { get; private set; } = NFSeProvider.Nacional;

    /// <summary>
    /// Codigo do municipio do Webservices em uso
    /// </summary>
    /// <value>The uf codigo.</value>
    public int CodigoMunicipio
    {
        get;
        set
        {
            if (field == value) return;

            var municipio = NFSeServiceManager.Instance.Services[value];
            Guard.Against<ArgumentException>(municipio == null, "Município não cadastrado.");

            field = value;
            Municipio = municipio?.Nome ?? string.Empty;
            Provedor = municipio?.Provider ?? NFSeProvider.Nacional;
        }
    }
}