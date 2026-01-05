// See https://aka.ms/new-console-template for more information
using OpenAC.Net.DFe.Core.Document;
using OpenAC.Net.NFSe.Nacional.Webservice;

Console.WriteLine("Hello, World!");

NFSeServiceManager manager = NFSeServiceManager.Instance;

NFSeServiceInfo infoNacional = new NFSeServiceInfo()
{
    UF  = OpenAC.Net.DFe.Core.Common.DFeSiglaUF.EX,
    Nome = "NFSe Nacional",
    Codigo = -1,
    Provider = NFSeProvider.Nacional,
};

NFSeEnvironment homoNacional = new();
homoNacional.Ambiente = OpenAC.Net.DFe.Core.Common.DFeTipoAmbiente.Homologacao;
homoNacional.Enderecos.Add(OpenAC.Net.NFSe.Nacional.Common.Types.TipoUrl.Enviar, "https://sefin.producaorestrita.nfse.gov.br/SefinNacional");
homoNacional.Enderecos.Add(OpenAC.Net.NFSe.Nacional.Common.Types.TipoUrl.EnviarEvento, "https://sefin.producaorestrita.nfse.gov.br/SefinNacional");
homoNacional.Enderecos.Add(OpenAC.Net.NFSe.Nacional.Common.Types.TipoUrl.ConsultarNsu, "https://adn.producaorestrita.nfse.gov.br/contribuintes");
homoNacional.Enderecos.Add(OpenAC.Net.NFSe.Nacional.Common.Types.TipoUrl.ConsultarChave, "https://adn.producaorestrita.nfse.gov.br/contribuintes");
homoNacional.Enderecos.Add(OpenAC.Net.NFSe.Nacional.Common.Types.TipoUrl.DownloadDanfse, "https://sefin.producaorestrita.nfse.gov.br/SefinNacional");
homoNacional.Enderecos.Add(OpenAC.Net.NFSe.Nacional.Common.Types.TipoUrl.ConsultarChaveDps, "https://sefin.producaorestrita.nfse.gov.br/SefinNacional");
homoNacional.Enderecos.Add(OpenAC.Net.NFSe.Nacional.Common.Types.TipoUrl.ConsultaExisteDps, "https://sefin.producaorestrita.nfse.gov.br/SefinNacional");
infoNacional.Ambientes.Add(homoNacional);

NFSeEnvironment prodNacional = new();
prodNacional.Ambiente = OpenAC.Net.DFe.Core.Common.DFeTipoAmbiente.Producao;
prodNacional.Enderecos.Add(OpenAC.Net.NFSe.Nacional.Common.Types.TipoUrl.Enviar, "https://sefin.nfse.gov.br/sefinnacional");
prodNacional.Enderecos.Add(OpenAC.Net.NFSe.Nacional.Common.Types.TipoUrl.EnviarEvento, "https://sefin.nfse.gov.br/sefinnacional");
prodNacional.Enderecos.Add(OpenAC.Net.NFSe.Nacional.Common.Types.TipoUrl.ConsultarNsu, "https://adn.nfse.gov.br/contribuintes");
prodNacional.Enderecos.Add(OpenAC.Net.NFSe.Nacional.Common.Types.TipoUrl.ConsultarChave, "https://adn.nfse.gov.br/contribuintes");
prodNacional.Enderecos.Add(OpenAC.Net.NFSe.Nacional.Common.Types.TipoUrl.DownloadDanfse, "https://sefin.nfse.gov.br/sefinnacional");
prodNacional.Enderecos.Add(OpenAC.Net.NFSe.Nacional.Common.Types.TipoUrl.ConsultarChaveDps, "https://sefin.nfse.gov.br/sefinnacional");
prodNacional.Enderecos.Add(OpenAC.Net.NFSe.Nacional.Common.Types.TipoUrl.ConsultaExisteDps, "https://sefin.nfse.gov.br/sefinnacional");
infoNacional.Ambientes.Add(prodNacional);

manager.Services.Webservices.Add(infoNacional);

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

string xml = manager.Services.GetXml();

manager.Save(@"c:\temp\Municipios.nfse");