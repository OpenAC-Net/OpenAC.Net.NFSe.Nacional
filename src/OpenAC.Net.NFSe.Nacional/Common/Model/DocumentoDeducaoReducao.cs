// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
// ***********************************************************************
// <copyright file="DocumentoDeducaoReducao.cs" company="OpenAC .Net">
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
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Representa um documento utilizado para dedução ou redução no contexto da NFSe.
/// </summary>
public sealed class DocumentoDeducaoReducao
{
    /// <summary>
    /// Chave da NFSe utilizada para dedução/redução (50 dígitos).
    /// </summary>
    [DFeElement(TipoCampo.StrNumber, "chNFSe", Min = 50, Max = 50, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? ChaveNFSe { get; set; }
    
    /// <summary>
    /// Chave da NFe utilizada para dedução/redução (44 dígitos).
    /// </summary>
    [DFeElement(TipoCampo.StrNumber, "chNFSe", Min = 44, Max = 44, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? ChaveNFe { get; set; }
    
    /// <summary>
    /// Chave da NFSe do município.
    /// </summary>
    [DFeElement("NFSeMun", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public ChaveNFSeMunicipio? ChaveNFSeMunicipio { get; set; }
    
    /// <summary>
    /// Documento NFNFS relacionado.
    /// </summary>
    [DFeElement("NFNFS", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public DocumentoNFNFS? DocumentoNFNFS { get; set; }
    
    /// <summary>
    /// Número do documento fiscal.
    /// </summary>
    [DFeElement(TipoCampo.StrNumber, "nDocFisc", Min = 1, Max = 255, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? NumeroDocumentoFiscal { get; set; }
    
    /// <summary>
    /// Número do documento.
    /// </summary>
    [DFeElement(TipoCampo.StrNumber, "nDoc", Min = 1, Max = 255, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? NumeroDocumento { get; set; }
    
    /// <summary>
    /// Tipo de dedução ou redução.
    /// </summary>
    [DFeElement(TipoCampo.Enum, "nDoc", Ocorrencia = Ocorrencia.Obrigatoria)]
    public TipoDeducaoReducao TipoDeducaoReducao { get; set; }
    
    /// <summary>
    /// Descrição da dedução ou redução.
    /// </summary>
    [DFeElement(TipoCampo.StrNumber, "xDescOutDed", Min = 1, Max = 255, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? DescricaoReducaoDeducao { get; set; }
    
    /// <summary>
    /// Data de emissão do documento.
    /// </summary>
    [DFeElement(TipoCampo.Dat, "dtEmiDoc", Ocorrencia = Ocorrencia.Obrigatoria)]
    public DateTime DataEmissao { get; set; }
    
    /// <summary>
    /// Valor dedutível ou redutível.
    /// </summary>
    [DFeElement(TipoCampo.De2, "vDedutivelRedutivel", Min = 4, Max = 18, Ocorrencia = Ocorrencia.Obrigatoria)]
    public decimal ValorDedutivelRedutivel { get; set; }

    /// <summary>
    /// Valor da dedução ou redução.
    /// </summary>
    [DFeElement(TipoCampo.De2, "vDeducaoReducao", Min = 4, Max = 18, Ocorrencia = Ocorrencia.Obrigatoria)]
    public decimal ValorReducaoDeducao { get; set; }

    /// <summary>
    /// Informações do fornecedor.
    /// </summary>
    [DFeElement("fornec", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public InfoPessoaNFSe? Fornecedor { get; set; }
}