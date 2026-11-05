// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author     : RGG
// Created     : 15-11-2025
//
// Last Modified By : RGG
// Last Modified On : 15-11-2025
// ***********************************************************************
// <copyright file="RTCInfoImovel.cs" company="OpenAC .Net">
//        The MIT License (MIT)
//  Copyright (c) 2014-2023 Grupo OpenAC.Net
// </copyright>
// ***********************************************************************

using System.Collections.Generic;
using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Collection;
using OpenAC.Net.DFe.Core.Serializer;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Grupo de informações de operações relacionadas a bens imóveis, exceto obras (TCRTCInfoImovel).
/// </summary>
public sealed partial class RTCInfoImovel : DFeParentItem<RTCInfoImovel, InfDps>
{
    /// <summary>
    /// Código do município do endereço do imóvel (Tabela do IBGE) (cMun).
    /// </summary>
    [DFeElement(TipoCampo.StrNumberFill, "cMun", Min = 7, Max = 7, Ocorrencia = Ocorrencia.NaoObrigatoria, Ordem = 1)]
    public string? CodigoMunicipio { get; set; }

    /// <summary>
    /// Grupo de informações relativas às operações de locação, cessão onerosa ou arrendamento de bens imóveis (gLocacao).
    /// </summary>
    [DFeElement("gLocacao", Ocorrencia = Ocorrencia.NaoObrigatoria, Ordem = 2)]
    public RTCLocacaoImovel? Locacao { get; set; }

    /// <summary>
    /// Grupo de informações relativas às unidades imobiliárias objeto da operação (gUnidImob).
    /// </summary>
    [DFeCollection("gUnidImob", MinSize = 0, MaxSize = 99, Ocorrencia = Ocorrencia.NaoObrigatoria, Ordem = 3)]
    public List<RTCUnidadesImobiliarias> UnidadesImobiliarias { get; set; } = new();

    /// <summary>
    /// Inscrição imobiliária fiscal (layout anterior).
    /// </summary>
    [DFeElement(TipoCampo.Str, "inscImobFisc", Min = 1, Max = 30, Ocorrencia = Ocorrencia.NaoObrigatoria, Ordem = 4)]
    public string? InscricaoImobiliariaFiscal { get; set; }

    #region Choice (Layout Anterior)

    /// <summary>
    /// Código CIB (Código do Imóvel na Prefeitura) (layout anterior).
    /// </summary>
    [DFeElement(TipoCampo.Str, "cCIB", Ocorrencia = Ocorrencia.NaoObrigatoria, Ordem = 5)]
    public string? CodigoCIB { get; set; }

    /// <summary>
    /// Endereço do imóvel (layout anterior).
    /// </summary>
    [DFeElement("end", Ocorrencia = Ocorrencia.NaoObrigatoria, Ordem = 5)]
    public EnderecoSimplesNFSe? Endereco { get; set; }

    #endregion

    /// <summary>
    /// Indica se a lista de unidades imobiliárias deve ser serializada.
    /// </summary>
    public bool ShouldSerializeUnidadesImobiliarias() => UnidadesImobiliarias is { Count: > 0 };
}
