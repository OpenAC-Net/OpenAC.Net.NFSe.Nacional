using System.Text.Json.Serialization;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>Códigos que identificam um serviço parametrizado pelo município.</summary>
public sealed class ServicoMunicipal
{
    /// <summary>Código de tributação nacional do serviço.</summary>
    [JsonPropertyName("cTribNac")]
    public string CodigoTributacaoNacional { get; set; } = string.Empty;

    /// <summary>Código de tributação municipal do serviço, quando definido.</summary>
    [JsonPropertyName("cTribMun")]
    public string? CodigoTributacaoMunicipal { get; set; }
}