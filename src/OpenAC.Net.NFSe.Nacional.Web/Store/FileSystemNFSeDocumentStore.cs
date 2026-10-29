using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenAC.Net.DFe.Core.Storage;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Storage;
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
    public Task SaveAsync(IDFeDocument document,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document);
        cancellationToken.ThrowIfCancellationRequested();
        if (!enabled) return Task.CompletedTask;
        if (document is not NFSeDocument nfse)
            throw new ArgumentException("O armazenamento web requer NFSeDocument para identificar a empresa e as regras fiscais.", nameof(document));
        var tenant = tenantContext.GetTenant(nfse.Arquivos);
        var tenantHash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(tenant)));
        var tenantRoot = Path.Combine(root, "Tenants", tenantHash);
        var source = nfse.Arquivos;
        // Resolve a raiz do host sem alterar os caminhos da configuração compartilhada.
        var files = new NFSeArquivoConfig
        {
            PathSalvar = tenantRoot,
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
        var directory = files.GetDocumentPath(nfse.Categoria, nfse.DocumentoContribuinte, nfse.Data);
        var nomeArquivo = nfse.NomeArquivo;
        var destino = new DFeDocument(Path.Combine(directory, nomeArquivo), document.Conteudo, document.ArquivoExiste);
        return DFeFileSystemDocumentStore.Instance.SaveAsync(destino, cancellationToken);
    }
}
