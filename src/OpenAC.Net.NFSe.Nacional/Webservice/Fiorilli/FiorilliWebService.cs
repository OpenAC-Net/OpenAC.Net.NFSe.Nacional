// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : OpenAC .Net
// ***********************************************************************
// <copyright file="FiorilliWebService.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2014-2026 Grupo OpenAC.Net
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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.Core.Logging;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional.Webservice.Fiorilli;

/// <summary>
/// Comunicação com o webservice SOAP da Fiorilli (<c>IssWebWSNacional</c>) que adota o layout
/// do padrão nacional (namespace <c>http://www.sped.fazenda.gov.br/nfse</c>).
/// </summary>
/// <remarks>
/// Diferentemente do provedor Nacional (REST/JSON), a Fiorilli expõe operações SOAP document/literal.
/// Os documentos (DPS, pedRegEvento e NFS-e) continuam sendo gerados/assinados pelos models comuns;
/// apenas o transporte (envelope SOAP e leitura da resposta XML) é específico deste provedor.
/// Operações que a Fiorilli não expõe (distribuição por NSU, download de DANFSe e consulta de eventos
/// por chave) lançam <see cref="NotSupportedException"/>.
/// </remarks>
public class FiorilliWebService : NFSeWebserviceBase
{
    #region Constructors

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="FiorilliWebService"/>.
    /// </summary>
    /// <param name="configuracaoNFSe">Configuração da NFSe.</param>
    /// <param name="serviceInfo">Informações do serviço.</param>
    public FiorilliWebService(ConfiguracaoNFSe configuracaoNFSe, NFSeServiceInfo serviceInfo) :
        base(configuracaoNFSe, serviceInfo)
    {
    }

    /// <summary>Inicializa o provedor com um transporte HTTP fornecido pelo host.</summary>
    public FiorilliWebService(ConfiguracaoNFSe configuracaoNFSe, NFSeServiceInfo serviceInfo,
        INFSeHttpTransport httpTransport) : base(configuracaoNFSe, serviceInfo, httpTransport)
    {
    }

    #endregion Constructors

    #region NFS-e

    /// <summary>
    /// Recepciona a DPS e gera a NFS-e de forma síncrona (operação <c>recepcionarDps</c>).
    /// </summary>
    /// <param name="dps">DPS a ser enviada.</param>
    /// <returns>Resposta do envio da DPS.</returns>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    public override async Task<NFSeResponse<RespostaEnvioDps>> EnviarAsync(Dps dps, CancellationToken cancellationToken = default)
    {
        dps.Assinar(Configuracao);
        ValidarSchema(SchemaNFSe.DPS, dps.Xml, dps.Versao);

        var documento = dps.Informacoes.Prestador.CPF ?? dps.Informacoes.Prestador.CNPJ;
        var prefixo = dps.Informacoes.NumeroDps.ZeroFill(6);

        await GravarDpsEmDiscoAsync(dps.Xml, $"{prefixo}_dps.xml", documento, dps.Informacoes.DhEmissao.DateTime, cancellationToken: cancellationToken);

        var corpo = $"<RecepcionarDpsEnvio xmlns=\"{FiorilliSoap.NsFiorilli}\">" +
                    FiorilliSoap.RemoverProlog(dps.Xml) +
                    "</RecepcionarDpsEnvio>";
        var envelope = FiorilliSoap.Envelope(corpo);

        var (sucessoHttp, resposta) = await EnviarSoapAsync("recepcionarDPS", envelope, $"Enviar-{prefixo}", documento, cancellationToken);

        var raiz = FiorilliSoap.CorpoResposta(resposta);
        var notaXml = FiorilliSoap.NotaXml(raiz);

        var resultado = new RespostaEnvioDps
        {
            XmlNFSe = notaXml
        };
        FiorilliSoap.DistribuirMensagens(resultado, FiorilliSoap.Mensagens(raiz));

        if (!notaXml.IsEmpty())
        {
            resultado.ChaveAcesso = FiorilliSoap.ChaveDaNota(notaXml);
            resultado.IdDps = dps.Informacoes.Id;
            await GravarNFSeEmDiscoAsync(notaXml, $"{(Configuracao.Arquivos.PadronizarNomes ? resultado.ChaveAcesso : prefixo)}_nfse.xml",
                documento, dps.Informacoes.DhEmissao.DateTime, cancellationToken: cancellationToken);
        }

        var sucesso = sucessoHttp && !notaXml.IsEmpty() && resultado.Erros.Count == 0;
        return NFSeResponse<RespostaEnvioDps>.Create(envelope, resposta, sucesso, resultado);
    }

