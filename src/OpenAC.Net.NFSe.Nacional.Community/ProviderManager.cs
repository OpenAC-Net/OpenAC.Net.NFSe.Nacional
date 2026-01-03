using System;
using System.Collections.Generic;
using System.Linq;
using OpenAC.Net.Core;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Common.Types;
using OpenAC.Net.NFSe.Nacional.Webservice;
using OpenAC.Net.NFSe.Nacional.Webservice.Nacional;

namespace OpenAC.Net.NFSe.Nacional.Community;

/// <summary>
/// Gerencia provedores NFSe registrados e fornece uma fábrica para instanciá-los.
/// </summary>
/// <remarks>
/// Mantém um dicionário de provedores por versão e valida o tipo base antes de criar instâncias.
/// </remarks>
public static class ProviderManager
{
    #region Constructors

    static ProviderManager()
    {
        Providers = new Dictionary<NFSeProvider, Dictionary<VersaoNFSe, Type>>
        {
            {
                NFSeProvider.Nacional, new Dictionary<VersaoNFSe, Type>
                {
                    { VersaoNFSe.Ve100, typeof(NacionalWebservice) },
                    { VersaoNFSe.Ve101, typeof(NacionalWebservice) }
                }
            }
        };
    }

    #endregion Constructors

    #region Propriedades

    /// <summary>
    /// Provedores cadastrados no OpenNFSe.
    /// </summary>
    /// <value>
    /// Dicionário que mapeia um <see cref="NFSeProvider"/> para outro dicionário
    /// que mapeia <see cref="VersaoNFSe"/> para o <see cref="Type"/> da implementação.
    /// </value>
    /// <remarks>
    /// Inicializado no construtor estático. A propriedade é somente leitura; alterar o
    /// conteúdo do dicionário em tempo de execução não é recomendado.
    /// </remarks>
    public static Dictionary<NFSeProvider, Dictionary<VersaoNFSe, Type>> Providers { get; }

    #endregion Propriedades

    #region Methods

    /// <summary>
    /// Retorna uma instância do provedor NFSe para o município nas configurações informadas.
    /// </summary>
    /// <param name="provider">Identificador do provedor.</param>
    /// <param name="config">Configuração do NFSe (não pode ser nulo).</param>
    /// <returns>Instância de <see cref="NFSeWebserviceBase"/> correspondente ao provedor e versão.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="config"/> for nulo.</exception>
    /// <exception cref="OpenException">Se o provedor ou versão não estiverem registrados ou a classe do provedor for incompatível.</exception>
    /// <exception cref="InvalidOperationException">Se a instância do provedor não puder ser criada.</exception>
    public static NFSeWebserviceBase GetProvider(NFSeProvider provider, ConfiguracaoNFSe config)
    {
        // ReSharper disable once PossibleNullReferenceException
        var providerType = Providers[provider][config.Geral.Versao];
        if (providerType == null) throw new OpenException("Provedor não encontrado!");
        if (!CheckBaseType(providerType)) throw new OpenException("Classe base do provedor incorreta!");

        // ReSharper disable once AssignNullToNotNullAttribute
        return (NFSeWebserviceBase?)Activator.CreateInstance(providerType, config) ?? throw new InvalidOperationException();
    }

    /// <summary>
    /// Retorna o enum do provedor associado ao tipo de provedor informado.
    /// </summary>
    /// <param name="providerType">Tipo da classe do provedor (ex.: typeof(NacionalWebservice)).</param>
    /// <returns>Valor de <see cref="NFSeProvider"/> correspondente ao tipo.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="providerType"/> for nulo.</exception>
    /// <exception cref="OpenException">Se não for encontrado provedor para o tipo informado.</exception>
    public static NFSeProvider GetProvider(Type providerType)
    {
        foreach (var providerEntry in Providers.Where(providerEntry =>
                     providerEntry.Value.Any(versionEntry => versionEntry.Value == providerType)))
        {
            return providerEntry.Key;
        }

        throw new OpenException("Provedor não encontrado para o tipo especificado.");
    }

    private static bool CheckBaseType(Type providerType)
    {
        return typeof(NFSeWebserviceBase).IsAssignableFrom(providerType);
    }

    #endregion Methods
}