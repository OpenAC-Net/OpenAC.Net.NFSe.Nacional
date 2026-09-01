// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp
// Author           : RFTD / OpenAC.Net Team
// Created          : 2026-08-16
// ***********************************************************************
// <copyright file="DANFSeNacionalReport.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014-2026 Grupo OpenAC.Net
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.Common.Types;
using OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp.Common;
using OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp.Configuracao;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp.Layout;

/// <summary>
/// Motor de renderização do DANFSe Padrão Nacional em PDF (NT 008 v1.02 / DANFSe v2.0).
/// </summary>
internal sealed class DANFSeNacionalReport
{
    #region Fields

    private const string LogoNacionalResourceName =
        "OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp.Resources.Images.NfseHorizontal.png";
    private const double AlturaDescricaoTributacaoMm = 3.8;
    private const double AlturaMinimaDescricaoServicoMm = 6.3;
    private const double AlturaMinimaInformacoesComplementaresMm = 15.0;

    private static readonly Lazy<byte[]?> LogoNacionalPadrao = new(CarregarLogoNacionalPadrao);
    private readonly NotaFiscalServico nota;
    private readonly DANFSeNacionalConfig config;
    private static readonly CultureInfo PtBr = new("pt-BR");

    private double xMm;
    private double yMm;
    private double largUtilMm;

    #endregion Fields

    #region Constructors

    public DANFSeNacionalReport(NotaFiscalServico nota, DANFSeNacionalConfig config)
    {
        this.nota = nota ?? throw new ArgumentNullException(nameof(nota));
        this.config = config ?? throw new ArgumentNullException(nameof(config));
    }

    #endregion Constructors

    #region Methods

    public void Render(PdfDocument doc)
    {
        if (doc == null) throw new ArgumentNullException(nameof(doc));

        var page = doc.AddPage();
        page.Size = PageSize.A4;
        page.Orientation = PageOrientation.Portrait;

        Render(page);
    }

    public void Render(PdfPage page)
    {
        using var gfx = XGraphics.FromPdfPage(page);

        xMm = config.MargemHorizontalMm;
        yMm = config.MargemVerticalMm;
        largUtilMm = DANFSeConstantes.PaginaLarguraMm - (2 * config.MargemHorizontalMm);
        var layout = CalcularLayout(gfx);

        // 1. Desenhar fundos sombreados oficiais (NT 008 / DANFSe v2.0)
        DesenharFundosSombreamento(gfx, layout);

        // 2. Marca d'água (Homologação, Cancelada ou Substituída) SOBRE os fundos sombreados
        DesenharMarcaDagua(gfx);

        // 3. Bordas, Linhas, Textos, QR-Code e Conteúdo
        xMm = config.MargemHorizontalMm;
        yMm = config.MargemVerticalMm;

        // Borda Externa da Página
        var rectBorda = new XRect(
            PdfDrawHelper.MmToPt(xMm),
            PdfDrawHelper.MmToPt(yMm),
            PdfDrawHelper.MmToPt(largUtilMm),
            PdfDrawHelper.MmToPt(DANFSeConstantes.PaginaAlturaMm - (2 * config.MargemVerticalMm)));
        gfx.DrawRectangle(PdfDrawHelper.PenBordaExterna, rectBorda);

        // Canhoto (Opcional)
        if (config.ExibirCanhoto)
            DesenharBandaCanhoto(gfx);

        // Cabeçalho
        DesenharBandaCabecalho(gfx);

        // Dados da NFS-e e QR-Code
        DesenharBandaDadosNFSe(gfx);

        // Prestador / Fornecedor
        DesenharBandaPrestador(gfx);

        // Tomador / Adquirente
        DesenharBandaTomador(gfx);

        // Destinatário
        DesenharBandaDestinatario(gfx);

        // Intermediário
        DesenharBandaIntermediario(gfx);

        // Serviço Prestado
        DesenharBandaServicoPrestado(gfx, layout.AlturaDescricaoServicoMm);

        // Tributação Municipal (ISSQN)
        DesenharBandaTributacaoMunicipal(gfx);

        // Tributação Federal (Exceto CBS)
        DesenharBandaTributacaoFederal(gfx);

        // Reforma Tributária IBS / CBS
        DesenharBandaTributacaoIBSCBS(gfx);

        // Valor Total da NFS-e
        DesenharBandaValorTotalNFSe(gfx);

        // Informações Complementares
        DesenharBandaInformacoesComplementares(gfx, layout.AlturaInformacoesComplementaresMm);
    }

    private LayoutAlturas CalcularLayout(XGraphics gfx)
    {
        var lineH = DANFSeConstantes.AlturaLinhaPadraoMm;
        var alturaFixaMm = DANFSeConstantes.AlturaCabecalhoMm + 6.8 + (3 * lineH) + (4 * lineH);

        if (config.ExibirCanhoto)
            alturaFixaMm += DANFSeConstantes.AlturaTituloBlocoMm + lineH;

        alturaFixaMm += PossuiTomador() ? 3 * lineH : DANFSeConstantes.AlturaLinhaVaziaMm;

        alturaFixaMm += PossuiDestinatario() ? 2 * lineH : DANFSeConstantes.AlturaLinhaVaziaMm;
        alturaFixaMm += PossuiIntermediario() ? 3 * lineH : DANFSeConstantes.AlturaLinhaVaziaMm;

        alturaFixaMm += lineH + AlturaDescricaoTributacaoMm;
        alturaFixaMm += 4 * lineH; // Tributação Municipal
        alturaFixaMm += 2 * lineH; // Tributação Federal
        alturaFixaMm += 4 * lineH; // Tributação IBS / CBS
        alturaFixaMm += 2 * lineH; // Totais
        alturaFixaMm += DANFSeConstantes.AlturaTituloBlocoMm;

        var alturaUtilMm = DANFSeConstantes.PaginaAlturaMm - (2 * config.MargemVerticalMm);
        var alturaFlexivelMm = alturaUtilMm - alturaFixaMm;
        var alturaFlexivelMinimaMm = AlturaMinimaDescricaoServicoMm +
                                     AlturaMinimaInformacoesComplementaresMm;
        if (alturaFlexivelMm < alturaFlexivelMinimaMm)
            throw new InvalidOperationException("O leiaute do DANFSe excede o espaço disponível em uma página A4.");

        var descricaoServico = nota.Informacoes.Dps.Informacoes.Servico.Informacoes.Descricao;
        var alturaTextoMm = PdfDrawHelper.MedirAlturaTextoMm(
            gfx,
            descricaoServico,
            largUtilMm - 2.0,
            DANFSeConstantes.FonteConteudoPt);
        var alturaNecessariaMm = Math.Max(
            AlturaMinimaDescricaoServicoMm,
            2.9 + alturaTextoMm);
        var alturaMaximaServicoMm = alturaFlexivelMm - AlturaMinimaInformacoesComplementaresMm;
        var alturaServicoMm = Math.Min(alturaNecessariaMm, alturaMaximaServicoMm);

        return new LayoutAlturas(
            alturaServicoMm,
            alturaFlexivelMm - alturaServicoMm);
    }

