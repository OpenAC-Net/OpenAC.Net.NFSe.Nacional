using Avalonia.Controls;
using Avalonia.Platform.Storage;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Nacional.Common.Types;
using OpenAC.Net.NFSe.Nacional.Webservice;

namespace OpenAC.Net.NFSe.Nacional.Demo;

public partial class MainWindow
{
    private string DataDirectory => Path.GetDirectoryName(configurationPath)!;

    private void InitializeSettings(string? configurationFile)
    {
        configurationPath = configurationFile ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OpenAC.Net", "NFSe.Nacional.Demo", "nfse.config");
        Directory.CreateDirectory(DataDirectory);
        configuration = OpenConfig.CreateOrLoad(configurationPath);
        ConfigurationPathText.Text = "Configuração local: " + configurationPath;
        Fill(EnvironmentCombo, new[] { DFeTipoAmbiente.Homologacao, DFeTipoAmbiente.Producao }, configuration.Get("Ambiente", DFeTipoAmbiente.Homologacao), value => value == DFeTipoAmbiente.Homologacao ? "Homologação" : "Produção");
        Fill(VersionCombo, Enum.GetValues<VersaoNFSe>(), configuration.Get("Versao", VersaoNFSe.Ve101), value => value == VersaoNFSe.Ve101 ? "1.01" : "1.00");
        MunicipalityInput.Value = configuration.Get("Municipio", 5002704);
        TimeoutInput.Value = Math.Clamp(configuration.Get("Timeout", 100), 1, 600);
        CertificatePathText.Text = configuration.Get("Certificado", "");
        CertificateSerialText.Text = configuration.Get("NumeroSerie", "");
        RememberPasswordCheck.IsChecked = configuration.Get("LembrarSenha", false);
        try { CertificatePasswordText.Text = configuration.GetCrypt("Senha", ""); } catch { CertificatePasswordText.Text = ""; }
        TaxIdText.Text = configuration.Get("Documento", "");
        RegistrationText.Text = configuration.Get("IM", "");
        SchemasPathText.Text = configuration.Get("Schemas", "");
        DocumentsPathText.Text = configuration.Get("Documentos", Path.Combine(DataDirectory, "Documentos"));
        ExchangePathText.Text = configuration.Get("Envios", Path.Combine(DataDirectory, "Envios"));
        BatchText.Text = configuration.Get("Lote", "1");
        SaveDocumentsCheck.IsChecked = configuration.Get("SalvarXML", false);
        SaveExchangeCheck.IsChecked = configuration.Get("SalvarEnvios", false);
        SignXmlCheck.IsChecked = configuration.Get("Assinar", true);
        ValidateSchemasCheck.IsChecked = configuration.Get("Validar", true);
        catalogPath = Path.Combine(DataDirectory, "Municipios.nfse");
        if (File.Exists(catalogPath))
        {
            try { NFSeServiceManager.Instance.Load(catalogPath); }
            catch (Exception exception) { ResultText.Text = "Não foi possível carregar o catálogo local: " + exception.Message; }
        }
    }

    private void ConnectSettingsEvents()
    {
        EnvironmentCombo.SelectionChanged += (_, _) => { if (initialized) UpdateContext(); };
        VersionCombo.SelectionChanged += (_, _) => { if (initialized) UpdateContext(); };
        MunicipalityInput.ValueChanged += (_, _) => { if (initialized) UpdateContext(); };
        BrowseCertificatePathButton.Click += async (_, _) => await RunAsync("Selecionar certificado", async _ =>
        {
            if (await PickFileAsync("Selecionar certificado", "Certificado PFX/P12", "*.pfx", "*.p12") is { } file)
                CertificatePathText.Text = file.TryGetLocalPath() ?? throw new InvalidOperationException("Selecione um arquivo local.");
        });
        BrowseSchemasPathButton.Click += async (_, _) => await SelectFolderAsync(SchemasPathText);
        BrowseDocumentsPathButton.Click += async (_, _) => await SelectFolderAsync(DocumentsPathText);
        BrowseExchangePathButton.Click += async (_, _) => await SelectFolderAsync(ExchangePathText);
        SaveSettingsButton.Click += async (_, _) => await RunAsync("Salvar configurações", _ =>
        {
            SaveConfiguration();
            StatusText.Text = "Configurações salvas";
            return Task.CompletedTask;
        });
    }

