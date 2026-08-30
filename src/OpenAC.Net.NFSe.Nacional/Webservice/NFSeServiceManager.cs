using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Common.Types;
using OpenAC.Net.NFSe.Nacional.Webservice.Nacional;
using OpenAC.Net.NFSe.Nacional.Storage;

namespace OpenAC.Net.NFSe.Nacional.Webservice;

/// <summary>
/// Gerenciador de serviços para NFSe.
/// </summary>
public sealed partial class NFSeServiceManager
{
    #region Fields

    /// <summary>
    /// Instância singleton do gerenciador de serviços.
    /// </summary>
    private static readonly NFSeServiceManager? instance;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="NFSeServiceManager"/>.
    /// </summary>
    private NFSeServiceManager()
    {
        Services = new NFSeServices();
        Providers = new Dictionary<NFSeProvider, Dictionary<VersaoNFSe, Type>>
        {
            {
                NFSeProvider.Nacional, new Dictionary<VersaoNFSe, Type>
                {
                    { VersaoNFSe.Ve100, typeof(NacionalWebservice) },
                    { VersaoNFSe.Ve101, typeof(NacionalWebservice) }
                }
            },
            {
                NFSeProvider.SimplISS, new Dictionary<VersaoNFSe, Type>
                {
                    { VersaoNFSe.Ve100, typeof(SimplISS.SimplISSWebService) },
                    { VersaoNFSe.Ve101, typeof(SimplISS.SimplISSWebService) }
                }
            },
            {
                NFSeProvider.Tiplan, new Dictionary<VersaoNFSe, Type>
                {
                    { VersaoNFSe.Ve100, typeof(Tiplan.TiplanWebService) },
                    { VersaoNFSe.Ve101, typeof(Tiplan.TiplanWebService) }
                }
            },
            {
                NFSeProvider.Fiorilli, new Dictionary<VersaoNFSe, Type>
                {
                    { VersaoNFSe.Ve100, typeof(Fiorilli.FiorilliWebService) },
                    { VersaoNFSe.Ve101, typeof(Fiorilli.FiorilliWebService) }
                }
            },
            {
                NFSeProvider.ISSNet, new Dictionary<VersaoNFSe, Type>
                {
                    { VersaoNFSe.Ve100, typeof(ISSNet.ISSNetWebService) },
                    { VersaoNFSe.Ve101, typeof(ISSNet.ISSNetWebService) }
                }
            }
        };

        Load();
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Municipios cadastrados no OpenNFSe
    /// </summary>
    /// <value>Os municipios</value>
    public NFSeServices Services { get; private set; }

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
    public Dictionary<NFSeProvider, Dictionary<VersaoNFSe, Type>> Providers { get; }

    /// <summary>
    /// Obtém a instância singleton do gerenciador de serviços.
    /// </summary>
    public static readonly NFSeServiceManager Instance = instance ??= new NFSeServiceManager();

    #endregion Properties

    #region Methods

    /// <summary>
    /// Salva o arquivo de cidades.
    /// </summary>
    /// <param name="path">Caminho para salvar o arquivo</param>
    public void Save(string path = "Municipios.nfse")
    {
        if (path == null) throw new ArgumentNullException(nameof(path));

        if (File.Exists(path)) File.Delete(path);

        using var fileStream = new FileStream(path, FileMode.CreateNew);
        Save(fileStream);
    }

    /// <summary>
    /// Salva o arquivo de cidades.
    /// </summary>
    /// <param name="stream">O stream.</param>
    public void Save(Stream stream)
    {
        foreach (var value in Enum.GetValues(typeof(TipoUrl)).Cast<TipoUrl>())
        {
            foreach (var m in Services.Webservices)
            {
                if (!m[DFeTipoAmbiente.Homologacao].Enderecos.ContainsKey(value))
                    m[DFeTipoAmbiente.Homologacao].Enderecos.Add(value, string.Empty);
                if (!m[DFeTipoAmbiente.Producao].Enderecos.ContainsKey(value))
                    m[DFeTipoAmbiente.Producao].Enderecos.Add(value, string.Empty);
            }
        }

        Services.Save(stream, DFeSaveOptions.None);
    }

    /// <summary>
    /// Carrega o arquivo de cidades.
    /// </summary>
    /// <param name="path">The path.</param>
    public void Load(string path = "")
    {
        byte[]? buffer = null;
        if (path.IsEmpty())
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var resourceStream = assembly.GetManifestResourceStream("OpenAC.Net.NFSe.Nacional.Resources.Municipios.nfse");
            if (resourceStream != null)
            {
                buffer = new byte[resourceStream.Length];
                _ = resourceStream.Read(buffer, 0, buffer.Length);
            }
        }
        else if (File.Exists(path))
        {
            buffer = File.ReadAllBytes(path);
        }

        if (buffer == null) throw new ArgumentException("Arquivo de cidades não encontrado");
        using var stream = new MemoryStream(buffer);
        Load(stream);
    }

    /// <summary>
    /// Carrega o arquivo de cidades.
    /// </summary>
    /// <param name="stream">The stream.</param>
    public void Load(Stream stream)
    {
        if (stream == null) throw new ArgumentException("Arquivo de cidades não encontrado");

        Services = NFSeServices.Load(stream);
    }

    /// <summary>Retorna um provedor configurado com o transporte HTTP informado.</summary>
    public NFSeWebserviceBase GetProvider(ConfiguracaoNFSe config, INFSeHttpTransport? httpTransport = null,
        INFSeDocumentStore? documentStore = null)
    {
        var serviceInfo = Services[config.WebServices.CodigoMunicipio] ?? 
                          throw new OpenException("Serviço não encontrado para o município informado!");

        var providerType = Providers[serviceInfo.Provider][config.Geral.Versao];
        if (providerType == null) throw new OpenException("Provedor não encontrado!");
        if (!CheckBaseType(providerType)) throw new OpenException("Classe base do provedor incorreta!");

        // ReSharper disable once AssignNullToNotNullAttribute
        var arguments = httpTransport == null
            ? new object[] { config, serviceInfo }
            : new object[] { config, serviceInfo, httpTransport };
        var provider = (NFSeWebserviceBase?)Activator.CreateInstance(providerType, arguments) ??
                       throw new InvalidOperationException();
        if (documentStore != null) provider.DocumentStore = documentStore;
        return provider;
    }

    private static bool CheckBaseType(Type providerType)
    {
        return typeof(NFSeWebserviceBase).IsAssignableFrom(providerType);
    }

    #endregion Methods
}
