// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : marcosgerene
// Last Modified On : 03-01-2026
// ***********************************************************************
// <copyright file="NFSeWebservice.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014-2023 Grupo OpenAC.Net
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
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.Common.Types;
using OpenAC.Net.NFSe.Nacional.Webservice.Nacional;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenAC.Net.NFSe.Nacional.Webservice.SimplISS;

public class SimplISSWebservice : NacionalWebservice
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public SimplISSWebservice(ConfiguracaoNFSe configuracaoNFSe, NFSeServiceInfo serviceInfo) : 
        base(configuracaoNFSe, serviceInfo)
    {
    }

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

        this.Log().Debug($"SimplISS: [Enviar][Envio] - {strEnvio}");

        GravarArquivoEmDisco(strEnvio, $"Enviar-{dps.Informacoes.NumeroDps:000000}-env.json", documento);

        string url = ServiceInfo[Configuracao.WebServices.Ambiente][TipoUrl.Enviar] ?? throw new InvalidOperationException("URL de envio não encontrada na configuração do serviço.");
        HttpResponseMessage httpResponse = await SendAsync(content, HttpMethod.Post, $"{url}/nfse");

        string strResponse = await httpResponse.Content.ReadAsStringAsync();

        this.Log().Debug($"SimplISS: [Enviar][Resposta] - {strResponse}");

        GravarArquivoEmDisco(strResponse, $"Enviar-{dps.Informacoes.NumeroDps:000000}-resp.json", documento);

        // Reserializar com case insensitive para normalizar o JSON
        object? obj = JsonSerializer.Deserialize<object>(strResponse, JsonOptions);
        string jsonNormalizado = JsonSerializer.Serialize(obj, JsonOptions);

        return NFSeResponse<RespostaEnvioDps>.Create(dps.Xml, strEnvio, jsonNormalizado, httpResponse.IsSuccessStatusCode);
    }

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

        this.Log().Debug($"SimplISS: [Evento][Envio] - {strEnvio}");

        GravarArquivoEmDisco(strEnvio, $"Evento-{evento.Informacoes.ChNFSe}{evento.Informacoes.Evento}-env.json", documento);

        string url = ServiceInfo[Configuracao.WebServices.Ambiente][TipoUrl.EnviarEvento] ?? throw new InvalidOperationException("URL de envio não encontrada na configuração do serviço.");
        HttpResponseMessage httpResponse = await SendAsync(content, HttpMethod.Post, $"{url}/nfse/{evento.Informacoes.ChNFSe}/eventos");

        string strResponse = await httpResponse.Content.ReadAsStringAsync();

        this.Log().Debug($"SimplISS: [Evento][Resposta] - {strResponse}");

        GravarArquivoEmDisco(strResponse, $"Evento-{evento.Informacoes.ChNFSe}{evento.Informacoes.Evento}-resp.json", documento);

        object? obj = JsonSerializer.Deserialize<object>(strResponse, JsonOptions);
        string jsonNormalizado = JsonSerializer.Serialize(obj);

        return NFSeResponse<RespostaEnvioEvento>.Create(evento.Xml, strEnvio, jsonNormalizado, httpResponse.IsSuccessStatusCode);
    }

    public override async Task<NFSeResponse<RespostaConsultaChaveDps>> ConsultaChaveDpsAsync(string id)
    {
        this.Log().Debug($"SimplISS: [ConsultaChaveDps][Envio] - {id}");

        string url = ServiceInfo[Configuracao.WebServices.Ambiente][TipoUrl.ConsultarChave] ?? throw new InvalidOperationException("URL de envio não encontrada na configuração do serviço.");
        HttpResponseMessage httpResponse = await SendAsync(null, HttpMethod.Get, $"{url}/nfse/{id}");

        string strResponse = await httpResponse.Content.ReadAsStringAsync();

        this.Log().Debug($"SimplISS: [ConsultaChaveDps][Resposta] - {strResponse}");

        GravarArquivoEmDisco(strResponse, $"ConsultaChaveDps-{id}-resp.json", "");

        object? obj = JsonSerializer.Deserialize<object>(strResponse, JsonOptions);
        string jsonNormalizado = JsonSerializer.Serialize(obj);

        return NFSeResponse<RespostaConsultaChaveDps>.Create("", "", jsonNormalizado, httpResponse.IsSuccessStatusCode);
    }
}