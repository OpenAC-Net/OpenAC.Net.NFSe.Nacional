// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
// ***********************************************************************
// <copyright file="InfoPessoaNFSe.cs" company="OpenAC .Net">
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

using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Serializer;

namespace OpenAC.Net.NFSe.Nacional.Common;

public class InfoPessoaNFSe
{
    #region Properties

    [DFeElement(TipoCampo.StrNumber, "CNPJ", Min = 14, Max = 14, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? CNPJ { get; set; }
    
    [DFeElement(TipoCampo.StrNumber, "CPF", Min = 11, Max = 11, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? CPF { get; set; }
    
    [DFeElement(TipoCampo.StrNumber, "NIF", Min = 1, Max = 40, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? Nif { get; set; }

    [DFeElement(TipoCampo.Enum, "cNaoNIF", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public MotivoNaoNIF? CodigoNaoNif { get; set; }

    [DFeElement(TipoCampo.Str, "CAEPF", Min = 14, Max = 14, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? NumeroCAEPF { get; set; }

    [DFeElement(TipoCampo.StrNumber, "IM", Min = 1, Max = 15, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? InscricaoMunicipal { get; set; }

    [DFeElement(TipoCampo.Str, "xNome", Min = 1, Max = 300, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? Nome { get; set; }

    [DFeElement("end", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public EnderecoNFSe Endereco { get; set; } = new();
    
    [DFeElement(TipoCampo.StrNumber, "fone", Min = 6, Max = 20, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? Telefone { get; set; }
    
    [DFeElement(TipoCampo.Str, "email", Min = 1, Max = 80, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? Email { get; set; }

    #endregion Properties
}