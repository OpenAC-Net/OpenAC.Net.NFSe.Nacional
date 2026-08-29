using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Storage;
using OpenAC.Net.NFSe.Nacional.Web.Accessor;
using OpenAC.Net.NFSe.Nacional.Web.Configuration;

namespace OpenAC.Net.NFSe.Nacional.Web.Store;

/// <summary>Persiste payloads técnicos e documentos fiscais no sistema de arquivos do host ASP.NET Core.</summary>
/// <remarks>
/// A raiz é resolvida a partir de <see cref="IHostEnvironment.ContentRootPath"/>. Os documentos são
/// separados por empresa e categoria, e colisões de nome são tratadas pelo armazenamento desktop.
/// </remarks>
public sealed class FileSystemNFSeDocumentStore : INFSeDocumentStore
{
    private readonly string root;
    private readonly bool enabled;
    private readonly INFSeTenantContextAccessor tenantContext;

    /// <summary>Cria o armazenamento usando a raiz do host e as opções registradas.</summary>
    /// <param name="environment">Ambiente do host usado para resolver o diretório raiz da aplicação.</param>
    /// <param name="options">Opções que controlam a persistência e o caminho relativo dos documentos.</param>
    /// <param name="tenantContext">Contexto usado para isolar os documentos por empresa.</param>
    public FileSystemNFSeDocumentStore(IHostEnvironment environment, IOptions<OpenNFSeNacionalWebOptions> options,
        INFSeTenantContextAccessor tenantContext)
    {
        enabled = options.Value.PersistenciaHabilitada;
        root = Path.GetFullPath(Path.Combine(environment.ContentRootPath, options.Value.CaminhoDocumentos));
        this.tenantContext = tenantContext;
    }

    /// <inheritdoc />
    public Task SaveAsync(NFSeDocument document, ConfiguracaoNFSe configuration,
        CancellationToken cancellationToken = default)
    {
        if (!enabled) return Task.CompletedTask;
        var tenantHash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(tenantContext.GetTenant(configuration))));
        var tenantRoot = Path.Combine(root, "Tenants", tenantHash);
        configuration.Arquivos.PathEnvio = Path.Combine(tenantRoot, "Technical");
        configuration.Arquivos.PathDps = Path.Combine(tenantRoot, "Fiscal", "DPS");
        configuration.Arquivos.PathNFSe = Path.Combine(tenantRoot, "Fiscal", "NFSe");
        return DesktopNFSeDocumentStore.Instancia.SaveAsync(document, configuration, cancellationToken);
    }
}