    private void DesenharFundosSombreamento(XGraphics gfx, LayoutAlturas layout)
    {
        var curY = config.MargemVerticalMm;
        var colW4 = largUtilMm / 4.0;
        var lineH = DANFSeConstantes.AlturaLinhaPadraoMm;
        var hTitulo = DANFSeConstantes.AlturaTituloBlocoMm;

        // Canhoto
        if (config.ExibirCanhoto)
        {
            PdfDrawHelper.DesenharFundoSombreado(gfx, xMm, curY, largUtilMm, hTitulo);
            curY += hTitulo + lineH;
        }

        // Cabeçalho
        var hCab = DANFSeConstantes.AlturaCabecalhoMm;
        PdfDrawHelper.DesenharFundoSombreado(gfx, xMm, curY, largUtilMm, hCab);
        curY += hCab;

        // Dados da NFS-e
        var chave = ObterChaveAcesso();
        var qrColWMm = config.ExibirQRCode && !string.IsNullOrWhiteSpace(chave) ? 42.0 : 0.0;
        var dadosWMm = largUtilMm - qrColWMm;
        var colW3 = dadosWMm / 3.0;
        var linha3Y = curY + 6.8 + (2 * lineH);
        PdfDrawHelper.DesenharFundoSombreado(gfx, xMm, linha3Y, colW3, lineH);
        curY = curY + 6.8 + (3 * lineH);

        // Prestador / Fornecedor
        PdfDrawHelper.DesenharFundoSombreado(gfx, xMm, curY, colW4, lineH);
        curY += 4 * lineH;

        // Tomador / Adquirente
        if (!PossuiTomador())
        {
            curY += DANFSeConstantes.AlturaLinhaVaziaMm;
        }
        else
        {
            PdfDrawHelper.DesenharFundoSombreado(gfx, xMm, curY, colW4, lineH);
            curY += 3 * lineH;
        }

        // Destinatário
        if (!PossuiDestinatario())
        {
            curY += DANFSeConstantes.AlturaLinhaVaziaMm;
        }
        else
        {
            PdfDrawHelper.DesenharFundoSombreado(gfx, xMm, curY, colW4, lineH);
            curY += 2 * lineH;
        }

        // Intermediário
        if (!PossuiIntermediario())
        {
            curY += DANFSeConstantes.AlturaLinhaVaziaMm;
        }
        else
        {
            PdfDrawHelper.DesenharFundoSombreado(gfx, xMm, curY, colW4, lineH);
            curY += 3 * lineH;
        }

        // Serviço Prestado
        PdfDrawHelper.DesenharFundoSombreado(gfx, xMm, curY, colW4, lineH);
        curY += lineH + AlturaDescricaoTributacaoMm + layout.AlturaDescricaoServicoMm;

        // Tributação Municipal (ISSQN)
        PdfDrawHelper.DesenharFundoSombreado(gfx, xMm, curY, colW4, lineH);
        curY += 4 * lineH;

        // Tributação Federal (Exceto CBS)
        PdfDrawHelper.DesenharFundoSombreado(gfx, xMm, curY, colW4, lineH);
        curY += 2 * lineH;

        // Tributação IBS / CBS
        PdfDrawHelper.DesenharFundoSombreado(gfx, xMm, curY, colW4, lineH);
        curY += 4 * lineH;

        // Valor Total da NFS-e
        PdfDrawHelper.DesenharFundoSombreado(gfx, xMm, curY, colW4, lineH);
        PdfDrawHelper.DesenharFundoSombreado(gfx, xMm + 3 * colW4, curY + lineH, colW4, lineH);
        curY += 2 * lineH;

        // Informações Complementares
        PdfDrawHelper.DesenharFundoSombreado(gfx, xMm, curY, largUtilMm, hTitulo);
    }

    #endregion Methods

    #region Bandas de Layout

    private void DesenharBandaCanhoto(XGraphics gfx)
    {
        var hTitulo = DANFSeConstantes.AlturaTituloBlocoMm;
        var hLinha = DANFSeConstantes.AlturaLinhaPadraoMm;
        var colW = largUtilMm / 4.0;

        PdfDrawHelper.DesenharTituloBloco(gfx, xMm, yMm, largUtilMm, hTitulo, "CANHOTO DE RECEBIMENTO");
        yMm += hTitulo;

        PdfDrawHelper.DesenharCampo(gfx, xMm, yMm, colW, hLinha, "Data da Cientificação", "");
        PdfDrawHelper.DesenharCampo(gfx, xMm + colW, yMm, colW * 2.0, hLinha, "Identificação e Assinatura do Recebedor", "");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 3 * colW, yMm, colW, hLinha, "Nº NFS-e / Chave", $"{nota.Informacoes.NumeroNFSe}");

