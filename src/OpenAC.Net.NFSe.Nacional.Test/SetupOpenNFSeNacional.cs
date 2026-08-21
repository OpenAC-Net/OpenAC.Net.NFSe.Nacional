using System.Net;
using DotNetEnv;
using OpenAC.Net.DFe.Core.Collection;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.DFe.Core.Extensions;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.Common.Types;
using OpenAC.Net.NFSe.Nacional.Webservice;

namespace OpenAC.Net.NFSe.Nacional.Test;

public class SetupOpenNFSeNacional
{
    #region Propriedades 
    public static string CodMunIBGE { get; private set; } = string.Empty;
    public static int TipoInscricaoFederal { get; private set; }
    public static string InscricaoFederal { get; private set; } = string.Empty;
    public static string InscricaoMunicipal { get; private set; } = string.Empty;

    // Estes NÃO estarão no .env - serão passados por parâmetro
    public static string NumDPS { get; private set; } = string.Empty;
    public static string SerieDPS { get; private set; } = string.Empty;
    public static string NumEvento { get; private set; } = string.Empty;
    #endregion

    static SetupOpenNFSeNacional()
    {
        var envPath = Path.Combine(AppContext.BaseDirectory, ".env");
        if (File.Exists(envPath))
        {
            Env.Load(envPath);
        }
    }

    /// <summary>
    /// Configuração padrão com versão 1.00
    /// </summary>
    public static void ConfiguracaoModeloAtual(
        OpenNFSeNacional openNFSeNacional,
        string numDps = "1",
        string serieDps = "1",
        string numEvento = "1")
    {
        ConfigurarBase(openNFSeNacional, VersaoNFSe.Ve100, "NfseModeloAtual", numDps, serieDps, numEvento);
    }

    /// <summary>
    /// Configuração padrão com versão 1.00
    /// </summary>
    public static void ConfiguracaoModeloAtual2(
        OpenNFSeNacional openNFSeNacional,
        string numDps = "1",
        string serieDps = "1",
        string numEvento = "1")
    {
        ConfigurarBase(openNFSeNacional, VersaoNFSe.Ve100, "NfseModeloAtual2", numDps, serieDps, numEvento);
    }


    /// <summary>
    /// Configuração modelo atual com versão 1.01
    /// </summary>
    public static void ConfiguracaoModeloNovoCenario1(
        OpenNFSeNacional openNFSeNacional,
        string numDps = "1",
        string serieDps = "1",
        string numEvento = "1")
    {
        ConfigurarBase(openNFSeNacional, VersaoNFSe.Ve101, "NfseModeloNovo1", numDps, serieDps, numEvento);
    }

    /// <summary>
    /// Configuração específica para testes de advocacia com IBS/CBS
    /// </summary>
    public static void ConfiguracaoModeloNovoCenario2(
        OpenNFSeNacional openNFSeNacional,
        string numDps = "10",
        string serieDps = "1",
        string numEvento = "1")
    {
        ConfigurarBase(openNFSeNacional, VersaoNFSe.Ve101, "NfseModeloNovo2", numDps, serieDps, numEvento);
    }

