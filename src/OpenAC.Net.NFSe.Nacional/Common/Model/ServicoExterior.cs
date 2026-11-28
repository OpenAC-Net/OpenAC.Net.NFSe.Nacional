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
using OpenAC.Net.DFe.Core.Collection;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Representa os dados de serviço prestado no exterior.
/// </summary>
public sealed partial class ServicoExterior : DFeParentItem<ServicoExterior, InfDps>
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
    /// Número da fatura ou do documento que comprova a transação.
    /// </summary>
    [DFeElement(TipoCampo.StrNumber, "nFatura", Min = 1, Max = 20, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? NumeroFatura { get; set; }

    /// <summary>
    /// Valor da fatura na moeda informada.
    /// </summary>
    [DFeElement(TipoCampo.De2, "vFatura", Max = 18, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public decimal? ValorFatura { get; set; }

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
    /// Número da Declaração Única de Importação.
    /// </summary>
    [DFeElement(TipoCampo.Str, "nDUIMP", Min = 15, Max = 15, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? NumeroDuimp { get; set; }

    /// <summary>
    /// Número da Declaração Única de Exportação.
    /// </summary>
    [DFeElement(TipoCampo.Str, "nDUE", Min = 14, Max = 14, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? NumeroDue { get; set; }

    /// <summary>
    /// Compartilhamento de informações com o MDIC.
    /// </summary>
    [DFeElement(TipoCampo.Enum, "mdic", Ocorrencia = Ocorrencia.Obrigatoria)]
    public CompartilharMDIC CompartilharMDIC { get; set; } = CompartilharMDIC.NaoEnviar;

    /// <summary>
    /// Número do contrato de câmbio ou do documento que comprova o pagamento.
    /// </summary>
    [DFeElement(TipoCampo.StrNumber, "nContCambio", Min = 1, Max = 20, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? NumeroContratoCambio { get; set; }

    internal bool ShouldSerializeValorServico() => !(Parent?.UsarLayoutRelease2 ?? false);
    internal bool ShouldSerializeNumeroDeclaracaoImportacao() => !(Parent?.UsarLayoutRelease2 ?? false);
    internal bool ShouldSerializeNumeroRegistroExportacao() => !(Parent?.UsarLayoutRelease2 ?? false);

    internal bool ShouldSerializeNumeroFatura() => Parent?.UsarLayoutRelease2 ?? true;
    internal bool ShouldSerializeValorFatura() => Parent?.UsarLayoutRelease2 ?? true;
    internal bool ShouldSerializeNumeroDuimp() => Parent?.UsarLayoutRelease2 ?? true;
    internal bool ShouldSerializeNumeroDue() => Parent?.UsarLayoutRelease2 ?? true;
    internal bool ShouldSerializeNumeroContratoCambio() => Parent?.UsarLayoutRelease2 ?? true;
}