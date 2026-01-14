// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
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

using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using OpenAC.Net.Core.Logging;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional.Webservice.Nacional;

/// <summary>
/// Classe responsável pela comunicação com o webservice da NFSe Nacional.
/// </summary>
public class NacionalWebservice : NFSeWebserviceBase
{
    #region Constructors

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="NacionalWebservice"/>.
    /// </summary>
    /// <param name="configuracaoNFSe">Configuração da NFSe.</param>
    /// <param name="serviceInfo">Informações do serviço</param>
    public NacionalWebservice(ConfiguracaoNFSe configuracaoNFSe, NFSeServiceInfo serviceInfo) : 
        base(configuracaoNFSe, serviceInfo)
    {
    }

    #endregion Constructors

    #region Methods

    #region DANFSe

    /// <summary>
    /// Retorna o DANFSe de uma NFS-e a partir de sua chave de acesso.
    /// </summary>
    /// <param name="chave">Chave de acesso da NFS-e.</param>
    /// <returns>Array de bytes contendo o DANFSe.</returns>
    public override async Task<byte[]> DownloadDANFSeAsync(string chave)
    {
        this.Log().Debug($"Webservice: [DANFSe][Envio] - {chave}");

        var url = ServiceInfo[Configuracao.WebServices.Ambiente][TipoUrl.DownloadDanfse];
        var httpResponse = await SendAsync(null, HttpMethod.Get, $"{url}/danfse/{chave}");

        this.Log().Debug($"Webservice: [DANFSe][Resposta] - {httpResponse.StatusCode}");

        httpResponse.EnsureSuccessStatusCode();
        return await httpResponse.Content.ReadAsByteArrayAsync();
    }

    #endregion DANFSe

    #region DFe

    /// <summary>
    /// Distribui os DF-e para contribuintes relacionados à NFS-e.
    /// </summary>
    /// <param name="nsu">Número NSU.</param>
    /// <returns>Resposta da consulta contendo os DF-e.</returns>
    public override async Task<NFSeResponse<RespostaConsultaDFe>> ConsultaNsuAsync(int nsu)
    {
        this.Log().Debug($"Webservice: [ConsultaNsu][Envio] - {nsu}");

        var url = ServiceInfo[Configuracao.WebServices.Ambiente][TipoUrl.ConsultarNsu];
        var httpResponse = await SendAsync(null, HttpMethod.Get, $"{url}/DFe/{nsu}");

        var strResponse = await httpResponse.Content.ReadAsStringAsync();

        this.Log().Debug($"Webservice: [ConsultaNsu][Resposta] - {strResponse}");

        GravarArquivoEmDisco(strResponse, $"ConsultaNsu-{nsu:000000}-resp.json", "");

        return NFSeResponse<RespostaConsultaDFe>.Create("", "", strResponse, httpResponse.IsSuccessStatusCode);
    }

    /// <summary>
    /// Distribui os DF-e vinculados à chave de acesso informada.
    /// </summary>
    /// <param name="chave">Chave de acesso da NFS-e.</param>
    /// <returns>Resposta da consulta contendo os DF-e.</returns>
    public override async Task<NFSeResponse<RespostaConsultaDFe>> ConsultaChaveAsync(string chave)
    {
        this.Log().Debug($"Webservice: [ConsultaChave][Envio] - {chave}");

        var url = ServiceInfo[Configuracao.WebServices.Ambiente][TipoUrl.ConsultarChave];
        var httpResponse = await SendAsync(null, HttpMethod.Get, $"{url}/NFSe/{chave}/Eventos");

        var strResponse = await httpResponse.Content.ReadAsStringAsync();

        this.Log().Debug($"Webservice: [ConsultaChave][Resposta] - {strResponse}");

        GravarArquivoEmDisco(strResponse, $"ConsultaChave-{chave}-resp.json", "");

        return NFSeResponse<RespostaConsultaDFe>.Create("", "", strResponse, httpResponse.IsSuccessStatusCode);
    }

    #endregion DFe

    #region DPS

    /// <summary>
    /// Retorna a chave de acesso da NFS-e a partir do identificador do DPS.
    /// </summary>
    /// <param name="id">Identificação do DPS.</param>
    /// <returns>Resposta da consulta contendo a chave de acesso.</returns>
    public override async Task<NFSeResponse<RespostaConsultaChaveDps>> ConsultaChaveDpsAsync(string id)
    {
        this.Log().Debug($"Webservice: [ConsultaChaveDps][Envio] - {id}");

        var url = ServiceInfo[Configuracao.WebServices.Ambiente][TipoUrl.ConsultarChaveDps];
        var httpResponse = await SendAsync(null, HttpMethod.Get, $"{url}/dps/{id}");

        var strResponse = await httpResponse.Content.ReadAsStringAsync();

        this.Log().Debug($"Webservice: [ConsultaChaveDps][Resposta] - {strResponse}");

        GravarArquivoEmDisco(strResponse, $"ConsultaChaveDps-{id}-resp.json", "");

        return NFSeResponse<RespostaConsultaChaveDps>.Create("", "", strResponse, httpResponse.IsSuccessStatusCode);
    }