    /// <summary>
    /// Obtém os dados de um tomador específico do arquivo .env
    /// </summary>
    /// <param name="numeroTomador">Número do tomador (ex: "1", "2", "3")</param>
    /// <returns>InfoPessoaNFSe com os dados do tomador</returns>
    public static InfoPessoaNFSe ObterTomador(string numeroTomador = "1")
    {
        var prefixo = $"Tomador{numeroTomador}";

        if (GetEnvOrThrow($"{prefixo}.CodMunicipio") == "9999999")
        {
            return new InfoPessoaNFSe
            {

                Nif = GetEnvOrThrow($"{prefixo}.Nif"),
                Nome = GetEnvOrThrow($"{prefixo}.Nome"),
                Email = Env.GetString($"{prefixo}.Email"),
                Telefone = Env.GetString($"{prefixo}.Telefone"),
                Endereco = new EnderecoNFSe
                {
                    Logradouro = GetEnvOrThrow($"{prefixo}.Logradouro"),
                    Numero = GetEnvOrThrow($"{prefixo}.Numero"),
                    Complemento = Env.GetString($"{prefixo}.Complemento"),
                    Bairro = GetEnvOrThrow($"{prefixo}.Bairro"),

                    Municipio = new MunicipioExterior
                    {
                        CodigoPais = GetEnvOrThrow($"{prefixo}.ISO"),
                        EnderecoPostal = GetEnvOrThrow($"{prefixo}.CEP"),
                        Cidade = GetEnvOrThrow($"{prefixo}.NomeMunicipio"),
                        EstadoProvincia = GetEnvOrThrow($"{prefixo}.State")
                    }
                }
            };
        }
        else
            return new InfoPessoaNFSe
            {

                CNPJ = GetEnvOrThrow($"{prefixo}.CNPJ"),
                Nome = GetEnvOrThrow($"{prefixo}.Nome"),
                Email = Env.GetString($"{prefixo}.Email"),
                Telefone = Env.GetString($"{prefixo}.Telefone"),
                Endereco = new EnderecoNFSe
                {
                    Logradouro = GetEnvOrThrow($"{prefixo}.Logradouro"),
                    Numero = GetEnvOrThrow($"{prefixo}.Numero"),
                    Complemento = Env.GetString($"{prefixo}.Complemento"),
                    Bairro = GetEnvOrThrow($"{prefixo}.Bairro"),

                    Municipio = new MunicipioNacional
                    {
                        CEP = GetEnvOrThrow($"{prefixo}.CEP"),
                        CodMunicipio = GetEnvOrThrow($"{prefixo}.CodMunicipio")
                    }
                }
            };


    }

    /// <summary>
    /// Obtém o Tomador1 (padrão)
    /// </summary>
    public static InfoPessoaNFSe ObterTomadorPadrao() => ObterTomador();

    /// <summary>
    /// Obtém o código do município do tomador
    /// </summary>
    /// <param name="numeroTomador">Número do tomador</param>
    /// <returns>Código do município IBGE (7 dígitos)</returns>
    public static string ObterCodMunicipioTomador(string numeroTomador = "1")
    {
        return GetEnvOrThrow($"Tomador{numeroTomador}.CodMunicipio");
    }

    private static void ConfigurarBase(
        OpenNFSeNacional openNFSeNacional,
        VersaoNFSe versaoSchema,
        string prefixoEnv,
        string numDps,
        string serieDps,
        string numEvento)
    {
        // Atualiza os valores dos campos (passados por parâmetro)
        NumDPS = numDps;
        SerieDPS = serieDps;
        NumEvento = numEvento;

        // Carrega do .env
        CodMunIBGE = GetEnvOrThrow($"{prefixoEnv}.CodMunIbge");
        TipoInscricaoFederal = int.Parse(GetEnvOrThrow($"{prefixoEnv}.TipoInscricaoFederal"));
        InscricaoFederal = GetEnvOrThrow($"{prefixoEnv}.InscricaoFederal");
        InscricaoMunicipal = GetEnvOrThrow($"{prefixoEnv}.InscricaoMunicipal");

        var certificadoPath = GetEnvOrThrow($"{prefixoEnv}.CertificadoPath");
        var certificadoSenha = GetEnvOrThrow($"{prefixoEnv}.CertificadoSenha");
        var pathSalvar = GetEnvOrThrow("Geral.PathSalvar");

        // Schema path é inferido da versão
        var pathSchemas = Path.Combine(AppContext.BaseDirectory, "Schemas", versaoSchema.GetDFeValue());
        openNFSeNacional.Configuracoes.Geral.Versao = versaoSchema;
        openNFSeNacional.Configuracoes.Certificados.CertificadoBytes =
            File.ReadAllBytes(certificadoPath);
        openNFSeNacional.Configuracoes.Certificados.Senha = certificadoSenha;
        openNFSeNacional.Configuracoes.Geral.Salvar = true;
        openNFSeNacional.Configuracoes.Geral.RetirarAcentos = true;
        openNFSeNacional.Configuracoes.Geral.RetirarEspacos = true;
        openNFSeNacional.Configuracoes.Arquivos.PathSalvar = pathSalvar;
        openNFSeNacional.Configuracoes.Arquivos.PathSchemas = pathSchemas;
    }

