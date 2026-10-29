using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Web.Common;

namespace OpenAC.Net.NFSe.Nacional.Web.Accessor;

/// <summary>Consulta a identidade associada a uma configuração sem expô-la nos nomes de arquivo.</summary>
public interface INFSeTenantContextAccessor
{
    /// <summary>Obtém a identidade da empresa associada à configuração.</summary>
    /// <param name="configuration">Configuração cuja empresa associada será consultada.</param>
    /// <returns>Identificador da empresa ou <see cref="NFSeTenant.Padrao"/> quando não houver associação explícita.</returns>
    string GetTenant(ConfiguracaoNFSe configuration);

    /// <summary>Obtém a empresa associada às regras de armazenamento do componente.</summary>
    string GetTenant(DFeArquivosConfigBase configuration);
}
