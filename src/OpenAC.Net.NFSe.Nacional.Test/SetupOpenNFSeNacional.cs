namespace OpenAC.Net.NFSe.Nacional.Test;

public class SetupOpenNFSeNacional
{
    public static void SetSetup(OpenNFSeNacional openNFSeNacional)
    {
        openNFSeNacional.Configuracoes.Certificados.CertificadoBytes = File.ReadAllBytes("");
        openNFSeNacional.Configuracoes.Certificados.Senha = "";
        openNFSeNacional.Configuracoes.Geral.Salvar = true;
        openNFSeNacional.Configuracoes.Geral.RetirarAcentos = true;
        openNFSeNacional.Configuracoes.Geral.RetirarEspacos = true;
        openNFSeNacional.Configuracoes.Arquivos.PathSalvar =
            "C:\\_Projects\\sites\\OpenAC.Net.NFSe.Nacional\\src\\OpenAC.Net.NFSe.Nacional.Test\\XML";
        openNFSeNacional.Configuracoes.Arquivos.PathSchemas =
            "C:\\_Projects\\sites\\OpenAC.Net.NFSe.Nacional\\src\\OpenAC.Net.NFSe.Nacional\\Schemas\\1.00";
    }
}