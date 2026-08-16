// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp
// Author           : RFTD / OpenAC.Net Team
// Created          : 2026-08-16
// ***********************************************************************
// <copyright file="PdfDrawHelper.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014-2026 Grupo OpenAC.Net
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.IO;
using System.Text;
using PdfSharp.Drawing;
using QRCoder;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp.Common;

/// <summary>
/// Utilitários de desenho em PDF (conversão mm -> pt, caixas com label/valor, quebra de texto, QR Code).
/// </summary>
internal static class PdfDrawHelper
{
    #region Cores e Pincéis

    public static readonly XColor CorCinzaClaro = XColor.FromArgb(242, 242, 242);
    public static readonly XColor CorCinzaBorda = XColor.FromArgb(200, 200, 200);
    public static readonly XColor CorCinzaMarcaDagua = XColor.FromArgb(215, 215, 215);
    public static readonly XColor CorVermelho = XColor.FromArgb(237, 28, 36);

    public static readonly XPen PenBordaExterna = new(XColors.Black, 0.75);
    public static readonly XPen PenBordaLinha = new(CorCinzaBorda, 0.35);
    public static readonly XPen PenLinhaSeparadora = new(XColors.Black, 0.5);

    public static readonly XBrush BrushFundoSombreado = new XSolidBrush(CorCinzaClaro);
    public static readonly XBrush BrushFundoBranco = new XSolidBrush(XColors.White);
    public static readonly XBrush BrushPreto = new XSolidBrush(XColors.Black);
    public static readonly XBrush BrushCinzaEscuro = new XSolidBrush(XColor.FromArgb(70, 70, 70));

    #endregion Cores e Pincéis

    #region Conversões de Unidade

    public static double MmToPt(double mm) => mm * 72.0 / 25.4;

    #endregion Conversões de Unidade

    #region Métodos de Desenho

    public static void DesenharRetanguloSombreado(XGraphics gfx, double xMm, double yMm, double wMm, double hMm)
    {
        var rect = new XRect(MmToPt(xMm), MmToPt(yMm), MmToPt(wMm), MmToPt(hMm));
        gfx.DrawRectangle(PenBordaLinha, BrushFundoSombreado, rect);
    }

    public static void DesenharTituloBlocoInline(XGraphics gfx, double xMm, double yMm, double wMm, double hMm, string titulo)
    {
        var rect = new XRect(MmToPt(xMm), MmToPt(yMm), MmToPt(wMm), MmToPt(hMm));
        gfx.DrawRectangle(PenBordaLinha, BrushFundoSombreado, rect);

        var font = new XFont(DANFSeConstantes.FontePadrao, DANFSeConstantes.FonteTituloBlocoPt, XFontStyleEx.Bold);
        var format = new XStringFormat
        {
            Alignment = XStringAlignment.Center,
            LineAlignment = XLineAlignment.Center
        };
        gfx.DrawString(titulo, font, BrushPreto, rect, format);
    }

    public static void DesenharTituloBloco(XGraphics gfx, double xMm, double yMm, double wMm, double hMm, string titulo)
    {
        var rect = new XRect(MmToPt(xMm), MmToPt(yMm), MmToPt(wMm), MmToPt(hMm));
        gfx.DrawRectangle(PenBordaLinha, BrushFundoSombreado, rect);

        var font = new XFont(DANFSeConstantes.FontePadrao, DANFSeConstantes.FonteTituloBlocoPt, XFontStyleEx.Bold);
        var format = new XStringFormat
        {
            Alignment = XStringAlignment.Near,
            LineAlignment = XLineAlignment.Center
        };
        var textRect = new XRect(rect.X + MmToPt(1.0), rect.Y, rect.Width - MmToPt(2.0), rect.Height);
        gfx.DrawString(titulo, font, BrushPreto, textRect, format);
    }

