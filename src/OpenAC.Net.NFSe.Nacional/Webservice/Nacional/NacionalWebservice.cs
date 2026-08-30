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

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.Core.Logging;
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

    /// <summary>Inicializa o provedor com um transporte HTTP fornecido pelo host.</summary>
    public NacionalWebservice(ConfiguracaoNFSe configuracaoNFSe, NFSeServiceInfo serviceInfo,
        INFSeHttpTransport httpTransport) : base(configuracaoNFSe, serviceInfo, httpTransport)
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
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    public override async Task<byte[]> DownloadDANFSeAsync(string chave, CancellationToken cancellationToken = default)
    {
        this.Log().Debug($"Webservice: [DANFSe][Envio] - {chave}");

        var url = ServiceInfo[Configuracao.WebServices.Ambiente][TipoUrl.DownloadDanfse];
        using var httpResponse = await SendAsync(null, HttpMethod.Get, $"{url}/danfse/{chave}", cancellationToken: cancellationToken);

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
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    public override async Task<NFSeResponse<RespostaConsultaDFe>> ConsultaNsuAsync(int nsu, CancellationToken cancellationToken = default)
    {
        this.Log().Debug($"Webservice: [ConsultaNsu][Envio] - {nsu}");

        var url = ServiceInfo[Configuracao.WebServices.Ambiente][TipoUrl.ConsultarNsu];
        using var httpResponse = await SendAsync(null, HttpMethod.Get, $"{url}/DFe/{nsu}", cancellationToken: cancellationToken);

        var strResponse = await httpResponse.Content.ReadAsStringAsync();

        this.Log().Debug($"Webservice: [ConsultaNsu][Resposta] - {strResponse}");

        await GravarArquivoEmDiscoAsync(strResponse, $"ConsultaNsu-{nsu:000000}-resp.json", "", cancellationToken);

        return NFSeResponse<RespostaConsultaDFe>.Create("", "", strResponse, httpResponse.IsSuccessStatusCode);
    }

    /// <summary>
    /// Distribui os DF-e vinculados à chave de acesso informada.
    /// </summary>
    /// <param name="chave">Chave de acesso da NFS-e.</param>
    /// <returns>Resposta da consulta contendo os DF-e.</returns>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    public override async Task<NFSeResponse<RespostaConsultaDFe>> ConsultaChaveAsync(string chave, CancellationToken cancellationToken = default)
    {
        this.Log().Debug($"Webservice: [ConsultaChave][Envio] - {chave}");

        var url = ServiceInfo[Configuracao.WebServices.Ambiente][TipoUrl.ConsultarChave];
        using var httpResponse = await SendAsync(null, HttpMethod.Get, $"{url}/NFSe/{chave}/Eventos", cancellationToken: cancellationToken);

        var strResponse = await httpResponse.Content.ReadAsStringAsync();

        this.Log().Debug($"Webservice: [ConsultaChave][Resposta] - {strResponse}");

        await GravarArquivoEmDiscoAsync(strResponse, $"ConsultaChave-{chave}-resp.json", "", cancellationToken);

        return NFSeResponse<RespostaConsultaDFe>.Create("", "", strResponse, httpResponse.IsSuccessStatusCode);
    }

    #endregion DFe

    #region DPS

    /// <summary>
    /// Retorna a chave de acesso da NFS-e a partir do identificador do DPS.
    /// </summary>
    /// <param name="id">Identificação do DPS.</param>
    /// <returns>Resposta da consulta contendo a chave de acesso.</returns>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    public override async Task<NFSeResponse<RespostaConsultaChaveDps>> ConsultaChaveDpsAsync(string id, CancellationToken cancellationToken = default)
    {
        this.Log().Debug($"Webservice: [ConsultaChaveDps][Envio] - {id}");

        var url = ServiceInfo[Configuracao.WebServices.Ambiente][TipoUrl.ConsultarChaveDps];
        using var httpResponse = await SendAsync(null, HttpMethod.Get, $"{url}/dps/{id}", cancellationToken: cancellationToken);

        var strResponse = await httpResponse.Content.ReadAsStringAsync();

        this.Log().Debug($"Webservice: [ConsultaChaveDps][Resposta] - {strResponse}");

        await GravarArquivoEmDiscoAsync(strResponse, $"ConsultaChaveDps-{id}-resp.json", "", cancellationToken);

        return NFSeResponse<RespostaConsultaChaveDps>.Create("", "", strResponse, httpResponse.IsSuccessStatusCode);
    }

    /// <summary>
    /// Verifica se uma NFS-e foi emitida a partir do Id do DPS.
    /// </summary>
    /// <param name="id">Identificação do DPS.</param>
    /// <returns>True se existir, caso contrário false.</returns>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    public override async Task<bool> ConsultaExisteDpsAsync(string id, CancellationToken cancellationToken = default)
    {
        this.Log().Debug($"Webservice: [ConsultaExisteDps][Envio] - {id}");

        var url = ServiceInfo[Configuracao.WebServices.Ambiente][TipoUrl.ConsultaExisteDps];
        using var httpResponse = await SendAsync(null, HttpMethod.Head, $"{url}/dps/{id}", cancellationToken: cancellationToken);

        var strResponse = await httpResponse.Content.ReadAsStringAsync();

        this.Log().Debug($"Webservice: [ConsultaExisteDps][Resposta] - {strResponse}");

        await GravarArquivoEmDiscoAsync(strResponse, $"ConsultaChaveDps-{id}-resp.json", "", cancellationToken);

        return httpResponse.StatusCode == HttpStatusCode.OK;
    }

    #endregion DPS

    #region Eventos

    /// <inheritdoc/>
    public override async Task<NFSeResponse<RespostaConsultaEvento>> ConsultaEventoAsync(string chaveAcesso, string tipoEvento, int numSeqEvento)
    {
        this.Log().Debug($"Webservice: [ConsultaEvento][Envio] - {chaveAcesso}, {tipoEvento}, {numSeqEvento}");

        var url = ServiceInfo[Configuracao.WebServices.Ambiente][TipoUrl.ConsultarEvento];
        var httpResponse = await SendAsync(null, HttpMethod.Get, $"{url}/nfse/{chaveAcesso}/eventos/{tipoEvento}/{numSeqEvento}");

        var strResponse = await httpResponse.Content.ReadAsStringAsync();

        this.Log().Debug($"Webservice: [ConsultaEvento][Resposta] - {strResponse}");

        GravarArquivoEmDisco(strResponse, $"ConsultaEvento-{chaveAcesso}{tipoEvento}{numSeqEvento}-resp.json", "");

        return NFSeResponse<RespostaConsultaEvento>.Create("", "", strResponse, httpResponse.IsSuccessStatusCode);
    }

    /// <summary>
    /// Recepciona o Pedido de Registro de Evento e gera Eventos de NFS-e, crédito, débito e apuração.
    /// </summary>
    /// <param name="evento">Evento a ser enviado.</param>
    /// <returns>Resposta do envio do evento.</returns>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    public override async Task<NFSeResponse<RespostaEnvioEvento>> EnviarEventoAsync(PedidoRegistroEvento evento, CancellationToken cancellationToken = default)
    {
        if (Configuracao.Geral.AssinarXml)
            evento.Assinar(Configuracao);

        ValidarSchema(SchemaNFSe.Evento, evento.Xml, evento.Versao);

        var documento = evento.Informacoes.CPFAutor ?? evento.Informacoes.CNPJAutor;

        var prefixoNomeArquivoDps = Configuracao.Arquivos.PadronizarNomes
            ? evento.Informacoes.Id
            : $"{evento.Informacoes.ChNFSe}{evento.Informacoes.Evento}";

        await GravarDpsEmDiscoAsync(evento.Xml, $"{prefixoNomeArquivoDps}_evento.xml",
                documento, evento.Informacoes.DhEvento.DateTime, true, cancellationToken);

        var envio = new EventoEnvio
        {
            XmlEvento = evento.Xml
        };

        var content = JsonContent.Create(envio);
        var strEnvio = await content.ReadAsStringAsync();

        this.Log().Debug($"Webservice: [Evento][Envio] - {strEnvio}");

        await GravarArquivoEmDiscoAsync(strEnvio, $"Evento-{prefixoNomeArquivoDps}-env.json",
            documento, cancellationToken);

        var url = ServiceInfo[Configuracao.WebServices.Ambiente][TipoUrl.EnviarEvento];
        using var httpResponse = await SendAsync(content, HttpMethod.Post, $"{url}/nfse/{evento.Informacoes.ChNFSe}/eventos", cancellationToken: cancellationToken);

        var strResponse = await httpResponse.Content.ReadAsStringAsync();

        this.Log().Debug($"Webservice: [Evento][Resposta] - {strResponse}");

        await GravarArquivoEmDiscoAsync(strResponse, $"Evento-{prefixoNomeArquivoDps}-resp.json",
            documento, cancellationToken);

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        var retorno = NFSeResponse<RespostaEnvioEvento>.Create(evento.Xml, strEnvio, strResponse, httpResponse.IsSuccessStatusCode, jsonOptions);

        if (retorno is not { Sucesso: true, Resultado.XmlEvento: not null }) return retorno;
        
        var prefixoNomeArquivoEventoNfse = Configuracao.Arquivos.PadronizarNomes
            ? evento.Informacoes.ChNFSe
            : $"{evento.Informacoes.ChNFSe}{evento.Informacoes.Evento}";

        var nSeqEvento = XDocument.Parse(retorno.Resultado.XmlEvento)
            .Descendants()
            .FirstOrDefault(x => x.Name.LocalName == "nSeqEvento")?.Value ?? "00";

        await GravarNFSeEmDiscoAsync(retorno.Resultado.XmlEvento, $"{prefixoNomeArquivoEventoNfse}_evento_{nSeqEvento}.xml", documento, evento.Informacoes.DhEvento.DateTime, true, cancellationToken);

        return retorno;
    }
    
    /// <inheritdoc/>
    public override async Task<NFSeResponse<RespostaConsultaEvento>> ConsultaEventoAsync(string chaveAcesso, string tipoEvento, int numSeqEvento, CancellationToken cancellationToken = default)
    {
        this.Log().Debug($"Webservice: [ConsultaEvento][Envio] - {chaveAcesso}, {tipoEvento}, {numSeqEvento}");

        var url = ServiceInfo[Configuracao.WebServices.Ambiente][TipoUrl.ConsultarEvento];
        var httpResponse = await SendAsync(null, HttpMethod.Get, $"{url}/nfse/{chaveAcesso}/eventos/{tipoEvento}/{numSeqEvento}", cancellationToken: cancellationToken);

        var strResponse = await httpResponse.Content.ReadAsStringAsync();

        this.Log().Debug($"Webservice: [ConsultaEvento][Resposta] - {strResponse}");

        await GravarArquivoEmDiscoAsync(strResponse, $"ConsultaEvento-{chaveAcesso}{tipoEvento}{numSeqEvento}-resp.json", "", cancellationToken);

        return NFSeResponse<RespostaConsultaEvento>.Create("", "", strResponse, httpResponse.IsSuccessStatusCode);
    }

    #endregion Eventos

    #region NFS-e

    /// <summary>
    /// Recepciona a DPS e gera a NFS-e de forma síncrona.
    /// </summary>
    /// <param name="dps">DPS a ser enviada.</param>
    /// <returns>Resposta do envio da DPS.</returns>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    public override async Task<NFSeResponse<RespostaEnvioDps>> EnviarAsync(Dps dps, CancellationToken cancellationToken = default)
    {
        if (Configuracao.Geral.AssinarXml)
        {
            Console.WriteLine("Assinando DPS");
            dps.Assinar(Configuracao);
        }
        else
            Console.WriteLine("SKIP assinatura DPS");

        ValidarSchema(SchemaNFSe.DPS, dps.Xml, dps.Versao);

        var documento = dps.Informacoes.Prestador.CPF ?? dps.Informacoes.Prestador.CNPJ;

        var prefixoNomeArquivoDps = Configuracao.Arquivos.PadronizarNomes
            ? dps.Informacoes.Id
            : dps.Informacoes.NumeroDps.ZeroFill(6);

        await GravarDpsEmDiscoAsync(dps.Xml, $"{prefixoNomeArquivoDps}_dps.xml",
            documento, dps.Informacoes.DhEmissao.DateTime, cancellationToken: cancellationToken);

        var envio = new DpsEnvio
        {
            XmlDps = dps.Xml
        };

        var content = JsonContent.Create(envio);
        var strEnvio = await content.ReadAsStringAsync();

        this.Log().Debug($"Webservice: [Enviar][Envio] - {strEnvio}");

        await GravarArquivoEmDiscoAsync(strEnvio, $"Enviar-{prefixoNomeArquivoDps}-env.json", documento, cancellationToken);

        var url = ServiceInfo[Configuracao.WebServices.Ambiente][TipoUrl.Enviar];
        using var httpResponse = await SendAsync(content, HttpMethod.Post, $"{url}/nfse", cancellationToken: cancellationToken);

        var strResponse = await httpResponse.Content.ReadAsStringAsync();

        this.Log().Debug($"Webservice: [Enviar][Resposta] - {strResponse}");

        await GravarArquivoEmDiscoAsync(strResponse, $"Enviar-{prefixoNomeArquivoDps}-resp.json", documento, cancellationToken);

        var retorno = NFSeResponse<RespostaEnvioDps>.Create(dps.Xml, strEnvio, strResponse, httpResponse.IsSuccessStatusCode);

        if (retorno is not { Sucesso: true, Resultado.XmlNFSe: not null }) return retorno;
        
        var prefixoNomeArquivoNfse = Configuracao.Arquivos.PadronizarNomes
            ? retorno.Resultado.ChaveAcesso
            : dps.Informacoes.NumeroDps.ZeroFill(6);

        await GravarNFSeEmDiscoAsync(retorno.Resultado.XmlNFSe, $"{prefixoNomeArquivoNfse}_nfse.xml", documento, dps.Informacoes.DhEmissao.DateTime, cancellationToken: cancellationToken);

        return retorno;
    }

    #endregion NFS-e

    #endregion Methods
}