    #endregion NFS-e

    #region Lote

    /// <summary>
    /// Recepciona um lote de DPS de forma assíncrona (operação <c>recepcionarLoteDps</c>).
    /// </summary>
    /// <param name="lote">Lote de DPS a ser enviado.</param>
    /// <returns>Resposta contendo o protocolo do lote.</returns>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    public override async Task<NFSeResponse<RespostaRecepcaoLote>> EnviarLoteAsync(LoteDps lote, CancellationToken cancellationToken = default)
    {
        var (envelope, documento) = MontarEnvelopeLote(lote, "RecepcionarLoteDpsEnvio");

        var (sucessoHttp, resposta) = await EnviarSoapAsync("recepcionarLoteDps", envelope,
            $"EnviarLote-{lote.NumeroLote}", documento, cancellationToken);

        var raiz = FiorilliSoap.CorpoResposta(resposta);
        var resultado = new RespostaRecepcaoLote
        {
            Protocolo = FiorilliSoap.Valor(raiz, "Protocolo") ?? string.Empty,
            Mensagens = FiorilliSoap.Mensagens(raiz)
        };
        PreencherLote(resultado, raiz);

        var sucesso = sucessoHttp && !resultado.Protocolo.IsEmpty() && !FiorilliSoap.TemErro(resultado.Mensagens);
        return NFSeResponse<RespostaRecepcaoLote>.Create(envelope, resposta, sucesso, resultado);
    }

    /// <summary>
    /// Recepciona um lote de DPS de forma síncrona (operação <c>recepcionarLoteDpsSincrono</c>).
    /// </summary>
    /// <param name="lote">Lote de DPS a ser enviado.</param>
    /// <returns>Resposta contendo o protocolo e as NFS-e geradas.</returns>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    public override async Task<NFSeResponse<RespostaRecepcaoLoteSincrono>> EnviarLoteSincronoAsync(LoteDps lote, CancellationToken cancellationToken = default)
    {
        var (envelope, documento) = MontarEnvelopeLote(lote, "RecepcionarLoteDpsSincronoEnvio");

        var (sucessoHttp, resposta) = await EnviarSoapAsync("recepcionarLoteDpsSincrono", envelope,
            $"EnviarLoteSincrono-{lote.NumeroLote}", documento, cancellationToken);

        var raiz = FiorilliSoap.CorpoResposta(resposta);
        var resultado = new RespostaRecepcaoLoteSincrono
        {
            Protocolo = FiorilliSoap.Valor(raiz, "Protocolo") ?? string.Empty,
            NotasFiscais = FiorilliSoap.Notas(raiz),
            Mensagens = FiorilliSoap.Mensagens(raiz)
        };
        PreencherLote(resultado, raiz);

        var sucesso = sucessoHttp && (resultado.NotasFiscais.Count > 0 || !resultado.Protocolo.IsEmpty()) &&
                      !FiorilliSoap.TemErro(resultado.Mensagens);
        return NFSeResponse<RespostaRecepcaoLoteSincrono>.Create(envelope, resposta, sucesso, resultado);
    }

