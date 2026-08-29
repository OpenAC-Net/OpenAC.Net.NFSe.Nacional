// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
// ***********************************************************************
// <copyright file="OpenNFSeNacional.cs" company="OpenAC .Net">
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
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using OpenAC.Net.Core.Logging;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.Webservice;

namespace OpenAC.Net.NFSe.Nacional;

/// <summary>
/// Classe principal para integração com a NFS-e Nacional.
/// </summary>
public sealed class OpenNFSeNacional : IOpenNFSeNacionalClient, IOpenLog
{
    #region Properties

    /// <summary>
    /// Configurações do Componente.
    /// </summary>
    public ConfiguracaoNFSe Configuracoes { get; } = new();

    #endregion Properties

    #region Methods

    /// <summary>
    /// Recepciona a DPS e gera a NFS-e de forma assíncrona.
    /// </summary>
    /// <param name="dps">Objeto <see cref="Dps"/> a ser enviado.</param>
    /// <returns>Resposta do envio contendo informações da NFS-e gerada.</returns>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    public Task<NFSeResponse<RespostaEnvioDps>> EnviarAsync(Dps dps, CancellationToken cancellationToken = default)
    {
        var provider = NFSeServiceManager.Instance.GetProvider(Configuracoes);
        var oldProtocol = ServicePointManager.SecurityProtocol;
        
        try
        {
            ServicePointManager.SecurityProtocol = Configuracoes.WebServices.Protocolos;
            return provider.EnviarAsync(dps, cancellationToken);
        }
        catch (Exception exception)
        {
            this.Log().Error("[Enviar]", exception);
            throw;
        }
        finally
        {
            ServicePointManager.SecurityProtocol = oldProtocol;
        }
    }

    /// <summary>
    /// Recepciona o Pedido de Registro de Evento e gera eventos de NFS-e, crédito, débito e apuração.
    /// </summary>
    /// <param name="evento">Objeto <see cref="PedidoRegistroEvento"/> representando o evento.</param>
    /// <returns>Resposta do envio do evento.</returns>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    public Task<NFSeResponse<RespostaEnvioEvento>> EnviarEventoAsync(PedidoRegistroEvento evento, CancellationToken cancellationToken = default)
    {
        var provider = NFSeServiceManager.Instance.GetProvider(Configuracoes);
        var oldProtocol = ServicePointManager.SecurityProtocol;
        
        try
        {
            ServicePointManager.SecurityProtocol = Configuracoes.WebServices.Protocolos;
            return provider.EnviarEventoAsync(evento, cancellationToken);
        }
        catch (Exception exception)
        {
            this.Log().Error("[EnviarEvento]", exception);
            throw;
        }
        finally
        {
            ServicePointManager.SecurityProtocol = oldProtocol;
        }
    }

    /// <summary>
    /// Distribui os DF-e para contribuintes relacionados à NFS-e.
    /// </summary>
    /// <param name="nsu">Número do NSU.</param>
    /// <returns>Dados da consulta dos DF-e.</returns>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    public Task<NFSeResponse<RespostaConsultaDFe>> ConsultaNsuAsync(int nsu, CancellationToken cancellationToken = default)
    {
        var provider = NFSeServiceManager.Instance.GetProvider(Configuracoes);
        var oldProtocol = ServicePointManager.SecurityProtocol;
        
        try
        {
            ServicePointManager.SecurityProtocol = Configuracoes.WebServices.Protocolos;
            return provider.ConsultaNsuAsync(nsu, cancellationToken);
        }
        catch (Exception exception)
        {
            this.Log().Error("[ConsultaNsu]", exception);
            throw;
        }
        finally
        {
            ServicePointManager.SecurityProtocol = oldProtocol;
        }
    }

