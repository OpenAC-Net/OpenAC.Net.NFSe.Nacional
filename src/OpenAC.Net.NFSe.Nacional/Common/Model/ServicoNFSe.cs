// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : RFTD
// Created          : 09-09-2023
//
// Last Modified By : RFTD
// Last Modified On : 09-09-2023
// ***********************************************************************
// <copyright file="ServicoNFSe.cs" company="OpenAC .Net">
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

namespace OpenAC.Net.NFSe.Nacional.Common.Model;

/// <summary>
/// Representa o serviço da NFSe, contendo informações obrigatórias e opcionais conforme o layout nacional.
/// </summary>
public sealed class ServicoNFSe
{
    /// <summary>
    /// Localidade da prestação do serviço.
    /// </summary>
    [DFeElement("locPrest", Ocorrencia = Ocorrencia.Obrigatoria)]
    public LocalidadeNFSe Localidade { get; set; } = new();

    /// <summary>
    /// Informações do serviço prestado.
    /// </summary>
    [DFeElement("cServ", Ocorrencia = Ocorrencia.Obrigatoria)]
    public InformacoesServico Informacoes { get; set; } = new();
    
    /// <summary>
    /// Informações sobre serviço prestado no exterior.
    /// </summary>
    [DFeElement("comExt", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public ServicoExterior? ServicoExterior { get; set; }
    
    /// <summary>
    /// Informações de locação, se aplicável.
    /// </summary>
    [DFeElement("lsadppu", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public InformacoesLocacao? InformacoesLocacao { get; set; }
    
    /// <summary>
    /// Informações sobre obra, se aplicável.
    /// </summary>
    [DFeElement("obra", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public ObraNFSe? Obra { get; set; }
    
    /// <summary>
    /// Informações sobre evento do serviço, se aplicável.
    /// </summary>
    [DFeElement("atvEvento", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public EventoServicoNFSe? Evento { get; set; }
    
    /// <summary>
    /// Informações sobre exploração rodoviária, se aplicável.
    /// </summary>
    [DFeElement("explRod", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public ExploracaoRodoviaria? ExploracaoRodoviaria { get; set; }
    
    /// <summary>
    /// Informações complementares do serviço.
    /// </summary>
    [DFeElement("infoCompl", Ocorrencia = Ocorrencia.NaoObrigatoria)]
    public InformacoesComplementares? InformacoesComplementares { get; set; }
}