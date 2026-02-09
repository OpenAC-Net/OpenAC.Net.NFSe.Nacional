using System.Collections.Generic;
using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional.Webservice;

/// <summary>
/// Representa o ambiente de serviço para NFSe Nacional.
/// Contém o tipo de ambiente (Produção ou Homologação) e os endereços dos serviços por tipo de URL.
/// </summary>
public class NFSeEnvironment
{
    #region Properties

    /// <summary>
    /// Obtém ou define o endereço do serviço para o tipo de URL especificado.
    /// </summary>
    /// <param name="tipo">Tipo de URL cujo endereço será lido ou atribuído.</param>
    /// <returns>Endereço do serviço como string.</returns>
    [DFeIgnore]
    public string this[TipoUrl tipo]
    {
        get => Enderecos[tipo];
        set => Enderecos[tipo] = value;
    }

    /// <summary>
    /// Obtém ou define o tipo de ambiente (Produção ou Homologação) para os serviços NFSe.
    /// </summary>
    [DFeAttribute(TipoCampo.Enum, "Tipo")]
    public DFeTipoAmbiente Ambiente { get; set; }

    /// <summary>
    /// Obtém ou define o dicionário de endereços dos serviços por tipo de URL.
    /// </summary>
    /// <remarks>
    /// A chave é um valor de <see cref="TipoUrl"/> e o valor é a URL do serviço.
    /// Inicializado por padrão com um dicionário vazio.
    /// </remarks>
    [DFeDictionary("Enderecos")]
    [DFeDictionaryKey(TipoCampo.Enum, "Tipo", AsAttribute = true)]
    [DFeDictionaryValue(TipoCampo.Str, "Endereco")]
    public Dictionary<TipoUrl, string> Enderecos { get; set; } = new();

    #endregion Properties
}