namespace OpenAC.Net.NFSe.Nacional.Test;
using OpenAC.Net.NFSe.Nacional.Common;
using System.Reflection;

public class SetupOpenNFSeNacional
{
    #region Propriedades 
    public static string CodMun = "3525300";
    public static int TipoInscricaoFederal = 2; //2 CNPJ; 1 CPF;
    public static string InscricaoFederal = "42250933000187";

    //Emissão
    public static string NumDPS = "7";
    public static string SerieDPS = "1";

    //Evento
    public static string NumEvento = "1";
    #endregion

    public static void Configuracao(OpenNFSeNacional openNFSeNacional)
    {
        openNFSeNacional.Configuracoes.Certificados.CertificadoBytes = File.ReadAllBytes("C:\\Users\\MICRO\\Documents\\DC SYSTEM\\42.250.933 LUCAS APARECIDO DE MORAES_42250933000187.pfx");
        openNFSeNacional.Configuracoes.Certificados.Senha = "Dcsystem031996";
        openNFSeNacional.Configuracoes.Geral.Salvar = true;
        openNFSeNacional.Configuracoes.Geral.RetirarAcentos = true;
        openNFSeNacional.Configuracoes.Geral.RetirarEspacos = true;
        openNFSeNacional.Configuracoes.Arquivos.PathSalvar =
            "C:\\_Projects\\sites\\OpenAC.Net.NFSe.Nacional\\src\\OpenAC.Net.NFSe.Nacional.Test\\XML";
        openNFSeNacional.Configuracoes.Arquivos.PathSchemas = AppContext.BaseDirectory + "\\Schemas\\1.00";
    }
}