    /// <summary>
    /// Consulta o processamento de um lote de DPS pelo protocolo (operação <c>consultarLoteDps</c>).
    /// </summary>
    /// <param name="filtro">Filtro com o protocolo e a identificação do transmissor.</param>
    /// <returns>Resposta contendo a situação e as NFS-e do lote.</returns>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    public override async Task<NFSeResponse<RespostaConsultaLote>> ConsultarLoteAsync(ConsultaLoteFiltro filtro, CancellationToken cancellationToken = default)
    {
        var documento = filtro.CNPJ ?? filtro.CPF;
        var corpo = $"<ConsultarLoteDpsEnvio xmlns=\"{FiorilliSoap.NsFiorilli}\">" +
                    FiorilliSoap.Elemento("CNPJ", filtro.CNPJ) +
                    FiorilliSoap.Elemento("CPF", filtro.CPF) +
                    FiorilliSoap.Elemento("IM", filtro.IM) +
                    FiorilliSoap.Elemento("Protocolo", filtro.Protocolo) +
                    "</ConsultarLoteDpsEnvio>";
        var envelope = FiorilliSoap.Envelope(corpo);

        var (sucessoHttp, resposta) = await EnviarSoapAsync("consultarLoteDps", envelope,
            $"ConsultarLote-{filtro.Protocolo}", documento, cancellationToken);

        var raiz = FiorilliSoap.CorpoResposta(resposta);
        var resultado = new RespostaConsultaLote
        {
            Situacao = int.TryParse(FiorilliSoap.Valor(raiz, "Situacao"), out var situacao) ? situacao : 0,
            NotasFiscais = FiorilliSoap.Notas(raiz),
            Mensagens = FiorilliSoap.Mensagens(raiz)
        };

        var sucesso = sucessoHttp && !FiorilliSoap.TemErro(resultado.Mensagens);
        return NFSeResponse<RespostaConsultaLote>.Create(envelope, resposta, sucesso, resultado);
    }

    #endregion Lote

    #region Consultas

    /// <summary>
    /// Consulta NFS-e por chave, Id do DPS ou número/série do DPS (operação <c>consultarNfse</c>).
    /// </summary>
    /// <param name="filtro">Filtro da consulta.</param>
    /// <returns>Resposta contendo as NFS-e encontradas.</returns>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    public override async Task<NFSeResponse<RespostaConsultaNFSe>> ConsultarNFSeAsync(ConsultaNFSeFiltro filtro, CancellationToken cancellationToken = default)
    {
        var documento = filtro.CNPJ ?? filtro.CPF;
        var corpo = $"<ConsultarNfseEnvio xmlns=\"{FiorilliSoap.NsFiorilli}\">" +
                    FiorilliSoap.Elemento("CNPJ", filtro.CNPJ) +
                    FiorilliSoap.Elemento("CPF", filtro.CPF) +
                    FiorilliSoap.Elemento("IM", filtro.IM) +
                    FiorilliSoap.Elemento("ChaveNFSe", filtro.ChaveNFSe) +
                    FiorilliSoap.Elemento("IdDPS", filtro.IdDPS) +
                    FiorilliSoap.Elemento("NumeroDPS", filtro.NumeroDPS) +
                    FiorilliSoap.Elemento("SerieDPS", filtro.SerieDPS) +
                    "</ConsultarNfseEnvio>";
        var envelope = FiorilliSoap.Envelope(corpo);

        var identificador = filtro.ChaveNFSe ?? filtro.IdDPS ?? filtro.NumeroDPS ?? "consulta";
        var (sucessoHttp, resposta) = await EnviarSoapAsync("consultarNfse", envelope,
            $"ConsultarNfse-{identificador}", documento, cancellationToken);

        var raiz = FiorilliSoap.CorpoResposta(resposta);
        var resultado = new RespostaConsultaNFSe
        {
            NotasFiscais = FiorilliSoap.Notas(raiz),
            Mensagens = FiorilliSoap.Mensagens(raiz)
        };

        var sucesso = sucessoHttp && !FiorilliSoap.TemErro(resultado.Mensagens);
        return NFSeResponse<RespostaConsultaNFSe>.Create(envelope, resposta, sucesso, resultado);
    }

