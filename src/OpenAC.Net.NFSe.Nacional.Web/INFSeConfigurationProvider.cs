namespace OpenAC.Net.NFSe.Nacional.Web;

/// <summary>Define o contrato para obter uma configuração independente para cada empresa.</summary>
/// <typeparam name="TConfiguration">Tipo da configuração fornecida.</typeparam>
public interface INFSeConfigurationProvider<TConfiguration>
{
    /// <summary>Obtém a configuração para a empresa informada.</summary>
    /// <param name="tenantId">Identificador estável da empresa no sistema consumidor.</param>
    /// <param name="cancellationToken">Token para cancelar a consulta da configuração.</param>
    /// <returns>Configuração que pode ser usada exclusivamente pela operação solicitada.</returns>
    ValueTask<TConfiguration> GetConfigurationAsync(string tenantId = NFSeTenant.Padrao,
        CancellationToken cancellationToken = default);
}
