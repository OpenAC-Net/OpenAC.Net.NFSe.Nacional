// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
// ***********************************************************************
// <copyright file="NFSeArquivoConfig.cs" company="OpenAC .Net">
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
using System.Reflection;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional;

public sealed class NFSeArquivoConfig : DFeArquivosConfigBase<SchemaNFSe>
{
    #region Constructors

    public NFSeArquivoConfig()
    {
        var path = Path.GetDirectoryName((Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).Location);
        if (Directory.Exists(path))
        {
            PathNFSe = Path.Combine(path, "NFSe");
            PathEnvio = Path.Combine(path, "Envio");
            PathDps = Path.Combine(path, "DPS");
        }
        else
        {
            PathNFSe = string.Empty;
            PathEnvio = string.Empty;
            PathDps = string.Empty;
        }
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Gets or sets the path n fe.
    /// </summary>
    /// <value>The path n fe.</value>
    public string PathNFSe { get; set; }

    /// <summary>
    /// Gets or sets the path lote.
    /// </summary>
    /// <value>The path lote.</value>
    public string PathEnvio { get; set; }

    /// <summary>
    /// Gets or sets the path lote.
    /// </summary>
    /// <value>The path lote.</value>
    public string PathDps { get; set; }

    #endregion Properties

    #region Methods

    public string GetPathNFSe(DateTime data, string cnpj = "")
    {
        return GetPath(PathNFSe, "NFSe", cnpj, data, "NFSe");
    }

    public string GetPathEnvio(DateTime data, string cnpj = "")
    {
        return GetPath(PathEnvio, "Envio", cnpj, data);
    }

    public string GetPathDps(DateTime data, string cnpj = "")
    {
        return GetPath(PathDps, "Dps", cnpj, data, "Rps");
    }
    
    public override string GetSchema(SchemaNFSe schema)
    {
        return schema switch
        {
            SchemaNFSe.DPS => Path.Combine(PathSchemas, "DPS_v1.00.xsd"),
            SchemaNFSe.Evento => Path.Combine(PathSchemas, "pedRegEvento_v1.00.xsd"),
            _ => throw new ArgumentOutOfRangeException(nameof(schema), schema, null)
        };
    }

    protected override void ArquivoServicoChange()
    {
        //
    }

    #endregion Methods
}