    /// <summary>
    /// Retorna a chave de acesso da NFS-e a partir do identificador do DPS.
    /// Implementado via <c>consultarNfse</c> (filtro por <c>IdDPS</c>).
    /// </summary>
    /// <param name="id">Identificação do DPS.</param>
    /// <returns>Resposta da consulta contendo a chave de acesso.</returns>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    public override async Task<NFSeResponse<RespostaConsultaChaveDps>> ConsultaChaveDpsAsync(string id, CancellationToken cancellationToken = default)
    {
        var consulta = await ConsultarNFSeAsync(new ConsultaNFSeFiltro { IdDPS = id }, cancellationToken);
        var notaXml = consulta.Resultado?.NotasFiscais.FirstOrDefault()?.Xml ?? string.Empty;

        var resultado = new RespostaConsultaChaveDps
        {
            IdDps = id,
            Chave = FiorilliSoap.ChaveDaNota(notaXml)
        };
        if (consulta.Resultado != null) FiorilliSoap.DistribuirMensagens(resultado, consulta.Resultado.Mensagens);

        var sucesso = consulta.Sucesso && !resultado.Chave.IsEmpty();
        return NFSeResponse<RespostaConsultaChaveDps>.Create(consulta.XmlEnvio, consulta.XmlRetorno ?? string.Empty, sucesso, resultado);
    }

    /// <summary>
    /// Verifica se uma NFS-e foi emitida a partir do Id do DPS (via <c>consultarNfse</c>).
    /// </summary>
    /// <param name="id">Identificação do DPS.</param>
    /// <returns>True se existir, caso contrário false.</returns>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    public override async Task<bool> ConsultaExisteDpsAsync(string id, CancellationToken cancellationToken = default)
    {
        var consulta = await ConsultarNFSeAsync(new ConsultaNFSeFiltro { IdDPS = id });
        return consulta.Sucesso && (consulta.Resultado?.NotasFiscais.Count ?? 0) > 0;
    }

    #endregion Consultas

    #region Eventos

    /// <summary>
    /// Recepciona o pedido de registro de evento (operação <c>cancelarNFSe</c> da Fiorilli).
    /// </summary>
    /// <param name="evento">Pedido de registro de evento (ex.: cancelamento).</param>
    /// <returns>Resposta do envio do evento.</returns>
    /// <remarks>
    /// A Fiorilli expõe o registro de evento pela operação <c>cancelarNFSe</c>, que exige a inscrição
    /// municipal do prestador no envelope. A inscrição é lida de
    /// <see cref="Common.NFSeWebserviceConfig.InscricaoMunicipal"/>.
    /// </remarks>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    public override async Task<NFSeResponse<RespostaEnvioEvento>> EnviarEventoAsync(PedidoRegistroEvento evento, CancellationToken cancellationToken = default)
    {
        var inscricaoMunicipal = Configuracao.WebServices.InscricaoMunicipal;
        if (string.IsNullOrEmpty(inscricaoMunicipal))
            throw new InvalidOperationException(
                "Informe a inscrição municipal em Configuracoes.WebServices.InscricaoMunicipal para registrar eventos no provedor Fiorilli.");

        evento.Assinar(Configuracao);
        var documento = evento.Informacoes.CPFAutor ?? evento.Informacoes.CNPJAutor;
        var eventoAssinado = FiorilliSoap.ReassinarEvento(
            evento.Xml,
            Configuracao.Certificados.ObterCertificado());
        ValidarSchema(SchemaNFSe.Evento, eventoAssinado, evento.Versao);

        await GravarDpsEmDiscoAsync(eventoAssinado, $"{evento.Informacoes.ChNFSe}{evento.Informacoes.Evento}_evento.xml",
            documento, evento.Informacoes.DhEvento.DateTime, true, cancellationToken);

        var corpo = $"<CancelarNFSeEnvio xmlns=\"{FiorilliSoap.NsFiorilli}\">" +
                    FiorilliSoap.Elemento("IM", inscricaoMunicipal) +
                    eventoAssinado +
                    "</CancelarNFSeEnvio>";
        var envelope = FiorilliSoap.Envelope(corpo);

        var (sucessoHttp, resposta) = await EnviarSoapAsync("cancelarNFSe", envelope,
            $"Cancelar-{evento.Informacoes.ChNFSe}", documento, cancellationToken);

        var raiz = FiorilliSoap.CorpoResposta(resposta);
        var resultado = new RespostaEnvioEvento
        {
            DataHoraProcessamento = ParseData(FiorilliSoap.Valor(raiz, "DataRecebimento"))
        };
        FiorilliSoap.DistribuirMensagens(resultado, FiorilliSoap.Mensagens(raiz));

        var status = FiorilliSoap.Valor(raiz, "status");
        if (!string.IsNullOrEmpty(status))
            this.Log().Debug($"Fiorilli: [cancelarNFSe][Status] - {status}");

        var sucesso = sucessoHttp && resultado.Erros.Count == 0;
        return NFSeResponse<RespostaEnvioEvento>.Create(envelope, resposta, sucesso, resultado);
    }

