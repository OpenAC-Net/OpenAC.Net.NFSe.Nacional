using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>Resultado da consulta de serviços parametrizados pelo município.</summary>
public sealed class RespostaConsultaServicosMunicipais : RespostaBase
{
    internal static readonly JsonSerializerOptions OpcoesJson = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new ConversorResposta() }
    };

    private static readonly JsonSerializerOptions OpcoesObjeto = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>Serviços parametrizados na competência consultada.</summary>
    [JsonPropertyName("servicos")]
    public List<ServicoMunicipal> Servicos { get; set; } = [];

    private sealed class ConversorResposta : JsonConverter<RespostaConsultaServicosMunicipais>
    {
        public override RespostaConsultaServicosMunicipais? Read(ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                return JsonSerializer.Deserialize<RespostaConsultaServicosMunicipais>(ref reader, OpcoesObjeto);

            return new RespostaConsultaServicosMunicipais
            {
                Servicos = JsonSerializer.Deserialize<List<ServicoMunicipal>>(ref reader, OpcoesObjeto)!
            };
        }

        public override void Write(Utf8JsonWriter writer, RespostaConsultaServicosMunicipais value,
            JsonSerializerOptions options) => JsonSerializer.Serialize(writer, value, OpcoesObjeto);
    }
}