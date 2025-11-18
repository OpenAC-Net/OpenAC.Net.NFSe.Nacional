using DotNetEnv;
using OpenAC.Net.NFSe.Nacional.Common.Model;

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
        ConfigurarBase(openNFSeNacional, "1.00", "NfseModeloAtual", numDps, serieDps, numEvento);
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
        ConfigurarBase(openNFSeNacional, "1.01", "NfseModeloNovo1", numDps, serieDps, numEvento);
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
        ConfigurarBase(openNFSeNacional, "1.01", "NfseModeloNovo2", numDps, serieDps, numEvento);
    }

    /// <summary>
    /// Obtém os dados de um tomador específico do arquivo .env
    /// </summary>
    /// <param name="numeroTomador">Número do tomador (ex: "1", "2", "3")</param>
    /// <returns>InfoPessoaNFSe com os dados do tomador</returns>
    public static InfoPessoaNFSe ObterTomador(string numeroTomador = "1")
    {
        var prefixo = $"Tomador{numeroTomador}";
        
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
    public static InfoPessoaNFSe ObterTomadorPadrao() => ObterTomador("1");

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
        string versaoSchema,
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
        var pathSchemas = Path.Combine(AppContext.BaseDirectory, "Schemas", versaoSchema);

        openNFSeNacional.Configuracoes.Certificados.CertificadoBytes =
            File.ReadAllBytes(certificadoPath);
        openNFSeNacional.Configuracoes.Certificados.Senha = certificadoSenha;
        openNFSeNacional.Configuracoes.Geral.Salvar = true;
        openNFSeNacional.Configuracoes.Geral.RetirarAcentos = true;
        openNFSeNacional.Configuracoes.Geral.RetirarEspacos = true;
        openNFSeNacional.Configuracoes.Arquivos.PathSalvar = pathSalvar;
        openNFSeNacional.Configuracoes.Arquivos.PathSchemas = pathSchemas;
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