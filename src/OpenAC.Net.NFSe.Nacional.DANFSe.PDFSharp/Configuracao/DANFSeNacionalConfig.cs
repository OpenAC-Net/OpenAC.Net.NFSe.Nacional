// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp
// Author           : RFTD / OpenAC.Net Team
// Created          : 2026-08-16
// ***********************************************************************
// <copyright file="DANFSeNacionalConfig.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014-2026 Grupo OpenAC.Net
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.IO;

namespace OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp.Configuracao;

/// <summary>
/// Configurações de impressão do DANFSe Padrão Nacional.
/// </summary>
public sealed class DANFSeNacionalConfig
{
    #region Properties

    /// <summary>
    /// Logotipo da NFS-e Nacional / Prefeitura / Brasão em bytes (PNG/JPG).
    /// </summary>
    public byte[]? LogoNacional { get; set; }

    /// <summary>
    /// Logotipo do Prestador de Serviços em bytes (PNG/JPG).
    /// </summary>
    public byte[]? LogoPrestador { get; set; }

    /// <summary>
    /// Caminho do arquivo de Logotipo do Prestador.
    /// </summary>
    public string? LogoPrestadorPath
    {
        get => null;
        set
        {
            if (!string.IsNullOrEmpty(value) && File.Exists(value))
                LogoPrestador = File.ReadAllBytes(value);
        }
    }

    /// <summary>
    /// Texto sobrescrito para o Município no cabeçalho. Se vazio, utiliza xLocEmi da NFS-e.
    /// </summary>
    public string CabecalhoMunicipio { get; set; } = string.Empty;

    /// <summary>
    /// Indica se deve exibir o canhoto de recebimento no topo da página.
    /// </summary>
    public bool ExibirCanhoto { get; set; } = false;

    /// <summary>
    /// Indica se deve gerar e exibir o QR-Code na identificação da NFS-e.
    /// </summary>
    public bool ExibirQRCode { get; set; } = true;

    /// <summary>
    /// Indica se deve imprimir a marca d'água de homologação / sem validade jurídica.
    /// </summary>
    public bool Homologacao { get; set; } = false;

    /// <summary>
    /// Indica se a nota está cancelada (adiciona marca d'água CANCELADA).
    /// </summary>
    public bool Cancelada { get; set; } = false;

    /// <summary>
    /// Indica se a nota está substituída (adiciona marca d'água SUBSTITUÍDA).
    /// </summary>
    public bool Substituida { get; set; } = false;

    /// <summary>
    /// Margem superior e inferior em milímetros (padrão: 3.0 mm).
    /// </summary>
    public double MargemVerticalMm { get; set; } = 3.0;

    /// <summary>
    /// Margem esquerda e direita em milímetros (padrão: 3.0 mm).
    /// </summary>
    public double MargemHorizontalMm { get; set; } = 3.0;

    /// <summary>
    /// Configurações de segurança e criptografia por senha do PDF.
    /// </summary>
    public DANFSeSegurancaConfig Seguranca { get; set; } = new();

    #endregion Properties
}
