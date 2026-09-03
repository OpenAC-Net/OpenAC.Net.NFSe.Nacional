// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : OpenAC .Net
// ***********************************************************************
// <copyright file="GovBRDto.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014-2026 Grupo OpenAC.Net
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

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAC.Net.NFSe.Nacional.Webservice.GovBR;

/// <summary>
/// DTOs do contrato JSON exposto pela API "ISS DIGITAL" da GovernancaBrasil (ex.: Jacarezinho/PR).
/// Não seguem o formato usado pelo provedor Nacional (<see cref="Common.Model.DpsEnvio"/>/
/// <see cref="Common.Model.RespostaEnvioDps"/>) - o envio é sempre em lote (mesmo para um único
/// documento) e as notas/eventos processados vêm todos dentro do mesmo array <c>lote</c>,
/// por isso ficam isolados aqui e são traduzidos manualmente pelo <see cref="GovBRWebService"/>.
/// </summary>
internal sealed class EnvioLoteDto
{
    /// <summary>XMLs (DPS e/ou pedido de registro de evento) compactados em GZip e em Base64. Máximo de 50 itens.</summary>
    [JsonPropertyName("loteXmlGZipB64")]
    public List<string> LoteXmlGZipB64 { get; set; } = new();

    /// <summary>Dados extras (ex.: e-mails de notificação) por item do lote. Enviado vazio quando não usado.</summary>
    [JsonPropertyName("dadosExtras")]
    public List<ItemDadosExtrasDto> DadosExtras { get; set; } = new();
}

/// <summary>Dados extras de um item do lote (notificação por e-mail).</summary>
internal sealed class ItemDadosExtrasDto
{
    /// <summary>Id da DPS ou do pedido de registro de evento relacionado.</summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>E-mails a notificar (máximo 10).</summary>
    [JsonPropertyName("emails")]
    public List<string> Emails { get; set; } = new();
}

/// <summary>Retorno do processamento de um lote (<c>EnviarSincrono</c>, <c>EnviarAssincrono</c> ou <c>ConsultarProtocolo</c>).</summary>
internal sealed class RetornoLoteDto
{
    /// <summary>Resultado do processamento de cada item do lote.</summary>
    [JsonPropertyName("lote")]
    public List<RetornoItemLoteDto> Lote { get; set; } = new();

    /// <summary>Ambiente do processamento ("PRODUCAO" ou "HOMOLOGACAO").</summary>
    [JsonPropertyName("tipoAmbiente")]
    public string? TipoAmbiente { get; set; }

    /// <summary>Versão do software ISS Digital instalado na Prefeitura.</summary>
    [JsonPropertyName("versaoAplicativo")]
    public string? VersaoAplicativo { get; set; }

    /// <summary>Data/hora do processamento do lote no servidor.</summary>
    [JsonPropertyName("dataHoraProcessamento")]
    public string? DataHoraProcessamento { get; set; }

    /// <summary>UUID do protocolo (envio assíncrono ou consulta de protocolo).</summary>
    [JsonPropertyName("protocolo")]
    public string? Protocolo { get; set; }

    /// <summary>Indica se o processamento do lote já foi concluído no servidor.</summary>
    [JsonPropertyName("processado")]
    public bool Processado { get; set; }
}

/// <summary>Resultado do processamento de um item (DPS ou evento) dentro do lote.</summary>
internal sealed class RetornoItemLoteDto
{
    /// <summary>Chave da DPS (45 dígitos) ou do evento (62 dígitos) que originou este item.</summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>Chave de acesso da NFS-e gerada (50 caracteres).</summary>
    [JsonPropertyName("chaveAcesso")]
    public string ChaveAcesso { get; set; } = string.Empty;

    /// <summary>Status do processamento ("SUCESSO" ou "ERRO").</summary>
    [JsonPropertyName("statusProcessamento")]
    public string StatusProcessamento { get; set; } = string.Empty;

