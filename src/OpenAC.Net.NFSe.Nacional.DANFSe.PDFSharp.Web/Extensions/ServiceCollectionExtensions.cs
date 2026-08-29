using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp.Web;
using OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp.Web.Configuration;
using OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp.Web.Service;
using OpenAC.Net.NFSe.Nacional.Web;
using OpenAC.Net.NFSe.Nacional.Web.Provider;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>Extensões para registrar a geração Web do DANFSe no container de serviços.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Registra o gerador Web do DANFSe com as configurações padrão.</summary>
    /// <param name="services">Coleção de serviços da aplicação.</param>
    /// <returns>A própria coleção, para encadeamento das configurações.</returns>
    public static IServiceCollection AddOpenDANFSeNacionalWeb(this IServiceCollection services) =>
        services.AddOpenDANFSeNacionalWeb(_ => { });

    /// <summary>Registra e configura o gerador Web do DANFSe.</summary>
    /// <param name="services">Coleção de serviços da aplicação.</param>
    /// <param name="configure">Ação usada para configurar a geração e a resposta HTTP.</param>
    /// <returns>A própria coleção, para encadeamento das configurações.</returns>
    public static IServiceCollection AddOpenDANFSeNacionalWeb(this IServiceCollection services,
        Action<DANFSeNacionalWebOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);
        services.TryAddSingleton<INFSeConfigurationProvider<DANFSeNacionalWebOptions>>(
            new DefaultNFSeConfigurationProvider<DANFSeNacionalWebOptions>(() =>
            {
                var configuration = new DANFSeNacionalWebOptions();
                configure(configuration);
                return configuration;
            }));
        services.TryAddSingleton<IOpenDANFSeNacionalWeb, OpenDANFSeNacionalWeb>();
        return services;
    }

    /// <summary>Registra o gerador usando um provider de configuração multiempresa.</summary>
    /// <typeparam name="TConfigurationProvider">Provider das configurações de impressão por empresa.</typeparam>
    /// <param name="services">Coleção de serviços da aplicação.</param>
    /// <returns>A própria coleção, para encadeamento das configurações.</returns>
    public static IServiceCollection AddOpenDANFSeNacionalWeb<TConfigurationProvider>(
        this IServiceCollection services)
        where TConfigurationProvider : class, INFSeConfigurationProvider<DANFSeNacionalWebOptions>
    {
        ArgumentNullException.ThrowIfNull(services);
        services.TryAddSingleton<INFSeConfigurationProvider<DANFSeNacionalWebOptions>,
            TConfigurationProvider>();
        services.TryAddSingleton<IOpenDANFSeNacionalWeb, OpenDANFSeNacionalWeb>();
        return services;
    }
}