    /// <summary>
    /// Distribui os DF-e vinculados à chave de acesso informada.
    /// </summary>
    /// <param name="chave">Chave de acesso da NFS-e.</param>
    /// <returns>Dados da consulta dos DF-e.</returns>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    public Task<NFSeResponse<RespostaConsultaDFe>> ConsultaChaveAsync(string chave, CancellationToken cancellationToken = default)
    {
        var provider = NFSeServiceManager.Instance.GetProvider(Configuracoes);
        var oldProtocol = ServicePointManager.SecurityProtocol;
        
        try
        {
            ServicePointManager.SecurityProtocol = Configuracoes.WebServices.Protocolos;
            return provider.ConsultaChaveAsync(chave, cancellationToken);
        }
        catch (Exception exception)
        {
            this.Log().Error("[ConsultaChave]", exception);
            throw;
        }
        finally
        {
            ServicePointManager.SecurityProtocol = oldProtocol;
        }
    }

    /// <summary>
    /// Retorna o DANFSe de uma NFS-e a partir de sua chave de acesso.
    /// </summary>
    /// <param name="chave">Chave de acesso da NFS-e.</param>
    /// <returns>Array de bytes contendo o DANFSe.</returns>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    public Task<byte[]> DownloadDANFSeAsync(string chave, CancellationToken cancellationToken = default)
    {
        var provider = NFSeServiceManager.Instance.GetProvider(Configuracoes);
        var oldProtocol = ServicePointManager.SecurityProtocol;
        
        try
        {
            ServicePointManager.SecurityProtocol = Configuracoes.WebServices.Protocolos;
            return provider.DownloadDANFSeAsync(chave, cancellationToken);
        }
        catch (Exception exception)
        {
            this.Log().Error("[DownloadDANFSe]", exception);
            throw;
        }
        finally
        {
            ServicePointManager.SecurityProtocol = oldProtocol;
        }
    }

    /// <summary>
    /// Retorna a chave de acesso da NFS-e a partir do identificador do DPS.
    /// </summary>
    /// <param name="id">Identificação da DPS.</param>
    /// <returns>Dados da consulta da chave de acesso.</returns>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    public Task<NFSeResponse<RespostaConsultaChaveDps>> ConsultaChaveDpsAsync(string id, CancellationToken cancellationToken = default)
    {
        var provider = NFSeServiceManager.Instance.GetProvider(Configuracoes);
        var oldProtocol = ServicePointManager.SecurityProtocol;
        
        try
        {
            ServicePointManager.SecurityProtocol = Configuracoes.WebServices.Protocolos;
            return provider.ConsultaChaveDpsAsync(id, cancellationToken);
        }
        catch (Exception exception)
        {
            this.Log().Error("[ConsultaChaveDps]", exception);
            throw;
        }
        finally
        {
            ServicePointManager.SecurityProtocol = oldProtocol;
        }
    }

    /// <summary>
    /// Verifica se uma NFS-e foi emitida a partir do Id do DPS.
    /// </summary>
    /// <param name="id">Identificação da DPS.</param>
    /// <returns>True se a NFS-e existe, caso contrário, false.</returns>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    public Task<bool> ConsultaExisteDpsAsync(string id, CancellationToken cancellationToken = default)
    {
        var provider = NFSeServiceManager.Instance.GetProvider(Configuracoes);
        var oldProtocol = ServicePointManager.SecurityProtocol;
        
        try
        {
            ServicePointManager.SecurityProtocol = Configuracoes.WebServices.Protocolos;
            return provider.ConsultaExisteDpsAsync(id, cancellationToken);
        }
        catch (Exception exception)
        {
            this.Log().Error("[ConsultaExisteDps]", exception);
            throw;
        }
        finally
        {
            ServicePointManager.SecurityProtocol = oldProtocol;
        }
    }

