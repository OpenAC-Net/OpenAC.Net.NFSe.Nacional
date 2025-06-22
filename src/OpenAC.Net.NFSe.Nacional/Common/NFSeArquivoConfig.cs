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
using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional.Common;

/// <summary>
/// Representa a configuração para arquivos NFSe.
/// </summary>
public sealed class NFSeArquivoConfig : DFeArquivosConfigBase<SchemaNFSe>
{
    #region Constructors

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="NFSeArquivoConfig"/>.
    /// </summary>
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
    /// Obtém ou define o caminho para arquivos NFSe.
    /// </summary>
    /// <value>O caminho para arquivos NFSe.</value>
    public string PathNFSe { get; set; }

    /// <summary>
    /// Obtém ou define o caminho para arquivos de envio.
    /// </summary>
    /// <value>O caminho para arquivos de envio.</value>
    public string PathEnvio { get; set; }

    /// <summary>
    /// Obtém ou define o caminho para arquivos DPS.
    /// </summary>
    /// <value>O caminho para arquivos DPS.</value>
    public string PathDps { get; set; }

    #endregion Properties

    #region Methods

    
    /// <summary>
    /// Obtém o caminho para arquivos NFSe com base na data e CNPJ especificados.
    /// </summary>
    /// <param name="data">A data a ser usada no caminho.</param>
    /// <param name="cnpj">O CNPJ a ser usado no caminho.</param>
    /// <returns>O caminho construído para arquivos NFSe.</returns>
    public string GetPathNFSe(DateTime data, string cnpj = "")
    {
        return GetPath(PathNFSe, "NFSe", cnpj, data, "NFSe");
    }

    
    /// <summary>
    /// Obtém o caminho para arquivos de envio com base na data e CNPJ especificados.
    /// </summary>
    /// <param name="data">A data a ser usada no caminho.</param>
    /// <param name="cnpj">O CNPJ a ser usado no caminho.</param>
    /// <returns>O caminho construído para arquivos de envio.</returns>
    public string GetPathEnvio(DateTime data, string cnpj = "")
    {
        return GetPath(PathEnvio, "Envio", cnpj, data);
    }

    /// <summary>
    /// Obtém o caminho para arquivos DPS com base na data e CNPJ especificados.
    /// </summary>
    /// <param name="data">A data a ser usada no caminho.</param>
    /// <param name="cnpj">O CNPJ a ser usado no caminho.</param>
    /// <returns>O caminho construído para arquivos DPS.</returns>
    public string GetPathDps(DateTime data, string cnpj = "")
    {
        return GetPath(PathDps, "Dps", cnpj, data, "Rps");
    }
    
    
    /// <summary>
    /// Obtém o caminho do esquema com base no tipo de esquema especificado.
    /// </summary>
    /// <param name="schema">O tipo de esquema.</param>
    /// <returns>O caminho para o arquivo de esquema.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Lançada quando o tipo de esquema não é reconhecido.</exception>
    public override string GetSchema(SchemaNFSe schema)
    {
        return schema switch
        {
            SchemaNFSe.DPS => Path.Combine(PathSchemas, "DPS_v1.00.xsd"),
            SchemaNFSe.Evento => Path.Combine(PathSchemas, "pedRegEvento_v1.00.xsd"),
            _ => throw new ArgumentOutOfRangeException(nameof(schema), schema, null)
        };
    }

    
    /// <summary>
    /// Manipula alterações no arquivo de serviço.
    /// </summary>
    protected override void ArquivoServicoChange()
    {
        // Ignorado
    }

    #endregion Methods
}