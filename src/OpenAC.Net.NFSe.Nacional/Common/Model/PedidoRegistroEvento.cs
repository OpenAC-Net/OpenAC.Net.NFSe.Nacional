// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
// ***********************************************************************
// <copyright file="PedidoRegistroEvento.cs" company="OpenAC .Net">
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

using OpenAC.Net.DFe.Core.Attributes;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.DFe.Core.Document;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Representa o pedido de registro de evento para a NFSe Nacional.
/// </summary>
[DFeSignInfoElement("infPedReg")]
[DFeRoot("pedRegEvento", Namespace = "http://www.sped.fazenda.gov.br/nfse")]
public sealed class PedidoRegistroEvento : DFeSignDocument<PedidoRegistroEvento>
{
    #region Constructors

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="PedidoRegistroEvento"/>.
    /// </summary>
    public PedidoRegistroEvento()
    {
        Signature = new DFeSignature();
    }

    #endregion Constructors
    
    #region Properties

    /// <summary>
    /// Versão do layout do pedido.
    /// </summary>
    [DFeAttribute(TipoCampo.Enum, "versao", Ocorrencia = Ocorrencia.Obrigatoria)]
    public VersaoNFSe Versao { get; set; } = VersaoNFSe.Ve100;

    /// <summary>
    /// Informações do pedido de registro de evento.
    /// </summary>
    [DFeElement("infPedReg", Ocorrencia = Ocorrencia.Obrigatoria)]
    public InfPedReg Informacoes { get; set; } = new();

    #endregion Properties
    
    #region Methods

    /// <summary>
    /// Realiza a assinatura digital do pedido utilizando as configurações fornecidas.
    /// </summary>
    /// <param name="configuracao">Configuração da NFSe.</param>
    public void Assinar(ConfiguracaoNFSe configuracao)
    {
        var options = DFeSaveOptions.DisableFormatting;
        if (configuracao.Geral.RetirarAcentos)
            options |= DFeSaveOptions.RemoveAccents;
        
        AssinarDocumento(configuracao.Certificados.ObterCertificado(), options, false);
    }

    #endregion Methods
}