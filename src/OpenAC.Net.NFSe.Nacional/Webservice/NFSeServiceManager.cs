using System.Collections.Generic;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.DFe.Core.Service;
using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional.Webservice;

/// <summary>
/// Gerenciador de serviços para NFSe.
/// </summary>
public sealed class NFSeServiceManager : DFeServices<TipoServico>
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
        Webservices.Add(new DFeServiceInfo<TipoServico>
        {
            Tipo = DFeTipoServico.NFSe,
            TipoEmissao = DFeTipoEmissao.Normal,
            Ambientes =
            [
                new DFeServiceEnvironment<TipoServico>
                {
                    Ambiente = DFeTipoAmbiente.Homologacao,
                    UF = DFeSiglaUF.AN,
                    Enderecos = new Dictionary<TipoServico, string>
                    {
                        { TipoServico.Adn, "https://adn.producaorestrita.nfse.gov.br/contribuintes" },
                        { TipoServico.Sefin, "https://sefin.producaorestrita.nfse.gov.br/SefinNacional" }
                    }
                },

                new DFeServiceEnvironment<TipoServico>
                {
                    Ambiente = DFeTipoAmbiente.Producao,
                    UF = DFeSiglaUF.AN,
                    Enderecos = new Dictionary<TipoServico, string>
                    {
                        { TipoServico.Adn, "https://adn.nfse.gov.br/contribuintes" },
                        { TipoServico.Sefin, "https://sefin.nfse.gov.br/sefinnacional" }
                    }
                }
            ]
        });
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Obtém a instância singleton do gerenciador de serviços.
    /// </summary>
    public static readonly NFSeServiceManager Instance = instance ??= new NFSeServiceManager();

    #endregion Properties
}