    private void ApplyConfiguration()
    {
        var config = client.Configuracoes;
        config.Geral.Versao = Selected<VersaoNFSe>(VersionCombo);
        config.Geral.AssinarXml = SignXmlCheck.IsChecked == true;
        config.WebServices.Ambiente = Selected<DFeTipoAmbiente>(EnvironmentCombo);
        config.WebServices.CodigoMunicipio = (int)(MunicipalityInput.Value ?? 5002704);
        config.WebServices.InscricaoMunicipal = RegistrationText.Text?.Trim();
        config.WebServices.ValidarSchemas = ValidateSchemasCheck.IsChecked == true;
        config.Certificados.Certificado = !string.IsNullOrWhiteSpace(CertificatePathText.Text) ? CertificatePathText.Text.Trim() : CertificateSerialText.Text?.Trim() ?? "";
        config.Certificados.Senha = CertificatePasswordText.Text ?? "";
        var versionFolder = config.Geral.Versao == VersaoNFSe.Ve101 ? "1.01" : "1.00";
        config.Arquivos.PathSchemas = string.IsNullOrWhiteSpace(SchemasPathText.Text) ? Path.Combine(AppContext.BaseDirectory, "Schemas", versionFolder) : SchemasPathText.Text.Trim();
        config.Arquivos.VersaoSchema = config.Geral.Versao;
        config.Arquivos.PathNFSe = DocumentsPathText.Text ?? "";
        config.Arquivos.PathDps = Path.Combine(config.Arquivos.PathNFSe, "DPS");
        config.Arquivos.PathEnvio = ExchangePathText.Text ?? "";
        config.Arquivos.PathSalvar = ExchangePathText.Text ?? "";
        config.Arquivos.Salvar = SaveDocumentsCheck.IsChecked == true;
        config.WebServices.Salvar = SaveExchangeCheck.IsChecked == true;
    }

    private void SaveConfiguration()
    {
        configuration.Set("Ambiente", Selected<DFeTipoAmbiente>(EnvironmentCombo));
        configuration.Set("Versao", Selected<VersaoNFSe>(VersionCombo));
        configuration.Set("Municipio", MunicipalityInput.Value ?? 5002704);
        configuration.Set("Timeout", TimeoutInput.Value ?? 100);
        configuration.Set("Certificado", CertificatePathText.Text ?? "");
        configuration.Set("NumeroSerie", CertificateSerialText.Text ?? "");
        configuration.Set("LembrarSenha", RememberPasswordCheck.IsChecked == true);
        configuration.SetCrypt("Senha", RememberPasswordCheck.IsChecked == true ? CertificatePasswordText.Text ?? "" : "");
        configuration.Set("Documento", TaxIdText.Text ?? "");
        configuration.Set("IM", RegistrationText.Text ?? "");
        configuration.Set("Schemas", SchemasPathText.Text ?? "");
        configuration.Set("Documentos", DocumentsPathText.Text ?? "");
        configuration.Set("Envios", ExchangePathText.Text ?? "");
        configuration.Set("Lote", BatchText.Text ?? "1");
        configuration.Set("SalvarXML", SaveDocumentsCheck.IsChecked == true);
        configuration.Set("SalvarEnvios", SaveExchangeCheck.IsChecked == true);
        configuration.Set("Assinar", SignXmlCheck.IsChecked == true);
        configuration.Set("Validar", ValidateSchemasCheck.IsChecked == true);
        configuration.Save();
    }

    private void UpdateContext()
    {
        var environment = Selected<DFeTipoAmbiente>(EnvironmentCombo);
        var code = (int)(MunicipalityInput.Value ?? 5002704);
        var service = NFSeServiceManager.Instance.Services[code];
        EnvironmentText.Text = environment == DFeTipoAmbiente.Producao ? "Produção" : "Homologação";
        ContextText.Text = $"NFS-e {(Selected<VersaoNFSe>(VersionCombo) == VersaoNFSe.Ve101 ? "1.01" : "1.00")} · IBGE {code} · {service?.Nome} · {service?.Provider}";
        EnvironmentBadge.Background = Avalonia.Media.Brush.Parse(environment == DFeTipoAmbiente.Producao ? "#FFE8DB" : "#EAF2FF");
    }

    private async Task SelectFolderAsync(TextBox target) => await RunAsync("Selecionar diretório", async _ =>
    {
        var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions { Title = "Selecionar diretório" });
        if (folders.FirstOrDefault() is { } folder) target.Text = folder.TryGetLocalPath() ?? target.Text;
    });

    internal static void Fill<T>(ComboBox combo, IEnumerable<T> values, T selected, Func<T, string>? label = null)
    {
        var choices = values.Select(value => new Choice<T>(value, label?.Invoke(value) ?? value?.ToString() ?? "")).ToArray();
        combo.ItemsSource = choices;
        combo.SelectedItem = choices.FirstOrDefault(choice => EqualityComparer<T>.Default.Equals(choice.Value, selected)) ?? choices.First();
    }

    internal static T Selected<T>(ComboBox combo) => ((Choice<T>)combo.SelectedItem!).Value;
    private sealed record Choice<T>(T Value, string Label) { public override string ToString() => Label; }
}
