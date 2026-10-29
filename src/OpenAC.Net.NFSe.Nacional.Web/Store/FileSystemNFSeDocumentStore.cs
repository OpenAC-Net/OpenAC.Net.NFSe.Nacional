using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.DFe.Core.Storage;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Web.Accessor;
using OpenAC.Net.NFSe.Nacional.Web.Configuration;

namespace OpenAC.Net.NFSe.Nacional.Web.Store;

/// <summary>Resolve diretórios por empresa e delega a persistência ao DFe.Core.</summary>
public sealed class FileSystemNFSeDocumentStore : IDFeDocumentStore
{
    private readonly string root;
    private readonly bool enabled;
    private readonly INFSeTenantContextAccessor tenantContext;

    /// <summary>Cria o armazenamento relativo à raiz do host, isolado por empresa.</summary>
    public FileSystemNFSeDocumentStore(IHostEnvironment environment, IOptions<OpenNFSeNacionalWebOptions> options,
        INFSeTenantContextAccessor tenantContext)
    {
        enabled = options.Value.PersistenciaHabilitada;
        root = Path.GetFullPath(Path.Combine(environment.ContentRootPath, options.Value.CaminhoDocumentos));
        this.tenantContext = tenantContext;
    }

    /// <inheritdoc />
    public Task SaveAsync(DFeDocument document, DFeArquivosConfigBase configuration,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!enabled) return Task.CompletedTask;
        var tenantHash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(tenantContext.GetTenant(configuration))));
        var tenantRoot = Path.Combine(root, "Tenants", tenantHash);
        string directory;
        if (configuration is NFSeArquivoConfig source)
        {
            // Resolve a raiz do host sem alterar os caminhos da configuração compartilhada.
            var files = new NFSeArquivoConfig
            {
                PathEnvio = Path.Combine(tenantRoot, "Technical"),
                PathDps = Path.Combine(tenantRoot, "Fiscal", "DPS"),
                PathNFSe = Path.Combine(tenantRoot, "Fiscal", "NFSe"),
                AdicionarLiteral = source.AdicionarLiteral,
                SepararPorCNPJ = source.SepararPorCNPJ,
                SepararPorModelo = source.SepararPorModelo,
                SepararPorAno = source.SepararPorAno,
                SepararPorMes = source.SepararPorMes,
                SepararPorDia = source.SepararPorDia
            };
            lock (source.OrdenacaoPath) files.OrdenacaoPath.AddRange(source.OrdenacaoPath);
            directory = files.GetDocumentPath(document);
        }
        else
        {
            directory = Path.Combine(tenantRoot, document.Tipo.ToString(), document.Data.ToString("yyyyMM"));
        }
        return DFeFileSystemDocumentStore.SaveToDirectoryAsync(document, directory, cancellationToken);
    }
}
