using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Nacional.Common.Types;
using OpenAC.Net.NFSe.Nacional.Webservice;

namespace OpenAC.Net.NFSe.Nacional.Demo;

internal sealed class MunicipalityDialog : Window
{
    private readonly NumericUpDown code = new() { Name = "MunicipalityCodeInput", Minimum = -1, Maximum = 9999999, FormatString = "0" };
    private readonly TextBox name = new() { Name = "MunicipalityNameText" };
    private readonly ComboBox state = new() { Name = "StateCombo" };
    private readonly ComboBox provider = new() { Name = "ProviderCombo" };
    private readonly TextBlock validation = new() { Foreground = Brushes.Firebrick, TextWrapping = TextWrapping.Wrap };
    private readonly Dictionary<(DFeTipoAmbiente, TipoUrl), TextBox> addresses = [];
    private readonly NFSeServiceInfo? original;

    internal MunicipalityDialog(NFSeServiceInfo? service)
    {
        original = service;
        Title = service == null ? "Novo município" : "Editar município e endereços";
        Width = 900;
        Height = 760;
        MinWidth = 680;
        MinHeight = 550;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        code.Value = service?.Codigo ?? 0;
        code.IsEnabled = service?.Codigo != -1;
        name.Text = service?.Nome ?? "";
        MainWindow.Fill(state, Enum.GetValues<DFeSiglaUF>(), service?.UF ?? DFeSiglaUF.MS);
        var providers = NFSeServiceManager.Instance.Providers.Keys.ToList();
        if (service != null && !providers.Contains(service.Provider)) providers.Add(service.Provider);
        MainWindow.Fill(provider, providers.OrderBy(value => value.ToString()), service?.Provider ?? NFSeProvider.Nacional);
        var root = new Grid { Margin = new Thickness(24), RowDefinitions = new RowDefinitions("Auto,Auto,*,Auto,Auto") };
        root.Children.Add(new TextBlock { Text = Title, FontSize = 21, FontWeight = FontWeight.SemiBold, Margin = new Thickness(0, 0, 0, 20) });
        var fields = new Grid { ColumnDefinitions = new ColumnDefinitions("165,12,*,12,90,12,150"), Margin = new Thickness(0, 0, 0, 18) };
        AddField(fields, 0, "CÓDIGO IBGE", code);
        AddField(fields, 2, "MUNICÍPIO", name);
        AddField(fields, 4, "UF", state);
        AddField(fields, 6, "PROVEDOR", provider);
        Grid.SetRow(fields, 1);
        root.Children.Add(fields);
        var tabs = new TabControl();
        foreach (var environment in new[] { DFeTipoAmbiente.Homologacao, DFeTipoAmbiente.Producao })
        {
            var panel = new StackPanel { Spacing = 12, Margin = new Thickness(4, 16, 8, 10) };
            var update = new Button { Content = "Preencher todos os endereços deste ambiente" };
            update.Click += async (_, _) =>
            {
                var result = await InputDialog.AskAsync(this, "Atualizar endereços", new InputField("url", "URL base para todos os serviços"));
                if (result == null) return;
                if (!MunicipalityCatalog.IsValidAddress(result["url"])) { validation.Text = "Informe uma URL HTTP ou HTTPS válida."; return; }
                foreach (var address in addresses.Where(pair => pair.Key.Item1 == environment)) address.Value.Text = result["url"];
                validation.Text = "";
            };
            panel.Children.Add(update);
            foreach (var type in Enum.GetValues<TipoUrl>())
            {
                var value = service?.Ambientes.FirstOrDefault(item => item.Ambiente == environment)?.Enderecos.GetValueOrDefault(type) ?? "";
                var input = new TextBox { Name = $"{environment}_{type}", Text = value, PlaceholderText = "URL base do serviço (opcional)" };
                addresses.Add((environment, type), input);
                var field = new StackPanel { Spacing = 5 };
                field.Children.Add(new TextBlock { Text = MunicipalityCatalog.UrlLabel(type), Classes = { "label" } });
                field.Children.Add(input);
                panel.Children.Add(field);
            }
            tabs.Items.Add(new TabItem { Header = environment == DFeTipoAmbiente.Producao ? "Produção" : "Homologação", Content = new ScrollViewer { Content = panel } });
        }
        Grid.SetRow(tabs, 2);
        root.Children.Add(tabs);
        Grid.SetRow(validation, 3);
        validation.Margin = new Thickness(0, 12);
        root.Children.Add(validation);
        var actions = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Spacing = 10 };
        var cancel = new Button { Name = "CancelMunicipalityButton", Content = "Cancelar" };
        var save = new Button { Name = "SaveMunicipalityButton", Content = "Salvar município", Classes = { "primary" } };
        cancel.Click += (_, _) => Close(null);
        save.Click += (_, _) => Save();
        actions.Children.Add(cancel);
        actions.Children.Add(save);
        Grid.SetRow(actions, 4);
        root.Children.Add(actions);
        Content = root;
        KeyDown += (_, args) => { if (args.Key == Key.Escape) Close(null); };
    }

    private void Save()
    {
        try
        {
            var result = new NFSeServiceInfo
            {
                Codigo = (int)(code.Value ?? 0), Nome = name.Text?.Trim() ?? "",
                UF = MainWindow.Selected<DFeSiglaUF>(state), Provider = MainWindow.Selected<NFSeProvider>(provider)
            };
            foreach (var environment in new[] { DFeTipoAmbiente.Homologacao, DFeTipoAmbiente.Producao })
            {
                var target = new NFSeEnvironment { Ambiente = environment };
                foreach (var type in Enum.GetValues<TipoUrl>()) target.Enderecos[type] = addresses[(environment, type)].Text?.Trim() ?? "";
                result.Ambientes.Add(target);
            }
            MunicipalityCatalog.Validate(result, NFSeServiceManager.Instance.Services.Webservices, original);
            Close(result);
        }
        catch (ArgumentException exception) { validation.Text = exception.Message; }
    }

    private static void AddField(Grid grid, int column, string label, Control control)
    {
        var panel = new StackPanel { Spacing = 6 };
        panel.Children.Add(new TextBlock { Text = label, Classes = { "label" } });
        panel.Children.Add(control);
        Grid.SetColumn(panel, column);
        grid.Children.Add(panel);
    }
}

