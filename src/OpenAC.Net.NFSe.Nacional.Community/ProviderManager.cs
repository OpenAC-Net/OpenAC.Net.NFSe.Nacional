using System;
using System.Collections.Generic;
using OpenAC.Net.Core;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Common.Types;
using OpenAC.Net.NFSe.Nacional.Webservice;

namespace OpenAC.Net.NFSe.Nacional.Community;

public static class ProviderManager
{
    #region Constructors

    static ProviderManager()
    {
        Providers = new Dictionary<NFSeProvider, Dictionary<VersaoNFSe, Type>>();
    }

    #endregion Constructors

    #region Propriedades

    /// <summary>
    /// Provedores cadastrados no OpenNFSe
    /// </summary>
    /// <value>Os provedores</value>
    public static Dictionary<NFSeProvider, Dictionary<VersaoNFSe, Type>> Providers { get; }

    #endregion Propriedades

    #region Methods

    /// <summary>
    /// Retorna o provedor para o municipio nas configurações informadas.
    /// </summary>
    /// <param name="config">A configuração.</param>
    /// <returns>Provedor NFSe.</returns>
    public static NFSeWebserviceBase GetProvider(NFSeProvider provider, ConfiguracaoNFSe config)
    {
        // ReSharper disable once PossibleNullReferenceException
        var providerType = Providers[provider][config.Geral.Versao];
        if (providerType == null) throw new OpenException("Provedor não encontrado!");
        if (!CheckBaseType(providerType)) throw new OpenException("Classe base do provedor incorreta!");

        // ReSharper disable once AssignNullToNotNullAttribute
        return (NFSeWebserviceBase)Activator.CreateInstance(providerType, config);
    }
    
    private static bool CheckBaseType(Type providerType)
    {
        return typeof(NFSeWebserviceBase).IsAssignableFrom(providerType);
    }

    #endregion Methods
}