// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp
// Author           : RFTD / OpenAC.Net Team
// Created          : 2026-08-16
// ***********************************************************************
// <copyright file="OpenDANFSeNacional.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014-2026 Grupo OpenAC.Net
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.IO;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp.Common;
using OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp.Configuracao;
using OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp.Layout;
using PdfSharp.Pdf;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp;

/// <summary>
/// Componente de impressão e exportação do DANFSe Padrão Nacional em PDF (100% Open Source / MIT).
/// </summary>
public class OpenDANFSeNacional
{
    #region Properties

    /// <summary>
    /// Configurações de impressão do DANFSe.
    /// </summary>
    public DANFSeNacionalConfig Configuracoes { get; set; }

    #endregion Properties

    #region Constructors

    static OpenDANFSeNacional()
    {
        DANFSeFontResolver.GarantirInicializacao();
    }

    /// <summary>
    /// Inicializa uma nova instância de <see cref="OpenDANFSeNacional"/>.
    /// </summary>
    /// <param name="config">Configurações opcionais.</param>
    public OpenDANFSeNacional(DANFSeNacionalConfig? config = null)
    {
        DANFSeFontResolver.GarantirInicializacao();
        Configuracoes = config ?? new DANFSeNacionalConfig();
    }

    #endregion Constructors

    #region Public Instance Methods

    /// <summary>
    /// Gera o documento PDF da NFS-e e salva no arquivo especificado.
    /// </summary>
    /// <param name="nota">Nota fiscal de serviço.</param>
    /// <param name="caminhoArquivo">Caminho do arquivo de destino.</param>
    public void ImprimirPDF(NotaFiscalServico nota, string caminhoArquivo)
    {
        if (string.IsNullOrWhiteSpace(caminhoArquivo))
            throw new ArgumentNullException(nameof(caminhoArquivo));

        using var doc = GerarPdfDocument(nota);
        doc.Save(caminhoArquivo);
    }

    /// <summary>
    /// Gera o documento PDF da NFS-e e salva na Stream informada.
    /// </summary>
    /// <param name="nota">Nota fiscal de serviço.</param>
    /// <param name="stream">Stream de destino.</param>
    public void ImprimirPDF(NotaFiscalServico nota, Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        using var doc = GerarPdfDocument(nota);
        doc.Save(stream, false);
    }

    /// <summary>
    /// Gera o documento PDF mesclado para múltiplas NFS-e e salva no arquivo especificado.
    /// </summary>
    /// <param name="notas">Array de notas fiscais de serviço.</param>
    /// <param name="caminhoArquivo">Caminho do arquivo de destino.</param>
    public void ImprimirPDF(NotaFiscalServico[] notas, string caminhoArquivo)
    {
        if (string.IsNullOrWhiteSpace(caminhoArquivo))
            throw new ArgumentNullException(nameof(caminhoArquivo));

        using var doc = GerarPdfDocument(notas);
        doc.Save(caminhoArquivo);
    }

    /// <summary>
    /// Gera o documento PDF mesclado para múltiplas NFS-e e salva na Stream informada.
    /// </summary>
    /// <param name="notas">Array de notas fiscais de serviço.</param>
    /// <param name="stream">Stream de destino.</param>
    public void ImprimirPDF(NotaFiscalServico[] notas, Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        using var doc = GerarPdfDocument(notas);
        doc.Save(stream, false);
    }

    /// <summary>
    /// Retorna os bytes do PDF gerado a partir da NFS-e.
    /// </summary>
    /// <param name="nota">Nota fiscal de serviço.</param>
    /// <returns>Bytes do PDF.</returns>
    public byte[] GerarPDF(NotaFiscalServico nota)
    {
        using var ms = new MemoryStream();
        ImprimirPDF(nota, ms);
        return ms.ToArray();
    }

    /// <summary>
    /// Retorna os bytes do PDF gerado a partir de múltiplas NFS-e.
    /// </summary>
    /// <param name="notas">Array de notas fiscais de serviço.</param>
    /// <returns>Bytes do PDF.</returns>
    public byte[] GerarPDF(NotaFiscalServico[] notas)
    {
        using var ms = new MemoryStream();
        ImprimirPDF(notas, ms);
        return ms.ToArray();
    }

    #endregion Public Instance Methods

    #region Static Helper Methods

    /// <summary>
    /// Gera o documento PDF da NFS-e e salva na Stream informada (estático).
    /// </summary>
    public static void GerarPDF(NotaFiscalServico nota, Stream stream, DANFSeNacionalConfig? config = null)
    {
        var danfse = new OpenDANFSeNacional(config);
        danfse.ImprimirPDF(nota, stream);
    }

    /// <summary>
    /// Gera o documento PDF da NFS-e e salva no arquivo informado (estático).
    /// </summary>
    public static void GerarPDF(NotaFiscalServico nota, string caminhoArquivo, DANFSeNacionalConfig? config = null)
    {
        var danfse = new OpenDANFSeNacional(config);
        danfse.ImprimirPDF(nota, caminhoArquivo);
    }

    /// <summary>
    /// Retorna os bytes do PDF gerado a partir da NFS-e (estático).
    /// </summary>
    public static byte[] GerarPDF(NotaFiscalServico nota, DANFSeNacionalConfig? config = null)
    {
        var danfse = new OpenDANFSeNacional(config);
        return danfse.GerarPDF(nota);
    }

    #endregion Static Helper Methods

    #region Private Methods

    private PdfDocument GerarPdfDocument(params NotaFiscalServico[] notas)
    {
        if (notas == null || notas.Length == 0)
            throw new ArgumentException("Nenhuma nota fiscal informada para impressão.", nameof(notas));

        var doc = new PdfDocument();
        doc.Info.Title = $"DANFSe Nacional - {notas[0].Informacoes.NumeroNFSe}";
        doc.Info.Author = "OpenAC .Net";
        doc.Info.Creator = "OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp";

        foreach (var nota in notas)
        {
            var report = new DANFSeNacionalReport(nota, Configuracoes);
            report.Render(doc);
        }

        AplicarSeguranca(doc);

        return doc;
    }

    private void AplicarSeguranca(PdfDocument doc)
    {
        var seguranca = Configuracoes.Seguranca;
        if (seguranca is not { TemCriptografia: true }) return;

        var sec = doc.SecuritySettings;
        if (!string.IsNullOrEmpty(seguranca.SenhaUsuario))
            sec.UserPassword = seguranca.SenhaUsuario!;

        if (!string.IsNullOrEmpty(seguranca.SenhaProprietario))
            sec.OwnerPassword = seguranca.SenhaProprietario!;

        sec.PermitPrint = seguranca.PermitirImpressao;
        sec.PermitFullQualityPrint = seguranca.PermitirImpressaoAltaQualidade;
        sec.PermitModifyDocument = seguranca.PermitirModificacao;
        sec.PermitExtractContent = seguranca.PermitirCopiarConteudo;
        sec.PermitAnnotations = seguranca.PermitirAnotacoes;
        sec.PermitFormsFill = seguranca.PermitirPreenchimentoFormularios;
        sec.PermitAssembleDocument = seguranca.PermitirMontarDocumento;
    }

    #endregion Private Methods
}
