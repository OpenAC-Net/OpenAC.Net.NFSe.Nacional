using OpenAC.Net.NFSe.Nacional.Webservice;

NFSeServiceManager manager = NFSeServiceManager.Instance;

/*
Exemplo de como adicionar uma cidade:

NFSeServiceInfo infoDoisCorregos = new NFSeServiceInfo()
{
    UF = OpenAC.Net.DFe.Core.Common.DFeSiglaUF.SP,
    Nome = "DOIS CÓRREGOS",
    Codigo = 3514106,
    Provider = NFSeProvider.SimplISS,
};

NFSeEnvironment homoDoisCorregos = new();
homoDoisCorregos.Ambiente = OpenAC.Net.DFe.Core.Common.DFeTipoAmbiente.Homologacao;
homoDoisCorregos.Enderecos.Add(OpenAC.Net.NFSe.Nacional.Common.Types.TipoUrl.Enviar, "https://producaorestrita.simplissweb.com.br");
homoDoisCorregos.Enderecos.Add(OpenAC.Net.NFSe.Nacional.Common.Types.TipoUrl.EnviarEvento, "https://producaorestrita.simplissweb.com.br");
homoDoisCorregos.Enderecos.Add(OpenAC.Net.NFSe.Nacional.Common.Types.TipoUrl.ConsultarNsu, "https://adn.producaorestrita.nfse.gov.br/contribuintes");
homoDoisCorregos.Enderecos.Add(OpenAC.Net.NFSe.Nacional.Common.Types.TipoUrl.ConsultarChave, "https://adn.producaorestrita.nfse.gov.br/contribuintes");
homoDoisCorregos.Enderecos.Add(OpenAC.Net.NFSe.Nacional.Common.Types.TipoUrl.DownloadDanfse, "https://sefin.producaorestrita.nfse.gov.br/SefinNacional");
homoDoisCorregos.Enderecos.Add(OpenAC.Net.NFSe.Nacional.Common.Types.TipoUrl.ConsultarChaveDps, "https://sefin.producaorestrita.nfse.gov.br/SefinNacional");
homoDoisCorregos.Enderecos.Add(OpenAC.Net.NFSe.Nacional.Common.Types.TipoUrl.ConsultaExisteDps, "https://sefin.producaorestrita.nfse.gov.br/SefinNacional");
infoDoisCorregos.Ambientes.Add(homoDoisCorregos);

NFSeEnvironment prodDoisCorregos = new();
prodDoisCorregos.Ambiente = OpenAC.Net.DFe.Core.Common.DFeTipoAmbiente.Producao;
prodDoisCorregos.Enderecos.Add(OpenAC.Net.NFSe.Nacional.Common.Types.TipoUrl.Enviar, "https://nfsedoiscorregos.simplissweb.com.br");
prodDoisCorregos.Enderecos.Add(OpenAC.Net.NFSe.Nacional.Common.Types.TipoUrl.EnviarEvento, "https://nfsedoiscorregos.simplissweb.com.br");
prodDoisCorregos.Enderecos.Add(OpenAC.Net.NFSe.Nacional.Common.Types.TipoUrl.ConsultarNsu, "https://adn.nfse.gov.br/contribuintes");
prodDoisCorregos.Enderecos.Add(OpenAC.Net.NFSe.Nacional.Common.Types.TipoUrl.ConsultarChave, "https://adn.nfse.gov.br/contribuintes");
prodDoisCorregos.Enderecos.Add(OpenAC.Net.NFSe.Nacional.Common.Types.TipoUrl.DownloadDanfse, "https://sefin.nfse.gov.br/sefinnacional");
prodDoisCorregos.Enderecos.Add(OpenAC.Net.NFSe.Nacional.Common.Types.TipoUrl.ConsultarChaveDps, "https://sefin.nfse.gov.br/sefinnacional");
prodDoisCorregos.Enderecos.Add(OpenAC.Net.NFSe.Nacional.Common.Types.TipoUrl.ConsultaExisteDps, "https://sefin.nfse.gov.br/sefinnacional");
infoDoisCorregos.Ambientes.Add(prodDoisCorregos);

manager.Services.Webservices.Add(infoDoisCorregos);
*/

manager.Save(@"c:\temp\Municipios.nfse");