    #endregion Eventos

    #region Não suportadas

    /// <summary>
    /// Não suportado pela Fiorilli.
    /// </summary>
    public override Task<byte[]> DownloadDANFSeAsync(string chave, CancellationToken cancellationToken = default) =>
        throw OperacaoNaoSuportada(nameof(DownloadDANFSeAsync));

    /// <summary>
    /// Não suportado pela Fiorilli (não há distribuição por NSU no WSDL).
    /// </summary>
    public override Task<NFSeResponse<RespostaConsultaDFe>> ConsultaNsuAsync(int nsu, CancellationToken cancellationToken = default) =>
        throw OperacaoNaoSuportada(nameof(ConsultaNsuAsync));

    /// <summary>
    /// Não suportado pela Fiorilli (não há consulta de eventos por chave no WSDL).
    /// </summary>
    public override Task<NFSeResponse<RespostaConsultaDFe>> ConsultaChaveAsync(string chave, CancellationToken cancellationToken = default) =>
        throw OperacaoNaoSuportada(nameof(ConsultaChaveAsync));

    /// <inheritdoc/>
    public override async Task<NFSeResponse<RespostaConsultaEvento>> ConsultaEventoAsync(string chaveAcesso, string tipoEvento, int numSeqEvento) =>
        throw OperacaoNaoSuportada(nameof(ConsultaChaveAsync));

    #endregion Não suportadas

    #region Helpers

    /// <summary>
    /// Monta o envelope de envio de lote (assíncrono ou síncrono) e retorna também o documento
    /// do transmissor para fins de gravação em disco.
    /// </summary>
    /// <param name="lote">Lote de DPS.</param>
    /// <param name="elementoEnvio">Nome do elemento invólucro da operação.</param>
    /// <returns>Envelope SOAP e documento do transmissor.</returns>
    private (string envelope, string? documento) MontarEnvelopeLote(LoteDps lote, string elementoEnvio)
    {
        var listaDps = new StringBuilder();
        foreach (var dps in lote.Documentos)
        {
            dps.Assinar(Configuracao);
            ValidarSchema(SchemaNFSe.DPS, dps.Xml, dps.Versao);
            listaDps.Append(FiorilliSoap.RemoverProlog(dps.Xml));
        }

        var atributoId = string.IsNullOrEmpty(lote.Id) ? string.Empty : $" Id=\"{FiorilliSoap.Escape(lote.Id)}\"";

        var corpo = $"<{elementoEnvio} xmlns=\"{FiorilliSoap.NsFiorilli}\">" +
                    $"<LoteDps{atributoId}>" +
                    FiorilliSoap.Elemento("NumeroLote", lote.NumeroLote.ToString(CultureInfo.InvariantCulture)) +
                    FiorilliSoap.Elemento("CNPJ", lote.CNPJ) +
                    FiorilliSoap.Elemento("CPF", lote.CPF) +
                    FiorilliSoap.Elemento("IM", lote.IM) +
                    FiorilliSoap.Elemento("QuantidadeDps", lote.QuantidadeDps.ToString(CultureInfo.InvariantCulture)) +
                    $"<ListaDps>{listaDps}</ListaDps>" +
                    "</LoteDps>" +
                    $"</{elementoEnvio}>";

        return (FiorilliSoap.Envelope(corpo), lote.CNPJ ?? lote.CPF);
    }

