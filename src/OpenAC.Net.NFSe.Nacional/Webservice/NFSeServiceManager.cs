using System.Collections.Generic;
using OpenAC.Net.DFe.Core.Collection;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.DFe.Core.Service;
using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional.Webservice;

/// <summary>
/// Gerenciador de serviços
/// </summary>
public sealed class NFSeServiceManager : DFeServices<TipoServico>
{
    #region Fields

    private static readonly NFSeServiceManager? instance;

    #endregion Fields

    #region Constructors

    private NFSeServiceManager()
    {
        Webservices.Add(new DFeServiceInfo<TipoServico>
        {
            Tipo = DFeTipoServico.NFSe,
            TipoEmissao = DFeTipoEmissao.Normal,
            Ambientes = new DFeCollection<DFeServiceEnvironment<TipoServico>>
            {
                new()
                {
                    Ambiente = DFeTipoAmbiente.Homologacao,
                    UF = DFeSiglaUF.AN,
                    Enderecos = new Dictionary<TipoServico, string>
                    {
                        {TipoServico.Adn, "https://adn.producaorestrita.nfse.gov.br/contribuintes"},
                        {TipoServico.Sefin, "https://sefin.producaorestrita.nfse.gov.br/SefinNacional"}
                    }
                },
                new()
                {
                    Ambiente = DFeTipoAmbiente.Producao,
                    UF = DFeSiglaUF.AN,
                    Enderecos = new Dictionary<TipoServico, string>
                    {
                        {TipoServico.Adn, "https://adn.nfse.gov.br/contribuinte"},
                        {TipoServico.Sefin, "https://sefin.nfse.gov.br/sefinnacional"}
                    }
                }
            }
        });
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Retorna a instancia do gerenciado de serviços
    /// </summary>
    public static readonly NFSeServiceManager Instance = instance ??= new NFSeServiceManager();

    #endregion Properties
}