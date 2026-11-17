using System.Globalization;
using System.Xml.Linq;
using Avalonia.Platform.Storage;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp;

namespace OpenAC.Net.NFSe.Nacional.Demo;

public partial class MainWindow
{
    private async Task LoadDocumentsAsync(CancellationToken token)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Carregar DPS, NFS-e ou pedido de evento", AllowMultiple = true,
            FileTypeFilter = [new FilePickerFileType("Documentos XML") { Patterns = ["*.xml"] }]
        });
        if (files.Count == 0) return;
        var loaded = new List<LoadedDocument>();
        foreach (var file in files)
        {
            await using var stream = await file.OpenReadAsync();
            using var reader = new StreamReader(stream);
            loaded.Add(LoadedDocument.Parse(file.Name, await reader.ReadToEndAsync(token)));
        }

        documents.Clear();
        documents.AddRange(loaded);
        DocumentNameText.Text = string.Join(", ", documents.Select(document => document.Name));
        DocumentCountText.Text = $"{documents.Count} XML carregado(s)";
        ResultText.Text = string.Join('\n', documents.Select(document => $"{document.Name} · {document.Kind}"));
        ReceivedXmlText.Text = Pretty(documents[0].Xml);
        ResponseTabs.SelectedIndex = 0;
    }

    private T RequireSingle<T>() where T : class
    {
        if (documents.Count != 1 || documents[0].Value is not T result)
            throw new InvalidOperationException($"Carregue um único XML do tipo {typeof(T).Name} para esta operação.");
        EnsureDocumentContext(result);
        return result;
    }

    private void EnsureDocumentContext(object document)
    {
        var config = client.Configuracoes;
        var context = document switch
        {
            Dps dps => (dps.Versao, dps.Informacoes.TipoAmbiente),
            PedidoRegistroEvento evento => (evento.Versao, evento.Informacoes.TipoAmbiente),
            _ => (config.Geral.Versao, config.WebServices.Ambiente)
        };
        if (context.Item1 != config.Geral.Versao || context.Item2 != config.WebServices.Ambiente)
            throw new InvalidOperationException(
                "A versão e o ambiente do XML devem coincidir com as configurações do demo.");
    }

    private LoteDps CreateBatch()
    {
        if (documents.Count == 0 || documents.Any(document => document.Value is not Dps))
            throw new InvalidOperationException("Carregue um ou mais XMLs de DPS para enviar um lote.");
        var (cnpj, cpf) = GetTaxId();
        if (!long.TryParse(BatchText.Text, out var number) || number < 1)
            throw new ArgumentException("Informe um número de lote maior que zero nas configurações.");
        foreach (var document in documents) EnsureDocumentContext(document.Value);
        return new LoteDps
        {
            NumeroLote = number, CNPJ = cnpj, CPF = cpf, IM = RegistrationText.Text?.Trim(),
            Documentos = documents.Select(document => (Dps)document.Value).ToList()
        };
    }

    private async Task ValidateDocumentsAsync(CancellationToken token)
    {
        if (documents.Count == 0) throw new InvalidOperationException("Carregue um XML para validar.");
        var schemasPath = client.Configuracoes.Arquivos.PathSchemas;
        var loaded = documents.ToArray();
        await Task.Run(() =>
        {
            foreach (var document in loaded)
            {
                token.ThrowIfCancellationRequested();
                var file = document.SchemaFile;
                var schemaPath = Path.Combine(schemasPath, file);
                if (!File.Exists(schemaPath))
                    throw new FileNotFoundException(
                        "Schema não encontrado. Configure a pasta dos XSDs da versão do XML.", schemaPath);
                if (!XmlSchemaValidation.ValidarXml(document.Xml, schemaPath, out var errors, out _))
                    throw new InvalidOperationException(document.Name + "\n" +
                                                        string.Join(Environment.NewLine, errors));
            }
        }, token);
        ResultText.Text = $"{loaded.Length} XML(s) validado(s) com sucesso.";
        ResponseTabs.SelectedIndex = 0;
    }

    private async Task SignDocumentsAsync(CancellationToken token)
    {
        if (documents.Count == 0 || documents.Any(document => document.Value is not (Dps or PedidoRegistroEvento)))
            throw new InvalidOperationException("Carregue DPS ou pedidos de evento para assinar.");
        foreach (var document in documents) EnsureDocumentContext(document.Value);
        await Task.Run(() =>
        {
            foreach (var document in documents)
            {
                token.ThrowIfCancellationRequested();
                switch (document.Value)
                {
                    case Dps dps: dps.Assinar(client.Configuracoes); break;
                    case PedidoRegistroEvento evento: evento.Assinar(client.Configuracoes); break;
                }
            }
        }, token);
        SentXmlText.Text = Pretty(documents[0].Xml);
        ResultText.Text = "Documentos assinados. Use Salvar XML para exportar um documento.";
        ResponseTabs.SelectedIndex = 0;
    }

    private async Task SaveDocumentAsync(CancellationToken token)
    {
        if (documents.Count != 1) throw new InvalidOperationException("Carregue um único XML para salvar.");
        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Salvar documento XML", SuggestedFileName = documents[0].Name, DefaultExtension = "xml",
            FileTypeChoices = [new FilePickerFileType("XML") { Patterns = ["*.xml"] }]
        });
        if (file == null) return;
        await using var stream = await file.OpenWriteAsync();
        stream.SetLength(0);
        await using var writer = new StreamWriter(stream);
        await writer.WriteAsync(documents[0].Xml.AsMemory(), token);
        ResultText.Text = "XML salvo em " + file.Name;
    }

    private string RequireKey()
    {
        var key = AccessKeyText.Text?.Trim();
        return !string.IsNullOrWhiteSpace(key)
            ? key
            : throw new ArgumentException("Informe a chave de acesso ou o identificador da DPS.");
    }

    private (string? Cnpj, string? Cpf) GetTaxId()
    {
        var id = string.Concat((TaxIdText.Text ?? "").Where(char.IsAsciiLetterOrDigit)).ToUpperInvariant();
        return id.Length switch
        {
            14 => (id, null),
            11 when id.All(char.IsAsciiDigit) => (null, id),
            _ => throw new ArgumentException(
                "Informe o CPF (11 dígitos) ou CNPJ (14 caracteres) do prestador nas configurações.")
        };
    }

    private async Task QueryNsuAsync(CancellationToken token)
    {
        var fields = await InputDialog.AskAsync(this, "Consultar distribuição por NSU",
            new InputField("nsu", "NSU inicial", "0"));
        if (fields == null) return;
        if (!int.TryParse(fields["nsu"], NumberStyles.None, CultureInfo.InvariantCulture, out var nsu) || nsu < 0)
            throw new ArgumentException("Informe um NSU inteiro maior ou igual a zero.");
        ShowResponse(await WithServiceTimeoutAsync(ct => serviceClient.ConsultaNsuAsync(nsu, ct), token));
    }

    private async Task QueryBatchAsync(CancellationToken token)
    {
        var fields = await InputDialog.AskAsync(this, "Consultar lote", new InputField("protocol", "Protocolo"));
        if (fields == null) return;
        var (cnpj, cpf) = GetTaxId();
        ShowResponse(await WithServiceTimeoutAsync(ct => serviceClient.ConsultarLoteAsync(new ConsultaLoteFiltro
        {
            Protocolo = fields["protocol"], CNPJ = cnpj, CPF = cpf, IM = RegistrationText.Text?.Trim()
        }, ct), token));
    }

    private async Task QueryNfseAsync(CancellationToken token)
    {
        var fields = await InputDialog.AskAsync(this, "Consultar NFS-e",
            new InputField("key", "Chave NFS-e (opcional)", AccessKeyText.Text ?? "", false),
            new InputField("dps", "Identificador DPS (opcional)", "", false),
            new InputField("number", "Número DPS (opcional)", "", false),
            new InputField("series", "Série DPS (opcional)", "", false));
        if (fields == null) return;
        if (fields.Values.All(string.IsNullOrWhiteSpace))
            throw new ArgumentException("Informe pelo menos um critério de consulta.");
        var (cnpj, cpf) = GetTaxId();
        ShowResponse(await WithServiceTimeoutAsync(ct => serviceClient.ConsultarNFSeAsync(new ConsultaNFSeFiltro
        {
            CNPJ = cnpj, CPF = cpf, IM = RegistrationText.Text?.Trim(),
            ChaveNFSe = NullIfEmpty(fields["key"]), IdDPS = NullIfEmpty(fields["dps"]),
            NumeroDPS = NullIfEmpty(fields["number"]), SerieDPS = NullIfEmpty(fields["series"])
        }, ct), token));
    }

    private async Task QueryEventAsync(CancellationToken token)
    {
        var key = RequireKey();
        var fields = await InputDialog.AskAsync(this, "Consultar evento",
            new InputField("type", "Código do evento", "101101"),
            new InputField("sequence", "Sequência do evento", "1"));
        if (fields == null) return;
        if (!int.TryParse(fields["sequence"], out var sequence) || sequence < 1)
            throw new ArgumentException("Informe uma sequência de evento maior que zero.");
        ShowResponse(
            await WithServiceTimeoutAsync(ct => serviceClient.ConsultaEventoAsync(key, fields["type"], sequence, ct),
                token));
    }

    private async Task DownloadPdfAsync(CancellationToken token)
    {
        var key = RequireKey();
        var file = await PickPdfDestinationAsync("DANFSe-" + key + ".pdf");
        if (file == null) return;
        var pdf = await WithServiceTimeoutAsync(ct => serviceClient.DownloadDANFSeAsync(key, ct), token);
        await WritePdfAsync(file, pdf, token);
    }

    private async Task GeneratePdfAsync(CancellationToken token)
    {
        var invoice = RequireSingle<NotaFiscalServico>();
        var file = await PickPdfDestinationAsync("DANFSe.pdf");
        if (file == null) return;
        var pdf = await Task.Run(() => new OpenDANFSeNacional().GerarPDF(invoice), token);
        await WritePdfAsync(file, pdf, token);
    }

    private async Task<IStorageFile?> PickPdfDestinationAsync(string name) => await StorageProvider.SaveFilePickerAsync(
        new FilePickerSaveOptions
        {
            Title = "Salvar DANFSe", SuggestedFileName = name, DefaultExtension = "pdf",
            FileTypeChoices = [new FilePickerFileType("PDF") { Patterns = ["*.pdf"] }]
        });

    private async Task WritePdfAsync(IStorageFile file, byte[] pdf, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        await using var stream = await file.OpenWriteAsync();
        stream.SetLength(0);
        await stream.WriteAsync(pdf, token);
        ResultText.Text = $"DANFSe salvo em {file.Name} ({pdf.Length:N0} bytes).";
        ResponseTabs.SelectedIndex = 0;
    }

    private async Task<T> WithServiceTimeoutAsync<T>(Func<CancellationToken, Task<T>> request, CancellationToken token)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(token);
        timeout.CancelAfter(TimeSpan.FromSeconds((double)(TimeoutInput.Value ?? 100)));
        try
        {
            return await request(timeout.Token);
        }
        catch (OperationCanceledException) when (!token.IsCancellationRequested && timeout.IsCancellationRequested)
        {
            throw new TimeoutException(
                "O serviço excedeu o tempo limite configurado. Consulte o documento antes de reenviar.");
        }
    }

    private static string? NullIfEmpty(string value) => string.IsNullOrWhiteSpace(value) ? null : value;
}

