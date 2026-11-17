// ***********************************************************************
// Assembly     : OpenAC.Net.NFSe.Nacional
// Author       : RGG
// Created      : 15-11-2025
//
// Last Modified By : RGG
// Last Modified On : 15-11-2025
// ***********************************************************************
// <copyright file="RTCInfoTributosIBSCBS.cs" company="OpenAC .Net">
//        The MIT License (MIT)
//  Copyright (c) 2014-2023 Grupo OpenAC.Net
// </copyright>
// ***********************************************************************

using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Collection;
using OpenAC.Net.DFe.Core.Serializer;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Grupo de informações relacionados aos tributos IBS e CBS (TCRTCInfoTribIBSCBS / TCRTCInfoTributosIBSCBS).
/// </summary>
public sealed partial class RTCInfoTributosIBSCBS : DFeParentItem<RTCInfoTributosIBSCBS, InfDps>
{
    private RTCInfoTributosSitClas grupoIBSCBS = null!;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="RTCInfoTributosIBSCBS"/>.
    /// </summary>
    public RTCInfoTributosIBSCBS()
    {
        GrupoIBSCBS = new RTCInfoTributosSitClas();
    }

    /// <summary>
    /// Código de Situação Tributária do IBS e da CBS (CST).
    /// </summary>
    [DFeElement(TipoCampo.Str, "CST", Min = 3, Max = 3, Ocorrencia = Ocorrencia.Obrigatoria, Ordem = 1)]
    public string CodigoSituacaoTributaria
    {
        get => !string.IsNullOrEmpty(field) ? field : GrupoIBSCBS.CodigoSituacaoTributaria;
        set
        {
            field = value;
            GrupoIBSCBS.CodigoSituacaoTributaria = value;
        }
    } = string.Empty;

    /// <summary>
    /// Código de Classificação Tributária do IBS e da CBS (cClassTrib).
    /// </summary>
    [DFeElement(TipoCampo.Str, "cClassTrib", Min = 6, Max = 6, Ocorrencia = Ocorrencia.Obrigatoria, Ordem = 2)]
    public string CodigoClassificacaoTributaria
    {
        get => !string.IsNullOrEmpty(field) ? field : GrupoIBSCBS.CodigoClassificacaoTributaria;
        set
        {
            field = value;
            GrupoIBSCBS.CodigoClassificacaoTributaria = value;
        }
    } = string.Empty;

    /// <summary>
    /// Grupo de informações relacionadas ao IBS e à CBS (gIBSCBS).
    /// </summary>
    [DFeElement("gIBSCBS", Ocorrencia = Ocorrencia.NaoObrigatoria, Ordem = 3)]
    public RTCInfoTributosSitClas GrupoIBSCBS
    {
        get => grupoIBSCBS;
        set
        {
            if (grupoIBSCBS == value) return;
            grupoIBSCBS = value ?? new RTCInfoTributosSitClas();
            if (grupoIBSCBS.Parent != Parent)
                grupoIBSCBS.Parent = Parent;

            if (!string.IsNullOrEmpty(grupoIBSCBS.CodigoSituacaoTributaria))
                CodigoSituacaoTributaria = grupoIBSCBS.CodigoSituacaoTributaria;
            else if (!string.IsNullOrEmpty(CodigoSituacaoTributaria))
                grupoIBSCBS.CodigoSituacaoTributaria = CodigoSituacaoTributaria;

            if (!string.IsNullOrEmpty(grupoIBSCBS.CodigoClassificacaoTributaria))
                CodigoClassificacaoTributaria = grupoIBSCBS.CodigoClassificacaoTributaria;
            else if (!string.IsNullOrEmpty(CodigoClassificacaoTributaria))
                grupoIBSCBS.CodigoClassificacaoTributaria = CodigoClassificacaoTributaria;
        }
    }

    /// <inheritdoc />
    protected override void OnParentChanged()
    {
        if (grupoIBSCBS != null && grupoIBSCBS.Parent != Parent)
            grupoIBSCBS.Parent = Parent;
    }

    internal bool ShouldSerializeCodigoSituacaoTributaria() => Parent?.UsarLayoutNovo101 ?? true;

    internal bool ShouldSerializeCodigoClassificacaoTributaria() => Parent?.UsarLayoutNovo101 ?? true;

    internal bool ShouldSerializeGrupoIBSCBS()
    {
        if (!(Parent?.UsarLayoutNovo101 ?? true))
            return true;

        return !string.IsNullOrEmpty(GrupoIBSCBS.CodigoCreditoPresumido) ||
               GrupoIBSCBS.GrupoTributacaoRegular != null ||
               GrupoIBSCBS.GrupoDiferimento != null;
    }
}
