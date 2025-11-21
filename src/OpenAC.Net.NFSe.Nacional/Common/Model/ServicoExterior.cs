// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
// ***********************************************************************
// <copyright file="ServicoExterior.cs" company="OpenAC .Net">
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
/// Representa os dados de serviço prestado no exterior.
/// </summary>
public sealed class ServicoExterior
{
    /// <summary>
    /// Modalidade da prestação do serviço.
    /// </summary>
    [DFeElement(TipoCampo.Enum, "mdPrestacao", Ocorrencia = Ocorrencia.Obrigatoria)]
    public ModoPrestacao Modo { get; set; } = ModoPrestacao.Desconhecido;

    /// <summary>
    /// Vínculo do prestador.
    /// </summary>
    [DFeElement(TipoCampo.Enum, "vincPrest", Ocorrencia = Ocorrencia.Obrigatoria)]
    public VinculoPrestador Vinculo { get; set; } = VinculoPrestador.SemVinculo;

    /// <summary>
    /// Código da moeda (3 dígitos).
    /// </summary>
    [DFeElement(TipoCampo.Int, "tpMoeda", Min = 3, Max = 3, Ocorrencia = Ocorrencia.Obrigatoria)]
    public int CodMoeda { get; set; }

    /// <summary>
    /// Valor do serviço na moeda informada.
    /// </summary>
    [DFeElement(TipoCampo.De2, "vServMoeda", Min = 4, Max = 18, Ocorrencia = Ocorrencia.Obrigatoria)]
    public decimal ValorServico { get; set; }

    /// <summary>
    /// Apoio ao comércio exterior pelo prestador.
    /// </summary>
    [DFeElement(TipoCampo.Enum, "mecAFComexP", Ocorrencia = Ocorrencia.Obrigatoria)]
    public ApoioComercioExteriorPrestador ApoioComercioExteriorPrestador { get; set; } = ApoioComercioExteriorPrestador.Desconhecido;

    /// <summary>
    /// Apoio ao comércio exterior pelo tomador.
    /// </summary>
    [DFeElement(TipoCampo.Enum, "mecAFComexT", Ocorrencia = Ocorrencia.Obrigatoria)]
    public ApoioComercioExteriorTomador ApoioComercioExteriorTomador { get; set; } = ApoioComercioExteriorTomador.Desconhecido;

    /// <summary>
    /// Movimentação temporária de bens.
    /// </summary>
    [DFeElement(TipoCampo.Enum, "movTempBens", Ocorrencia = Ocorrencia.Obrigatoria)]
    public MovimentacaoTemporariaBens MovimentacaoTemporariaBens { get; set; } = MovimentacaoTemporariaBens.Desconhecido;

    /// <summary>
    /// Número da Declaração de Importação (DI).
    /// </summary>
    [DFeElement(TipoCampo.StrNumber, "nDI", Min = 1, Max = 12, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? NumeroDeclaracaoImportacao { get; set; }

    /// <summary>
    /// Número do Registro de Exportação (RE).
    /// </summary>
    [DFeElement(TipoCampo.StrNumber, "nRE", Min = 1, Max = 12, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? NumeroRegistroExportacao { get; set; }

    /// <summary>
    /// Compartilhamento de informações com o MDIC.
    /// </summary>
    [DFeElement(TipoCampo.Enum, "mdic", Ocorrencia = Ocorrencia.Obrigatoria)]
    public CompartilharMDIC CompartilharMDIC { get; set; } = CompartilharMDIC.NaoEnviar;
}