internal static class MunicipalityCatalog
{
    internal static bool IsValidAddress(string value) => Uri.TryCreate(value, UriKind.Absolute, out var uri)
        && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp) && !string.IsNullOrWhiteSpace(uri.Host);

    internal static void Validate(NFSeServiceInfo service, IEnumerable<NFSeServiceInfo> services, NFSeServiceInfo? original = null)
    {
        if (service.Codigo != -1 && service.Codigo is < 1000000 or > 9999999)
            throw new ArgumentException("Informe um código IBGE com 7 dígitos.");
        if (service.Codigo == -1 && original?.Codigo != -1)
            throw new ArgumentException("O código -1 é reservado ao serviço nacional padrão.");
        if (string.IsNullOrWhiteSpace(service.Nome)) throw new ArgumentException("Informe o nome do município.");
        if (services.Any(item => !ReferenceEquals(item, original) && item.Codigo == service.Codigo))
            throw new ArgumentException("Já existe um município com este código IBGE.");
        foreach (var environment in service.Ambientes)
            foreach (var (type, address) in environment.Enderecos)
                if (!string.IsNullOrWhiteSpace(address) && !IsValidAddress(address))
                    throw new ArgumentException($"{environment.Ambiente} · {UrlLabel(type)}: informe uma URL HTTP ou HTTPS válida.");
    }

    internal static string UrlLabel(TipoUrl type) => type switch
    {
        TipoUrl.Enviar => "Enviar DPS / lote",
        TipoUrl.EnviarEvento => "Enviar evento",
        TipoUrl.ConsultarNsu => "Consultar NSU",
        TipoUrl.ConsultarChave => "Consultar chave",
        TipoUrl.DownloadDanfse => "Download DANFSe",
        TipoUrl.ConsultarChaveDps => "Consultar chave pela DPS",
        TipoUrl.ConsultaExisteDps => "Verificar existência de DPS",
        TipoUrl.ConsultarEvento => "Consultar evento",
        _ => type.ToString()
    };
}

public sealed class MunicipalityItem(NFSeServiceInfo service)
{
    public NFSeServiceInfo Service { get; } = service;
    public int Code => Service.Codigo;
    public string Name => Service.Nome;
    public string State => Service.UF.ToString();
    public string Provider => Service.Provider.ToString();
}
