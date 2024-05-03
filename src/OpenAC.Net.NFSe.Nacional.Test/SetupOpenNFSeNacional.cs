namespace OpenAC.Net.NFSe.Nacional.Test;

public class SetupOpenNFSeNacional
{
    public static void SetSetup(OpenNFSeNacional openNFSeNacional)
    {
        openNFSeNacional.Configuracoes.Certificados.SelecionarCertificado();
        openNFSeNacional.Configuracoes.Geral.Salvar = true;
        openNFSeNacional.Configuracoes.Geral.RetirarAcentos = true;
        openNFSeNacional.Configuracoes.Geral.RetirarEspacos = true;
        openNFSeNacional.Configuracoes.Arquivos.PathSalvar = 
    }
}