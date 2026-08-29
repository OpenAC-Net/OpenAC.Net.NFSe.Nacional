using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Storage;

namespace OpenAC.Net.NFSe.Nacional.Test;

public class TestDocumentStore
{
    [Test]
    public async Task EscritasConcorrentes_NaoSobrescrevemDocumentos()
    {
        var root = Path.Combine(Path.GetTempPath(), $"openac-nfse-{Guid.NewGuid():N}");
        try
        {
            var configuration = new ConfiguracaoNFSe();
            configuration.Arquivos.PathEnvio = root;
            var document = new NFSeDocument(NFSeDocumentType.SolicitacaoTecnica, "payload", "envio.json",
                null, DateTime.Today);

            await Task.WhenAll(Enumerable.Range(0, 20).Select(_ =>
                DesktopNFSeDocumentStore.Instancia.SaveAsync(document, configuration)));

            await Assert.That(Directory.GetFiles(root, "envio*.json", SearchOption.AllDirectories).Length)
                .IsEqualTo(20);
        }
        finally
        {
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }

    [Test]
    public async Task NullStore_NaoPersisteDocumento()
    {
        var document = new NFSeDocument(NFSeDocumentType.NFSe, "xml", "nota.xml", null, DateTime.Today);
        var task = NullNFSeDocumentStore.Instancia.SaveAsync(document, new ConfiguracaoNFSe());
        await task;
        await Assert.That(task.IsCompletedSuccessfully).IsTrue();
    }
}