    /// <summary>
    /// Preenche os campos comuns de resposta de lote (número do lote e data de recebimento).
    /// </summary>
    /// <param name="resposta">Resposta a ser preenchida.</param>
    /// <param name="raiz">Elemento invólucro da resposta.</param>
    private static void PreencherLote(RespostaRecepcaoLote resposta, XElement? raiz)
    {
        if (long.TryParse(FiorilliSoap.Valor(raiz, "NumeroLote"), out var numeroLote))
            resposta.NumeroLote = numeroLote;

        resposta.DataRecebimento = ParseData(FiorilliSoap.Valor(raiz, "DataRecebimento"));
    }

    /// <summary>
    /// Converte um texto em <see cref="DateTimeOffset"/> (retorna default quando inválido).
    /// </summary>
    /// <param name="valor">Texto da data.</param>
    /// <returns>Data convertida ou default.</returns>
    private static DateTimeOffset ParseData(string? valor) =>
        DateTimeOffset.TryParse(valor, CultureInfo.InvariantCulture, DateTimeStyles.None, out var data)
            ? data
            : default;

    /// <summary>
    /// Envia o envelope SOAP, grava requisição/resposta em disco e retorna o status e o corpo da resposta.
    /// </summary>
    /// <param name="soapAction">Valor do cabeçalho SOAPAction.</param>
    /// <param name="envelope">Envelope SOAP.</param>
    /// <param name="nomeArquivo">Prefixo do nome dos arquivos gravados em disco.</param>
    /// <param name="documento">Documento do prestador/transmissor para composição do caminho.</param>
    /// <param name="cancellationToken">Token para cancelar o envio e a persistência dos payloads.</param>
    /// <returns>Indicador de sucesso HTTP (sem fault) e o corpo da resposta.</returns>
    private async Task<(bool sucesso, string resposta)> EnviarSoapAsync(string soapAction, string envelope,
        string nomeArquivo, string? documento, CancellationToken cancellationToken = default)
    {
        this.Log().Debug($"Fiorilli: [{soapAction}][Envio] - {envelope}");
        await GravarArquivoEmDiscoAsync(envelope, $"{nomeArquivo}-env.xml", documento);

        var url = ObterUrl();
        var content = new StringContent(envelope, Encoding.UTF8, "text/xml");
        var headers = new[] { new KeyValuePair<string, string>("SOAPAction", $"\"{soapAction}\"") };

        using var httpResponse = await SendAsync(content, HttpMethod.Post, url, headers, cancellationToken: cancellationToken);
        var resposta = await httpResponse.Content.ReadAsStringAsync();

        this.Log().Debug($"Fiorilli: [{soapAction}][Resposta] - {resposta}");
        await GravarArquivoEmDiscoAsync(resposta, $"{nomeArquivo}-resp.xml", documento);

        var possuiFault = FiorilliSoap.PossuiFault(resposta, out var faultMsg);
        if (possuiFault) this.Log().Debug($"Fiorilli: [{soapAction}][Fault] - {faultMsg}");

        return (httpResponse.IsSuccessStatusCode && !possuiFault, resposta);
    }

    /// <summary>
    /// Obtém o endpoint SOAP configurado para o ambiente atual.
    /// </summary>
    /// <returns>URL do webservice.</returns>
    /// <exception cref="InvalidOperationException">Quando o endpoint não está configurado.</exception>
    private string ObterUrl()
    {
        var ambiente = ServiceInfo[Configuracao.WebServices.Ambiente];
        if (!ambiente.Enderecos.TryGetValue(TipoUrl.Enviar, out var url) || string.IsNullOrEmpty(url))
            throw new InvalidOperationException("Endpoint SOAP do provedor Fiorilli não configurado (TipoUrl.Enviar).");

        return url;
    }

    #endregion Helpers
}
