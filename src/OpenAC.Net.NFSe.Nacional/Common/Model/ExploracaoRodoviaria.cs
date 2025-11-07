// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
// ***********************************************************************
// <copyright file="ExploracaoRodoviaria.cs" company="OpenAC .Net">
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
using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Representa os dados de exploração rodoviária.
/// </summary>
public sealed class ExploracaoRodoviaria
{
    /// <summary>
    /// Categoria do veículo.
    /// </summary>
    [DFeElement(TipoCampo.Enum, "categVeic", Ocorrencia = Ocorrencia.Obrigatoria)]
    public CategoriaVeiculo Categoria { get; set; }

    /// <summary>
    /// Número de eixos do veículo.
    /// </summary>
    [DFeElement(TipoCampo.Int, "nEixos", Min = 1, Max = 2, Ocorrencia = Ocorrencia.Obrigatoria)]
    public int NumeroEixos { get; set; }
    
    /// <summary>
    /// Tipo de rodagem do veículo.
    /// </summary>
    [DFeElement(TipoCampo.Enum, "categVeic", Ocorrencia = Ocorrencia.Obrigatoria)]
    public TipoRodagem TipoRodagem { get; set; }
    
    /// <summary>
    /// Orientação da pesagem (sentido).
    /// </summary>
    [DFeElement(TipoCampo.Int, "sentido", Min = 1, Max = 3, Ocorrencia = Ocorrencia.Obrigatoria)]
    public int OrientacaoPesagem { get; set; }

    /// <summary>
    /// Placa do veículo.
    /// </summary>
    [DFeElement(TipoCampo.Str, "placa", Min = 7, Max = 7, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string Placa { get; set; } = string.Empty;
    
    /// <summary>
    /// Código de acesso do pedágio.
    /// </summary>
    [DFeElement(TipoCampo.Str, "codAcessoPed", Min = 10, Max = 10, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string CodAcessoPedagio { get; set; } = string.Empty;
    
    /// <summary>
    /// Código do contrato.
    /// </summary>
    [DFeElement(TipoCampo.Str, "codContrato", Min = 4, Max = 4, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string CodContrato { get; set; } = string.Empty;
}