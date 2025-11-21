// ***********************************************************************
// Assembly : OpenAC.Net.NFSe.Nacional
// Author : RGG
// Created :14-11-2025
//
// Last Modified By : RGG
// Last Modified On :15-11-2025
// ***********************************************************************
// <copyright file="RTCInfoIBSCBS.cs" company="OpenAC .Net">
// The MIT License (MIT)
// Copyright (c)2014-2023 Grupo OpenAC.Net
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
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
/// Grupo de informações declaradas pelo emitente referentes ao IBS e à CBS.
/// </summary>
public sealed class RTCInfoIBSCBS
{
    /// <summary>
    /// Indicador da finalidade da emissão de NFS-e (finNFSe).
    /// TSRTCFinNFSe.
    /// </summary>
    [DFeElement(TipoCampo.Enum, "finNFSe", Ocorrencia = Ocorrencia.Obrigatoria)]
    public RTCFinNFSe FinalidadeNFSe { get; set; }

    /// <summary>
    /// Indica operação de uso ou consumo pessoal (art.57) (indFinal).
    /// TSRTCIndFinal.
    /// </summary>
    [DFeElement(TipoCampo.Enum, "indFinal", Ocorrencia = Ocorrencia.Obrigatoria)]
    public RTCIndFinal IndicadorUsoFinal { get; set; }

    /// <summary>
    /// Código indicador da operação de fornecimento (cIndOp).
    /// TSRTCCodIndOp:6 dígitos numéricos.
    /// </summary>
    [DFeElement(TipoCampo.StrNumber, "cIndOp", Min =6, Max =6, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string CodigoIndicadorOperacao { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de Operação com Entes Governamentais ou outros serviços sobre bens imóveis (tpOper).
    /// TSRTCTpOper.
    /// </summary>
    [DFeElement(TipoCampo.Enum, "tpOper", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public RTCTpOper? TipoOperacao { get; set; }

    /// <summary>
    /// Grupo de NFS-e referenciadas (gRefNFSe).
    /// </summary>
    [DFeElement("gRefNFSe", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public InfoRefNFSe? ReferenciasNFSe { get; set; }

    /// <summary>
    /// Tipo de ente governamental (tpEnteGov).
    /// TSRTCTpEnteGov.
    /// </summary>
    [DFeElement(TipoCampo.Enum, "tpEnteGov", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public RTCTpEnteGov? TipoEnteGovernamental { get; set; }

    /// <summary>
    /// A respeito do Destinatário dos serviços (indDest).
    /// TSRTCIndDest.
    /// </summary>
    [DFeElement(TipoCampo.Enum, "indDest", Ocorrencia = Ocorrencia.Obrigatoria)]
    public RTCIndDest IndicadorDestinatario { get; set; }

    /// <summary>
    /// Grupo de informações relativas ao Destinatário (dest).
    /// </summary>
    [DFeElement("dest", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public RTCInfoDest? Destinatario { get; set; }

    /// <summary>
    /// Grupo de informações de operações relacionadas a bens imóveis, exceto obras (imovel).
    /// </summary>
    [DFeElement("imovel", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public RTCInfoImovel? Imovel { get; set; }

    /// <summary>
    /// Grupo de informações relativas aos valores do serviço prestado para IBS e CBS (valores).
    /// </summary>
    [DFeElement("valores", Ocorrencia = Ocorrencia.Obrigatoria)]
    public RTCInfoValoresIBSCBS Valores { get; set; } = new();
}