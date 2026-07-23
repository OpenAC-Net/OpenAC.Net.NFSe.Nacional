using System.Collections.Generic;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Classe base para respostas de provedores baseados em SOAP/XML (ex.: Fiorilli).
/// Diferente de <see cref="RespostaBase"/> (orientada a JSON), concentra as mensagens
/// de processamento em uma única lista, refletindo o elemento <c>ListaMensagens</c> do WSDL.
/// </summary>
public abstract class RespostaSoapBase
{
    /// <summary>
    /// Mensagens de processamento retornadas pelo provedor (alertas e/ou erros).
    /// </summary>
    public List<MensagemProcessamento> Mensagens { get; set; } = new();
}
