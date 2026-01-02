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

using OpenAC.Net.Core.Logging;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.Common.Types;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace OpenAC.Net.NFSe.Nacional.Webservice;

/// <summary>
/// Classe responsável pela comunicação com o webservice da NFSe Nacional.
/// </summary>
public sealed class NFSeWebservice : IOpenLog
{
    #region Internal Types

    /// <summary>
    /// Tipos de arquivos manipulados pelo webservice.
    /// </summary>
    private enum TipoArquivo
    {
        /// <summary>
        /// Arquivo de comunicação com o webservice.
        /// </summary>
        Webservice,
        /// <summary>
        /// Arquivo de RPS (Recibo Provisório de Serviços).
        /// </summary>
        Rps,
        /// <summary>
        /// Arquivo de NFS-e (Nota Fiscal de Serviço eletrônica).
        /// </summary>
        NFSe
    }

    #endregion Internal Types

    #region Fields

    /// <summary>
    /// Configuração utilizada pelo webservice.
    /// </summary>
    private readonly ConfiguracaoNFSe configuracao;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="NFSeWebservice"/>.
    /// </summary>
    /// <param name="configuracaoNFSe">Configuração da NFSe.</param>
    public NFSeWebservice(ConfiguracaoNFSe configuracaoNFSe)
    {
        configuracao = configuracaoNFSe;
    }

    #endregion Constructors

    #region Methods

    #region DANFSe

