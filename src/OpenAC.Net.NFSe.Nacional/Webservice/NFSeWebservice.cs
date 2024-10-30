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
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using OpenAC.Net.Core.Logging;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional.Webservice;

/// <summary>
/// Classe para comunicação com o webservice da NFSe.
/// </summary>
public sealed class NFSeWebservice : IOpenLog
{
    #region Internal Types

    private enum TipoArquivo
    {
        Webservice,
        Rps,
        NFSe
    }
    
    #endregion Internal Types
    
    #region Fields

    private readonly ConfiguracaoNFSe configuracao;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Inicializa uma nova intancia da classe.
    /// </summary>
    /// <param name="configuracaoNFSe"></param>
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
    /// <param name="chave">Chave de acesso</param>
    /// <returns>Byte array da DANFSe</returns>
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
    /// <param name="nsu">Numero Nsu</param>
    /// <returns>Dados da consulta</returns>
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
    /// Distribui os DF-e vinculados à chave de acesso informada
    /// </summary>
    /// <param name="chave">chave da NFSe</param>
    /// <returns>Dados da consulta</returns>
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
    /// <param name="id">Identificação da Dps</param>
    /// <returns>Dados da consulta</returns>
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
    /// <param name="id">Identificação da Dps</param>
    /// <returns></returns>
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
    /// <param name="evento">Evento</param>
    /// <returns></returns>
    public async Task<NFSeResponse<RespostaEnvioEvento>> EnviarEventoAsync(PedidoRegistroEvento evento)
    {
        evento.Assinar(configuracao);
        
        ValidarSchema(SchemaNFSe.Evento, evento.Xml);
        
        var documento = evento.Informacoes.CPFAutor ?? evento.Informacoes.CNPJAutor;
        
        GravarDpsEmDisco(evento.Xml, $"{evento.Informacoes.NumeroPedido:000000}_evento.xml", 
            documento, evento.Informacoes.DhEvento.DateTime);
        
        var envio = new EventoEnvio()
        {
            XmlEvento = evento.Xml
        };
        
        var content = JsonContent.Create(envio);
        var strEnvio = await content.ReadAsStringAsync();
        
        this.Log().Debug($"Webservice: [Evento][Envio] - {strEnvio}");
        
        GravarArquivoEmDisco(strEnvio, $"Evento-{evento.Informacoes.NumeroPedido:000000}-env.json", documento);
        
        var url = NFSeServiceManager.Instance[DFeTipoEmissao.Normal][configuracao.WebServices.Ambiente, DFeSiglaUF.AN][TipoServico.Sefin];
        var httpResponse = await SendAsync(content, HttpMethod.Post, $"{url}/nfse/{evento.Informacoes.ChNFSe}/eventos");
        
        var strResponse = await httpResponse.Content.ReadAsStringAsync();
        
        this.Log().Debug($"Webservice: [Evento][Resposta] - {strResponse}");
        
        GravarArquivoEmDisco(strResponse, $"Evento-{evento.Informacoes.NumeroPedido:000000}-resp.json", documento);

        return new NFSeResponse<RespostaEnvioEvento>(evento.Xml, strEnvio, strResponse, httpResponse.IsSuccessStatusCode);
    }

    #endregion Eventos

    #region NFS-e

    /// <summary>
    /// Recepciona a DPS e Gera a NFS-e de forma síncrona.
    /// </summary>
    /// <param name="dps"></param>
    /// <returns></returns>
    public async Task<NFSeResponse<RespostaEnvioDps>> EnviarAsync(Dps dps)
    {
        dps.Assinar(configuracao);
        
        ValidarSchema(SchemaNFSe.DPS, dps.Xml);
        
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
    /// <param name="xml"></param>
    /// <returns>Se estiver tudo OK retorna null, caso contrário as mensagens de alertas e erros.</returns>
    private void ValidarSchema(SchemaNFSe schema, string xml)
    {
        var schemaFile = configuracao.Arquivos.GetSchema(schema);
        if (!File.Exists(schemaFile))
            throw new XmlSchemaException("Nao encontrou o arquivo schema do xml => " + schemaFile);
            
        if (XmlSchemaValidation.ValidarXml(xml, schemaFile, out var errosSchema, out _)) return;

        throw new XmlSchemaValidationException("Erros gerado ao validar o schema do xml" + Environment.NewLine + 
                                               string.Join(Environment.NewLine, errosSchema));
    }

    /// <summary>
    /// Grava o xml da Dps no disco
    /// </summary>
    /// <param name="conteudoArquivo"></param>
    /// <param name="nomeArquivo"></param>
    /// <param name="documento"></param>
    /// <param name="data"></param>
    private void GravarDpsEmDisco(string conteudoArquivo, string nomeArquivo, string? documento, DateTime data)
    {
        if (configuracao.Arquivos.Salvar == false) return;

        GravarArquivoEmDisco(TipoArquivo.Rps, conteudoArquivo, nomeArquivo, documento, data);
    }

    /// <summary>
    /// Grava o xml da NFSe no disco
    /// </summary>
    /// <param name="conteudoArquivo"></param>
    /// <param name="nomeArquivo"></param>
    /// <param name="documento"></param>
    /// <param name="data"></param>
    private void GravarNFSeEmDisco(string conteudoArquivo, string nomeArquivo, string? documento, DateTime data)
    {
        if (configuracao.Arquivos.Salvar == false) return;

        GravarArquivoEmDisco(TipoArquivo.NFSe, conteudoArquivo, nomeArquivo, documento, data);
    }

    /// <summary>
    /// Grava o xml de comunicação com o webservice no disco
    /// </summary>
    /// <param name="conteudoArquivo"></param>
    /// <param name="nomeArquivo"></param>
    /// <param name="documento"></param>
    private void GravarArquivoEmDisco(string conteudoArquivo, string nomeArquivo, string? documento)
    {
        if (configuracao.Geral.Salvar == false) return;

        GravarArquivoEmDisco(TipoArquivo.Webservice, conteudoArquivo, nomeArquivo, documento);
    }
    
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