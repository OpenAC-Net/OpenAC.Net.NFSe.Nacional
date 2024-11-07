using System;
using System.Text.Json.Serialization;
using OpenAC.Net.NFSe.Nacional.Common.Converter;
using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

public class DFe
{
    [JsonPropertyName("NSU")]
    public int NSU { get; set; }
    
    [JsonPropertyName("ChaveAcesso")]
    public string ChaveAcesso { get; set; }
    
    [JsonPropertyName("TipoDocumento")]
    [JsonConverter(typeof(JsonStringEnumConverter<TipoDocumento>))]
    public TipoDocumento TipoDocumento { get; set; }
    
    [JsonPropertyName("TipoEvento")]
    [JsonConverter(typeof(JsonStringEnumConverter<TipoEvento>))]
    public TipoEvento TipoEvento { get; set; }
    
    [JsonPropertyName("ArquivoXml")]
    [JsonConverter(typeof(XmlGzipJsonConverter))]
    public string ArquivoXml { get; set; }
    
    [JsonPropertyName("DataHoraRecebimento")]
    public DateTimeOffset DataHoraRecebimento { get; set; }
    
    [JsonPropertyName("DataHoraGeracao")]
    public DateTimeOffset DataHoraGeracao { get; set; }
}