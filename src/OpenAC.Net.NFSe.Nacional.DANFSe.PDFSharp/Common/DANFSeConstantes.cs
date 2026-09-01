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

    /// <summary>Largura da página A4 em milímetros (210.0 mm).</summary>
    public const double PaginaLarguraMm = 210.0;

    /// <summary>Altura da página A4 em milímetros (297.0 mm).</summary>
    public const double PaginaAlturaMm = 297.0;

    /// <summary>Margem padrão da página em milímetros (1.5 mm).</summary>
    public const double MargemPadraoMm = 1.5;

    /// <summary>Largura útil imprimível em milímetros (207.0 mm).</summary>
    public const double LarguraUtilMm = 207.0;

    /// <summary>Altura padrão do cabeçalho em milímetros (12.0 mm).</summary>
    public const double AlturaCabecalhoMm = 12.0;

    /// <summary>Altura padrão de uma linha em milímetros (6.4 mm).</summary>
    public const double AlturaLinhaPadraoMm = 6.4;

    /// <summary>Altura de uma linha vazia em milímetros (4.5 mm).</summary>
    public const double AlturaLinhaVaziaMm = 4.5;

    /// <summary>Altura da barra de título de bloco em milímetros (3.5 mm).</summary>
    public const double AlturaTituloBlocoMm = 3.5;

    /// <summary>Tamanho padrão do QR Code em milímetros (16.0 mm).</summary>
    public const double QrCodeTamanhoMm = 16.0;

    #endregion Dimensões em Milímetros (A4)

    #region Fontes e Tamanhos em Pontos

    /// <summary>Família de fonte padrão do documento.</summary>
    public const string FontePadrao = "Arial";

    /// <summary>Tamanho da fonte do título do documento em pontos.</summary>
    public const double FonteTituloDocPt = 9.0;

    /// <summary>Tamanho da fonte para mensagem sem validade jurídica em pontos.</summary>
    public const double FonteSemValidadePt = 8.5;

    /// <summary>Tamanho da fonte do município em pontos.</summary>
    public const double FonteMunicipioPt = 8.0;

    /// <summary>Tamanho da fonte do ambiente emissor em pontos.</summary>
    public const double FonteAmbientePt = 6.0;

    /// <summary>Tamanho da fonte dos títulos de blocos em pontos.</summary>
    public const double FonteTituloBlocoPt = 7.0;

    /// <summary>Tamanho da fonte dos rótulos dos campos em pontos.</summary>
    public const double FonteLabelCampoPt = 6.0;

    /// <summary>Tamanho da fonte do conteúdo dos campos em pontos.</summary>
    public const double FonteConteudoPt = 7.0;

    /// <summary>Tamanho da fonte do texto complementar do QR Code em pontos.</summary>
    public const double FonteTextoQrPt = 5.0;

    /// <summary>Tamanho base da fonte da marca d'água em pontos.</summary>
    public const double FonteMarcaDaguaPt = 42.0;

    #endregion Fontes e Tamanhos em Pontos

    #region Mensagens Padrão (NT 008)

    /// <summary>URL base para consulta pública da NFS-e Nacional.</summary>
    public const string UrlConsultaPublica = "https://www.nfse.gov.br/ConsultaPublica/?tpc=1&chave=";

    /// <summary>Texto explicativo que acompanha o QR Code.</summary>
    public const string TextoQrComplemento = "A autenticidade desta NFS-e pode ser verificada pela leitura deste código QR ou pela consulta da chave de acesso no portal nacional da NFS-e";

    /// <summary>Mensagem padrão quando o tomador não foi identificado.</summary>
    public const string MsgTomadorVazio = "TOMADOR/ADQUIRENTE DA OPERAÇÃO NÃO IDENTIFICADO NA NFS-e";

    /// <summary>Mensagem padrão quando o destinatário não foi identificado.</summary>
    public const string MsgDestVazio = "DESTINATÁRIO DA OPERAÇÃO NÃO IDENTIFICADO NA NFS-e";

    /// <summary>Mensagem padrão quando o destinatário é o próprio tomador.</summary>
    public const string MsgDestIgualTomador = "O DESTINATÁRIO É O PRÓPRIO TOMADOR/ADQUIRENTE DA OPERAÇÃO";

    /// <summary>Mensagem padrão quando o intermediário não foi identificado.</summary>
    public const string MsgIntermVazio = "INTERMEDIÁRIO DA OPERAÇÃO NÃO IDENTIFICADO NA NFS-e";

    /// <summary>Mensagem padrão quando a operação não é sujeita ao ISSQN.</summary>
    public const string MsgIssqnVazio = "TRIBUTAÇÃO MUNICIPAL (ISSQN) - OPERAÇÃO NÃO SUJEITA AO ISSQN";

    /// <summary>Mensagem de marca d'água para ambiente de homologação / sem validade jurídica.</summary>
    public const string MsgSemValidade = "NFS-e SEM VALIDADE JURÍDICA";

    /// <summary>Mensagem de marca d'água para notas canceladas.</summary>
    public const string MsgCancelada = "CANCELADA";

    /// <summary>Mensagem de marca d'água para notas substituídas.</summary>
    public const string MsgSubstituida = "SUBSTITUÍDA";

    #endregion Mensagens Padrão (NT 008)
}
