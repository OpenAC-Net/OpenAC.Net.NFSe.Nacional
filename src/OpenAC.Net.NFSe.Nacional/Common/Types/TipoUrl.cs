namespace OpenAC.Net.NFSe.Nacional.Common.Types;

/// <summary>
/// Tipos de URL utilizadas pelos webservices da NFSe Nacional.
/// </summary>
public enum TipoUrl
{
    /// <summary>Enviar lote RPS ou NFSe.</summary>
    Enviar,

    /// <summary>Enviar evento relacionado a uma NFSe (ex.: cancelamento).</summary>
    EnviarEvento,

    /// <summary>Consultar por NSU (Número Sequencial Único).</summary>
    ConsultarNsu,

    /// <summary>Consultar NFSe por chave.</summary>
    ConsultarChave,

    /// <summary>Download do DANFSe (Documento Auxiliar da NFSe).</summary>
    DownloadDanfse,

    /// <summary>Consultar NFSe por chave do DPS (Documento de Prestação de Serviços).</summary>
    ConsultarChaveDps,

    /// <summary>Verificar existência do DPS.</summary>
    ConsultaExisteDps
}