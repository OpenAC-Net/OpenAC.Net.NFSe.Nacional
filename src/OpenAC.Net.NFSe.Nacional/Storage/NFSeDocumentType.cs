namespace OpenAC.Net.NFSe.Nacional.Storage;

/// <summary>Classifica separadamente documentos fiscais e payloads técnicos.</summary>
public enum NFSeDocumentType
{
    /// <summary>Payload técnico enviado ao webservice.</summary>
    SolicitacaoTecnica,

    /// <summary>Payload técnico recebido do webservice.</summary>
    RespostaTecnica,

    /// <summary>Declaração de Prestação de Serviços enviada para emissão.</summary>
    Dps,

    /// <summary>Nota Fiscal de Serviço eletrônica autorizada.</summary>
    NFSe
}
