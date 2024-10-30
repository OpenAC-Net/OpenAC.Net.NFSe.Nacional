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

public sealed class OpenNFSeNacional
{
    #region Fields
    
    private readonly NFSeWebservice webservice;
    
    #endregion Fields

    #region Constructors

    public OpenNFSeNacional()
    {
        webservice = new NFSeWebservice(Configuracoes);
    }

    #endregion Constructors
    
    #region Properties

    public ConfiguracaoNFSe Configuracoes { get; } = new();

    #endregion Properties

    #region Methods

    /// <summary>
    /// Recepciona a DPS e Gera a NFS-e de forma síncrona.
    /// </summary>
    /// <param name="dps"></param>
    /// <returns></returns>
    public Task<NFSeResponse<RespostaEnvioDps>> EnviarAsync(Dps dps) => webservice.EnviarAsync(dps);
    
    /// <summary>
    /// Recepciona o Pedido de Registro de Evento e gera Eventos de NFS-e, crédito, débito e apuração.
    /// </summary>
    /// <param name="evento">Evento</param>
    /// <returns></returns>
    public Task<NFSeResponse<RespostaEnvioEvento>> EnviarEventoAsync(PedidoRegistroEvento evento) => webservice.EnviarEventoAsync(evento);
    
    /// <summary>
    /// Distribui os DF-e para contribuintes relacionados à NFS-e.
    /// </summary>
    /// <param name="nsu">Numero da Nsu</param>
    /// <returns>Dados da consulta</returns>
    public Task<NFSeResponse<RespostaConsultaDFe>> ConsultaNsuAsync(int nsu) => webservice.ConsultaNsuAsync(nsu);
    
    /// <summary>
    /// Distribui os DF-e vinculados à chave de acesso informada
    /// </summary>
    /// <param name="chave">chave da NFSe</param>
    /// <returns>Dados da consulta</returns>
    public Task<NFSeResponse<RespostaConsultaDFe>> ConsultaChaveAsync(string chave) => webservice.ConsultaChaveAsync(chave);
    
    /// <summary>
    /// Retorna o DANFSe de uma NFS-e a partir de sua chave de acesso.
    /// </summary>
    /// <param name="chave">Chave de acesso</param>
    /// <returns>Byte array da DANFSe</returns>
    public Task<byte[]> DownloadDANFSeAsync(string chave) => webservice.DownloadDANFSeAsync(chave);
    
    /// <summary>
    /// Retorna a chave de acesso da NFS-e a partir do identificador do DPS.
    /// </summary>
    /// <param name="id">Identificação da Dps</param>
    /// <returns>Dados da consulta</returns>
    public Task<NFSeResponse<RespostaConsultaChaveDps>> ConsultaChaveDpsAsync(string id) => webservice.ConsultaChaveDpsAsync(id);
    
    /// <summary>
    /// Verifica se uma NFS-e foi emitida a partir do Id do DPS.
    /// </summary>
    /// <param name="id">Identificação da Dps</param>
    /// <returns></returns>
    public Task<bool> ConsultaExisteDpsAsync(string id) => webservice.ConsultaExisteDpsAsync(id);

    #endregion
}