    /// <summary>
    /// Configuração para o provedor Fiorilli (SOAP), versão 1.00.
    /// Lê as variáveis com prefixo "Fiorilli" do arquivo .env e registra o município
    /// no <see cref="NFSeServiceManager"/> em tempo de execução (o recurso embarcado não traz Fiorilli).
    /// </summary>
    public static void ConfigurarFiorilli(
        OpenNFSeNacional openNFSeNacional,
        string numDps = "1",
        string serieDps = "1",
        string numEvento = "1")
    {
        NumDPS = numDps;
        SerieDPS = serieDps;
        NumEvento = numEvento;

        CodMunIBGE = GetEnvOrThrow("Fiorilli.CodMunIbge");
        TipoInscricaoFederal = int.Parse(GetEnvOrThrow("Fiorilli.TipoInscricaoFederal"));
        InscricaoFederal = GetEnvOrThrow("Fiorilli.InscricaoFederal");
        InscricaoMunicipal = GetEnvOrThrow("Fiorilli.InscricaoMunicipal");

        var endpoint = Env.GetString("Fiorilli.Endpoint");
        if (string.IsNullOrWhiteSpace(endpoint))
            endpoint = "http://fi1.fiorilli.com.br:5663/IssWeb-ejb/IssWebWSNacional/IssWebWSNacionalPortType";

        var certificadoPath = GetEnvOrThrow("Fiorilli.CertificadoPath");
        var certificadoSenha = GetEnvOrThrow("Fiorilli.CertificadoSenha");
        var pathSalvar = GetEnvOrThrow("Geral.PathSalvar");

        RegistrarMunicipioFiorilli(int.Parse(CodMunIBGE), endpoint);

        var pathSchemas = Path.Combine(AppContext.BaseDirectory, "Schemas", VersaoNFSe.Ve101.GetDFeValue());
        openNFSeNacional.Configuracoes.Geral.Versao = VersaoNFSe.Ve101;
        openNFSeNacional.Configuracoes.Certificados.CertificadoBytes = File.ReadAllBytes(certificadoPath);
        openNFSeNacional.Configuracoes.Certificados.Senha = certificadoSenha;
        openNFSeNacional.Configuracoes.Geral.Salvar = true;
        openNFSeNacional.Configuracoes.Geral.RetirarAcentos = true;
        openNFSeNacional.Configuracoes.Geral.RetirarEspacos = true;
        openNFSeNacional.Configuracoes.Arquivos.PathSalvar = pathSalvar;
        openNFSeNacional.Configuracoes.Arquivos.PathSchemas = pathSchemas;
        openNFSeNacional.Configuracoes.WebServices.Ambiente = DFeTipoAmbiente.Homologacao;
        openNFSeNacional.Configuracoes.WebServices.CodigoMunicipio = int.Parse(CodMunIBGE);
        openNFSeNacional.Configuracoes.WebServices.InscricaoMunicipal = InscricaoMunicipal;
        openNFSeNacional.Configuracoes.WebServices.Protocolos = SecurityProtocolType.Tls12;
    }

    /// <summary>
    /// Registra (uma única vez) o município Fiorilli no <see cref="NFSeServiceManager"/>,
    /// apontando o endpoint SOAP em <see cref="TipoUrl.Enviar"/> para o ambiente de homologação.
    /// </summary>
    private static void RegistrarMunicipioFiorilli(int codigoMunicipio, string endpoint)
    {
        var services = NFSeServiceManager.Instance.Services;
        if (services.Webservices.Any(x => x.Codigo == codigoMunicipio)) return;

        services.Webservices.Add(new NFSeServiceInfo
        {
            Codigo = codigoMunicipio,
            Provider = NFSeProvider.Fiorilli,
            Nome = "Fiorilli (Teste)",
            UF = DFeSiglaUF.SP,
            Ambientes = new DFeCollection<NFSeEnvironment>
            {
                new NFSeEnvironment
                {
                    Ambiente = DFeTipoAmbiente.Homologacao,
                    Enderecos = new Dictionary<TipoUrl, string>
                    {
                        { TipoUrl.Enviar, endpoint }
                    }
                }
            }
        });
    }

    private static string GetEnvOrThrow(string key)
    {
        var value = Env.GetString(key);
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                $"Variável de ambiente '{key}' não encontrada. " +
                $"Certifique-se de que o arquivo .env existe e contém esta configuração."
            );
        }
        return value;
    }
}