    public static void DesenharCampo(
        XGraphics gfx,
        double xMm,
        double yMm,
        double wMm,
        double hMm,
        string label,
        string valor,
        bool sombreado = false,
        bool negrito = false,
        XStringAlignment alinhamento = XStringAlignment.Near)
    {
        var rect = new XRect(MmToPt(xMm), MmToPt(yMm), MmToPt(wMm), MmToPt(hMm));
        var brushFundo = sombreado ? BrushFundoSombreado : BrushFundoBranco;
        gfx.DrawRectangle(PenBordaLinha, brushFundo, rect);

        var paddingLeftPt = MmToPt(0.8);
        var paddingRightPt = MmToPt(0.8);
        var innerWidthPt = rect.Width - (paddingLeftPt + paddingRightPt);

        // Label superior
        if (!string.IsNullOrEmpty(label))
        {
            var fontLabel = new XFont(DANFSeConstantes.FontePadrao, DANFSeConstantes.FonteLabelCampoPt, XFontStyleEx.Bold);
            var labelRect = new XRect(rect.X + paddingLeftPt, rect.Y + MmToPt(0.4), innerWidthPt, MmToPt(2.2));
            gfx.DrawString(label, fontLabel, BrushCinzaEscuro, labelRect, XStringFormats.TopLeft);
        }

        // Valor inferior
        var valorTexto = string.IsNullOrWhiteSpace(valor) ? "-" : valor.Trim();
        var styleValor = negrito ? XFontStyleEx.Bold : XFontStyleEx.Regular;
        var fontValor = new XFont(DANFSeConstantes.FontePadrao, DANFSeConstantes.FonteConteudoPt, styleValor);

        var posYMm = string.IsNullOrEmpty(label) ? 0.8 : 2.6;
        var valorRect = new XRect(rect.X + paddingLeftPt, rect.Y + MmToPt(posYMm), innerWidthPt, rect.Height - MmToPt(posYMm) - MmToPt(0.3));

        var format = new XStringFormat
        {
            Alignment = alinhamento,
            LineAlignment = XLineAlignment.Near
        };

        // Truncar ou formatar texto caso ultrapasse a largura
        var textoAjustado = TruncarTexto(gfx, valorTexto, fontValor, innerWidthPt);
        gfx.DrawString(textoAjustado, fontValor, BrushPreto, valorRect, format);
    }

    public static void DesenharLinhaVazia(XGraphics gfx, double xMm, double yMm, double wMm, double hMm, string mensagem)
    {
        var rect = new XRect(MmToPt(xMm), MmToPt(yMm), MmToPt(wMm), MmToPt(hMm));
        gfx.DrawRectangle(PenBordaLinha, BrushFundoSombreado, rect);

        var font = new XFont(DANFSeConstantes.FontePadrao, DANFSeConstantes.FonteAmbientePt, XFontStyleEx.Italic);
        var format = new XStringFormat
        {
            Alignment = XStringAlignment.Center,
            LineAlignment = XLineAlignment.Center
        };
        gfx.DrawString(mensagem, font, BrushCinzaEscuro, rect, format);
    }

    public static void DesenharLinhaSeparadora(XGraphics gfx, double xMm, double yMm, double wMm)
    {
        var x1 = MmToPt(xMm);
        var y1 = MmToPt(yMm);
        var x2 = MmToPt(xMm + wMm);
        gfx.DrawLine(PenLinhaSeparadora, x1, y1, x2, y1);
    }

    public static void DesenharTextoMultiLinhas(
        XGraphics gfx,
        double xMm,
        double yMm,
        double wMm,
        double hMm,
        string texto,
        double fontSizePt = DANFSeConstantes.FonteConteudoPt,
        bool negrito = false)
    {
        if (string.IsNullOrWhiteSpace(texto)) return;

        var font = new XFont(DANFSeConstantes.FontePadrao, fontSizePt, negrito ? XFontStyleEx.Bold : XFontStyleEx.Regular);
        var lineHeightPt = fontSizePt * 1.25;
        var maxWidthPt = MmToPt(wMm);
        var maxHeightPt = MmToPt(hMm);
        var startXPt = MmToPt(xMm);
        var currentYPt = MmToPt(yMm) + (fontSizePt * 0.9);

        var lines = texto.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

        foreach (var rawLine in lines)
        {
            if (currentYPt > MmToPt(yMm) + maxHeightPt)
                break;

            if (string.IsNullOrEmpty(rawLine))
            {
                currentYPt += lineHeightPt;
                continue;
            }

            var words = rawLine.Split(' ');
            var currentLine = new StringBuilder();

            foreach (var word in words)
            {
                var testLine = currentLine.Length == 0 ? word : $"{currentLine} {word}";
                var size = gfx.MeasureString(testLine, font);

                if (size.Width <= maxWidthPt)
                {
                    currentLine.Clear();
                    currentLine.Append(testLine);
                }
                else
                {
                    if (currentLine.Length > 0)
                    {
                        gfx.DrawString(currentLine.ToString(), font, BrushPreto, startXPt, currentYPt);
                        currentYPt += lineHeightPt;

                        if (currentYPt > MmToPt(yMm) + maxHeightPt)
                            break;
                    }

                    currentLine.Clear();
                    currentLine.Append(word);
                }
            }

            if (currentLine.Length > 0 && currentYPt <= MmToPt(yMm) + maxHeightPt)
            {
                gfx.DrawString(currentLine.ToString(), font, BrushPreto, startXPt, currentYPt);
                currentYPt += lineHeightPt;
            }
        }
    }

