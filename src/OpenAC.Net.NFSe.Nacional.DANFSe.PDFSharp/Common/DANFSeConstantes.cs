// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp
// Author           : RFTD / OpenAC.Net Team
// Created          : 2026-08-16
// ***********************************************************************
// <copyright file="DANFSeConstantes.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014-2026 Grupo OpenAC.Net
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp.Common;

/// <summary>
/// Constantes de layout do DANFSe Padrão Nacional (NT 008 v2.0).
/// </summary>
public static class DANFSeConstantes
{
    #region Dimensões em Milímetros (A4)

    public const double PaginaLarguraMm = 210.0;
    public const double PaginaAlturaMm = 297.0;
    public const double MargemPadraoMm = 3.0;
    public const double LarguraUtilMm = 204.0;

    public const double AlturaCabecalhoMm = 12.0;
    public const double AlturaLinhaPadraoMm = 6.4;
    public const double AlturaLinhaVaziaMm = 4.5;
    public const double AlturaTituloBlocoMm = 3.5;

    public const double QrCodeTamanhoMm = 16.0;

    #endregion Dimensões em Milímetros (A4)

    #region Fontes e Tamanhos em Pontos

    public const string FontePadrao = "Arial";

    public const double FonteTituloDocPt = 9.0;
    public const double FonteSemValidadePt = 8.5;
    public const double FonteMunicipioPt = 8.0;
    public const double FonteAmbientePt = 6.0;

    public const double FonteTituloBlocoPt = 7.0;
    public const double FonteLabelCampoPt = 5.5;
    public const double FonteConteudoPt = 6.8;
    public const double FonteTextoQrPt = 5.0;
    public const double FonteMarcaDaguaPt = 42.0;

    #endregion Fontes e Tamanhos em Pontos

    #region Mensagens Padrão (NT 008)

    public const string UrlConsultaPublica = "https://www.nfse.gov.br/ConsultaPublica/?tpc=1&chave=";
    public const string TextoQrComplemento = "A autenticidade desta NFS-e pode ser verificada pela leitura deste código QR ou pela consulta da chave de acesso no portal nacional da NFS-e";

    public const string MsgTomadorVazio = "TOMADOR/ADQUIRENTE DA OPERAÇÃO NÃO IDENTIFICADO NA NFS-e";
    public const string MsgDestVazio = "DESTINATÁRIO DA OPERAÇÃO NÃO IDENTIFICADO NA NFS-e";
    public const string MsgDestIgualTomador = "O DESTINATÁRIO É O PRÓPRIO TOMADOR/ADQUIRENTE DA OPERAÇÃO";
    public const string MsgIntermVazio = "INTERMEDIÁRIO DA OPERAÇÃO NÃO IDENTIFICADO NA NFS-e";
    public const string MsgIssqnVazio = "TRIBUTAÇÃO MUNICIPAL (ISSQN) - OPERAÇÃO NÃO SUJEITA AO ISSQN";
    public const string MsgSemValidade = "NFS-e SEM VALIDADE JURÍDICA";
    public const string MsgCancelada = "CANCELADA";
    public const string MsgSubstituida = "SUBSTITUÍDA";

    #endregion Mensagens Padrão (NT 008)
}
