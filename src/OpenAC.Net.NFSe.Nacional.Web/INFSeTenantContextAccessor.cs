using OpenAC.Net.NFSe.Nacional.Common;

namespace OpenAC.Net.NFSe.Nacional.Web;

/// <summary>Consulta a identidade associada a uma configuração sem expô-la nos nomes de arquivo.</summary>
public interface INFSeTenantContextAccessor
{
    /// <summary>Obtém a identidade da empresa associada à configuração.</summary>
    /// <param name="configuration">Configuração cuja empresa associada será consultada.</param>
    /// <returns>Identificador da empresa ou <see cref="NFSeTenant.Padrao"/> quando não houver associação explícita.</returns>
    string GetTenant(ConfiguracaoNFSe configuration);
}
