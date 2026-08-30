using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using OpenAC.Net.NFSe.Nacional.Common.Converter;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Representa a resposta da consulta de evento.
/// </summary>
public sealed class RespostaConsultaEvento : RespostaBase
{
    /// <summary>
    /// Lista de eventos.
    /// </summary>
    [JsonPropertyName("eventos")]
    public List<Evento> Eventos { get; set; } = [];

    /// <summary>
    /// Representa um evento consultado
    /// </summary>
    public class Evento
    {
        /// <summary>
        /// Tipo de evento.
        /// </summary>
        [JsonPropertyName("chaveAcesso")]
        public string ChaveAcesso { get; set; } = string.Empty;

        /// <summary>
        /// Chave de acesso.
        /// </summary>
        [JsonPropertyName("tipoEvento")]
        public int TipoEvento { get; set; }

        /// <summary>
        /// Número do pedido de registro do evento.
        /// </summary>
        [JsonPropertyName("numeroPedidoRegistroEvento")]
        public int NumeroPedidoRegistroEvento { get; set; }

        /// <summary>
        /// Data e hora do recebimento.
        /// </summary>
        [JsonPropertyName("dataHoraRecebimento")]
        public DateTime DataHoraRecebimento { get; set; }

        /// <summary>
        /// Obtém ou define o XML do evento compactado em GZip e codificado em Base64.
        /// </summary>
        [JsonPropertyName("arquivoXml")]
        [JsonConverter(typeof(XmlGzipJsonConverter))]
        public string XmlEvento { get; set; } = string.Empty;
    }
}