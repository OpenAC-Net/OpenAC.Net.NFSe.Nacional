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
/// Motor de renderização do DANFSe Padrão Nacional em PDF (NT 008 v2.0).
/// </summary>
internal sealed class DANFSeNacionalReport
{
    #region Fields

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

        var page1 = doc.AddPage();
        page1.Size = PageSize.A4;
        page1.Orientation = PageOrientation.Portrait;

        Render(page1, out var servicoExcedente, out var complementaresExcedente);

        var numPagina = 2;
        while (!string.IsNullOrWhiteSpace(servicoExcedente) || !string.IsNullOrWhiteSpace(complementaresExcedente))
        {
            var pageCont = doc.AddPage();
            pageCont.Size = PageSize.A4;
            pageCont.Orientation = PageOrientation.Portrait;

            RenderContinuacao(pageCont, numPagina++, ref servicoExcedente, ref complementaresExcedente);
        }
    }

    public void Render(PdfPage page)
    {
        Render(page, out _, out _);
    }

    public void Render(PdfPage page, out string? servicoExcedente, out string? complementaresExcedente)
    {
        using var gfx = XGraphics.FromPdfPage(page);

        xMm = config.MargemHorizontalMm;
        yMm = config.MargemVerticalMm;
        largUtilMm = DANFSeConstantes.PaginaLarguraMm - (2 * config.MargemHorizontalMm);

        // 1. Desenhar fundos sombreados oficiais (NT 008 / DANFSe v2.0)
        DesenharFundosSombreamento(gfx);

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

        // Destinatário (quando aplicável)
        if (PossuiDestinatario() || DestinatarioEhTomador())
            DesenharBandaDestinatario(gfx);

        // Intermediário (quando aplicável)
        if (PossuiIntermediario())
            DesenharBandaIntermediario(gfx);

        // Serviço Prestado
        DesenharBandaServicoPrestado(gfx, out servicoExcedente);

        // Tributação Municipal (ISSQN)
        DesenharBandaTributacaoMunicipal(gfx);

        // Tributação Federal (Exceto CBS)
        DesenharBandaTributacaoFederal(gfx);

        // Reforma Tributária IBS / CBS (quando aplicável)
        if (PossuiIBSCBS())
            DesenharBandaTributacaoIBSCBS(gfx);

        // Valor Total da NFS-e
        DesenharBandaValorTotalNFSe(gfx);

        // Informações Complementares
        DesenharBandaInformacoesComplementares(gfx, out complementaresExcedente);
    }

    private void DesenharFundosSombreamento(XGraphics gfx)
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
        if (PossuiDestinatario() || DestinatarioEhTomador())
        {
            if (!PossuiDestinatario())
            {
                curY += DANFSeConstantes.AlturaLinhaVaziaMm;
            }
            else
            {
                PdfDrawHelper.DesenharFundoSombreado(gfx, xMm, curY, colW4, lineH);
                curY += 2 * lineH;
            }
        }

        // Intermediário
        if (PossuiIntermediario())
        {
            if (nota.Informacoes.Dps.Informacoes.Intermediario == null)
            {
                curY += DANFSeConstantes.AlturaLinhaVaziaMm;
            }
            else
            {
                PdfDrawHelper.DesenharFundoSombreado(gfx, xMm, curY, colW4, lineH);
                curY += 2 * lineH;
            }
        }

        // Serviço Prestado
        PdfDrawHelper.DesenharFundoSombreado(gfx, xMm, curY, colW4, lineH);
        curY += lineH + 5.2 + 14.0;

        // Tributação Municipal (ISSQN)
        var vNfse = nota.Informacoes.Valores;
        var vDps = nota.Informacoes.Dps.Informacoes.Valores;
        var tribMun = vDps.Tributos.Municipal;
        if (vNfse.ValorISSQN == null && tribMun.Aliquota == null && (vNfse.ValorBc == null || vNfse.ValorBc == 0))
        {
            curY += DANFSeConstantes.AlturaLinhaVaziaMm;
        }
        else
        {
            PdfDrawHelper.DesenharFundoSombreado(gfx, xMm, curY, colW4, lineH);
            curY += 4 * lineH;
        }

        // Tributação Federal (Exceto CBS)
        PdfDrawHelper.DesenharFundoSombreado(gfx, xMm, curY, colW4, lineH);
        curY += 2 * lineH;

        // Tributação IBS / CBS
        if (PossuiIBSCBS())
        {
            PdfDrawHelper.DesenharFundoSombreado(gfx, xMm, curY, colW4, lineH);
            curY += 3 * lineH;
        }

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
        var logoBytes = config.LogoNacional ?? config.LogoPrestador;
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

        var municipio = !string.IsNullOrWhiteSpace(config.CabecalhoMunicipio)
            ? config.CabecalhoMunicipio
            : nota.Informacoes.LocalEmissao;

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
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, curY, colW, lineH, "COMPETÊNCIA", dps.Competencia > DateTime.MinValue ? dps.Competencia.ToString("MM/yyyy") : "-");
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
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW, lineH, "Município / Sigla UF", $"{nota.Informacoes.LocalEmissao} / {end.UF}");
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
        var munNac = end?.Municipio as MunicipioNacional;

        // Linha 1
        PdfDrawHelper.DesenharTituloBlocoInline(gfx, xMm, yMm, colW, lineH, "Tomador / Adquirente");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, yMm, colW, lineH, "CNPJ/CPF/NIF", FormatarCpfCnpj(tomador?.CNPJ ?? tomador?.CPF ?? tomador?.Nif));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW, lineH, "Indicador Municipal (Inscrição)", tomador?.InscricaoMunicipal ?? "-");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 3 * colW, yMm, colW, lineH, "Telefone", FormatarTelefone(tomador?.Telefone));

        yMm += lineH;

        // Linha 2
        PdfDrawHelper.DesenharCampo(gfx, xMm, yMm, colW * 2.0, lineH, "Nome / Nome Empresarial", tomador?.Nome ?? "-");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW, lineH, "Município / Sigla UF", munNac != null ? $"{munNac.CodMunicipio}" : "-");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 3 * colW, yMm, colW, lineH, "Código IBGE / CEP", munNac != null ? $"{munNac.CodMunicipio} / {FormatarCep(munNac.CEP)}" : "-");

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
            var msg = DestinatarioEhTomador() ? DANFSeConstantes.MsgDestIgualTomador : DANFSeConstantes.MsgDestVazio;
            PdfDrawHelper.DesenharLinhaVazia(gfx, xMm, yMm, largUtilMm, DANFSeConstantes.AlturaLinhaVaziaMm, msg);
            yMm += DANFSeConstantes.AlturaLinhaVaziaMm;
            PdfDrawHelper.DesenharLinhaSeparadora(gfx, xMm, yMm, largUtilMm);
            return;
        }

        var dest = nota.Informacoes.Dps.Informacoes.IBSCBS?.Destinatario;
        var end = dest?.Endereco;
        var munNac = end?.Municipio as MunicipioNacional;

        // Linha 1
        PdfDrawHelper.DesenharTituloBlocoInline(gfx, xMm, yMm, colW, lineH, "Destinatário da Operação");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, yMm, colW * 2.0, lineH, "CNPJ/CPF/NIF", FormatarCpfCnpj(dest?.CNPJ ?? dest?.CPF ?? dest?.NIF));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 3 * colW, yMm, colW, lineH, "Telefone", FormatarTelefone(dest?.Telefone));

        yMm += lineH;

        // Linha 2
        PdfDrawHelper.DesenharCampo(gfx, xMm, yMm, colW * 2.0, lineH, "Nome / Nome Empresarial", dest?.Nome ?? "-");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW, lineH, "Município / Sigla UF", munNac != null ? $"{munNac.CodMunicipio}" : "-");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 3 * colW, yMm, colW, lineH, "Código IBGE / CEP", munNac != null ? $"{munNac.CodMunicipio} / {FormatarCep(munNac.CEP)}" : "-");

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

        PdfDrawHelper.DesenharTituloBlocoInline(gfx, xMm, yMm, colW, lineH, "Intermediário da Operação");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, yMm, colW, lineH, "CNPJ/CPF/NIF", FormatarCpfCnpj(interm?.CNPJ ?? interm?.CPF));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW, lineH, "Indicador Municipal", interm?.InscricaoMunicipal ?? "-");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 3 * colW, yMm, colW, lineH, "Telefone", FormatarTelefone(interm?.Telefone));

        yMm += lineH;

        PdfDrawHelper.DesenharCampo(gfx, xMm, yMm, colW * 2.0, lineH, "Nome / Nome Empresarial", interm?.Nome ?? "-");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW * 2.0, lineH, "E-mail", interm?.Email?.ToLower() ?? "-");

        yMm += lineH;
        PdfDrawHelper.DesenharLinhaSeparadora(gfx, xMm, yMm, largUtilMm);
    }

    private void DesenharBandaServicoPrestado(XGraphics gfx, out string? servicoExcedente)
    {
        var colW = largUtilMm / 4.0;
        var lineH = DANFSeConstantes.AlturaLinhaPadraoMm;
        var s = nota.Informacoes.Dps.Informacoes.Servico;
        var info = s.Informacoes;

        // Linha 1
        PdfDrawHelper.DesenharTituloBlocoInline(gfx, xMm, yMm, colW, lineH, "Serviço Prestado");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, yMm, colW, lineH, "Cód. Tributação Nac. / Mun.", $"{info.CodTributacaoNacional} / {info.CodTributacaoMunicipio ?? "-"}");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW, lineH, "Código da NBS", info.CodNBS ?? "-");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 3 * colW, yMm, colW, lineH, "Local da Prestação / UF / País", $"{nota.Informacoes.LocalPrestacao} / BR");

        yMm += lineH;

        // Linha 2: Descrição do Código de Tributação Nacional
        PdfDrawHelper.DesenharCampo(gfx, xMm, yMm, largUtilMm, 5.2, "Descrição do Código de Tributação Nacional", nota.Informacoes.DescricaoTributoNacional);
        yMm += 5.2;

        // Linha 3: Descrição do Serviço (texto livre com quebra e Auto-Fit)
        var hDesc = 14.0;
        PdfDrawHelper.DesenharCampo(gfx, xMm, yMm, largUtilMm, hDesc, "Descrição do Serviço", "");
        var excedente = PdfDrawHelper.DesenharTextoAutoFitComExcedente(
            gfx,
            xMm + 1.0,
            yMm + 3.0,
            largUtilMm - 2.0,
            hDesc - 3.5,
            info.Descricao,
            DANFSeConstantes.FonteConteudoPt,
            5.2);

        servicoExcedente = string.IsNullOrWhiteSpace(excedente) ? null : excedente;

        yMm += hDesc;
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
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW * 2.0, lineH, "Município / Sigla UF / País da Incidência", $"{nota.Informacoes.LocalIncidencia} / BR");

        yMm += lineH;

        // Linha 2
        PdfDrawHelper.DesenharCampo(gfx, xMm + 0 * colW, yMm, colW, lineH, "Regime Especial de Tributação", ObterDescricaoRegimeEspecial(regime.RegimeEspecial));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, yMm, colW, lineH, "Tipo de Imunidade do ISSQN", tribMun.TipoImunidade?.ToString() ?? "-");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW, lineH, "Suspensão da Exigibilidade", tribMun.Suspensao?.TipoSuspensao.ToString() ?? "-");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 3 * colW, yMm, colW, lineH, "Nº Processo Suspensão", tribMun.Suspensao?.NumeroProcesso ?? "-");

        yMm += lineH;

        // Linha 3
        PdfDrawHelper.DesenharCampo(gfx, xMm + 0 * colW, yMm, colW, lineH, "Benefício Municipal", vNfse.TipoBeneficioMunicipal.ToString());
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, yMm, colW, lineH, "Cálculo do BM", FormatarMoeda(vNfse.ValorBcBeneficioMunicipal));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW, lineH, "Total Deduções / Reduções", FormatarMoeda(vDps.ValoresDeducaoReducao?.Valor));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 3 * colW, yMm, colW, lineH, "Desconto Incondicionado", FormatarMoeda(vDps.ValoresDesconto?.ValorIncodicional));

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
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW, lineH, "Contribuição Previdenciária", FormatarMoedaOpcional(tribFed?.ValorCP));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 3 * colW, yMm, colW, lineH, "Contribuições Sociais", FormatarMoedaOpcional(valorContribuicoesSociais));

        yMm += lineH;

        // Linha 2
        PdfDrawHelper.DesenharCampo(gfx, xMm + 0 * colW, yMm, colW, lineH, "", "");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, yMm, colW, lineH, "PIS - Débito de Apuração Própria", FormatarMoedaOpcional(valorPis));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW, lineH, "COFINS - Débito de Apuração", FormatarMoedaOpcional(valorCofins));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 3 * colW, yMm, colW, lineH, "Outras Retenções", "-");

        yMm += lineH;
        PdfDrawHelper.DesenharLinhaSeparadora(gfx, xMm, yMm, largUtilMm);
    }

    private void DesenharBandaTributacaoIBSCBS(XGraphics gfx)
    {
        var colW = largUtilMm / 4.0;
        var lineH = DANFSeConstantes.AlturaLinhaPadraoMm;
        var ibscbs = nota.Informacoes.IBSCBS;
        if (ibscbs == null) return;

        var cstTrib = nota.Informacoes.Dps.Informacoes.IBSCBS?.Valores.Tributos.GrupoIBSCBS;
        var cstTexto = cstTrib != null ? $"{cstTrib.CodigoSituacaoTributaria} / {cstTrib.CodigoClassificacaoTributaria}" : "-";

        // Linha 1
        PdfDrawHelper.DesenharTituloBlocoInline(gfx, xMm, yMm, colW, lineH, "Tributação IBS / CBS");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, yMm, colW, lineH, "CST / Classificação", cstTexto);
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW * 2.0, lineH, "Localidade Incidência / Sigla UF", $"{ibscbs.DescricaoLocalidadeIncidencia} / {ibscbs.CodigoLocalidadeIncidencia}");

        yMm += lineH;

        // Linha 2
        PdfDrawHelper.DesenharCampo(gfx, xMm + 0 * colW, yMm, colW, lineH, "Exclusões e Reduções da BC", FormatarMoeda(ibscbs.Valores.ValorCalcReeRepRes));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, yMm, colW, lineH, "BC Após Exclusões e Reduções", FormatarMoeda(ibscbs.Valores.ValorBaseCalculo));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW, lineH, "Alíquota IBS UF / Mun", $"{FormatarPercentual(ibscbs.Valores.Estado.PercentualIBSUf)} / {FormatarPercentual(ibscbs.Valores.Municipio.PercentualIBSMunicipal)}");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 3 * colW, yMm, colW, lineH, "Alíquota CBS", FormatarPercentual(ibscbs.Valores.Federal.PercentualCBS));

        yMm += lineH;

        // Linha 3
        PdfDrawHelper.DesenharCampo(gfx, xMm + 0 * colW, yMm, colW, lineH, "Valor Total Apurado - IBS", FormatarMoeda(ibscbs.Totais.TotalIBS.ValorTotalIBS));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, yMm, colW, lineH, "Valor Total Apurado - CBS", FormatarMoeda(ibscbs.Totais.TotalCBS.ValorCBS));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW * 2.0, lineH, "Total IBS + CBS", FormatarMoeda(ibscbs.Totais.TotalIBS.ValorTotalIBS + ibscbs.Totais.TotalCBS.ValorCBS));

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
        var vDescIncond = vDps.ValoresDesconto?.ValorIncodicional ?? 0;
        var vDescCond = vDps.ValoresDesconto?.ValorCondicional ?? 0;
        var vRetencoes = vNfse.TotalRetido ?? 0;
        var vLiq = vNfse.ValorLiquido ?? (vServicos - vDescIncond - vRetencoes);
        var totalIBSCBS = (nota.Informacoes.IBSCBS?.Totais.TotalIBS.ValorTotalIBS ?? 0) + (nota.Informacoes.IBSCBS?.Totais.TotalCBS.ValorCBS ?? 0);

        // Linha 1
        PdfDrawHelper.DesenharTituloBlocoInline(gfx, xMm, yMm, colW, lineH, "Valor Total da NFS-e");
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, yMm, colW, lineH, "Valor da Operação / Serviço", FormatarMoeda(vServicos));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW, lineH, "Desconto Incondicionado", FormatarMoeda(vDescIncond));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 3 * colW, yMm, colW, lineH, "Desconto Condicionado", FormatarMoeda(vDescCond));

        yMm += lineH;

        // Linha 2
        PdfDrawHelper.DesenharCampo(gfx, xMm + 0 * colW, yMm, colW, lineH, "Total Retenções (ISS / Federais)", FormatarMoeda(vRetencoes));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 1 * colW, yMm, colW, lineH, "Valor Líquido da NFS-e", FormatarMoeda(vLiq), negrito: true);
        PdfDrawHelper.DesenharCampo(gfx, xMm + 2 * colW, yMm, colW, lineH, "Total IBS / CBS", FormatarMoeda(totalIBSCBS));
        PdfDrawHelper.DesenharCampo(gfx, xMm + 3 * colW, yMm, colW, lineH, "Valor Líquido + IBS/CBS", FormatarMoeda(vLiq + totalIBSCBS), sombreado: true, negrito: true);

        yMm += lineH;
        PdfDrawHelper.DesenharLinhaSeparadora(gfx, xMm, yMm, largUtilMm);
    }

    private void DesenharBandaInformacoesComplementares(XGraphics gfx, out string? complementaresExcedente)
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

        var espacoRestanteMm = (DANFSeConstantes.PaginaAlturaMm - config.MargemVerticalMm) - yMm;
        if (espacoRestanteMm < 15.0) espacoRestanteMm = 15.0;

        var excedente = PdfDrawHelper.DesenharTextoAutoFitComExcedente(
            gfx,
            xMm + 1.0,
            yMm + 1.0,
            largUtilMm - 2.0,
            espacoRestanteMm - 2.0,
            complementares.ToString(),
            DANFSeConstantes.FonteConteudoPt,
            5.2);

        complementaresExcedente = string.IsNullOrWhiteSpace(excedente) ? null : excedente;
    }

    private void RenderContinuacao(PdfPage page, int numPagina, ref string? servicoRestante, ref string? complementaresRestante)
    {
        using var gfx = XGraphics.FromPdfPage(page);

        var xMmCont = config.MargemHorizontalMm;
        var yMmCont = config.MargemVerticalMm;
        var largUtilMmCont = DANFSeConstantes.PaginaLarguraMm - (2 * config.MargemHorizontalMm);

        // 1. Sombreamento do cabeçalho de continuação
        var hCab = 15.0;
        PdfDrawHelper.DesenharFundoSombreado(gfx, xMmCont, yMmCont, largUtilMmCont, hCab);

        // 2. Marca d'água
        DesenharMarcaDagua(gfx);

        // 3. Borda Externa da Página
        var rectBorda = new XRect(
            PdfDrawHelper.MmToPt(xMmCont),
            PdfDrawHelper.MmToPt(yMmCont),
            PdfDrawHelper.MmToPt(largUtilMmCont),
            PdfDrawHelper.MmToPt(DANFSeConstantes.PaginaAlturaMm - (2 * config.MargemVerticalMm)));
        gfx.DrawRectangle(PdfDrawHelper.PenBordaExterna, rectBorda);

        // 4. Cabeçalho de Continuação
        DesenharCabecalhoContinuacao(gfx, xMmCont, yMmCont, largUtilMmCont, hCab, numPagina);
        yMmCont += hCab;
        PdfDrawHelper.DesenharLinhaSeparadora(gfx, xMmCont, yMmCont, largUtilMmCont);

        // 5. Bloco de Continuação da Descrição do Serviço (se houver)
        if (!string.IsNullOrWhiteSpace(servicoRestante))
        {
            var hTitulo = DANFSeConstantes.AlturaTituloBlocoMm;
            PdfDrawHelper.DesenharFundoSombreado(gfx, xMmCont, yMmCont, largUtilMmCont, hTitulo);
            PdfDrawHelper.DesenharTituloBloco(gfx, xMmCont, yMmCont, largUtilMmCont, hTitulo, "CONTINUAÇÃO DA DESCRIÇÃO DO SERVIÇO");
            yMmCont += hTitulo;

            var alturaDisponivelMm = !string.IsNullOrWhiteSpace(complementaresRestante)
                ? ((DANFSeConstantes.PaginaAlturaMm - config.MargemVerticalMm) - yMmCont) / 2.0 - 5.0
                : (DANFSeConstantes.PaginaAlturaMm - config.MargemVerticalMm) - yMmCont - 2.0;

            if (alturaDisponivelMm < 20.0) alturaDisponivelMm = 20.0;

            PdfDrawHelper.DesenharRetanguloSombreado(gfx, xMmCont, yMmCont, largUtilMmCont, alturaDisponivelMm);
            var excedenteServico = PdfDrawHelper.DesenharTextoAutoFitComExcedente(
                gfx,
                xMmCont + 1.0,
                yMmCont + 1.0,
                largUtilMmCont - 2.0,
                alturaDisponivelMm - 2.0,
                servicoRestante!,
                DANFSeConstantes.FonteConteudoPt,
                5.2);

            servicoRestante = string.IsNullOrWhiteSpace(excedenteServico) ? null : excedenteServico;
            yMmCont += alturaDisponivelMm;
            PdfDrawHelper.DesenharLinhaSeparadora(gfx, xMmCont, yMmCont, largUtilMmCont);
        }

        // 6. Bloco de Continuação das Informações Complementares (se houver)
        if (!string.IsNullOrWhiteSpace(complementaresRestante))
        {
            var hTitulo = DANFSeConstantes.AlturaTituloBlocoMm;
            PdfDrawHelper.DesenharFundoSombreado(gfx, xMmCont, yMmCont, largUtilMmCont, hTitulo);
            PdfDrawHelper.DesenharTituloBloco(gfx, xMmCont, yMmCont, largUtilMmCont, hTitulo, "CONTINUAÇÃO DAS INFORMAÇÕES COMPLEMENTARES");
            yMmCont += hTitulo;

            var alturaDisponivelMm = (DANFSeConstantes.PaginaAlturaMm - config.MargemVerticalMm) - yMmCont;
            if (alturaDisponivelMm < 20.0) alturaDisponivelMm = 20.0;

            PdfDrawHelper.DesenharRetanguloSombreado(gfx, xMmCont, yMmCont, largUtilMmCont, alturaDisponivelMm);
            var excedenteCompl = PdfDrawHelper.DesenharTextoAutoFitComExcedente(
                gfx,
                xMmCont + 1.0,
                yMmCont + 1.0,
                largUtilMmCont - 2.0,
                alturaDisponivelMm - 2.0,
                complementaresRestante!,
                DANFSeConstantes.FonteConteudoPt,
                5.2);

            complementaresRestante = string.IsNullOrWhiteSpace(excedenteCompl) ? null : excedenteCompl;
            yMmCont += alturaDisponivelMm;
        }
    }

    private void DesenharCabecalhoContinuacao(XGraphics gfx, double x, double y, double w, double h, int numPagina)
    {
        var chave = ObterChaveAcesso();
        var dps = nota.Informacoes.Dps.Informacoes;
        var emit = nota.Informacoes.Emitente;
        var tomador = dps.Tomador;

        var fontTituloDoc = new XFont(DANFSeConstantes.FontePadrao, 8.5, XFontStyleEx.Bold);
        var fontTexto = new XFont(DANFSeConstantes.FontePadrao, 6.5, XFontStyleEx.Regular);
        var fontBold = new XFont(DANFSeConstantes.FontePadrao, 6.5, XFontStyleEx.Bold);

        // Linha 1: Título e Paginação
        var colW = w / 3.0;
        gfx.DrawString("DANFSe v2.0 - Documento Auxiliar da NFS-e", fontTituloDoc, PdfDrawHelper.BrushPreto,
            new XRect(PdfDrawHelper.MmToPt(x + 1.0), PdfDrawHelper.MmToPt(y + 1.0), PdfDrawHelper.MmToPt(w - 30.0), PdfDrawHelper.MmToPt(4.0)), XStringFormats.TopLeft);

        gfx.DrawString($"Folha {numPagina}", fontBold, PdfDrawHelper.BrushPreto,
            new XRect(PdfDrawHelper.MmToPt(x + w - 29.0), PdfDrawHelper.MmToPt(y + 1.0), PdfDrawHelper.MmToPt(28.0), PdfDrawHelper.MmToPt(4.0)), XStringFormats.TopRight);

        // Linha 2: Chave de Acesso e Dados da Emissão
        var chaveTexto = !string.IsNullOrWhiteSpace(chave) ? $"Chave de Acesso: {FormatarChaveAcesso(chave)}" : $"Nº NFS-e: {nota.Informacoes.NumeroNFSe}";
        var emissaoTexto = $"Série DPS: {dps.Serie}  |  Nº DPS: {dps.NumeroDps}  |  Emissão: {nota.Informacoes.DhProcessamento:dd/MM/yyyy HH:mm:ss}";
        gfx.DrawString($"{chaveTexto}   ({emissaoTexto})", fontBold, PdfDrawHelper.BrushPreto,
            new XRect(PdfDrawHelper.MmToPt(x + 1.0), PdfDrawHelper.MmToPt(y + 5.2), PdfDrawHelper.MmToPt(w - 2.0), PdfDrawHelper.MmToPt(3.5)), XStringFormats.TopLeft);

        // Linha 3: Prestador e Tomador
        var prestadorTexto = $"Prestador: {FormatarCpfCnpj(emit.CNPJ ?? emit.CPF)} - {emit.RazaoSocial}";
        var tomadorNome = tomador != null ? $"{FormatarCpfCnpj(tomador.CNPJ ?? tomador.CPF ?? tomador.Nif)} - {tomador.Nome}" : "Não identificado";
        var tomadorTexto = $"Tomador: {tomadorNome}";

        gfx.DrawString(prestadorTexto, fontTexto, PdfDrawHelper.BrushPreto,
            new XRect(PdfDrawHelper.MmToPt(x + 1.0), PdfDrawHelper.MmToPt(y + 9.5), PdfDrawHelper.MmToPt(colW * 1.5 - 1.0), PdfDrawHelper.MmToPt(4.0)), XStringFormats.TopLeft);

        gfx.DrawString(tomadorTexto, fontTexto, PdfDrawHelper.BrushPreto,
            new XRect(PdfDrawHelper.MmToPt(x + colW * 1.5), PdfDrawHelper.MmToPt(y + 9.5), PdfDrawHelper.MmToPt(colW * 1.5 - 1.0), PdfDrawHelper.MmToPt(4.0)), XStringFormats.TopLeft);
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

    private static string FormatarMoeda(decimal? valor) => (valor ?? 0).ToString("N2", PtBr);
    private static string FormatarPercentual(decimal? valor) => (valor ?? 0).ToString("N2", PtBr) + " %";
    private static string FormatarMoedaOpcional(decimal? valor) => valor?.ToString("N2", PtBr) ?? "-";
    private static string FormatarPercentualOpcional(decimal? valor) => valor == null ? "-" : valor.Value.ToString("N2", PtBr) + " %";

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
        StatusNFSe.Gerada => "Emitida com Sucesso",
        StatusNFSe.SubstituicaoGerada => "Substituída",
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
    private bool DestinatarioEhTomador() => PossuiTomador() && !PossuiDestinatario();
    private bool PossuiIntermediario() => nota.Informacoes.Dps.Informacoes.Intermediario != null;
    private bool PossuiIBSCBS() => nota.Informacoes.IBSCBS != null;

    #endregion Helpers de Formatação e Dados
}