    /// <summary>XML gerado (NFS-e ou evento), compactado em GZip e em Base64.</summary>
    [JsonPropertyName("xmlGZipB64")]
    public string XmlGZipB64 { get; set; } = string.Empty;

    /// <summary>Código de autenticidade da nota (9 caracteres).</summary>
    [JsonPropertyName("codAutenticidade")]
    public string? CodAutenticidade { get; set; }

    /// <summary>Erros ocorridos no processamento deste item.</summary>
    [JsonPropertyName("erros")]
    public List<MsgDto> Erros { get; set; } = new();
}

/// <summary>Mensagem de erro retornada pela API.</summary>
internal sealed class MsgDto
{
    /// <summary>Código do erro.</summary>
    [JsonPropertyName("codigo")]
    public string Codigo { get; set; } = string.Empty;

    /// <summary>Descrição do erro.</summary>
    [JsonPropertyName("descricao")]
    public string Descricao { get; set; } = string.Empty;
}

/// <summary>Retorno de <c>ConsultarNFSe</c> e <c>ConsultarDPS</c>.</summary>
internal sealed class RetornoConsultaDto
{
    /// <summary>Quantidade total de registros da pesquisa, independente da paginação.</summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }

    /// <summary>Notas retornadas na pesquisa.</summary>
    [JsonPropertyName("notas")]
    public List<DadosNotaDto> Notas { get; set; } = new();
}

/// <summary>Dados de uma NFS-e retornada por consulta.</summary>
internal sealed class DadosNotaDto
{
    /// <summary>Chave da NFS-e (50 dígitos).</summary>
    [JsonPropertyName("chave")]
    public string Chave { get; set; } = string.Empty;

    /// <summary>XML da NFS-e, compactado em GZip e em Base64.</summary>
    [JsonPropertyName("xmlGZipB64")]
    public string XmlGZipB64 { get; set; } = string.Empty;

    /// <summary>Situação da nota ("NORMAL", "CANCELADA" ou "SUBSTITUIDA").</summary>
    [JsonPropertyName("situacao")]
    public string? Situacao { get; set; }

    /// <summary>Código de autenticidade da nota (9 caracteres).</summary>
    [JsonPropertyName("codAutenticidade")]
    public string? CodAutenticidade { get; set; }

    /// <summary>Número da nota substituída por esta, quando aplicável.</summary>
    [JsonPropertyName("numeroNotaSubstituida")]
    public long NumeroNotaSubstituida { get; set; }

    /// <summary>Número da nota que substituiu esta, quando aplicável.</summary>
    [JsonPropertyName("numeroNotaSubstituidora")]
    public long NumeroNotaSubstituidora { get; set; }
}

/// <summary>Item retornado por <c>ConsultarEventos</c>.</summary>
internal sealed class DadosEventoDto
{
    /// <summary>Chave do evento (59 dígitos).</summary>
    [JsonPropertyName("chave")]
    public string Chave { get; set; } = string.Empty;

    /// <summary>Código do tipo de evento (6 dígitos), conforme manual da NFS-e Nacional.</summary>
    [JsonPropertyName("tipo")]
    public string Tipo { get; set; } = string.Empty;

    /// <summary>Data/hora do registro do evento.</summary>
    [JsonPropertyName("dataInclusao")]
    public string? DataInclusao { get; set; }

    /// <summary>XML do evento, compactado em GZip e em Base64.</summary>
    [JsonPropertyName("xmlGZipB64")]
    public string XmlGZipB64 { get; set; } = string.Empty;
}

/// <summary>Retorno de <c>DownloadPDFChave</c>/<c>DownloadPDFNumero</c>.</summary>
internal sealed class RetornoPdfDto
{
    /// <summary>Conteúdo do PDF do DANFSe, compactado em GZip e em Base64.</summary>
    [JsonPropertyName("pdfGZipB64")]
    public string PdfGZipB64 { get; set; } = string.Empty;
}
