// ***********************************************************************
// Assembly     : OpenAC.Net.NFSe.Nacional
// Author  : RGG
// Created      : 15-11-2025
//
// Last Modified By : RGG
// Last Modified On : 15-11-2025
// ***********************************************************************
// <copyright file="RTCInfoValoresIBSCBS.cs" company="OpenAC .Net">
//        The MIT License (MIT)
//  Copyright (c) 2014-2023 Grupo OpenAC.Net
// </copyright>
// ***********************************************************************

using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Collection;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Grupo de informações relativas aos valores do serviço prestado para IBS e CBS.
/// </summary>
public sealed partial class RTCInfoValoresIBSCBS : DFeParentItem<RTCInfoValoresIBSCBS, InfDps>
{
    private RTCInfoTributosIBSCBS tributos = null!;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="RTCInfoValoresIBSCBS"/>.
    /// </summary>
    public RTCInfoValoresIBSCBS()
    {
        Tributos = new RTCInfoTributosIBSCBS();
    }

    /// <summary>
    /// Grupo de informações relativas a valores incluídos neste documento e recebidos por motivo de estarem relacionadas
    /// a operações de terceiros, objeto de reembolso, repasse ou ressarcimento pelo recebedor, já tributados e aqui referenciados (layout anterior).
    /// </summary>
    [DFeElement("gReeRepRes", Ocorrencia = Ocorrencia.NaoObrigatoria, Ordem = 1)]
    public RTCInfoReeRepRes? ReeRepRes { get; set; }

    /// <summary>
    /// Grupo de informações relacionados aos tributos IBS e CBS.
    /// </summary>
    [DFeElement("trib", Ocorrencia = Ocorrencia.Obrigatoria, Ordem = 2)]
    public RTCInfoTributosIBSCBS Tributos
    {
        get => tributos;
        set
        {
            if (tributos == value) return;
            tributos = value ?? new RTCInfoTributosIBSCBS();
            if (tributos.Parent != Parent)
                tributos.Parent = Parent;
        }
    }

    /// <inheritdoc />
    protected override void OnParentChanged()
    {
        if (tributos != null && tributos.Parent != Parent)
            tributos.Parent = Parent;
    }

    internal bool ShouldSerializeReeRepRes() => !(Parent?.UsarLayoutNovo101 ?? true) && ReeRepRes != null;
}