    /// <summary>
    /// Retorna o DANFSe de uma NFS-e a partir de sua chave de acesso.
    /// </summary>
    /// <param name="chave">Chave de acesso da NFS-e.</param>
    /// <returns>Array de bytes contendo o DANFSe.</returns>
    public async Task<byte[]> DownloadDANFSeAsync(string chave)
    {
        this.Log().Debug($"Webservice: [DANFSe][Envio] - {chave}");

        var url = NFSeServiceManager.Instance[DFeTipoEmissao.Normal][configuracao.WebServices.Ambiente, DFeSiglaUF.AN][TipoServico.Sefin];
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
    public async Task<NFSeResponse<RespostaConsultaDFe>> ConsultaNsuAsync(int nsu)
    {
        this.Log().Debug($"Webservice: [ConsultaNsu][Envio] - {nsu}");

        var url = NFSeServiceManager.Instance[DFeTipoEmissao.Normal][configuracao.WebServices.Ambiente, DFeSiglaUF.AN][TipoServico.Adn];
        var httpResponse = await SendAsync(null, HttpMethod.Get, $"{url}/DFe/{nsu}");

        var strResponse = await httpResponse.Content.ReadAsStringAsync();

        this.Log().Debug($"Webservice: [ConsultaNsu][Resposta] - {strResponse}");

        GravarArquivoEmDisco(strResponse, $"ConsultaNsu-{nsu:000000}-resp.json", "");

        return new NFSeResponse<RespostaConsultaDFe>("", "", strResponse, httpResponse.IsSuccessStatusCode);
    }

    /// <summary>
    /// Distribui os DF-e vinculados à chave de acesso informada.
    /// </summary>
    /// <param name="chave">Chave de acesso da NFS-e.</param>
    /// <returns>Resposta da consulta contendo os DF-e.</returns>
    public async Task<NFSeResponse<RespostaConsultaDFe>> ConsultaChaveAsync(string chave)
    {
        this.Log().Debug($"Webservice: [ConsultaChave][Envio] - {chave}");

        var url = NFSeServiceManager.Instance[DFeTipoEmissao.Normal][configuracao.WebServices.Ambiente, DFeSiglaUF.AN][TipoServico.Adn];
        var httpResponse = await SendAsync(null, HttpMethod.Get, $"{url}/NFSe/{chave}/Eventos");

        var strResponse = await httpResponse.Content.ReadAsStringAsync();

        this.Log().Debug($"Webservice: [ConsultaChave][Resposta] - {strResponse}");

        GravarArquivoEmDisco(strResponse, $"ConsultaChave-{chave}-resp.json", "");

        return new NFSeResponse<RespostaConsultaDFe>("", "", strResponse, httpResponse.IsSuccessStatusCode);
    }

    #endregion DFe

    #region DPS

    /// <summary>
    /// Retorna a chave de acesso da NFS-e a partir do identificador do DPS.
    /// </summary>
    /// <param name="id">Identificação do DPS.</param>
    /// <returns>Resposta da consulta contendo a chave de acesso.</returns>
    public async Task<NFSeResponse<RespostaConsultaChaveDps>> ConsultaChaveDpsAsync(string id)
    {
        this.Log().Debug($"Webservice: [ConsultaChaveDps][Envio] - {id}");

        var url = NFSeServiceManager.Instance[DFeTipoEmissao.Normal][configuracao.WebServices.Ambiente, DFeSiglaUF.AN][TipoServico.Sefin];
        var httpResponse = await SendAsync(null, HttpMethod.Get, $"{url}/dps/{id}");

        var strResponse = await httpResponse.Content.ReadAsStringAsync();

        this.Log().Debug($"Webservice: [ConsultaChaveDps][Resposta] - {strResponse}");

        GravarArquivoEmDisco(strResponse, $"ConsultaChaveDps-{id}-resp.json", "");

        return new NFSeResponse<RespostaConsultaChaveDps>("", "", strResponse, httpResponse.IsSuccessStatusCode);
    }

    /// <summary>
    /// Verifica se uma NFS-e foi emitida a partir do Id do DPS.
    /// </summary>
    /// <param name="id">Identificação do DPS.</param>
    /// <returns>True se existir, caso contrário false.</returns>
    public async Task<bool> ConsultaExisteDpsAsync(string id)
    {
        this.Log().Debug($"Webservice: [ConsultaExisteDps][Envio] - {id}");

        var url = NFSeServiceManager.Instance[DFeTipoEmissao.Normal][configuracao.WebServices.Ambiente, DFeSiglaUF.AN][TipoServico.Sefin];
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
    public async Task<NFSeResponse<RespostaEnvioEvento>> EnviarEventoAsync(PedidoRegistroEvento evento)
    {
        evento.Assinar(configuracao);

        ValidarSchema(SchemaNFSe.Evento, evento.Xml, evento.Versao);

        var documento = evento.Informacoes.CPFAutor ?? evento.Informacoes.CNPJAutor;

        GravarDpsEmDisco(evento.Xml, $"{evento.Informacoes.ChNFSe}{evento.Informacoes.Evento}_evento.xml",
            documento, evento.Informacoes.DhEvento.DateTime);

        var envio = new EventoEnvio()
        {
            XmlEvento = evento.Xml
        };

        var content = JsonContent.Create(envio);
        var strEnvio = await content.ReadAsStringAsync();

        this.Log().Debug($"Webservice: [Evento][Envio] - {strEnvio}");

        GravarArquivoEmDisco(strEnvio, $"Evento-{evento.Informacoes.ChNFSe}{evento.Informacoes.Evento}-env.json", documento);

        var url = NFSeServiceManager.Instance[DFeTipoEmissao.Normal][configuracao.WebServices.Ambiente, DFeSiglaUF.AN][TipoServico.Sefin];
        var httpResponse = await SendAsync(content, HttpMethod.Post, $"{url}/nfse/{evento.Informacoes.ChNFSe}/eventos");

        var strResponse = await httpResponse.Content.ReadAsStringAsync();

        this.Log().Debug($"Webservice: [Evento][Resposta] - {strResponse}");

        GravarArquivoEmDisco(strResponse, $"Evento-{evento.Informacoes.ChNFSe}{evento.Informacoes.Evento}-resp.json", documento);

        return new NFSeResponse<RespostaEnvioEvento>(evento.Xml, strEnvio, strResponse, httpResponse.IsSuccessStatusCode);
    }

    #endregion Eventos

    #region NFS-e

    /// <summary>
    /// Recepciona a DPS e gera a NFS-e de forma síncrona.
    /// </summary>
    /// <param name="dps">DPS a ser enviada.</param>
    /// <returns>Resposta do envio da DPS.</returns>
    public async Task<NFSeResponse<RespostaEnvioDps>> EnviarAsync(Dps dps)
    {
        dps.Assinar(configuracao);

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

        var url = NFSeServiceManager.Instance[DFeTipoEmissao.Normal][configuracao.WebServices.Ambiente, DFeSiglaUF.AN][TipoServico.Sefin];
        var httpResponse = await SendAsync(content, HttpMethod.Post, $"{url}/nfse");

        var strResponse = await httpResponse.Content.ReadAsStringAsync();

        this.Log().Debug($"Webservice: [Enviar][Resposta] - {strResponse}");

        GravarArquivoEmDisco(strResponse, $"Enviar-{dps.Informacoes.NumeroDps:000000}-resp.json", documento);

        return new NFSeResponse<RespostaEnvioDps>(dps.Xml, strEnvio, strResponse, httpResponse.IsSuccessStatusCode);

    }

    #endregion NFS-e

    #region Commom

    /// <summary>
    /// Envia uma requisição HTTP para o webservice.
    /// </summary>
    /// <param name="content">Conteúdo da requisição.</param>
    /// <param name="method">Método HTTP.</param>
    /// <param name="url">URL do serviço.</param>
    /// <returns>Resposta HTTP.</returns>
    private async Task<HttpResponseMessage> SendAsync(HttpContent? content, HttpMethod method, string url)
    {
        var handler = new HttpClientHandler();

        handler.SslProtocols = (SslProtocols)configuracao.WebServices.Protocolos;
        handler.ClientCertificates.Add(configuracao.Certificados.ObterCertificado());
        var client = new HttpClient(handler);

        var request = new HttpRequestMessage(method, url);

        var assemblyName = GetType().Assembly.GetName();
        var productValue = new ProductInfoHeaderValue("OpenAC.Net.NFSe.Nacional", assemblyName!.Version!.ToString());
        var commentValue = new ProductInfoHeaderValue("(+https://github.com/OpenAC-Net/OpenAC.Net.NFSe.Nacional)");

        request.Headers.UserAgent.Add(productValue);
        request.Headers.UserAgent.Add(commentValue);
        request.Content = content;

        return await client.SendAsync(request);
    }

    /// <summary>
    /// Valida o XML de acordo com o schema.
    /// </summary>
    /// <param name="schema">O schema que será usado na verificação.</param>
    /// <param name="xml">Conteúdo XML a ser validado.</param>
    /// <param name="versao">Versão do Schema a ser carregado</param>
    /// <exception cref="XmlSchemaException">Lançada se o arquivo de schema não for encontrado.</exception>
    /// <exception cref="XmlSchemaValidationException">Lançada se houver erros de validação.</exception>
    private void ValidarSchema(SchemaNFSe schema, string xml, VersaoNFSe versao = VersaoNFSe.Ve100)
    {
        configuracao.Arquivos.VersaoSchema = versao;
        var schemaFile = configuracao.Arquivos.GetSchema(schema);
        if (!File.Exists(schemaFile))
            throw new XmlSchemaException("Nao encontrou o arquivo schema do xml => " + schemaFile);

        if (XmlSchemaValidation.ValidarXml(xml, schemaFile, out var errosSchema, out _)) return;

        throw new XmlSchemaValidationException("Erros gerado ao validar o schema do xml" + Environment.NewLine +
                                               string.Join(Environment.NewLine, errosSchema));
    }

    /// <summary>
    /// Grava o xml da Dps no disco.
    /// </summary>
    /// <param name="conteudoArquivo">Conteúdo do arquivo.</param>
    /// <param name="nomeArquivo">Nome do arquivo.</param>
    /// <param name="documento">Documento do prestador.</param>
    /// <param name="data">Data de emissão.</param>
    private void GravarDpsEmDisco(string conteudoArquivo, string nomeArquivo, string? documento, DateTime data)
    {
        if (configuracao.Arquivos.Salvar == false) return;

        GravarArquivoEmDisco(TipoArquivo.Rps, conteudoArquivo, nomeArquivo, documento, data);
    }

    /// <summary>
    /// Grava o xml da NFSe no disco.
    /// </summary>
    /// <param name="conteudoArquivo">Conteúdo do arquivo.</param>
    /// <param name="nomeArquivo">Nome do arquivo.</param>
    /// <param name="documento">Documento do prestador.</param>
    /// <param name="data">Data de emissão.</param>
    private void GravarNFSeEmDisco(string conteudoArquivo, string nomeArquivo, string? documento, DateTime data)
    {
        if (configuracao.Arquivos.Salvar == false) return;

        GravarArquivoEmDisco(TipoArquivo.NFSe, conteudoArquivo, nomeArquivo, documento, data);
    }

    /// <summary>
    /// Grava o xml de comunicação com o webservice no disco.
    /// </summary>
    /// <param name="conteudoArquivo">Conteúdo do arquivo.</param>
    /// <param name="nomeArquivo">Nome do arquivo.</param>
    /// <param name="documento">Documento do prestador.</param>
    private void GravarArquivoEmDisco(string conteudoArquivo, string nomeArquivo, string? documento)
    {
        if (configuracao.Geral.Salvar == false) return;

        GravarArquivoEmDisco(TipoArquivo.Webservice, conteudoArquivo, nomeArquivo, documento);
    }

    /// <summary>
    /// Grava o arquivo no disco conforme o tipo especificado.
    /// </summary>
    /// <param name="tipo">Tipo do arquivo.</param>
    /// <param name="conteudoArquivo">Conteúdo do arquivo.</param>
    /// <param name="nomeArquivo">Nome do arquivo.</param>
    /// <param name="documento">Documento do prestador.</param>
    /// <param name="data">Data de emissão (opcional).</param>
    private void GravarArquivoEmDisco(TipoArquivo tipo, string conteudoArquivo, string nomeArquivo, string? documento, DateTime? data = null)
    {
        nomeArquivo = tipo switch
        {
            TipoArquivo.Webservice => Path.Combine(configuracao.Arquivos.GetPathEnvio(data ?? DateTime.Today, documento ?? string.Empty), nomeArquivo),
            TipoArquivo.Rps => Path.Combine(configuracao.Arquivos.GetPathDps(data ?? DateTime.Today, documento ?? string.Empty), nomeArquivo),
            TipoArquivo.NFSe => Path.Combine(configuracao.Arquivos.GetPathNFSe(data ?? DateTime.Today, documento ?? string.Empty), nomeArquivo),
            _ => throw new ArgumentOutOfRangeException(nameof(tipo), tipo, null)
        };

        File.WriteAllText(nomeArquivo, conteudoArquivo, Encoding.UTF8);
    }


    #endregion Commom

    #endregion Methods
}