    /// <summary>
    /// Recepciona um lote de DPS de forma assíncrona, retornando o protocolo para acompanhamento.
    /// </summary>
    /// <param name="lote">Lote de DPS a ser enviado.</param>
    /// <returns>Resposta contendo o protocolo do lote.</returns>
    /// <remarks>Suportado apenas por provedores que expõem envio em lote (ex.: Fiorilli).</remarks>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    public Task<NFSeResponse<RespostaRecepcaoLote>> EnviarLoteAsync(LoteDps lote, CancellationToken cancellationToken = default)
    {
        var provider = NFSeServiceManager.Instance.GetProvider(Configuracoes);
        var oldProtocol = ServicePointManager.SecurityProtocol;

        try
        {
            ServicePointManager.SecurityProtocol = Configuracoes.WebServices.Protocolos;
            return provider.EnviarLoteAsync(lote, cancellationToken);
        }
        catch (Exception exception)
        {
            this.Log().Error("[EnviarLote]", exception);
            throw;
        }
        finally
        {
            ServicePointManager.SecurityProtocol = oldProtocol;
        }
    }

    /// <summary>
    /// Recepciona um lote de DPS de forma síncrona, retornando as NFS-e geradas.
    /// </summary>
    /// <param name="lote">Lote de DPS a ser enviado.</param>
    /// <returns>Resposta contendo o protocolo e as NFS-e geradas.</returns>
    /// <remarks>Suportado apenas por provedores que expõem envio em lote síncrono (ex.: Fiorilli).</remarks>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    public Task<NFSeResponse<RespostaRecepcaoLoteSincrono>> EnviarLoteSincronoAsync(LoteDps lote, CancellationToken cancellationToken = default)
    {
        var provider = NFSeServiceManager.Instance.GetProvider(Configuracoes);
        var oldProtocol = ServicePointManager.SecurityProtocol;

        try
        {
            ServicePointManager.SecurityProtocol = Configuracoes.WebServices.Protocolos;
            return provider.EnviarLoteSincronoAsync(lote, cancellationToken);
        }
        catch (Exception exception)
        {
            this.Log().Error("[EnviarLoteSincrono]", exception);
            throw;
        }
        finally
        {
            ServicePointManager.SecurityProtocol = oldProtocol;
        }
    }

    /// <summary>
    /// Consulta o processamento de um lote de DPS a partir do protocolo.
    /// </summary>
    /// <param name="filtro">Filtro com o protocolo e a identificação do transmissor.</param>
    /// <returns>Resposta contendo a situação e as NFS-e do lote.</returns>
    /// <remarks>Suportado apenas por provedores que expõem consulta de lote (ex.: Fiorilli).</remarks>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    public Task<NFSeResponse<RespostaConsultaLote>> ConsultarLoteAsync(ConsultaLoteFiltro filtro, CancellationToken cancellationToken = default)
    {
        var provider = NFSeServiceManager.Instance.GetProvider(Configuracoes);
        var oldProtocol = ServicePointManager.SecurityProtocol;

        try
        {
            ServicePointManager.SecurityProtocol = Configuracoes.WebServices.Protocolos;
            return provider.ConsultarLoteAsync(filtro, cancellationToken);
        }
        catch (Exception exception)
        {
            this.Log().Error("[ConsultarLote]", exception);
            throw;
        }
        finally
        {
            ServicePointManager.SecurityProtocol = oldProtocol;
        }
    }

    /// <summary>
    /// Consulta NFS-e por chave, Id do DPS ou número/série do DPS.
    /// </summary>
    /// <param name="filtro">Filtro da consulta.</param>
    /// <returns>Resposta contendo as NFS-e encontradas.</returns>
    /// <remarks>Suportado apenas por provedores que expõem consulta de NFS-e (ex.: Fiorilli).</remarks>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    public Task<NFSeResponse<RespostaConsultaNFSe>> ConsultarNFSeAsync(ConsultaNFSeFiltro filtro, CancellationToken cancellationToken = default)
    {
        var provider = NFSeServiceManager.Instance.GetProvider(Configuracoes);
        var oldProtocol = ServicePointManager.SecurityProtocol;

        try
        {
            ServicePointManager.SecurityProtocol = Configuracoes.WebServices.Protocolos;
            return provider.ConsultarNFSeAsync(filtro, cancellationToken);
        }
        catch (Exception exception)
        {
            this.Log().Error("[ConsultarNFSe]", exception);
            throw;
        }
        finally
        {
            ServicePointManager.SecurityProtocol = oldProtocol;
        }
    }

    #endregion Methods
}
