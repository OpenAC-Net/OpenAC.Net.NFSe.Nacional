// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : Adriano Trentim
// Created          : 03-01-2025
//
// Last Modified By : Adriano Trentim
// Last Modified On : 16-06-2026
// ***********************************************************************
// <copyright file="ISSNetWebService.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014-2026 Grupo OpenAC.Net
//
//	 Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//	 The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//	 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// </copyright>
// <summary></summary>
// ***********************************************************************

using OpenAC.Net.Core.Logging;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.Common.Types;
using OpenAC.Net.NFSe.Nacional.Webservice.Nacional;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using OpenAC.Net.DFe.Core.Common;

namespace OpenAC.Net.NFSe.Nacional.Webservice.ISSNet;

public class ISSNetWebService : NacionalWebservice
{
    public ISSNetWebService(ConfiguracaoNFSe configuracaoNFSe, NFSeServiceInfo serviceInfo) :
        base(configuracaoNFSe, serviceInfo)
    {
    }

    public override async Task<NFSeResponse<RespostaEnvioDps>> EnviarAsync(Dps dps)
    {
        var options = DFeSaveOptions.DisableFormatting;
        if (Configuracao.Geral.RetirarAcentos)
            options |= DFeSaveOptions.RemoveAccents;

        options |= DFeSaveOptions.OmitDeclaration;
        dps.Assinar(Configuracao, options);

        ValidarSchema(SchemaNFSe.DPS, dps.Xml, dps.Versao);

        var documento = dps.Informacoes.Prestador.CPF ?? dps.Informacoes.Prestador.CNPJ ?? throw new InvalidOperationException("CPF ou CNPJ do prestador deve ser informado.");

        GravarDpsEmDisco(dps.Xml, $"{dps.Informacoes.NumeroDps:000000}_dps.xml",
            documento, dps.Informacoes.DhEmissao.DateTime);

        GravarArquivoEmDisco(dps.Xml, $"Enviar-{dps.Informacoes.NumeroDps:000000}-env.xml", documento);

        var xmlEnvio = $@"<GerarNfseEnvio xmlns=""http://www.sped.fazenda.gov.br/nfse"" xmlns:ns2=""http://www.w3.org/2000/09/xmldsig#"">{dps.Xml}</GerarNfseEnvio>";

        this.Log().Debug($"ISSNet: [Enviar][Envio] - {xmlEnvio}");

        var xmlCabecalho = $@"<cabecalho versao=""1.01""><versaoDados>1.01</versaoDados></cabecalho>";

        var soapEnvelope = $@"
            <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:nfse=""http://www.sped.fazenda.gov.br/nfse"">
                <soapenv:Header/>
                <soapenv:Body>
                    <nfse:GerarNfse>
                        <nfseCabecMsg>{xmlCabecalho}</nfseCabecMsg>
                        <nfseDadosMsg>{xmlEnvio}</nfseDadosMsg>
                    </nfse:GerarNfse>
                </soapenv:Body>
            </soapenv:Envelope>";

        var content = new StringContent(soapEnvelope, System.Text.Encoding.UTF8, "text/xml");
        var soapAction = "http://www.sped.fazenda.gov.br/nfse/GerarNfse";
        content.Headers.Add("SOAPAction", $"\"{soapAction}\"");

        var url = ServiceInfo[Configuracao.WebServices.Ambiente][TipoUrl.Enviar] ?? throw new InvalidOperationException("URL de envio não encontrada na configuração do serviço.");
        var httpResponse = await SendAsync(content, HttpMethod.Post, $"{url}/nfse.asmx");

        var strResponse = await httpResponse.Content.ReadAsStringAsync();

        this.Log().Debug($"ISSNet: [Enviar][Resposta] - {strResponse}");

        GravarArquivoEmDisco(strResponse, $"Enviar-{dps.Informacoes.NumeroDps:000000}-resp.xml", documento);

        return NFSeResponse<RespostaEnvioDps>.Create(dps.Xml, strResponse, httpResponse.IsSuccessStatusCode, await TrataRetorno<RespostaEnvioDps>(strResponse, "NFse"));
    }

