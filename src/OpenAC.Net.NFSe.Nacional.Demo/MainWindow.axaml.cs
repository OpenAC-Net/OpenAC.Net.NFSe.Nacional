using System.Xml.Linq;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using NLog;
using NLog.Config;
using NLog.Targets;
using OpenAC.Net.NFSe.Nacional.Common.Model;

namespace OpenAC.Net.NFSe.Nacional.Demo;

public partial class MainWindow : Window
{
    private readonly OpenNFSeNacional client = new();
    private readonly IOpenNFSeNacionalClient serviceClient;
    private CancellationTokenSource? operation;
    private bool closeAfterOperation;
    private bool closed;
    private bool initialized;
    private readonly List<LoadedDocument> documents = [];
    private OpenConfig configuration = null!;
    private string configurationPath = "";
    private string catalogPath = "";

    public MainWindow() : this(null)
    {
    }

    internal MainWindow(string? configurationFile, IOpenNFSeNacionalClient? serviceClient = null)
    {
        this.serviceClient = serviceClient ?? client;
        InitializeComponent();
        InitializeSettings(configurationFile);
        InitializeLogging();
        ConnectEvents();
        initialized = true;
        UpdateContext();
        RefreshServices();
    }

    private void ConnectEvents()
    {
        Navigation.SelectionChanged += (_, _) =>
        {
            OperationsPage.IsVisible = Navigation.SelectedIndex == 0;
            SettingsPage.IsVisible = Navigation.SelectedIndex == 1;
            ServicesPage.IsVisible = Navigation.SelectedIndex == 2;
        };
        OpenXmlButton.Click += async (_, _) => await RunAsync("Carregar XML", LoadDocumentsAsync);
        ValidateButton.Click += async (_, _) => await RunAsync("Validar XML", ValidateDocumentsAsync);
        SignButton.Click += async (_, _) => await RunAsync("Assinar XML", SignDocumentsAsync);
        SaveXmlButton.Click += async (_, _) => await RunAsync("Salvar XML", SaveDocumentAsync);
        SendButton.Click += async (_, _) => await RunAsync("Enviar DPS", async token =>
            ShowResponse(
                await WithServiceTimeoutAsync(ct => serviceClient.EnviarAsync(RequireSingle<Dps>(), ct), token)));
        SendEventButton.Click += async (_, _) => await RunAsync("Enviar evento", async token =>
            ShowResponse(await WithServiceTimeoutAsync(
                ct => serviceClient.EnviarEventoAsync(RequireSingle<PedidoRegistroEvento>(), ct), token)));
        SendBatchButton.Click += async (_, _) => await RunAsync("Enviar lote", async token =>
            ShowResponse(await WithServiceTimeoutAsync(ct => serviceClient.EnviarLoteAsync(CreateBatch(), ct), token)));
        SendSyncBatchButton.Click += async (_, _) => await RunAsync("Enviar lote síncrono", async token =>
            ShowResponse(await WithServiceTimeoutAsync(ct => serviceClient.EnviarLoteSincronoAsync(CreateBatch(), ct),
                token)));
        QueryButton.Click += async (_, _) => await RunAsync("Consultar chave", async token =>
            ShowResponse(await WithServiceTimeoutAsync(ct => serviceClient.ConsultaChaveAsync(RequireKey(), ct),
                token)));
        DpsKeyButton.Click += async (_, _) => await RunAsync("Consultar chave pela DPS", async token =>
            ShowResponse(await WithServiceTimeoutAsync(ct => serviceClient.ConsultaChaveDpsAsync(RequireKey(), ct),
                token)));
        DpsExistsButton.Click += async (_, _) => await RunAsync("Verificar DPS", async token =>
            ResultText.Text =
                await WithServiceTimeoutAsync(ct => serviceClient.ConsultaExisteDpsAsync(RequireKey(), ct), token)
                    ? "DPS encontrada."
                    : "DPS não encontrada.");
        NsuButton.Click += async (_, _) => await RunAsync("Consultar NSU", QueryNsuAsync);
        QueryBatchButton.Click += async (_, _) => await RunAsync("Consultar lote", QueryBatchAsync);
        QueryNfseButton.Click += async (_, _) => await RunAsync("Consultar NFS-e", QueryNfseAsync);
        QueryEventButton.Click += async (_, _) => await RunAsync("Consultar evento", QueryEventAsync);
        DownloadButton.Click += async (_, _) => await RunAsync("Baixar DANFSe", DownloadPdfAsync);
        PdfButton.Click += async (_, _) => await RunAsync("Gerar DANFSe", GeneratePdfAsync);
        CancelOperationButton.Click += (_, _) => operation?.Cancel();
        CopyButton.Click += async (_, _) => await RunAsync("Copiar retorno", async _ =>
        {
            TextBox[] editors = [ResultText, SentXmlText, ReceivedXmlText, TransportText, LogText];
            if (Clipboard != null)
                await Clipboard.SetTextAsync(editors[Math.Max(0, ResponseTabs.SelectedIndex)].Text ?? "");
        });
        ClearButton.Click += (_, _) => ResultText.Text = SentXmlText.Text =
            ReceivedXmlText.Text = TransportText.Text = LogText.Text = LastOperationText.Text = "";
        Closing += (_, args) =>
        {
            if (operation == null) return;
            args.Cancel = true;
            closeAfterOperation = true;
            operation.Cancel();
        };
        Closed += (_, _) =>
        {
            closed = true;
            LogManager.Shutdown();
        };
        ConnectSettingsEvents();
        ConnectServicesEvents();
    }

