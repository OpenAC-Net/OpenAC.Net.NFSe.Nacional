using System.Threading;
using System.Threading.Tasks;
using OpenAC.Net.NFSe.Nacional.Common;

namespace OpenAC.Net.NFSe.Nacional.Storage;

/// <summary>Descarta documentos quando a persistência está desabilitada.</summary>
public sealed class NullNFSeDocumentStore : INFSeDocumentStore
{
    /// <summary>Instância compartilhada.</summary>
    public static NullNFSeDocumentStore Instancia { get; } = new();
    private NullNFSeDocumentStore() { }
    /// <inheritdoc />
    public Task SaveAsync(NFSeDocument document, ConfiguracaoNFSe configuration,
        CancellationToken cancellationToken = default) => Task.CompletedTask;
}
