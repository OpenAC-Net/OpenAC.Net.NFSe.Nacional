using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Web.Accessor;
using OpenAC.Net.NFSe.Nacional.Web.Client;
using OpenAC.Net.NFSe.Nacional.Web.Common;
using OpenAC.Net.NFSe.Nacional.Web.Configuration;
using OpenAC.Net.NFSe.Nacional.Web.Provider;
using OpenAC.Net.NFSe.Nacional.Web.Store;
using OpenAC.Net.NFSe.Nacional.Web.Transport;
using OpenAC.Net.NFSe.Nacional.Webservice;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>Extensões de registro da NFS-e Nacional no container do ASP.NET Core.</summary>
public static class OpenNFSeNacionalServiceCollectionExtensions
{
    /// <summary>Registra a integração com configuração criada para cada escopo HTTP.</summary>
    /// <param name="services">Coleção de serviços da aplicação.</param>
    /// <param name="configure">Ação que cria a configuração fiscal e de certificado de cada escopo.</param>
    /// <param name="configureInfrastructure">Ação opcional que configura transporte, resiliência e persistência.</param>
    /// <returns>A mesma coleção de serviços, permitindo encadeamento de chamadas.</returns>
    public static IServiceCollection AddOpenNFSeNacionalWeb(this IServiceCollection services,
        Action<ConfiguracaoNFSe> configure,
        Action<OpenNFSeNacionalWebOptions>? configureInfrastructure = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        var infrastructure = new OpenNFSeNacionalWebOptions();
        configureInfrastructure?.Invoke(infrastructure);
        services.AddOptions<OpenNFSeNacionalWebOptions>()
            .Configure(options =>
            {
                options.TimeoutOperacao = infrastructure.TimeoutOperacao;
                options.TimeoutTentativa = infrastructure.TimeoutTentativa;
                options.TempoVidaHandler = infrastructure.TempoVidaHandler;
                options.TempoVidaConexaoPool = infrastructure.TempoVidaConexaoPool;
                options.PersistenciaHabilitada = infrastructure.PersistenciaHabilitada;
                options.CaminhoDocumentos = infrastructure.CaminhoDocumentos;
                options.CircuitBreakerHabilitado = infrastructure.CircuitBreakerHabilitado;
                foreach (var item in infrastructure.TimeoutsPorOperacao)
                    options.TimeoutsPorOperacao[item.Key] = item.Value;
                options.MargemExpiracaoCertificado = infrastructure.MargemExpiracaoCertificado;
            });

        services.AddScoped(serviceProvider =>
        {
            var configuration = new ConfiguracaoNFSe();
            configure(configuration);
            serviceProvider.GetRequiredService<NFSeHttpClientRegistry>()
                .Bind(configuration, NFSeTenant.Padrao);
            return configuration;
        });
        services.TryAddSingleton<INFSeConfigurationProvider<ConfiguracaoNFSe>>(
            new DefaultNFSeConfigurationProvider<ConfiguracaoNFSe>(() =>
            {
                var configuration = new ConfiguracaoNFSe();
                configure(configuration);
                return configuration;
            }));
        AddCoreServices(services);
        return services;
    }

    /// <summary>Registra a integração multiempresa usando provedores definidos pela aplicação.</summary>
    /// <typeparam name="TConfigurationProvider">Provider que obtém uma configuração independente por empresa.</typeparam>
    /// <typeparam name="TCertificateProvider">Provider que carrega ou renova o certificado por empresa.</typeparam>
    /// <param name="services">Coleção de serviços da aplicação.</param>
    /// <param name="configureInfrastructure">Ação opcional que configura transporte, resiliência e persistência.</param>
    /// <returns>A mesma coleção de serviços, permitindo encadeamento de chamadas.</returns>
    public static IServiceCollection AddOpenNFSeNacionalWebMultiTenant<TConfigurationProvider, TCertificateProvider>(
        this IServiceCollection services,
        Action<OpenNFSeNacionalWebOptions>? configureInfrastructure = null)
        where TConfigurationProvider : class, INFSeConfigurationProvider<ConfiguracaoNFSe>
        where TCertificateProvider : class, INFSeCertificateProvider
    {
        ArgumentNullException.ThrowIfNull(services);
        var infrastructure = new OpenNFSeNacionalWebOptions();
        configureInfrastructure?.Invoke(infrastructure);
        services.AddOptions<OpenNFSeNacionalWebOptions>()
            .Configure(options => Copy(infrastructure, options));
        services.TryAddSingleton<INFSeConfigurationProvider<ConfiguracaoNFSe>,
            TConfigurationProvider>();
        services.TryAddSingleton<INFSeCertificateProvider, TCertificateProvider>();
        AddCoreServices(services);
        services.TryAddSingleton<IOpenNFSeNacionalClientFactory,
            OpenNFSeNacionalClientFactory>();
        return services;
    }

    private static void AddCoreServices(IServiceCollection services)
    {
        services.AddHttpClient();
        services.TryAddSingleton<NFSeHttpClientRegistry>();
        services.TryAddSingleton<INFSeTenantContextAccessor>(serviceProvider =>
            serviceProvider.GetRequiredService<NFSeHttpClientRegistry>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<HttpClientFactoryOptions>,
            NFSeHttpClientOptions>());
        services.TryAddSingleton<OpenAC.Net.NFSe.Nacional.Storage.INFSeDocumentStore,
            FileSystemNFSeDocumentStore>();
        services.TryAddSingleton<INFSeHttpTransport, NFSeHttpTransport>();
        services.TryAddScoped<OpenAC.Net.NFSe.Nacional.IOpenNFSeNacionalClient,
            OpenNFSeNacionalWebClient>();
    }

    private static void Copy(OpenNFSeNacionalWebOptions source,
        OpenNFSeNacionalWebOptions target)
    {
        target.TimeoutOperacao = source.TimeoutOperacao;
        target.TimeoutTentativa = source.TimeoutTentativa;
        target.TempoVidaHandler = source.TempoVidaHandler;
        target.TempoVidaConexaoPool = source.TempoVidaConexaoPool;
        target.PersistenciaHabilitada = source.PersistenciaHabilitada;
        target.CaminhoDocumentos = source.CaminhoDocumentos;
        target.CircuitBreakerHabilitado = source.CircuitBreakerHabilitado;
        foreach (var item in source.TimeoutsPorOperacao)
            target.TimeoutsPorOperacao[item.Key] = item.Value;
        target.MargemExpiracaoCertificado = source.MargemExpiracaoCertificado;
    }
}