    private async Task RunAsync(string name, Func<CancellationToken, Task> action)
    {
        if (operation != null) return;
        using var cancellation = new CancellationTokenSource();
        operation = cancellation;
        Workspace.IsEnabled = Navigation.IsEnabled = false;
        BusyProgress.IsVisible = CancelOperationButton.IsVisible = true;
        StatusText.Text = name + "…";
        AppendLog(name);
        try
        {
            ApplyConfiguration();
            await action(cancellation.Token);
            StatusText.Text = "Pronto para operar";
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
            StatusText.Text = "Operação cancelada";
            AppendLog("Operação cancelada pelo usuário.");
        }
        catch (Exception exception)
        {
            StatusText.Text = "Não foi possível concluir a operação";
            ResultText.Text = exception.Message;
            ResponseTabs.SelectedIndex = 0;
            AppendLog($"{name}: {exception.Message}");
        }
        finally
        {
            LastOperationText.Text = $"{name} · {DateTime.Now:HH:mm:ss}";
            Workspace.IsEnabled = Navigation.IsEnabled = true;
            BusyProgress.IsVisible = CancelOperationButton.IsVisible = false;
            operation = null;
            if (closeAfterOperation) Close();
        }
    }

    private void ShowResponse<T>(NFSeResponse<T> response) where T : class, new()
    {
        SentXmlText.Text = Pretty(response.XmlEnvio);
        ReceivedXmlText.Text = Pretty(response.XmlRetorno);
        TransportText.Text = $"ENVIO\n\n{Pretty(response.JsonEnvio)}\n\nRETORNO\n\n{Pretty(response.JsonRetorno)}";
        ResultText.Text = (response.Sucesso ? "Sucesso" : "Falha no processamento") + "\n\n" +
                          (response.JsonRetorno is { Length: > 0 } json ? Pretty(json) : Pretty(response.XmlRetorno));
        ResponseTabs.SelectedIndex = 0;
        AppendLog(
            response.Sucesso ? "Operação concluída com sucesso." : "O serviço retornou falha. Consulte o retorno.");
    }

    private static string Pretty(string? content)
    {
        if (string.IsNullOrWhiteSpace(content)) return "";
        try
        {
            return XDocument.Parse(content).ToString();
        }
        catch (System.Xml.XmlException)
        {
        }

        try
        {
            using var json = JsonDocument.Parse(content);
            return JsonSerializer.Serialize(json, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (JsonException)
        {
            return content;
        }
    }

    private void AppendLog(string message)
    {
        if (!closed)
            LogText.Text = string.Join('\n',
                (LogText.Text + $"{DateTime.Now:HH:mm:ss}  {message}\n").Split('\n').TakeLast(500));
    }

    private void InitializeLogging()
    {
        var config = new LoggingConfiguration();
        config.AddRule(LogLevel.Debug, LogLevel.Fatal,
            new DemoLogTarget(line => Dispatcher.UIThread.Post(() => AppendLog(line))) { Layout = "${message}" });
        LogManager.Configuration = config;
    }

    private async Task<IStorageFile?> PickFileAsync(string title, string name, params string[] patterns)
        => (await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title, FileTypeFilter = [new FilePickerFileType(name) { Patterns = patterns }]
        })).FirstOrDefault();
}

internal sealed class DemoLogTarget(Action<string> write) : TargetWithLayout
{
    protected override void Write(LogEventInfo logEvent) => write(Layout.Render(logEvent));
}