internal sealed class LoadedDocument(string name, string xml, object value, string kind, string schemaFile)
{
    public string Name { get; } = name;

    public string Xml => Value switch
    {
        Dps dps when !string.IsNullOrWhiteSpace(dps.Xml) => dps.Xml,
        PedidoRegistroEvento evento when !string.IsNullOrWhiteSpace(evento.Xml) => evento.Xml,
        _ => xml
    };

    public object Value { get; } = value;
    public string Kind { get; } = kind;
    public string SchemaFile { get; } = schemaFile;

    public static LoadedDocument Parse(string name, string xml)
    {
        var root = XDocument.Parse(xml).Root ?? throw new ArgumentException("XML vazio.");
        var version = (string?)root.Attribute("versao");
        if (version is not ("1.00" or "1.01"))
            throw new ArgumentException($"{name}: versão não suportada. Use 1.00 ou 1.01.");
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xml));
        return root.Name.LocalName switch
        {
            "DPS" => new(name, xml, Dps.Load(stream), "DPS", $"DPS_v{version}.xsd"),
            "NFSe" => new(name, xml, NotaFiscalServico.Load(stream), "NFS-e", $"NFSe_v{version}.xsd"),
            "pedRegEvento" => new(name, xml, PedidoRegistroEvento.Load(stream), "Pedido de evento",
                $"pedRegEvento_v{version}.xsd"),
            _ => throw new ArgumentException($"{name}: carregue uma DPS, NFS-e ou pedRegEvento.")
        };
    }
}