        yMm += hLinha;
        PdfDrawHelper.DesenharLinhaSeparadora(gfx, xMm, yMm, largUtilMm);
    }

    private void DesenharBandaCabecalho(XGraphics gfx)
    {
        var hCab = DANFSeConstantes.AlturaCabecalhoMm;
        PdfDrawHelper.DesenharRetanguloSombreado(gfx, xMm, yMm, largUtilMm, hCab);

        // Logo NFS-e / Prestador (lado esquerdo)
        var logoW = 40.0;
        var logoBytes = config.LogoNacional ?? config.LogoPrestador ?? LogoNacionalPadrao.Value;
        if (logoBytes != null && logoBytes.Length > 0)
        {
            PdfDrawHelper.DesenharImagem(gfx, xMm + 1.5, yMm + 1.0, logoW, hCab - 2.0, logoBytes);
        }

        // Centro - Título do Documento
        var centroX = xMm + logoW + 2.0;
        var centroW = largUtilMm - logoW - 55.0;

        var fontTituloDoc = new XFont(DANFSeConstantes.FontePadrao, DANFSeConstantes.FonteTituloDocPt, XFontStyleEx.Bold);
        var rectDanfse = new XRect(PdfDrawHelper.MmToPt(centroX), PdfDrawHelper.MmToPt(yMm + 1.0), PdfDrawHelper.MmToPt(centroW), PdfDrawHelper.MmToPt(4.5));
        gfx.DrawString("DANFSe v2.0", fontTituloDoc, PdfDrawHelper.BrushPreto, rectDanfse, XStringFormats.Center);

        var rectSub = new XRect(PdfDrawHelper.MmToPt(centroX), PdfDrawHelper.MmToPt(yMm + 5.0), PdfDrawHelper.MmToPt(centroW), PdfDrawHelper.MmToPt(4.0));
        gfx.DrawString("Documento Auxiliar da NFS-e", fontTituloDoc, PdfDrawHelper.BrushPreto, rectSub, XStringFormats.Center);

        if (config.Homologacao)
        {
            var fontSemVal = new XFont(DANFSeConstantes.FontePadrao, DANFSeConstantes.FonteSemValidadePt, XFontStyleEx.Bold);
            var rectSemVal = new XRect(PdfDrawHelper.MmToPt(centroX), PdfDrawHelper.MmToPt(yMm + 8.5), PdfDrawHelper.MmToPt(centroW), PdfDrawHelper.MmToPt(3.0));
            gfx.DrawString(DANFSeConstantes.MsgSemValidade, fontSemVal, new XSolidBrush(PdfDrawHelper.CorVermelho), rectSemVal, XStringFormats.Center);
        }

        // Lado Direito - Município e Ambientes
        var dirX = xMm + largUtilMm - 52.0;
        var dirW = 51.0;

        var emitente = nota.Informacoes.Emitente;
        var municipio = !string.IsNullOrWhiteSpace(config.CabecalhoMunicipio)
            ? config.CabecalhoMunicipio
            : ObterMunicipioUf(emitente.Endereco.CodMunicipio, nota.Informacoes.LocalEmissao, emitente.Endereco.UF);

        var fontMun = new XFont(DANFSeConstantes.FontePadrao, DANFSeConstantes.FonteMunicipioPt, XFontStyleEx.Bold);
        var fontAmb = new XFont(DANFSeConstantes.FontePadrao, DANFSeConstantes.FonteAmbientePt, XFontStyleEx.Regular);

        var rectMun = new XRect(PdfDrawHelper.MmToPt(dirX), PdfDrawHelper.MmToPt(yMm + 1.0), PdfDrawHelper.MmToPt(dirW), PdfDrawHelper.MmToPt(4.0));
        gfx.DrawString($"Município: {municipio}", fontMun, PdfDrawHelper.BrushPreto, rectMun, XStringFormats.Center);

        var rectAmbGer = new XRect(PdfDrawHelper.MmToPt(dirX), PdfDrawHelper.MmToPt(yMm + 5.2), PdfDrawHelper.MmToPt(dirW), PdfDrawHelper.MmToPt(3.0));
        gfx.DrawString($"Ambiente Gerador: {ObterDescricaoAmbienteGerador()}", fontAmb, PdfDrawHelper.BrushPreto, rectAmbGer, XStringFormats.Center);

        var rectTpAmb = new XRect(PdfDrawHelper.MmToPt(dirX), PdfDrawHelper.MmToPt(yMm + 8.2), PdfDrawHelper.MmToPt(dirW), PdfDrawHelper.MmToPt(3.0));
        var tipoAmbTexto = config.Homologacao ? "Homologação" : "Produção";
        gfx.DrawString($"Tipo de Ambiente: {tipoAmbTexto}", fontAmb, PdfDrawHelper.BrushPreto, rectTpAmb, XStringFormats.Center);

        yMm += hCab;
        PdfDrawHelper.DesenharLinhaSeparadora(gfx, xMm, yMm, largUtilMm);
    }

    private void DesenharBandaDadosNFSe(XGraphics gfx)
    {
        var chave = ObterChaveAcesso();
        var dps = nota.Informacoes.Dps.Informacoes;

        var qrSizeMm = DANFSeConstantes.QrCodeTamanhoMm;
        var qrColWMm = config.ExibirQRCode && !string.IsNullOrWhiteSpace(chave) ? 42.0 : 0.0;
        var dadosWMm = largUtilMm - qrColWMm;
        var colW = dadosWMm / 3.0;
        var lineH = 6.4;

        // Chave de Acesso
        var fontLabel = new XFont(DANFSeConstantes.FontePadrao, DANFSeConstantes.FonteTituloBlocoPt, XFontStyleEx.Bold);
        var fontChave = new XFont(DANFSeConstantes.FontePadrao, 7.5, XFontStyleEx.Bold);

        gfx.DrawString("CHAVE DE ACESSO DA NFS-e", fontLabel, PdfDrawHelper.BrushPreto,
            new XRect(PdfDrawHelper.MmToPt(xMm + 1.0), PdfDrawHelper.MmToPt(yMm + 0.5), PdfDrawHelper.MmToPt(dadosWMm), PdfDrawHelper.MmToPt(3.0)), XStringFormats.TopLeft);

        gfx.DrawString(FormatarChaveAcesso(chave), fontChave, PdfDrawHelper.BrushPreto,
            new XRect(PdfDrawHelper.MmToPt(xMm + 1.0), PdfDrawHelper.MmToPt(yMm + 3.2), PdfDrawHelper.MmToPt(dadosWMm), PdfDrawHelper.MmToPt(3.5)), XStringFormats.TopLeft);

        var curY = yMm + 6.8;

        // Linha 1: Número da NFS-e / Competência / Data Emissão NFS-e
        PdfDrawHelper.DesenharCampo(gfx, xMm + 0 * colW, curY, colW, lineH, "NÚMERO DA NFS-e", nota.Informacoes.NumeroNFSe.ToString());
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, curY, colW, lineH, "COMPETÊNCIA", dps.Competencia > DateTime.MinValue ? dps.Competencia.ToString("dd/MM/yyyy") : "-");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, curY, colW, lineH, "DATA/HORA EMISSÃO NFS-e", nota.Informacoes.DhProcessamento.ToString("dd/MM/yyyy HH:mm:ss"));

        curY += lineH;

        // Linha 2: Número DPS / Série DPS / Data Emissão DPS
        PdfDrawHelper.DesenharCampo(gfx, xMm + 0 * colW, curY, colW, lineH, "NÚMERO DA DPS", dps.NumeroDps);
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, curY, colW, lineH, "SÉRIE DA DPS", dps.Serie);
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, curY, colW, lineH, "DATA/HORA EMISSÃO DPS", dps.DhEmissao.ToString("dd/MM/yyyy HH:mm:ss"));

        curY += lineH;

        // Linha 3: Emitente / Situação / Finalidade
        PdfDrawHelper.DesenharCampo(gfx, xMm + 0 * colW, curY, colW, lineH, "EMITENTE DA NFS-e", "Prestador", sombreado: true);
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, curY, colW, lineH, "SITUAÇÃO DA NFS-e", ObterSituacaoNFSe());
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, curY, colW, lineH, "FINALIDADE", ObterFinalidade());

        // QR Code no canto direito
        if (qrColWMm > 0)
        {
            var qrX = xMm + dadosWMm + (qrColWMm - qrSizeMm) / 2.0;
            var qrY = yMm + 1.5;
            PdfDrawHelper.DesenharQrCode(gfx, qrX, qrY, qrSizeMm, DANFSeConstantes.UrlConsultaPublica + chave);

            PdfDrawHelper.DesenharTextoMultiLinhas(
                gfx,
                xMm + dadosWMm + 1.0,
                qrY + qrSizeMm + 0.5,
                qrColWMm - 2.0,
                7.0,
                DANFSeConstantes.TextoQrComplemento,
                DANFSeConstantes.FonteTextoQrPt);
        }

        yMm = curY + lineH;
        PdfDrawHelper.DesenharLinhaSeparadora(gfx, xMm, yMm, largUtilMm);
    }

    private void DesenharBandaPrestador(XGraphics gfx)
    {
        var colW = largUtilMm / 4.0;
        var lineH = DANFSeConstantes.AlturaLinhaPadraoMm;
        var emit = nota.Informacoes.Emitente;
        var end = emit.Endereco;
        var reg = nota.Informacoes.Dps.Informacoes.Prestador.Regime;

        // Linha 1
        PdfDrawHelper.DesenharTituloBlocoInline(gfx, xMm, yMm, colW, lineH, "Prestador / Fornecedor");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, yMm, colW, lineH, "CNPJ/CPF/NIF", FormatarCpfCnpj(emit.CNPJ ?? emit.CPF));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW, lineH, "Indicador Municipal (Inscrição)", emit.InscricaoMunicipal ?? "-");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 3 * colW, yMm, colW, lineH, "Telefone", FormatarTelefone(emit.Telefone));

        yMm += lineH;

        // Linha 2
        PdfDrawHelper.DesenharCampo(gfx, xMm, yMm, colW * 2.0, lineH, "Nome / Nome Empresarial", emit.RazaoSocial);
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW, lineH, "Município / Sigla UF", ObterMunicipioUf(end.CodMunicipio, nota.Informacoes.LocalEmissao, end.UF));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 3 * colW, yMm, colW, lineH, "Código IBGE / CEP", $"{end.CodMunicipio} / {FormatarCep(end.CEP)}");

        yMm += lineH;

        // Linha 3
        var endCompleto = $"{end.Logradouro}, {end.Numero} {end.Complemento} - {end.Bairro}";
        PdfDrawHelper.DesenharCampo(gfx, xMm, yMm, colW * 2.0, lineH, "Endereço", endCompleto);
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW * 2.0, lineH, "E-mail", emit.Email?.ToLower() ?? "-");

        yMm += lineH;

        // Linha 4
        PdfDrawHelper.DesenharCampo(gfx, xMm, yMm, colW * 2.0, lineH, "Simples Nacional na Data de Competência", ObterSimplesNacionalDescricao(reg.OptanteSimplesNacional));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW * 2.0, lineH, "Regime de Apuração Tributária pelo SN", ObterDescricaoRegimeApuracao(reg.RegimeApuracao));

        yMm += lineH;
        PdfDrawHelper.DesenharLinhaSeparadora(gfx, xMm, yMm, largUtilMm);
    }

    private void DesenharBandaTomador(XGraphics gfx)
    {
        var colW = largUtilMm / 4.0;
        var lineH = DANFSeConstantes.AlturaLinhaPadraoMm;
        var tomador = nota.Informacoes.Dps.Informacoes.Tomador;

        if (!PossuiTomador())
        {
            PdfDrawHelper.DesenharLinhaVazia(gfx, xMm, yMm, largUtilMm, DANFSeConstantes.AlturaLinhaVaziaMm, DANFSeConstantes.MsgTomadorVazio);
            yMm += DANFSeConstantes.AlturaLinhaVaziaMm;
            PdfDrawHelper.DesenharLinhaSeparadora(gfx, xMm, yMm, largUtilMm);
            return;
        }

        var end = tomador?.Endereco;
        // Linha 1
        PdfDrawHelper.DesenharTituloBlocoInline(gfx, xMm, yMm, colW, lineH, "Tomador / Adquirente");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, yMm, colW, lineH, "CNPJ/CPF/NIF", FormatarCpfCnpj(tomador?.CNPJ ?? tomador?.CPF ?? tomador?.Nif));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW, lineH, "Indicador Municipal (Inscrição)", tomador?.InscricaoMunicipal ?? "-");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 3 * colW, yMm, colW, lineH, "Telefone", FormatarTelefone(tomador?.Telefone));

        yMm += lineH;

        // Linha 2
        PdfDrawHelper.DesenharCampo(gfx, xMm, yMm, colW * 2.0, lineH, "Nome / Nome Empresarial", tomador?.Nome ?? "-");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW, lineH, "Município / Sigla UF", ObterMunicipioUf(end?.Municipio));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 3 * colW, yMm, colW, lineH, "Código IBGE / CEP", ObterCodigoIbgeCep(end?.Municipio));

        yMm += lineH;

        // Linha 3
        var endCompleto = end != null ? $"{end.Logradouro}, {end.Numero} {end.Complemento} - {end.Bairro}" : "-";
        PdfDrawHelper.DesenharCampo(gfx, xMm, yMm, colW * 2.0, lineH, "Endereço", endCompleto);
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW * 2.0, lineH, "E-mail", tomador?.Email?.ToLower() ?? "-");

        yMm += lineH;
        PdfDrawHelper.DesenharLinhaSeparadora(gfx, xMm, yMm, largUtilMm);
    }

    private void DesenharBandaDestinatario(XGraphics gfx)
    {
        var colW = largUtilMm / 4.0;
        var lineH = DANFSeConstantes.AlturaLinhaPadraoMm;

        if (!PossuiDestinatario())
        {
            PdfDrawHelper.DesenharLinhaVazia(gfx, xMm, yMm, largUtilMm, DANFSeConstantes.AlturaLinhaVaziaMm, DANFSeConstantes.MsgDestVazio);
            yMm += DANFSeConstantes.AlturaLinhaVaziaMm;
            PdfDrawHelper.DesenharLinhaSeparadora(gfx, xMm, yMm, largUtilMm);
            return;
        }

        var dest = nota.Informacoes.Dps.Informacoes.IBSCBS?.Destinatario;
        var end = dest?.Endereco;
        // Linha 1
        PdfDrawHelper.DesenharTituloBlocoInline(gfx, xMm, yMm, colW, lineH, "Destinatário da Operação");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, yMm, colW * 2.0, lineH, "CNPJ/CPF/NIF", FormatarCpfCnpj(dest?.CNPJ ?? dest?.CPF ?? dest?.NIF));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 3 * colW, yMm, colW, lineH, "Telefone", FormatarTelefone(dest?.Telefone));

        yMm += lineH;

        // Linha 2
        PdfDrawHelper.DesenharCampo(gfx, xMm, yMm, colW * 2.0, lineH, "Nome / Nome Empresarial", dest?.Nome ?? "-");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW, lineH, "Município / Sigla UF", ObterMunicipioUf(end?.Municipio));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 3 * colW, yMm, colW, lineH, "Código IBGE / CEP", ObterCodigoIbgeCep(end?.Municipio));

        yMm += lineH;
        PdfDrawHelper.DesenharLinhaSeparadora(gfx, xMm, yMm, largUtilMm);
    }

    private void DesenharBandaIntermediario(XGraphics gfx)
    {
        var colW = largUtilMm / 4.0;
        var lineH = DANFSeConstantes.AlturaLinhaPadraoMm;
        var interm = nota.Informacoes.Dps.Informacoes.Intermediario;

        if (!PossuiIntermediario())
        {
            PdfDrawHelper.DesenharLinhaVazia(gfx, xMm, yMm, largUtilMm, DANFSeConstantes.AlturaLinhaVaziaMm, DANFSeConstantes.MsgIntermVazio);
            yMm += DANFSeConstantes.AlturaLinhaVaziaMm;
            PdfDrawHelper.DesenharLinhaSeparadora(gfx, xMm, yMm, largUtilMm);
            return;
        }

        var end = interm?.Endereco;

        PdfDrawHelper.DesenharTituloBlocoInline(gfx, xMm, yMm, colW, lineH, "Intermediário da Operação");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, yMm, colW, lineH, "CNPJ/CPF/NIF", FormatarCpfCnpj(interm?.CNPJ ?? interm?.CPF));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW, lineH, "Indicador Municipal", interm?.InscricaoMunicipal ?? "-");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 3 * colW, yMm, colW, lineH, "Telefone", FormatarTelefone(interm?.Telefone));

        yMm += lineH;

        PdfDrawHelper.DesenharCampo(gfx, xMm, yMm, colW * 2.0, lineH, "Nome / Nome Empresarial", interm?.Nome ?? "-");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW, lineH, "Município / Sigla UF", ObterMunicipioUf(end?.Municipio));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 3 * colW, yMm, colW, lineH, "Código IBGE / CEP", ObterCodigoIbgeCep(end?.Municipio));

        yMm += lineH;

        var endCompleto = end != null ? $"{end.Logradouro}, {end.Numero} {end.Complemento} - {end.Bairro}" : "-";
        PdfDrawHelper.DesenharCampo(gfx, xMm, yMm, colW * 2.0, lineH, "Endereço", endCompleto);
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW * 2.0, lineH, "E-mail", interm?.Email?.ToLower() ?? "-");

        yMm += lineH;
        PdfDrawHelper.DesenharLinhaSeparadora(gfx, xMm, yMm, largUtilMm);
    }

    private void DesenharBandaServicoPrestado(XGraphics gfx, double alturaDescricaoMm)
    {
        var colW = largUtilMm / 4.0;
        var lineH = DANFSeConstantes.AlturaLinhaPadraoMm;
        var s = nota.Informacoes.Dps.Informacoes.Servico;
        var info = s.Informacoes;

        // Linha 1
        PdfDrawHelper.DesenharTituloBlocoInline(gfx, xMm, yMm, colW, lineH, "Serviço Prestado");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, yMm, colW, lineH, "Cód. Tributação Nac. / Mun.", $"{info.CodTributacaoNacional} / {info.CodTributacaoMunicipio ?? "-"}");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW, lineH, "Código da NBS", info.CodNBS ?? "-");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 3 * colW, yMm, colW, lineH, "Local da Prestação / UF / País", $"{ObterMunicipioUf(s.Localidade.CodMunicipioPrestacao, nota.Informacoes.LocalPrestacao)} / {ObterPais(s.Localidade.CodPaisPrestacao)}");

        yMm += lineH;

        // Linha 2: Descrição do Código de Tributação Nacional
        PdfDrawHelper.DesenharCampo(gfx, xMm, yMm, largUtilMm, AlturaDescricaoTributacaoMm, "", nota.Informacoes.DescricaoTributoNacional);
        yMm += AlturaDescricaoTributacaoMm;

        // Linha 3: Descrição do Serviço com altura dinâmica e fonte mínima normativa
        PdfDrawHelper.DesenharCampo(
            gfx,
            xMm,
            yMm,
            largUtilMm,
            alturaDescricaoMm,
            "Descrição do Serviço",
            info.Descricao,
            multilinha: true);

        yMm += alturaDescricaoMm;
        PdfDrawHelper.DesenharLinhaSeparadora(gfx, xMm, yMm, largUtilMm);
    }

    private void DesenharBandaTributacaoMunicipal(XGraphics gfx)
    {
        var colW = largUtilMm / 4.0;
        var lineH = DANFSeConstantes.AlturaLinhaPadraoMm;
        var vNfse = nota.Informacoes.Valores;
        var vDps = nota.Informacoes.Dps.Informacoes.Valores;
        var tribMun = vDps.Tributos.Municipal;
        var regime = nota.Informacoes.Dps.Informacoes.Prestador.Regime;

        // Linha 1
        PdfDrawHelper.DesenharTituloBlocoInline(gfx, xMm, yMm, colW, lineH, "Tributação Municipal (ISSQN)");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, yMm, colW, lineH, "Tipo de Tributação", ObterDescricaoTributacaoISSQN(tribMun.ISSQN));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW * 2.0, lineH, "Município / Sigla UF / País da Incidência", $"{ObterMunicipioUf(nota.Informacoes.CodLocalIncidencia, nota.Informacoes.LocalIncidencia)} / {ObterPais(tribMun.CodPais)}");

        yMm += lineH;

        // Linha 2
        PdfDrawHelper.DesenharCampo(gfx, xMm + 0 * colW, yMm, colW, lineH, "Regime Especial de Tributação", ObterDescricaoRegimeEspecial(regime.RegimeEspecial));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, yMm, colW, lineH, "Tipo de Imunidade do ISSQN", tribMun.TipoImunidade?.ToString() ?? "-");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW, lineH, "Suspensão da Exigibilidade", tribMun.Suspensao?.TipoSuspensao.ToString() ?? "-");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 3 * colW, yMm, colW, lineH, "Nº Processo Suspensão", tribMun.Suspensao?.NumeroProcesso ?? "-");

        yMm += lineH;

        // Linha 3
        PdfDrawHelper.DesenharCampo(gfx, xMm + 0 * colW, yMm, colW, lineH, "Benefício Municipal",
            vNfse.TipoBeneficioMunicipalInformado
                ? ObterDescricaoBeneficioMunicipal(vNfse.TipoBeneficioMunicipal)
                : "-");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, yMm, colW, lineH, "Cálculo do BM", FormatarMoedaOpcional(vNfse.ValorBcBeneficioMunicipal));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW, lineH, "Total Deduções / Reduções", FormatarMoedaOpcional(vDps.ValoresDeducaoReducao?.Valor));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 3 * colW, yMm, colW, lineH, "Desconto Incondicionado", FormatarMoedaOpcional(vDps.ValoresDesconto?.ValorIncodicional));

        yMm += lineH;

        // Linha 4
        PdfDrawHelper.DesenharCampo(gfx, xMm + 0 * colW, yMm, colW, lineH, "BC ISSQN", FormatarMoedaOpcional(vNfse.ValorBc));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, yMm, colW, lineH, "Alíquota Aplicada", FormatarPercentualOpcional(vNfse.Aliquota));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW, lineH, "Retenção do ISSQN", ObterDescricaoRetencaoISSQN(tribMun.TipoRetencaoISSQN));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 3 * colW, yMm, colW, lineH, "ISSQN Apurado", FormatarMoedaOpcional(vNfse.ValorISSQN));

        yMm += lineH;
        PdfDrawHelper.DesenharLinhaSeparadora(gfx, xMm, yMm, largUtilMm);
    }

    private void DesenharBandaTributacaoFederal(XGraphics gfx)
    {
        var colW = largUtilMm / 4.0;
        var lineH = DANFSeConstantes.AlturaLinhaPadraoMm;
        var vDps = nota.Informacoes.Dps.Informacoes.Valores;
        var tribFed = vDps.Tributos.Federal;
        var pisCofins = tribFed?.PisCofins;
        var pisCofinsRetidos = pisCofins?.TipoRetencao == TipoRetencaoPisCofinsCsll.PisCofinsRetidos;
        var valorContribuicoesSociais = pisCofinsRetidos
            ? SomarValoresInformados(tribFed?.ValorCSLL, pisCofins?.ValorPis, pisCofins?.ValorCofins)
            : tribFed?.ValorCSLL;
        var valorPis = pisCofinsRetidos ? 0m : pisCofins?.ValorPis;
        var valorCofins = pisCofinsRetidos ? 0m : pisCofins?.ValorCofins;

        // Linha 1
        PdfDrawHelper.DesenharTituloBlocoInline(gfx, xMm, yMm, colW, lineH, "Tributação Federal (Exceto CBS)");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, yMm, colW, lineH, "IRRF", FormatarMoedaOpcional(tribFed?.ValorIRRF));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW, lineH, "Contribuição Previdenciária - Retida", FormatarMoedaOpcional(tribFed?.ValorCP));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 3 * colW, yMm, colW, lineH, "Contribuições Sociais - Retidas", FormatarMoedaOpcional(valorContribuicoesSociais));

        yMm += lineH;

        // Linha 2
        PdfDrawHelper.DesenharCampo(gfx, xMm + 0 * colW, yMm, colW, lineH, "PIS - Débito de Apuração Própria", FormatarMoedaOpcional(valorPis));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, yMm, colW, lineH, "COFINS - Débito de Apuração Própria", FormatarMoedaOpcional(valorCofins));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW * 2.0, lineH, "Descrição Contrib. Sociais - Retidas",
            ObterDescricaoRetencaoPisCofins(pisCofins?.TipoRetencao));

        yMm += lineH;
        PdfDrawHelper.DesenharLinhaSeparadora(gfx, xMm, yMm, largUtilMm);
    }

    private void DesenharBandaTributacaoIBSCBS(XGraphics gfx)
    {
        var colW = largUtilMm / 4.0;
        var lineH = DANFSeConstantes.AlturaLinhaPadraoMm;
        var declarado = nota.Informacoes.Dps.Informacoes.IBSCBS;
        var calculado = nota.Informacoes.IBSCBS;
        var classificacao = declarado?.Valores.Tributos.GrupoIBSCBS;
        var valores = calculado?.Valores;
        var totalIbs = calculado?.Totais.TotalIBS;
        var totalCbs = calculado?.Totais.TotalCBS;

        var cstTexto = classificacao == null
            ? "- / -"
            : $"{ValorOuTraco(classificacao.CodigoSituacaoTributaria)} / {ValorOuTraco(classificacao.CodigoClassificacaoTributaria)}";
        var municipioIncidencia = calculado == null ||
                                  (string.IsNullOrWhiteSpace(calculado.CodigoLocalidadeIncidencia) &&
                                   string.IsNullOrWhiteSpace(calculado.DescricaoLocalidadeIncidencia))
            ? "- / -"
            : ObterMunicipioUf(calculado.CodigoLocalidadeIncidencia, calculado.DescricaoLocalidadeIncidencia);
        var indicadorLocal = $"{ValorOuTraco(declarado?.CodigoIndicadorOperacao)} / " +
                             $"{ValorOuTraco(calculado?.CodigoLocalidadeIncidencia)} / {municipioIncidencia}";

        // Linha 1
        PdfDrawHelper.DesenharTituloBlocoInline(gfx, xMm, yMm, colW, lineH, "Tributação IBS / CBS");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, yMm, colW, lineH, "CST / cClassTrib", cstTexto);
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW * 2.0, lineH,
            "Indicador de Operação / Código IBGE Incidência / Município Incidência / Sigla UF", indicadorLocal);

        yMm += lineH;

        // Linha 2
        PdfDrawHelper.DesenharCampo(gfx, xMm + 0 * colW, yMm, colW, lineH, "Exclusões e Reduções da Base de Cálculo", FormatarMoedaOpcional(calculado == null ? null : valores!.ValorCalcReeRepRes));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, yMm, colW, lineH, "Base de Cálculo Após Exclusões e Reduções", FormatarMoedaOpcional(calculado == null ? null : valores!.ValorBaseCalculo));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW, lineH, "Red. Alíquota IBS / Red. Alíquota CBS",
            FormatarPercentuais(valores?.Estado.PercentualReducaoAliquotaUf, valores?.Municipio.PercentualReducaoAliquotaMunicipal,
                valores?.Federal.PercentualReducaoAliquotaCBS));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 3 * colW, yMm, colW, lineH, "Alíquota - IBS UF / IBS Mun",
            FormatarPercentuais(calculado == null ? null : valores!.Estado.PercentualIBSUf,
                calculado == null ? null : valores!.Municipio.PercentualIBSMunicipal));

        yMm += lineH;

        // Linha 3
        PdfDrawHelper.DesenharCampo(gfx, xMm + 0 * colW, yMm, colW, lineH, "Alíq. Efetiva Municipal - IBS",
            FormatarPercentualOpcional(calculado == null ? null : valores!.Municipio.PercentualAliquotaEfetivaMunicipal));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, yMm, colW, lineH, "Valor Apurado Municipal - IBS",
            FormatarMoedaOpcional(totalIbs == null ? null : totalIbs.TotalMunicipal.ValorIBSMunicipal));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW, lineH, "Alíq. Efetiva Estadual - IBS",
            FormatarPercentualOpcional(calculado == null ? null : valores!.Estado.PercentualAliquotaEfetivaUf));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 3 * colW, yMm, colW, lineH, "Valor Apurado Estadual - IBS",
            FormatarMoedaOpcional(totalIbs == null ? null : totalIbs.TotalEstadual.ValorIBSUF));

        yMm += lineH;

        // Linha 4
        PdfDrawHelper.DesenharCampo(gfx, xMm + 0 * colW, yMm, colW, lineH, "Valor Total Apurado - IBS",
            FormatarMoedaOpcional(totalIbs == null ? null : totalIbs.ValorTotalIBS));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, yMm, colW, lineH, "Alíquota - CBS",
            FormatarPercentualOpcional(calculado == null ? null : valores!.Federal.PercentualCBS));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW, lineH, "Alíquota Efetiva - CBS",
            FormatarPercentualOpcional(calculado == null ? null : valores!.Federal.PercentualAliquotaEfetivaCBS));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 3 * colW, yMm, colW, lineH, "Valor Total Apurado - CBS",
            FormatarMoedaOpcional(totalCbs == null ? null : totalCbs.ValorCBS));

        yMm += lineH;
        PdfDrawHelper.DesenharLinhaSeparadora(gfx, xMm, yMm, largUtilMm);
    }

    private void DesenharBandaValorTotalNFSe(XGraphics gfx)
    {
        var colW = largUtilMm / 4.0;
        var lineH = DANFSeConstantes.AlturaLinhaPadraoMm;
        var vNfse = nota.Informacoes.Valores;
        var vDps = nota.Informacoes.Dps.Informacoes.Valores;

        var vServicos = vDps.ValoresServico.Valor;
        var vDescIncond = vDps.ValoresDesconto?.ValorIncodicional;
        var vDescCond = vDps.ValoresDesconto?.ValorCondicional;
        var totaisIBSCBS = nota.Informacoes.IBSCBS?.Totais;
        decimal? totalIBSCBS = totaisIBSCBS == null
            ? null
            : totaisIBSCBS.TotalIBS.ValorTotalIBS + totaisIBSCBS.TotalCBS.ValorCBS;
        var valorTotalNF = totaisIBSCBS?.ValorTotalNF;

        // Linha 1
        PdfDrawHelper.DesenharTituloBlocoInline(gfx, xMm, yMm, colW, lineH, "Valor Total da NFS-e");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, yMm, colW, lineH, "Valor da Operação / Serviço", FormatarMoeda(vServicos));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW, lineH, "Desconto Incondicionado", FormatarMoedaOpcional(vDescIncond));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 3 * colW, yMm, colW, lineH, "Desconto Condicionado", FormatarMoedaOpcional(vDescCond));

        yMm += lineH;

        // Linha 2
        PdfDrawHelper.DesenharCampo(gfx, xMm + 0 * colW, yMm, colW, lineH, "Total Retenções (ISS / Federais)", FormatarMoedaOpcional(vNfse.TotalRetido));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, yMm, colW, lineH, "Valor Líquido da NFS-e", FormatarMoedaOpcional(vNfse.ValorLiquido), negrito: true);
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW, lineH, "Total IBS / CBS", FormatarMoedaOpcional(totalIBSCBS));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 3 * colW, yMm, colW, lineH, "Valor Líquido + IBS/CBS", FormatarMoedaOpcional(valorTotalNF), sombreado: true, negrito: true);

        yMm += lineH;
        PdfDrawHelper.DesenharLinhaSeparadora(gfx, xMm, yMm, largUtilMm);
    }

    private void DesenharBandaInformacoesComplementares(XGraphics gfx, double alturaConteudoMm)
    {
        var hTitulo = DANFSeConstantes.AlturaTituloBlocoMm;
        PdfDrawHelper.DesenharTituloBloco(gfx, xMm, yMm, largUtilMm, hTitulo, "INFORMAÇÕES COMPLEMENTARES");
        yMm += hTitulo;

        var complementares = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(nota.Informacoes.Valores.OutrasInformacoes))
            complementares.AppendLine(nota.Informacoes.Valores.OutrasInformacoes);

        var complServ = nota.Informacoes.Dps.Informacoes.Servico.InformacoesComplementares;
        if (!string.IsNullOrWhiteSpace(complServ?.Informacoes))
            complementares.AppendLine(complServ!.Informacoes);

        // Tributos aproximados (IBPT / Lei 12.741)
        var totTrib = nota.Informacoes.Dps.Informacoes.Valores.Tributos.Total;
        if (totTrib.ValorTotal != null)
        {
            complementares.AppendLine($"Valor aproximado dos tributos: Federais: {FormatarMoeda(totTrib.ValorTotal.TotalFederal)}, " +
                                   $"Estaduais: {FormatarMoeda(totTrib.ValorTotal.TotalEstadual)}, " +
                                   $"Municipais: {FormatarMoeda(totTrib.ValorTotal.TotalMunicipal)} (Lei 12.741/2012)");
        }

        PdfDrawHelper.DesenharTextoComReticencias(
            gfx,
            xMm + 1.0,
            yMm + 1.0,
            largUtilMm - 2.0,
            alturaConteudoMm - 2.0,
            complementares.ToString(),
            DANFSeConstantes.FonteConteudoPt);

        yMm += alturaConteudoMm;
    }

    private void DesenharMarcaDagua(XGraphics gfx)
    {
        string? texto = null;
        if (config.Cancelada)
            texto = DANFSeConstantes.MsgCancelada;
        else if (config.Substituida)
            texto = DANFSeConstantes.MsgSubstituida;
        else if (config.Homologacao)
            texto = DANFSeConstantes.MsgSemValidade;

        if (!string.IsNullOrWhiteSpace(texto))
        {
            PdfDrawHelper.DesenharMarcaDagua(gfx, DANFSeConstantes.PaginaLarguraMm, DANFSeConstantes.PaginaAlturaMm, texto!);
        }
    }

    #endregion Bandas de Layout

    #region Helpers de Formatação e Dados

    private string ObterChaveAcesso()
    {
        var id = nota.Informacoes.Id;
        if (string.IsNullOrWhiteSpace(id)) return string.Empty;
        return Regex.Replace(id, "[^0-9]", "");
    }

    private static string FormatarChaveAcesso(string chave)
    {
        if (chave.Length != 50) return chave;
        var sb = new StringBuilder();
        for (int i = 0; i < chave.Length; i += 4)
        {
            if (i > 0) sb.Append(' ');
            var chunkLen = Math.Min(4, chave.Length - i);
            sb.Append(chave.Substring(i, chunkLen));
        }
        return sb.ToString();
    }

    private static string FormatarCpfCnpj(string? doc)
    {
        if (string.IsNullOrWhiteSpace(doc)) return "-";
        var num = Regex.Replace(doc, "[^0-9a-zA-Z]", "");
        if (num.Length == 11) return $"{num.Substring(0, 3)}.{num.Substring(3, 3)}.{num.Substring(6, 3)}-{num.Substring(9, 2)}";
        if (num.Length == 14) return $"{num.Substring(0, 2)}.{num.Substring(2, 3)}.{num.Substring(5, 3)}/{num.Substring(8, 4)}-{num.Substring(12, 2)}";
        return doc!;
    }

    private static string FormatarCep(string? cep)
    {
        if (string.IsNullOrWhiteSpace(cep)) return "-";
        var num = Regex.Replace(cep, "[^0-9]", "");
        return num.Length == 8 ? $"{num.Substring(0, 5)}-{num.Substring(5)}" : cep!;
    }

    private static string FormatarTelefone(string? fone)
    {
        if (string.IsNullOrWhiteSpace(fone)) return "-";
        var num = Regex.Replace(fone, "[^0-9]", "");
        if (num.Length == 10) return $"({num.Substring(0, 2)}) {num.Substring(2, 4)}-{num.Substring(6, 4)}";
        if (num.Length == 11) return $"({num.Substring(0, 2)}) {num.Substring(2, 5)}-{num.Substring(7, 4)}";
        return fone!;
    }

    private static string ObterMunicipioUf(IMunicipio? municipio) => municipio switch
    {
        MunicipioNacional nacional => ObterMunicipioUf(nacional.CodMunicipio),
        MunicipioExterior exterior => $"{exterior.Cidade} / {exterior.EstadoProvincia}",
        _ => "-"
    };

    private static string ObterMunicipioUf(string? codigoIbge, string? descricaoFallback = null,
        string? ufFallback = null)
    {
        var municipioUf = MunicipioIbgeResolver.ObterMunicipioUf(codigoIbge);
        if (!string.IsNullOrWhiteSpace(municipioUf)) return municipioUf!;

        if (!string.IsNullOrWhiteSpace(descricaoFallback))
        {
            return string.IsNullOrWhiteSpace(ufFallback)
                ? descricaoFallback!
                : $"{descricaoFallback} / {ufFallback}";
        }

        return string.IsNullOrWhiteSpace(codigoIbge) ? "-" : codigoIbge!;
    }

    private static string ObterCodigoIbgeCep(IMunicipio? municipio) => municipio switch
    {
        MunicipioNacional nacional => $"{nacional.CodMunicipio} / {FormatarCep(nacional.CEP)}",
        MunicipioExterior exterior => string.IsNullOrWhiteSpace(exterior.EnderecoPostal) ? "-" : exterior.EnderecoPostal,
        _ => "-"
    };

    private static string ObterPais(string? codigoPais) =>
        string.IsNullOrWhiteSpace(codigoPais) ? "-" : codigoPais!;

    private static byte[]? CarregarLogoNacionalPadrao()
    {
        var assembly = typeof(DANFSeNacionalReport).Assembly;
        using var stream = assembly.GetManifestResourceStream(LogoNacionalResourceName);
        if (stream == null) return null;

        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }

    private static string FormatarMoeda(decimal? valor) => (valor ?? 0).ToString("N2", PtBr);
    private static string FormatarPercentual(decimal? valor) => (valor ?? 0).ToString("N2", PtBr) + " %";
    private static string FormatarMoedaOpcional(decimal? valor) => valor?.ToString("N2", PtBr) ?? "-";
    private static string FormatarPercentualOpcional(decimal? valor) => valor == null ? "-" : valor.Value.ToString("N2", PtBr) + " %";
    private static string FormatarPercentuais(decimal? primeiro, decimal? segundo) =>
        $"{FormatarPercentualOpcional(primeiro)} / {FormatarPercentualOpcional(segundo)}";
    private static string FormatarPercentuais(decimal? primeiro, decimal? segundo, decimal? terceiro) =>
        $"{FormatarPercentualOpcional(primeiro)} / {FormatarPercentualOpcional(segundo)} / {FormatarPercentualOpcional(terceiro)}";
    private static string ValorOuTraco(string? valor) => string.IsNullOrWhiteSpace(valor) ? "-" : valor!;

    private static decimal? SomarValoresInformados(decimal? valorCsll, decimal? valorPis, decimal? valorCofins) =>
        valorCsll.HasValue || valorPis.HasValue || valorCofins.HasValue
            ? (valorCsll ?? 0m) + (valorPis ?? 0m) + (valorCofins ?? 0m)
            : null;

    private static string ObterDescricaoTributacaoISSQN(TributoISSQN tributacao) => tributacao switch
    {
        TributoISSQN.OperacaoTributavel => "Operação Tributável",
        TributoISSQN.Imunidade => "Imunidade",
        TributoISSQN.ExportacaoServico => "Exportação de Serviço",
        TributoISSQN.NaoIncidencia => "Não Incidência",
        _ => tributacao.ToString()
    };

    private static string ObterDescricaoRetencaoISSQN(TipoRetencaoISSQN? retencao) => retencao switch
    {
        TipoRetencaoISSQN.NaoRetido => "Não Retido",
        TipoRetencaoISSQN.RetidoTomador => "Retido pelo Tomador",
        TipoRetencaoISSQN.RetidoIntermediario => "Retido pelo Intermediário",
        _ => "-"
    };

    private static string ObterDescricaoRetencaoPisCofins(TipoRetencaoPisCofinsCsll? retencao) => retencao switch
    {
        TipoRetencaoPisCofinsCsll.PisCofinsCsllNaoRetidos => "PIS/COFINS/CSLL Não Retidos",
        TipoRetencaoPisCofinsCsll.PisCofinsRetidos => "PIS/COFINS Retidos",
        TipoRetencaoPisCofinsCsll.PisCofinsNaoRetidos => "PIS/COFINS Não Retidos",
        TipoRetencaoPisCofinsCsll.PisCofinsCsllRetidos => "PIS/COFINS/CSLL Retidos",
        TipoRetencaoPisCofinsCsll.PisCofinsRetidosCsllNaoRetido => "PIS/COFINS Retidos, CSLL Não Retida",
        TipoRetencaoPisCofinsCsll.PisRetidoCofinsCsllNaoRetidos => "PIS Retido, COFINS/CSLL Não Retidos",
        TipoRetencaoPisCofinsCsll.CofinsRetidoPisCSllNaoRetidos => "COFINS Retida, PIS/CSLL Não Retidos",
        TipoRetencaoPisCofinsCsll.PisNaoRetidoCofinsCsllRetidos => "PIS Não Retido, COFINS/CSLL Retidos",
        TipoRetencaoPisCofinsCsll.PisCofinsNaoRetidosCsllRetido => "PIS/COFINS Não Retidos, CSLL Retida",
        TipoRetencaoPisCofinsCsll.CofinsNaoRetidoPisCSllRetidos => "COFINS Não Retida, PIS/CSLL Retidos",
        _ => "-"
    };

    private static string ObterDescricaoBeneficioMunicipal(TipoBeneficioMunicipal? beneficio) => beneficio switch
    {
        TipoBeneficioMunicipal.Isencao => "Isenção",
        TipoBeneficioMunicipal.ReducaoBCPerc => "Redução percentual da BC",
        TipoBeneficioMunicipal.ReducaoBCValor => "Redução em valor da BC",
        TipoBeneficioMunicipal.AliquotaDiferenciada => "Alíquota diferenciada",
        _ => "-"
    };

    private static string ObterDescricaoRegimeEspecial(RegimeEspecial regime) => regime switch
    {
        RegimeEspecial.Nenhum => "-",
        RegimeEspecial.Cooperativa => "Ato Cooperado (Cooperativa)",
        RegimeEspecial.Estimativa => "Estimativa",
        RegimeEspecial.MicroempresaMunicipal => "Microempresa Municipal",
        RegimeEspecial.NotarioRegistrador => "Notário ou Registrador",
        RegimeEspecial.ProfissionalAutonomo => "Profissional Autônomo",
        RegimeEspecial.SociedadeProfissionais => "Sociedade de Profissionais",
        _ => regime.ToString()
    };

    private string ObterDescricaoAmbienteGerador() => nota.Informacoes.AmbienteGerador switch
    {
        AmbienteGerador.Nacional => "Sefin Nacional NFS-e",
        AmbienteGerador.Prefeitura => "Prefeitura Municipal",
        _ => nota.Informacoes.AmbienteGerador.ToString()
    };

    private string ObterSituacaoNFSe() => nota.Informacoes.SituacaoNFSe switch
    {
        StatusNFSe.Gerada => "NFS-e Gerada",
        StatusNFSe.SubstituicaoGerada => "NFS-e de Substituição Gerada",
        StatusNFSe.DecisaoJudicial => "NFS-e de Decisão Judicial",
        StatusNFSe.Avulsa => "NFS-e Avulsa",
        _ => nota.Informacoes.SituacaoNFSe.ToString()
    };

    private string ObterFinalidade() => nota.Informacoes.Dps.Informacoes.IBSCBS?.FinalidadeNFSe switch
    {
        RTCFinNFSe.Regular => "NFS-e regular",
        _ => "-"
    };

    private static string ObterSimplesNacionalDescricao(OptanteSimplesNacional opcao) => opcao switch
    {
        OptanteSimplesNacional.NaoOptante => "Não Optante",
        OptanteSimplesNacional.OptanteMEEPP => "Optante - ME ou EPP",
        OptanteSimplesNacional.OptanteMEI => "Optante - MEI",
        _ => opcao.ToString()
    };

    private static string ObterDescricaoRegimeApuracao(RegimeApuracao? regime) => regime switch
    {
        RegimeApuracao.TributosFederaisMunicipalSN =>
            "Regime de apuração dos tributos federais e municipal pelo Simples Nacional",
        RegimeApuracao.TributosFederaisSNISSQNPorForaSN =>
            "Regime de apuração dos tributos federais pelo Simples Nacional e ISSQN por fora do Simples Nacional",
        RegimeApuracao.TributosFederaisMunicipalForaSN =>
            "Regime de apuração dos tributos federais e municipal por fora do Simples Nacional",
        _ => "-"
    };

    private bool PossuiTomador() => nota.Informacoes.Dps.Informacoes.Tomador != null;
    private bool PossuiDestinatario() => nota.Informacoes.Dps.Informacoes.IBSCBS?.Destinatario != null;
    private bool PossuiIntermediario() => nota.Informacoes.Dps.Informacoes.Intermediario != null;

    private sealed class LayoutAlturas
    {
        public LayoutAlturas(double alturaDescricaoServicoMm, double alturaInformacoesComplementaresMm)
        {
            AlturaDescricaoServicoMm = alturaDescricaoServicoMm;
            AlturaInformacoesComplementaresMm = alturaInformacoesComplementaresMm;
        }

        public double AlturaDescricaoServicoMm { get; }
        public double AlturaInformacoesComplementaresMm { get; }
    }

    #endregion Helpers de Formatação e Dados
}
