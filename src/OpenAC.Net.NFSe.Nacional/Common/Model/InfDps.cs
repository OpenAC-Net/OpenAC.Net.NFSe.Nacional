// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RGG
// Last Modified On : 14-11-2025
// ***********************************************************************
// <copyright file="InfDps.cs" company="OpenAC .Net">
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

using System;
using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Collection;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Representa as informações principais da DPS (Documento de Prestação de Serviço).
/// </summary>
public sealed partial class InfDps : DFeParentItem<InfDps, Dps>
{
    #region Constants

    /// <summary>
    /// Data em que o layout 1.01 de Produção passa a ser idêntico ao de Homologação (07/12/2026).
    /// </summary>
    public static readonly DateTime DataUnificacaoLayout101 = new(2026, 12, 7);

    #endregion Constants

    #region Fields

    private RTCFinNFSe finalidadeNFSe = RTCFinNFSe.Regular;
    private ServicoNFSe servico = null!;
    private RTCInfoIBSCBS? ibscbs;
    private RTCIndDest? indicadorDestinatario;
    private RTCInfoDest? destinatario;
    private ValoresDps valores = null!;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="InfDps"/>.
    /// </summary>
    public InfDps()
    {
        Servico = new ServicoNFSe();
        Valores = new ValoresDps();
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Indica se deve utilizar o novo layout 1.01 (Homologação ou Produção a partir de 07/12/2026).
    /// </summary>
    [DFeIgnore]
    internal bool UsarLayoutNovo101 =>
        (Parent?.Versao ?? VersaoNFSe.Ve100) == VersaoNFSe.Ve101 &&
        (TipoAmbiente == DFeTipoAmbiente.Homologacao ||
         DataEmissaoLayout >= DataUnificacaoLayout101);

    [DFeIgnore]
    internal bool UsarLayoutRelease2 => !UsarLayoutMunicipal && UsarLayoutNovo101;

    [DFeIgnore]
    internal bool UsarLayoutMunicipal { get; set; }

    internal RTCIndDest? IndicadorDestinatarioRaiz => indicadorDestinatario;

    internal RTCInfoDest? DestinatarioRaiz => destinatario;

    private DateTime DataEmissaoLayout => DhEmissao != default ? DhEmissao.Date : DateTime.Today;

    /// <summary>
    /// Identificador único da DPS.
    /// </summary>
    [DFeAttribute(TipoCampo.Str, "Id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de ambiente (Produção ou Homologação).
    /// </summary>
    [DFeElement(TipoCampo.Enum, "tpAmb", Ocorrencia = Ocorrencia.Obrigatoria)]
    public DFeTipoAmbiente TipoAmbiente { get; set; } = DFeTipoAmbiente.Homologacao;

    /// <summary>
    /// Data e hora de emissão da DPS.
    /// </summary>
    [DFeElement(TipoCampo.DatHorTz, "dhEmi", Ocorrencia = Ocorrencia.Obrigatoria)]
    public DateTimeOffset DhEmissao { get; set; }

    /// <summary>
    /// Versão do aplicativo emissor.
    /// </summary>
    [DFeElement(TipoCampo.Str, "verAplic", Min = 1, Max = 20, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string VersaoAplicacao { get; set; } = "OpenAC.NFSe.Nacional";

    /// <summary>
    /// Série da DPS.
    /// </summary>
    [DFeElement(TipoCampo.Str, "serie", Min = 1, Max = 5, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string Serie { get; set; } = string.Empty;

    /// <summary>
    /// Número da DPS.
    /// </summary>
    [DFeElement(TipoCampo.StrNumber, "nDPS", Min = 1, Max = 15, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string NumeroDps { get; set; } = "1";

    /// <summary>
    /// Data de competência.
    /// </summary>
    [DFeElement(TipoCampo.Dat, "dCompet", Ocorrencia = Ocorrencia.Obrigatoria)]
    public DateTime Competencia { get; set; }

    /// <summary>
    /// Tipo de emitente da DPS.
    /// </summary>
    [DFeElement(TipoCampo.Enum, "tpEmit", Ocorrencia = Ocorrencia.Obrigatoria)]
    public EmitenteDps TipoEmitente { get; set; } = EmitenteDps.Prestador;

    /// <summary>
    /// Motivo da Emissão da DPS pelo Tomador/Intermediário.
    /// </summary>
    [DFeElement(TipoCampo.Enum, "cMotivoEmisTI", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public MotivoEmissaoTomadoIntermediario? MotivoEmissao { get; set; }

    /// <summary>
    /// Indicador da finalidade da emissão de NFS-e.
    /// </summary>
    [DFeElement(TipoCampo.Enum, "finNFSe", Ocorrencia = Ocorrencia.Obrigatoria)]
    public RTCFinNFSe FinalidadeNFSe
    {
        get => ibscbs is { FinalidadeNFSeInformada: true } ? ibscbs.FinalidadeNFSe : finalidadeNFSe;
        set
        {
            finalidadeNFSe = value;
            if (ibscbs != null)
                ibscbs.FinalidadeNFSe = value;
        }
    }

    /// <summary>
    /// Chave de Acesso da NFS-e rejeitada pelo Tomador/Intermediário.
    /// </summary>
    [DFeElement(TipoCampo.Str, "chNFSeRej", Min = 50, Max = 50, Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public string? ChaveNFSeRejeitada { get; set; }

    /// <summary>
    /// Código da localidade do emitente.
    /// </summary>
    [DFeElement(TipoCampo.Str, "cLocEmi", Min = 0, Max = 7, Ocorrencia = Ocorrencia.Obrigatoria)]
    public string LocalidadeEmitente { get; set; } = string.Empty;

    /// <summary>
    /// Informações da DPS substituída, se houver.
    /// </summary>
    [DFeElement("subst", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public DpsSubstituida? Substituida { get; set; }

    /// <summary>
    /// Informações do prestador de serviço.
    /// </summary>
    [DFeElement("prest", Ocorrencia = Ocorrencia.Obrigatoria)]
    public PrestadorDps Prestador { get; set; } = new();

    /// <summary>
    /// Informações do tomador de serviço, se houver.
    /// </summary>
    [DFeElement("toma", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public InfoPessoaNFSe? Tomador { get; set; }

    /// <summary>
    /// Identifica se o destinatário é o próprio tomador. Lê também o indicador dos XMLs anteriores.
    /// </summary>
    [DFeIgnore]
    public RTCIndDest? IndicadorDestinatario
    {
        get => indicadorDestinatario ?? IBSCBS?.IndicadorDestinatario;
        set => indicadorDestinatario = value;
    }

    [DFeElement(TipoCampo.Enum, "indDest", Ocorrencia = Ocorrencia.Obrigatoria)]
    internal RTCIndDest IndicadorDestinatarioXml
    {
        get => IndicadorDestinatario ?? RTCIndDest.ProprioTomador;
        set => IndicadorDestinatario = value;
    }

    /// <summary>
    /// Identifica o destinatário na DPS. Lê também o grupo dos XMLs anteriores.
    /// </summary>
    [DFeElement("dest", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public RTCInfoDest? Destinatario
    {
        get => destinatario ?? IBSCBS?.Destinatario;
        set => destinatario = value;
    }

    /// <summary>
    /// Informações do intermediário, se houver.
    /// </summary>
    [DFeElement("interm", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public InfoPessoaNFSe? Intermediario { get; set; }

    /// <summary>
    /// Informações do serviço prestado.
    /// </summary>
    [DFeElement("serv", Ocorrencia = Ocorrencia.Obrigatoria)]
    public ServicoNFSe Servico
    {
        get => servico;
        set
        {
            if (servico == value) return;
            servico = value;
            if (servico != null && servico.Parent != this)
                servico.Parent = this;
        }
    }

    /// <summary>
    /// Valores da DPS.
    /// </summary>
    [DFeElement("valores", Ocorrencia = Ocorrencia.Obrigatoria)]
    public ValoresDps Valores
    {
        get => valores;
        set
        {
            valores = value;
            if (valores != null)
                valores.Parent = this;
        }
    }

    /// <summary>
    /// Grupo de informações declaradas pelo emitente referentes ao IBS e à CBS
    /// </summary>
    [DFeElement("IBSCBS", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public RTCInfoIBSCBS? IBSCBS
    {
        get => ibscbs;
        set
        {
            if (ibscbs == value) return;
            ibscbs = value;
            if (ibscbs != null)
            {
                if (ibscbs.Parent != this)
                    ibscbs.Parent = this;
                if (ibscbs.FinalidadeNFSeInformada)
                    finalidadeNFSe = ibscbs.FinalidadeNFSe;
                else
                    ibscbs.FinalidadeNFSe = finalidadeNFSe;
            }
        }
    }

    #endregion Properties

    #region Methods

    internal InfoPessoaNFSe? ObterEmitente() => TipoEmitente switch
    {
        EmitenteDps.Tomador => Tomador,
        EmitenteDps.Intermediário => Intermediario,
        _ => Prestador
    };

    internal bool ShouldSerializeFinalidadeNFSe() => UsarLayoutNovo101;

    internal bool ShouldSerializeIndicadorDestinatarioXml() => UsarLayoutRelease2;

    internal bool ShouldSerializeDestinatario() => UsarLayoutRelease2;

    #endregion Methods
}