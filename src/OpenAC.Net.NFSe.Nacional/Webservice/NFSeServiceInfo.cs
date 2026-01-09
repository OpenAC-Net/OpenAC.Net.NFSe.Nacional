using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Collection;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.DFe.Core.Serializer;
using System;
using System.Linq;

namespace OpenAC.Net.NFSe.Nacional.Webservice;

/// <summary>
/// Representa informações do serviço de NFSe de um município:
/// código IBGE, provedor(es), nome e UF.
/// </summary>
public class NFSeServiceInfo
{
    /// <summary>
    /// Retorna o ambiente de NFSe correspondente ao <see cref="DFeTipoAmbiente"/> informado.
    /// </summary>
    /// <param name="ambiente">Tipo de ambiente desejado.</param>
    /// <returns>A instância de <see cref="NFSeEnvironment"/> encontrada.</returns>
    /// <exception cref="InvalidOperationException">Disparada quando o ambiente não existe em <see cref="Ambientes"/>.</exception>
    [DFeIgnore]
    public NFSeEnvironment this[DFeTipoAmbiente ambiente] =>
        Ambientes?.SingleOrDefault(x => x.Ambiente == ambiente) ?? throw new InvalidOperationException();
    
    /// <summary>
    /// Define ou retorna o codigo IBGE do municipio
    /// </summary>
    /// <value>The codigo.</value>
    [DFeAttribute(TipoCampo.Int, "Id")]
    public int Codigo { get; set; }
    
    /// <summary>
    /// Define ou retorna o provedor de NFSe.
    /// </summary>
    /// <value>The provedor.</value>
    [DFeElement(TipoCampo.Enum, "Tipo")]
    public NFSeProvider Provider { get; set; }

    /// <summary>
    /// Define ou retorna o nome do municipio
    /// </summary>
    /// <value>The nome.</value>
    [DFeElement(TipoCampo.Str, "Nome")]
    public string Nome { get; set; } = "";

    /// <summary>
    /// Define ou retorna a UF do municipio.
    /// </summary>
    /// <value>The uf.</value>
    [DFeElement(TipoCampo.Enum, "UF")]
    public DFeSiglaUF UF { get; set; }
    
    [DFeCollection("Ambientes")]
    [DFeItem(typeof(NFSeEnvironment), "Ambiente")]
    public DFeCollection<NFSeEnvironment> Ambientes { get; set; } = new();
}