    public static void DesenharQrCode(XGraphics gfx, double xMm, double yMm, double sizeMm, string conteudo)
    {
        if (string.IsNullOrWhiteSpace(conteudo)) return;

        try
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(conteudo, QRCodeGenerator.ECCLevel.M);
            var matrix = qrCodeData.ModuleMatrix;
            var moduleCount = matrix.Count;
            if (moduleCount == 0) return;

            var totalSizePt = MmToPt(sizeMm);
            var moduleSizePt = totalSizePt / moduleCount;
            var startXPt = MmToPt(xMm);
            var startYPt = MmToPt(yMm);

            for (var row = 0; row < moduleCount; row++)
            {
                var rowBits = matrix[row];
                for (var col = 0; col < rowBits.Length; col++)
                {
                    if (rowBits[col])
                    {
                        gfx.DrawRectangle(BrushPreto, startXPt + (col * moduleSizePt), startYPt + (row * moduleSizePt), moduleSizePt + 0.05, moduleSizePt + 0.05);
                    }
                }
            }
        }
        catch
        {
            // Ignora se não conseguir desenhar o QR
        }
    }

    public static void DesenharImagem(XGraphics gfx, double xMm, double yMm, double maxWMm, double maxHMm, byte[]? imagemBytes)
    {
        if (imagemBytes == null || imagemBytes.Length == 0) return;

        try
        {
            using var ms = new MemoryStream(imagemBytes);
            using var xImage = XImage.FromStream(ms);

            var maxWidthPt = MmToPt(maxWMm);
            var maxHeightPt = MmToPt(maxHMm);

            var ratioW = maxWidthPt / xImage.PixelWidth;
            var ratioH = maxHeightPt / xImage.PixelHeight;
            var ratio = Math.Min(ratioW, ratioH);

            var destWidth = xImage.PixelWidth * ratio;
            var destHeight = xImage.PixelHeight * ratio;

            var posX = MmToPt(xMm) + (maxWidthPt - destWidth) / 2.0;
            var posY = MmToPt(yMm) + (maxHMm > 0 ? (maxHeightPt - destHeight) / 2.0 : 0);

            gfx.DrawImage(xImage, posX, posY, destWidth, destHeight);
        }
        catch
        {
            // Imagem inválida ou não suportada
        }
    }

    public static void DesenharMarcaDagua(XGraphics gfx, double larguraPaginaMm, double alturaPaginaMm, string texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) return;

        var state = gfx.Save();
        var centroX = MmToPt(larguraPaginaMm / 2.0);
        var centroY = MmToPt(alturaPaginaMm / 2.0);

        gfx.TranslateTransform(centroX, centroY);
        gfx.RotateTransform(-35);

        var font = new XFont(DANFSeConstantes.FontePadrao, DANFSeConstantes.FonteMarcaDaguaPt, XFontStyleEx.Bold);
        var brush = new XSolidBrush(CorCinzaMarcaDagua);
        var format = new XStringFormat
        {
            Alignment = XStringAlignment.Center,
            LineAlignment = XLineAlignment.Center
        };

        gfx.DrawString(texto, font, brush, 0, 0, format);
        gfx.Restore(state);
    }

    private static string TruncarTexto(XGraphics gfx, string texto, XFont font, double maxWidthPt)
    {
        var size = gfx.MeasureString(texto, font);
        if (size.Width <= maxWidthPt) return texto;

        var ellipsis = "...";
        var ellipsisWidth = gfx.MeasureString(ellipsis, font).Width;
        if (ellipsisWidth >= maxWidthPt) return string.Empty;

        var len = texto.Length;
        while (len > 1 && (gfx.MeasureString(texto.Substring(0, len), font).Width + ellipsisWidth) > maxWidthPt)
        {
            len--;
        }

        return texto.Substring(0, len) + ellipsis;
    }

    #endregion Métodos de Desenho
}
