namespace OpenAC.Net.NFSe.Nacional.Web.Configuration;

/// <summary>Configura limites específicos dos métodos da integração ASP.NET Core. As políticas comuns pertencem ao Core.</summary>
public sealed class OpenNFSeNacionalWebOptions
{
    /// <summary>Obtém os timeouts específicos indexados pelo nome do método do cliente.</summary>
    public IDictionary<string, TimeSpan> TimeoutsPorOperacao { get; } = new Dictionary<string, TimeSpan>();
}