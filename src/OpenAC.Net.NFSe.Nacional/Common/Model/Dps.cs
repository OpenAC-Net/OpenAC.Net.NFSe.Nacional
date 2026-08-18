// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
// ***********************************************************************
// <copyright file="Dps.cs" company="OpenAC .Net">
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

using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.DFe.Core.Document;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Representa o documento DPS (Documento de Prestação de Serviço) para NFSe Nacional.
/// </summary>
[DFeSignInfoElement("infDPS")]
[DFeRoot("DPS", Namespace = "http://www.sped.fazenda.gov.br/nfse")]
public sealed partial class Dps : DFeSignDocument<Dps>
{
    #region Constructors

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="Dps"/>.
    /// </summary>
    public Dps()
    {
        Signature = new DFeSignature();
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Obtém ou define a versão do layout do DPS.
    /// </summary>
    [DFeAttribute(TipoCampo.Enum, "versao", Ocorrencia = Ocorrencia.Obrigatoria)]
    public VersaoNFSe Versao { get; set; } = VersaoNFSe.Ve100;

    /// <summary>
    /// Obtém ou define as informações do DPS.
    /// </summary>
    [DFeElement("infDPS", Ocorrencia = Ocorrencia.Obrigatoria)]
    public InfDps Informacoes { get; set; } = new();

    #endregion Properties

    #region Methods

    /// <summary>
    /// Assina o documento DPS utilizando as configurações fornecidas.
    /// </summary>
    /// <param name="configuracao">Configuração da NFSe.</param>
    public void Assinar(ConfiguracaoNFSe configuracao)
    {
        var options = DFeSaveOptions.DisableFormatting;
        if (configuracao.Geral.RetirarAcentos)
            options |= DFeSaveOptions.RemoveAccents;

        Assinar(configuracao, options);
    }

    /// <summary>
    /// Assina o documento DPS utilizando as configurações fornecidas e as opções de salvamento especificadas.
    /// </summary>
    /// <param name="configuracao">Configuração da NFSe.</param>
    /// <param name="options">Opções de salvamento do documento.</param>
    public void Assinar(ConfiguracaoNFSe configuracao, DFeSaveOptions options)
    {        
        GerarId();
        AssinarDocumento(configuracao.Certificados.ObterCertificado(), options, false);
    }

    /// <summary>
    /// Gera o identificador da DPS quando ainda não informado. Chamado automaticamente por
    /// <see cref="Assinar(OpenAC.Net.NFSe.Nacional.Common.ConfiguracaoNFSe)"/>; um <see cref="InfDps.Id"/> já preenchido é preservado.
    /// </summary>
    /// <remarks>
    /// Layout <c>TSIdDPS</c> (45 posições): <c>"DPS"</c> + Cód.Mun.(7) + Tipo Insc.(1) +
    /// Insc.Federal(14) + Série(5) + Núm.DPS(15). O tipo 1 (CPF) exige 14 dígitos, com <c>000</c> à
    /// esquerda; o tipo 2 (CNPJ) admite CNPJ alfanumérico. Como todos os campos são
    /// <see cref="string"/>, o preenchimento é explícito - especificador de formato numérico
    /// (<c>D7</c>, <c>D5</c>, <c>D15</c>) não se aplica a <see cref="string"/>.
    /// </remarks>
    public void GerarId()
    {
        if (!Informacoes.Id.IsEmpty()) return;

        var tipo = string.IsNullOrEmpty(Informacoes.Prestador.CNPJ) ? "1" : "2";
        var documento = string.IsNullOrEmpty(Informacoes.Prestador.CPF)
            ? Informacoes.Prestador.CNPJ ?? string.Empty
            : $"000{Informacoes.Prestador.CPF}";

        Informacoes.Id = "DPS" +
                         Informacoes.LocalidadeEmitente.PadLeft(7, '0') +
                         tipo +
                         documento.PadLeft(14, '0') +
                         Informacoes.Serie.PadLeft(5, '0') +
                         Informacoes.NumeroDps.PadLeft(15, '0');
    }

    #endregion Methods
}