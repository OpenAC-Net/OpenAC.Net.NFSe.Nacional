using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenAC.Net.NFSe.Nacional.Common;

namespace OpenAC.Net.NFSe.Nacional.Storage;

/// <summary>Armazenamento padrão em filesystem para consumidores desktop.</summary>
public sealed class DesktopNFSeDocumentStore : INFSeDocumentStore
{
    /// <summary>Instância padrão.</summary>
    public static DesktopNFSeDocumentStore Instancia { get; } = new();

    private DesktopNFSeDocumentStore() { }

    /// <inheritdoc />
    public async Task SaveAsync(NFSeDocument document, ConfiguracaoNFSe configuration,
        CancellationToken cancellationToken = default)
    {
        var directory = document.Tipo switch
        {
            NFSeTipoDocumento.SolicitacaoTecnica or NFSeTipoDocumento.RespostaTecnica =>
                configuration.Arquivos.GetPathEnvio(document.Data, document.DocumentoContribuinte ?? string.Empty),
            NFSeTipoDocumento.Dps => configuration.Arquivos.GetPathDps(document.Data, document.DocumentoContribuinte ?? string.Empty),
            NFSeTipoDocumento.NFSe => configuration.Arquivos.GetPathNFSe(document.Data, document.DocumentoContribuinte ?? string.Empty),
            _ => throw new ArgumentOutOfRangeException()
        };
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, document.NomeArquivo);
        var name = Path.GetFileNameWithoutExtension(path);
        var extension = Path.GetExtension(path);
        for (var index = 0; ; index++)
        {
            var candidate = index == 0 ? path : Path.Combine(directory, $"{name}_{index}{extension}");
            try
            {
                using var stream = new FileStream(candidate, FileMode.CreateNew, FileAccess.Write, FileShare.Read,
                    4096, true);
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                await writer.WriteAsync(document.Conteudo).ConfigureAwait(false);
                await writer.FlushAsync().ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                return;
            }
            catch (IOException) when (!document.PreservarNomeArquivo)
            {
                // Outra requisição venceu a criação atômica; tenta o próximo sufixo.
            }
        }
    }
}
