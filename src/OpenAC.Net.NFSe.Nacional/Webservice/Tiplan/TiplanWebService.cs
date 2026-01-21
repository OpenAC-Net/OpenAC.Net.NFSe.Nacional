// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RGG
// Created          : 14-01-2026
//
// Last Modified By : RGG
// Last Modified On : 14-01-2026
// ***********************************************************************
// <copyright file="TiplanWebService.cs" company="OpenAC .Net">
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
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenAC.Net.NFSe.Nacional.Webservice.Tiplan;
/// <summary>
/// Classe de serviço web para integração com a Tiplan.
/// </summary>
public class TiplanWebService : NacionalWebservice
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    /// <summary>
    /// Construtor da classe TiplanWebService.
    /// </summary>
    /// <param name="configuracaoNFSe"></param>
    /// <param name="serviceInfo"></param>
    public TiplanWebService(ConfiguracaoNFSe configuracaoNFSe, NFSeServiceInfo serviceInfo) :
        base(configuracaoNFSe, serviceInfo)
    {
    }
    /// <summary>
    /// Envio da DPS para geração da NFS-e de forma assíncrona.
    /// </summary>
    /// <param name="dps"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public override async Task<NFSeResponse<RespostaEnvioDps>> EnviarAsync(Dps dps)
    {
        dps.Assinar(Configuracao);

        ValidarSchema(SchemaNFSe.DPS, dps.Xml, dps.Versao);

        string documento = dps.Informacoes.Prestador.CPF ?? dps.Informacoes.Prestador.CNPJ ?? throw new InvalidOperationException("CPF ou CNPJ do prestador deve ser informado.");

        GravarDpsEmDisco(dps.Xml, $"{dps.Informacoes.NumeroDps:000000}_dps.xml",
            documento, dps.Informacoes.DhEmissao.DateTime);

        DpsEnvio envio = new DpsEnvio { XmlDps = dps.Xml };
        JsonContent content = JsonContent.Create(envio);
        string strEnvio = await content.ReadAsStringAsync();

        this.Log().Debug($"Tiplan: [Enviar][Envio] - {strEnvio}");

        GravarArquivoEmDisco(strEnvio, $"Enviar-{dps.Informacoes.NumeroDps:000000}-env.json", documento);

        string url = ServiceInfo[Configuracao.WebServices.Ambiente][TipoUrl.Enviar] ?? throw new InvalidOperationException("URL de envio não encontrada na configuração do serviço.");
        HttpResponseMessage httpResponse = await SendAsync(content, HttpMethod.Post, $"{url}/adn/dps/recepcao");

        string strResponse = await httpResponse.Content.ReadAsStringAsync();

        this.Log().Debug($"Tiplan: [Enviar][Resposta] - {strResponse}");

        GravarArquivoEmDisco(strResponse, $"Enviar-{dps.Informacoes.NumeroDps:000000}-resp.json", documento);

        return NFSeResponse<RespostaEnvioDps>.Create(dps.Xml, strEnvio, strResponse, httpResponse.IsSuccessStatusCode, JsonOptions);
    }
    /// <summary>
    /// Envio de evento assincrono.
    /// </summary>
    /// <param name="evento"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public override async Task<NFSeResponse<RespostaEnvioEvento>> EnviarEventoAsync(PedidoRegistroEvento evento)
    {
        evento.Assinar(Configuracao);
        ValidarSchema(SchemaNFSe.Evento, evento.Xml, evento.Versao);

        string? documento = evento.Informacoes.CPFAutor ?? evento.Informacoes.CNPJAutor ?? throw new InvalidOperationException("CPF ou CNPJ do autor do evento deve ser informado.");

        GravarDpsEmDisco(evento.Xml, $"{evento.Informacoes.ChNFSe}{evento.Informacoes.Evento}_evento.xml",
            documento, evento.Informacoes.DhEvento.DateTime);

        EventoEnvio envio = new EventoEnvio { XmlEvento = evento.Xml };
        JsonContent content = JsonContent.Create(envio);
        string strEnvio = await content.ReadAsStringAsync();

        this.Log().Debug($"Tiplan: [Evento][Envio] - {strEnvio}");

        GravarArquivoEmDisco(strEnvio, $"Evento-{evento.Informacoes.ChNFSe}{evento.Informacoes.Evento}-env.json", documento);

        string url = ServiceInfo[Configuracao.WebServices.Ambiente][TipoUrl.EnviarEvento] ?? throw new InvalidOperationException("URL de envio não encontrada na configuração do serviço.");
        HttpResponseMessage httpResponse = await SendAsync(content, HttpMethod.Post, $"{url}/adn/dps/evento");

        string strResponse = await httpResponse.Content.ReadAsStringAsync();

        this.Log().Debug($"Tiplan: [Evento][Resposta] - {strResponse}");

        GravarArquivoEmDisco(strResponse, $"Evento-{evento.Informacoes.ChNFSe}{evento.Informacoes.Evento}-resp.json", documento);

        return NFSeResponse<RespostaEnvioEvento>.Create(evento.Xml, strEnvio, strResponse, httpResponse.IsSuccessStatusCode, JsonOptions);
    }

}