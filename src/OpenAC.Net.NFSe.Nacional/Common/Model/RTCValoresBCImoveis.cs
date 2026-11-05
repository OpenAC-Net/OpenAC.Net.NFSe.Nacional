// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 04-11-2026
//
// Last Modified By : RFTD
// Last Modified On : 04-11-2026
// ***********************************************************************
// <copyright file="RTCValoresBCImoveis.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014-2026 Grupo OpenAC.Net
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
using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Grupo de informações relativas a valores de ajuste (dedução/redução) da Base de Cálculo do IBS e da CBS
/// referentes às operações de locação, cessão onerosa ou arrendamento de bens imóveis (TCRTCValoresBCImoveis).
/// </summary>
public sealed partial class RTCValoresBCImoveis
{
    /// <summary>
    /// Tipo de parcela não integrante da base de cálculo do IBS e da CBS (tpAjusteBCLocImoveis).
    /// </summary>
    [DFeElement(TipoCampo.Enum, "tpAjusteBCLocImoveis", Ocorrencia = Ocorrencia.Obrigatoria, Ordem = 1)]
    public AjusteBCLocImoveis TipoAjusteBCLocImoveis { get; set; }

    /// <summary>
    /// Descrição do tipo de parcela não integrante da base de cálculo do IBS e da CBS quando a opção for "99" (xTpAjusteBCLocImoveis).
    /// </summary>
    [DFeElement(TipoCampo.Str, "xTpAjusteBCLocImoveis", Min = 1, Max = 150, Ocorrencia = Ocorrencia.NaoObrigatoria, Ordem = 2)]
    public string? DescricaoTipoAjusteBCLocImoveis { get; set; }

    /// <summary>
    /// Valor monetário utilizado para não inclusão na base de cálculo do IBS e da CBS da NFS-e que está sendo emitida (R$) (vAjusteBCLocImoveis).
    /// </summary>
    [DFeElement(TipoCampo.De2, "vAjusteBCLocImoveis", Min = 4, Max = 18, Ocorrencia = Ocorrencia.Obrigatoria, Ordem = 3)]
    public decimal ValorAjusteBCLocImoveis { get; set; }
}
