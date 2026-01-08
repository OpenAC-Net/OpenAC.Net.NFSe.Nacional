using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Collection;
using OpenAC.Net.DFe.Core.Document;
using System;
using System.Linq;

namespace OpenAC.Net.NFSe.Nacional.Webservice;

/// <summary>
/// Representa os serviços de NFSe disponíveis.
/// Fornece a coleção de webservices e um indexador para recuperar informações por código do município.
/// </summary>
/// <remarks>
/// Herdada de <see cref="DFeDocument{T}"/> para suportar serialização/ligação com o formato DFe.
/// </remarks>
[DFeRoot("NFSeServices", Namespace = "https://www.openac.net.br/")]
public class NFSeServices : DFeDocument<NFSeServices>
{
    #region Properties

    /// <summary>
    /// Recupera a informação do serviço de NFSe para o código do município informado.
    /// </summary>
    /// <param name="codigo">Código do município.</param>
    /// <returns>Instância de <see cref="NFSeServiceInfo"/> correspondente ao código. Quando não encontrado, instanciará o NFSeNacional</returns>
    [DFeIgnore]
    public NFSeServiceInfo this[int codigo] => Webservices.SingleOrDefault(x => x.Codigo == codigo) ?? Webservices.SingleOrDefault(x => x.Codigo == -1);

    /// <summary>
    /// Conjunto de informações dos webservices de NFSe disponíveis.
    /// </summary>
    /// <value>Retorna uma coleção de <see cref="NFSeServiceInfo"/>.</value>
    [DFeCollection("Services")]
    [DFeItem(typeof(NFSeServiceInfo), "serviceInfo")]
    public DFeCollection<NFSeServiceInfo> Webservices { get; set; } = new();

    #endregion Properties
}