    /// <summary>
    /// Verifica se uma NFS-e foi emitida a partir do Id do DPS.
    /// </summary>
    /// <param name="id">Identificação do DPS.</param>
    /// <returns>True se existir, caso contrário false.</returns>
    public override async Task<bool> ConsultaExisteDpsAsync(string id)
    {
        this.Log().Debug($"Webservice: [ConsultaExisteDps][Envio] - {id}");

        var url = ServiceInfo[Configuracao.WebServices.Ambiente][TipoUrl.ConsultaExisteDps];
        var httpResponse = await SendAsync(null, HttpMethod.Head, $"{url}/dps/{id}");

        var strResponse = await httpResponse.Content.ReadAsStringAsync();

        this.Log().Debug($"Webservice: [ConsultaExisteDps][Resposta] - {strResponse}");

        GravarArquivoEmDisco(strResponse, $"ConsultaChaveDps-{id}-resp.json", "");

        return httpResponse.StatusCode == HttpStatusCode.OK;
    }

    #endregion DPS

    #region Eventos

    /// <summary>
    /// Recepciona o Pedido de Registro de Evento e gera Eventos de NFS-e, crédito, débito e apuração.
    /// </summary>
    /// <param name="evento">Evento a ser enviado.</param>
    /// <returns>Resposta do envio do evento.</returns>
    public override async Task<NFSeResponse<RespostaEnvioEvento>> EnviarEventoAsync(PedidoRegistroEvento evento)
    {
        evento.Assinar(Configuracao);

        ValidarSchema(SchemaNFSe.Evento, evento.Xml, evento.Versao);

        var documento = evento.Informacoes.CPFAutor ?? evento.Informacoes.CNPJAutor;

        GravarDpsEmDisco(evento.Xml, $"{evento.Informacoes.ChNFSe}{evento.Informacoes.Evento}_evento.xml",
            documento, evento.Informacoes.DhEvento.DateTime);

        var envio = new EventoEnvio
        {
            XmlEvento = evento.Xml
        };

        var content = JsonContent.Create(envio);
        var strEnvio = await content.ReadAsStringAsync();

        this.Log().Debug($"Webservice: [Evento][Envio] - {strEnvio}");

        GravarArquivoEmDisco(strEnvio, $"Evento-{evento.Informacoes.ChNFSe}{evento.Informacoes.Evento}-env.json",
            documento);

        var url = ServiceInfo[Configuracao.WebServices.Ambiente][TipoUrl.EnviarEvento];
        var httpResponse = await SendAsync(content, HttpMethod.Post, $"{url}/nfse/{evento.Informacoes.ChNFSe}/eventos");

        var strResponse = await httpResponse.Content.ReadAsStringAsync();

        this.Log().Debug($"Webservice: [Evento][Resposta] - {strResponse}");

        GravarArquivoEmDisco(strResponse, $"Evento-{evento.Informacoes.ChNFSe}{evento.Informacoes.Evento}-resp.json",
            documento);

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        return NFSeResponse<RespostaEnvioEvento>.Create(evento.Xml, strEnvio, strResponse,
            httpResponse.IsSuccessStatusCode, jsonOptions);
    }

    #endregion Eventos

    #region NFS-e

    /// <summary>
    /// Recepciona a DPS e gera a NFS-e de forma síncrona.
    /// </summary>
    /// <param name="dps">DPS a ser enviada.</param>
    /// <returns>Resposta do envio da DPS.</returns>
    public override async Task<NFSeResponse<RespostaEnvioDps>> EnviarAsync(Dps dps)
    {
        dps.Assinar(Configuracao);

        ValidarSchema(SchemaNFSe.DPS, dps.Xml, dps.Versao);

        var documento = dps.Informacoes.Prestador.CPF ?? dps.Informacoes.Prestador.CNPJ;

        GravarDpsEmDisco(dps.Xml, $"{dps.Informacoes.NumeroDps:000000}_dps.xml",
            documento, dps.Informacoes.DhEmissao.DateTime);

        var envio = new DpsEnvio
        {
            XmlDps = dps.Xml
        };

        var content = JsonContent.Create(envio);
        var strEnvio = await content.ReadAsStringAsync();

        this.Log().Debug($"Webservice: [Enviar][Envio] - {strEnvio}");

        GravarArquivoEmDisco(strEnvio, $"Enviar-{dps.Informacoes.NumeroDps:000000}-env.json", documento);

        var url = ServiceInfo[Configuracao.WebServices.Ambiente][TipoUrl.Enviar];
        var httpResponse = await SendAsync(content, HttpMethod.Post, $"{url}/nfse");

        var strResponse = await httpResponse.Content.ReadAsStringAsync();

        this.Log().Debug($"Webservice: [Enviar][Resposta] - {strResponse}");

        GravarArquivoEmDisco(strResponse, $"Enviar-{dps.Informacoes.NumeroDps:000000}-resp.json", documento);

        return NFSeResponse<RespostaEnvioDps>.Create(dps.Xml, strEnvio, strResponse, httpResponse.IsSuccessStatusCode);
    }

    #endregion NFS-e

    #endregion Methods
}