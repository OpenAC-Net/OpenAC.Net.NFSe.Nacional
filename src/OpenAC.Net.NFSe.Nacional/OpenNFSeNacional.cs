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

using System.Threading.Tasks;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.Webservice;

namespace OpenAC.Net.NFSe.Nacional;

/// <summary>
/// Classe principal para integração com a NFS-e Nacional.
/// </summary>
public sealed class OpenNFSeNacional
{
    #region Fields

    private readonly NFSeWebservice webservice;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Inicializa uma nova instância de <see cref="OpenNFSeNacional"/>.
    /// </summary>
    public OpenNFSeNacional()
    {
        webservice = new NFSeWebservice(Configuracoes);
    }

    #endregion Constructors

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
    public Task<NFSeResponse<RespostaEnvioDps>> EnviarAsync(Dps dps) => webservice.EnviarAsync(dps);

    /// <summary>
    /// Recepciona o Pedido de Registro de Evento e gera eventos de NFS-e, crédito, débito e apuração.
    /// </summary>
    /// <param name="evento">Objeto <see cref="PedidoRegistroEvento"/> representando o evento.</param>
    /// <returns>Resposta do envio do evento.</returns>
    public Task<NFSeResponse<RespostaEnvioEvento>> EnviarEventoAsync(PedidoRegistroEvento evento) => webservice.EnviarEventoAsync(evento);

    /// <summary>
    /// Distribui os DF-e para contribuintes relacionados à NFS-e.
    /// </summary>
    /// <param name="nsu">Número do NSU.</param>
    /// <returns>Dados da consulta dos DF-e.</returns>
    public Task<NFSeResponse<RespostaConsultaDFe>> ConsultaNsuAsync(int nsu) => webservice.ConsultaNsuAsync(nsu);

    /// <summary>
    /// Distribui os DF-e vinculados à chave de acesso informada.
    /// </summary>
    /// <param name="chave">Chave de acesso da NFS-e.</param>
    /// <returns>Dados da consulta dos DF-e.</returns>
    public Task<NFSeResponse<RespostaConsultaDFe>> ConsultaChaveAsync(string chave) => webservice.ConsultaChaveAsync(chave);

    /// <summary>
    /// Retorna o DANFSe de uma NFS-e a partir de sua chave de acesso.
    /// </summary>
    /// <param name="chave">Chave de acesso da NFS-e.</param>
    /// <returns>Array de bytes contendo o DANFSe.</returns>
    public Task<byte[]> DownloadDANFSeAsync(string chave) => webservice.DownloadDANFSeAsync(chave);

    /// <summary>
    /// Retorna a chave de acesso da NFS-e a partir do identificador do DPS.
    /// </summary>
    /// <param name="id">Identificação da DPS.</param>
    /// <returns>Dados da consulta da chave de acesso.</returns>
    public Task<NFSeResponse<RespostaConsultaChaveDps>> ConsultaChaveDpsAsync(string id) => webservice.ConsultaChaveDpsAsync(id);

    /// <summary>
    /// Verifica se uma NFS-e foi emitida a partir do Id do DPS.
    /// </summary>
    /// <param name="id">Identificação da DPS.</param>
    /// <returns>True se a NFS-e existe, caso contrário, false.</returns>
    public Task<bool> ConsultaExisteDpsAsync(string id) => webservice.ConsultaExisteDpsAsync(id);

    #endregion
}