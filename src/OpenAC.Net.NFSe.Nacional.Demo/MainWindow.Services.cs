using Avalonia.Platform.Storage;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Nacional.Webservice;

namespace OpenAC.Net.NFSe.Nacional.Demo;

public partial class MainWindow
{
    private void ConnectServicesEvents()
    {
        ServiceSearchText.TextChanged += (_, _) => RefreshServices();
        NewServiceButton.Click += async (_, _) => await RunAsync("Cadastrar município", token => EditServiceAsync(null, token));
        EditServiceButton.Click += async (_, _) => await RunAsync("Editar município", token => EditServiceAsync(SelectedService(), token));
        ServiceList.DoubleTapped += async (_, _) =>
        {
            if (ServiceList.SelectedItem is MunicipalityItem item)
                await RunAsync("Editar município", token => EditServiceAsync(item.Service, token));
        };
        UseServiceButton.Click += async (_, _) => await RunAsync("Selecionar município", _ =>
        {
            var service = SelectedService();
            if (service.Codigo == -1) throw new ArgumentException("O serviço nacional é usado automaticamente para municípios sem cadastro específico. Informe o código IBGE em Configurações.");
            MunicipalityInput.Value = service.Codigo;
            ApplyConfiguration();
            SaveConfiguration();
            UpdateContext();
            ResultText.Text = $"Município selecionado: {service.Nome} · {service.Codigo}.";
            return Task.CompletedTask;
        });
        LoadServicesButton.Click += async (_, _) => await RunAsync("Importar catálogo", async token =>
        {
            var file = await PickFileAsync("Importar catálogo de municípios", "Catálogo NFS-e", "*.nfse", "*.xml");
            if (file == null) return;
            await using var stream = await file.OpenReadAsync();
            var imported = NFSeServices.Load(stream);
            ValidateImportedCatalog(imported);
            token.ThrowIfCancellationRequested();
            PersistCatalog(imported);
            RefreshServices();
            UpdateContext();
            ResultText.Text = $"Catálogo importado: {imported.Webservices.Count} municípios / serviços.";
        });
        SaveServicesButton.Click += async (_, _) => await RunAsync("Exportar catálogo", async token =>
        {
            var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Exportar municípios", SuggestedFileName = "Municipios.nfse", DefaultExtension = "nfse",
                FileTypeChoices = [new FilePickerFileType("Catálogo NFS-e") { Patterns = ["*.nfse"] }]
            });
            if (file == null) return;
            token.ThrowIfCancellationRequested();
            await using var stream = await file.OpenWriteAsync();
            stream.SetLength(0);
            NFSeServiceManager.Instance.Save(stream);
            ResultText.Text = "Catálogo exportado para " + file.Name;
        });
    }

    private NFSeServiceInfo SelectedService() => (ServiceList.SelectedItem as MunicipalityItem)?.Service
        ?? throw new ArgumentException("Selecione um município na lista.");

    private void RefreshServices()
    {
        var selectedCode = (ServiceList.SelectedItem as MunicipalityItem)?.Code;
        var search = ServiceSearchText.Text?.Trim() ?? "";
        var items = NFSeServiceManager.Instance.Services.Webservices.OrderBy(service => service.Nome)
            .Select(service => new MunicipalityItem(service))
            .Where(item => $"{item.Code} {item.Name} {item.State} {item.Provider}".Contains(search, StringComparison.OrdinalIgnoreCase)).ToArray();
        ServiceList.ItemsSource = items;
        ServiceList.SelectedItem = items.FirstOrDefault(item => item.Code == selectedCode);
    }

    private async Task EditServiceAsync(NFSeServiceInfo? original, CancellationToken token)
    {
        var result = await new MunicipalityDialog(original) { Icon = Icon }.ShowDialog<NFSeServiceInfo?>(this);
        if (result == null) return;
        token.ThrowIfCancellationRequested();
        // Serialize a separate catalog first so a failed write cannot change the live configuration.
        using var snapshot = new MemoryStream();
        NFSeServiceManager.Instance.Services.Save(snapshot);
        snapshot.Position = 0;
        var updated = NFSeServices.Load(snapshot);
        var previous = updated.Webservices.FirstOrDefault(item => item.Codigo == original?.Codigo);
        if (previous != null) updated.Webservices.Remove(previous);
        updated.Webservices.Add(result);
        PersistCatalog(updated);
        RefreshServices();
        ServiceList.SelectedItem = ((IEnumerable<MunicipalityItem>)ServiceList.ItemsSource!).FirstOrDefault(item => item.Code == result.Codigo);
        UpdateContext();
        ResultText.Text = $"Município salvo: {result.Nome}. Catálogo local: {catalogPath}";
    }

    private void PersistCatalog(NFSeServices catalog)
    {
        using var serialized = new MemoryStream();
        catalog.Save(serialized);
        var temporary = catalogPath + ".tmp";
        try
        {
            File.WriteAllBytes(temporary, serialized.ToArray());
            File.Move(temporary, catalogPath, true);
        }
        finally { if (File.Exists(temporary)) File.Delete(temporary); }
        serialized.Position = 0;
        NFSeServiceManager.Instance.Load(serialized);
    }

    internal static void ValidateImportedCatalog(NFSeServices catalog)
    {
        if (catalog.Webservices.Count == 0) throw new ArgumentException("O catálogo não contém municípios.");
        if (catalog.Webservices.GroupBy(service => service.Codigo).Any(group => group.Count() > 1))
            throw new ArgumentException("O catálogo contém códigos IBGE duplicados.");
        if (!catalog.Webservices.Any(service => service.Codigo == -1))
            throw new ArgumentException("O catálogo precisa conter o serviço nacional padrão (código -1).");
        foreach (var service in catalog.Webservices)
        {
            foreach (var environment in new[] { DFeTipoAmbiente.Homologacao, DFeTipoAmbiente.Producao })
                if (service.Ambientes.Count(item => item.Ambiente == environment) != 1)
                    throw new ArgumentException($"{service.Nome}: é necessário um ambiente de homologação e um de produção.");
        }
    }
}
