// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
// ***********************************************************************
// <copyright file="MotivoRejeicao.cs" company="OpenAC .Net">
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

namespace OpenAC.Net.NFSe.Nacional.Common.Types;

/// <summary>
/// Motivo da Rejeição da NFS-e.
/// </summary>
public enum MotivoRejeicao
{
    /// <summary>
    /// 1 - NFS-e em duplicidade.
    /// </summary>
    [DFeEnum("1")]
    Duplicidade,

    /// <summary>
    /// 2 - NFS-e já emitida pelo tomador.
    /// </summary>
    [DFeEnum("2")]
    EmitidadaTomador,

    /// <summary>
    /// 3 - Não ocorrência do fato gerador.
    /// </summary>
    [DFeEnum("3")]
    NaoOcorrencia,

    /// <summary>
    /// 4 - Erro quanto a responsabilidade tributária.
    /// </summary>
    [DFeEnum("4")]
    ErroResponsabilidadeTributaria,

    /// <summary>
    /// 5 - Erro quanto ao valor do serviço, valor das deduções ou serviço prestado ou data do fato gerador.
    /// </summary>
    [DFeEnum("5")]
    ErroValores,

    /// <summary>
    /// 9 - Outros.
    /// </summary>
    [DFeEnum("9")]
    Outros
}