    public override async Task<NFSeResponse<RespostaEnvioEvento>> EnviarEventoAsync(PedidoRegistroEvento evento)
    {
        evento.Assinar(Configuracao);
        ValidarSchema(SchemaNFSe.Evento, evento.Xml, evento.Versao);

        var documento = evento.Informacoes.CPFAutor ?? evento.Informacoes.CNPJAutor ?? throw new InvalidOperationException("CPF ou CNPJ do autor do evento deve ser informado.");

        GravarDpsEmDisco(evento.Xml, $"{evento.Informacoes.ChNFSe}{evento.Informacoes.Evento}_evento.xml",
            documento, evento.Informacoes.DhEvento.DateTime);

        EventoEnvio envio = new EventoEnvio { XmlEvento = evento.Xml };
        JsonContent content = JsonContent.Create(envio);
        string strEnvio = await content.ReadAsStringAsync();

        this.Log().Debug($"ISSNet: [Evento][Envio] - {strEnvio}");

        GravarArquivoEmDisco(strEnvio, $"Evento-{evento.Informacoes.ChNFSe}{evento.Informacoes.Evento}-env.json", documento);

        string url = ServiceInfo[Configuracao.WebServices.Ambiente][TipoUrl.EnviarEvento] ?? throw new InvalidOperationException("URL de envio não encontrada na configuração do serviço.");
        HttpResponseMessage httpResponse = await SendAsync(content, HttpMethod.Post, $"{url}/nfse/{evento.Informacoes.ChNFSe}/eventos");

        string strResponse = await httpResponse.Content.ReadAsStringAsync();

        this.Log().Debug($"ISSNet: [Evento][Resposta] - {strResponse}");

        GravarArquivoEmDisco(strResponse, $"Evento-{evento.Informacoes.ChNFSe}{evento.Informacoes.Evento}-resp.json", documento);

        return NFSeResponse<RespostaEnvioEvento>.Create(evento.Xml, strResponse, httpResponse.IsSuccessStatusCode, await TrataRetorno<RespostaEnvioEvento>(strResponse, "NFse"));
    }

    private async Task<T> TrataRetorno<T>(string xmlResposta, string xmlRootTag) where T : RespostaBase, new()
    {
        var resultado = new T();

        try
        {
            var doc = XDocument.Parse(xmlResposta);

            XNamespace nsSped = "http://www.sped.fazenda.gov.br/nfse";

            var elementosMensagem = doc.Descendants(nsSped + "MensagemRetorno");

            var listaMensagensErros = elementosMensagem
                .Select(elemento =>
                    new MensagemProcessamento
                    {
                        Codigo = elemento.Element(nsSped + "Codigo")?.Value ?? string.Empty,
                        Descricao = elemento.Element(nsSped + "Mensagem")?.Value ?? string.Empty,
                        Mensagem = elemento.Element(nsSped + "Correcao")?.Value ?? string.Empty,
                        Complemento = string.Empty,
                        Parametros = new List<string>()
                    })
                .ToList();

            if (listaMensagensErros.Any())
                resultado.Erros = listaMensagensErros;
            else
            {
                if (typeof(T) is RespostaEnvioDps)
                {
                    var elementNfse = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == xmlRootTag);
                    (resultado as RespostaEnvioDps).XmlNFSe = elementNfse.ToString();
                }
                else
                {

                }
                
                //var serializer = new XmlSerializer(typeof(NotaFiscalServico));

                //using var reader = new StringReader(xmlNfseApenas);

                //resultado = (T)serializer.Deserialize(reader);
            }
        }
        catch (Exception ex)
        {
            this.Log().Error(ex);

            resultado = null;
        }

        return resultado;
    }
}
