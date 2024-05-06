namespace OpenAC.Net.NFSe.Nacional.Test;
using OpenAC.Net.NFSe.Nacional.Common;

public class SetupOpenNFSeNacional
{
    #region Propriedades 
    public static string NumDPS = "2";
    public static string CodMun = "3525300";
    public static int TipoInscricaoFederal = 2; //2 CNPJ; 1 CPF;
    public static string InscricaoFederal = "42250933000187";
    public static string SerieDPS = "1";
    #endregion

    public static void Configuracao(OpenNFSeNacional openNFSeNacional)
    {
        openNFSeNacional.Configuracoes.Certificados.CertificadoBytes = File.ReadAllBytes("Caminho do certificado");
        openNFSeNacional.Configuracoes.Certificados.Senha = "Senha do certificado";
        openNFSeNacional.Configuracoes.Geral.Salvar = true;
        openNFSeNacional.Configuracoes.Geral.RetirarAcentos = true;
        openNFSeNacional.Configuracoes.Geral.RetirarEspacos = true;
        openNFSeNacional.Configuracoes.Arquivos.PathSalvar =
            "C:\\_Projects\\sites\\OpenAC.Net.NFSe.Nacional\\src\\OpenAC.Net.NFSe.Nacional.Test\\XML";
        openNFSeNacional.Configuracoes.Arquivos.PathSchemas =
            "C:\\_Projects\\sites\\OpenAC.Net.NFSe.Nacional\\src\\OpenAC.Net.NFSe.Nacional\\Schemas\\1.00";

        var dps = new Dps();
        dps.Versao = "1.00";
        dps.Informacoes = new Common.InfDps();
    }
}