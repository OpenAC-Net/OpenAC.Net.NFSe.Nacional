using System;
using System.Text.Json.Serialization;
using OpenAC.Net.NFSe.Nacional.Common.Converter;
using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Representa um Documento Fiscal Eletrônico (DFe) utilizado no sistema.
/// </summary>
public class DFe
{
    /// <summary>
    /// Número Sequencial Único do documento.
    /// </summary>
    [JsonPropertyName("NSU")]
    public int NSU { get; set; }
    
    /// <summary>
    /// Chave de acesso do documento.
    /// </summary>
    [JsonPropertyName("ChaveAcesso")]
    public string ChaveAcesso { get; set; } = string.Empty;
    
    /// <summary>
    /// Tipo do documento fiscal eletrônico.
    /// </summary>
    [JsonPropertyName("TipoDocumento")]
    [JsonConverter(typeof(JsonStringEnumConverter<TipoDocumento>))]
    public TipoDocumento TipoDocumento { get; set; }
    
    /// <summary>
    /// Tipo do evento relacionado ao documento.
    /// </summary>
    [JsonPropertyName("TipoEvento")]
    [JsonConverter(typeof(JsonStringEnumConverter<TipoEvento>))]
    public TipoEvento TipoEvento { get; set; }
    
    /// <summary>
    /// Conteúdo do arquivo XML, possivelmente compactado.
    /// </summary>
    [JsonPropertyName("ArquivoXml")]
    [JsonConverter(typeof(XmlGzipJsonConverter))]
    public string ArquivoXml { get; set; } = string.Empty;
    
    /// <summary>
    /// Data e hora do recebimento do documento.
    /// </summary>
    [JsonPropertyName("DataHoraRecebimento")]
    public DateTimeOffset DataHoraRecebimento { get; set; }
    
    /// <summary>
    /// Data e hora da geração do documento.
    /// </summary>
    [JsonPropertyName("DataHoraGeracao")]
    public DateTimeOffset DataHoraGeracao { get; set; }
}