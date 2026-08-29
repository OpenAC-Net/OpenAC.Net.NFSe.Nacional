namespace OpenAC.Net.NFSe.Nacional.Web;

/// <summary>Cria clientes de NFS-e com configuração, certificado e pool HTTP isolados por empresa.</summary>
public interface IOpenNFSeNacionalClientFactory
{
    /// <summary>Cria um cliente usando a configuração e o certificado atuais da empresa.</summary>
    /// <param name="tenantId">Identificador estável da empresa no sistema consumidor.</param>
    /// <param name="cancellationToken">Token para cancelar o carregamento da configuração ou do certificado.</param>
    /// <returns>Cliente pronto para executar operações em nome da empresa.</returns>
    /// <exception cref="ArgumentException">O identificador da empresa é nulo, vazio ou contém apenas espaços.</exception>
    /// <exception cref="InvalidOperationException">O certificado está expirado ou dentro da margem mínima de expiração.</exception>
    ValueTask<IOpenNFSeNacionalClient> CreateAsync(string tenantId = NFSeTenant.Padrao,
        CancellationToken cancellationToken = default);
}
