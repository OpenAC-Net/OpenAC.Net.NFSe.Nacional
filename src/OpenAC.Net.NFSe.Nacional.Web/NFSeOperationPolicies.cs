namespace OpenAC.Net.NFSe.Nacional.Web;

/// <summary>Disponibiliza a matriz de idempotência das operações do cliente de NFS-e.</summary>
public static class NFSeOperationPolicies
{
    private static readonly HashSet<string> SafeOperations = new(StringComparer.Ordinal)
    {
        nameof(IOpenNFSeNacionalClient.ConsultaNsuAsync),
        nameof(IOpenNFSeNacionalClient.ConsultaChaveAsync),
        nameof(IOpenNFSeNacionalClient.DownloadDANFSeAsync),
        nameof(IOpenNFSeNacionalClient.ConsultaChaveDpsAsync),
        nameof(IOpenNFSeNacionalClient.ConsultaExisteDpsAsync),
        nameof(IOpenNFSeNacionalClient.ConsultarLoteAsync),
        nameof(IOpenNFSeNacionalClient.ConsultarNFSeAsync)
    };

    /// <summary>Indica se uma operação pode ser repetida automaticamente.</summary>
    /// <param name="operation">Nome do método definido em <see cref="IOpenNFSeNacionalClient"/>.</param>
    /// <returns><see langword="true"/> quando a operação não altera estado fiscal; caso contrário, <see langword="false"/>.</returns>
    public static bool IsIdempotent(string operation) => SafeOperations.Contains(operation);
}
