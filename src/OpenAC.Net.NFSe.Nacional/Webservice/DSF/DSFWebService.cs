// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : OpenAC .Net
// ***********************************************************************
// <copyright file="DSFWebService.cs" company="OpenAC .Net">
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
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.Common.Types;
using OpenAC.Net.NFSe.Nacional.Webservice.Nacional;

namespace OpenAC.Net.NFSe.Nacional.Webservice.DSF;

/// <summary>
/// Comunicação com a API SOD da DSF, que adota o modelo nacional em ambiente municipal
/// (caminho base <c>/notafiscal-ws</c>).
/// </summary>
/// <remarks>
/// A emissão, o registro de eventos e a consulta da chave pelo Id do DPS usam os mesmos
/// campos da API nacional e são herdados de <see cref="NacionalWebservice"/>. A assinatura exigida
/// (RSA-SHA1 com C14N) é a padrão dos documentos.
/// O DANFSe, a distribuição por NSU e a consulta por chave com eventos não constam no manual da DSF e são atendidos pelo ADN nacional,
/// dependendo do compartilhamento das notas pelo município. A consulta de eventos lança
/// <see cref="NotSupportedException"/>.
/// As diferenças do JSON retornado pela DSF são tratadas em <see cref="SendAsync"/>.
/// </remarks>
public class DSFWebService : NacionalWebservice
{
    #region Fields

    private static readonly string[] ListasMensagens = ["erros", "erro", "alertas"];

    private static readonly string[] CamposMensagem = ["codigo", "mensagem", "descricao", "complemento", "parametros"];

    private static readonly JsonSerializerOptions OpcoesEscrita = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="DSFWebService"/>.
    /// </summary>
    /// <param name="configuracaoNFSe">Configuração da NFSe.</param>
    /// <param name="serviceInfo">Informações do serviço.</param>
    public DSFWebService(ConfiguracaoNFSe configuracaoNFSe, NFSeServiceInfo serviceInfo) :
        base(configuracaoNFSe, serviceInfo)
    {
    }

    /// <summary>Inicializa o provedor com um transporte HTTP fornecido pelo host.</summary>
    public DSFWebService(ConfiguracaoNFSe configuracaoNFSe, NFSeServiceInfo serviceInfo,
        INFSeHttpTransport httpTransport) : base(configuracaoNFSe, serviceInfo, httpTransport)
    {
    }

    #endregion Constructors

    #region Consultas

    /// <summary>
    /// Consulta a NFS-e pela chave de acesso ou, na ausência dela, pelo Id do DPS.
    /// </summary>
    /// <param name="filtro">Filtro da consulta. Informe <see cref="ConsultaNFSeFiltro.ChaveNFSe"/> ou <see cref="ConsultaNFSeFiltro.IdDPS"/>.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Resposta contendo a NFS-e encontrada.</returns>
    /// <exception cref="InvalidOperationException">Quando o filtro não tem chave nem Id do DPS.</exception>
    public override async Task<NFSeResponse<RespostaConsultaNFSe>> ConsultarNFSeAsync(ConsultaNFSeFiltro filtro,
        CancellationToken cancellationToken = default)
    {
        var chave = filtro.ChaveNFSe == null ? null : RemoverPrefixoChave(filtro.ChaveNFSe);
        if (string.IsNullOrEmpty(chave) && !string.IsNullOrEmpty(filtro.IdDPS))
        {
            var consulta = await ConsultaChaveDpsAsync(filtro.IdDPS!, cancellationToken);
            if (!consulta.Sucesso)
            {
                var falha = new RespostaConsultaNFSe();
                if (consulta.Resultado != null) falha.Mensagens.AddRange(consulta.Resultado.Erros);
                return NFSeResponse<RespostaConsultaNFSe>.Create(string.Empty, consulta.JsonRetorno ?? string.Empty,
                    false, falha);
            }

            chave = consulta.Resultado?.Chave;
        }

        if (string.IsNullOrEmpty(chave))
            throw new InvalidOperationException("Informe a ChaveNFSe ou o IdDPS para consultar no provedor DSF.");

        var url = ServiceInfo[Configuracao.WebServices.Ambiente][TipoUrl.Enviar];
        using var httpResponse = await SendAsync(null, HttpMethod.Get, $"{url}/nfse/{chave}",
            cancellationToken: cancellationToken);

        var strResponse = await httpResponse.Content.ReadAsStringAsync();
        await GravarArquivoEmDiscoAsync(strResponse, $"ConsultarNFSe-{chave}-resp.json", filtro.CNPJ ?? filtro.CPF,
            cancellationToken);

        // O retorno da consulta tem os mesmos campos do retorno do envio (chaveAcesso + nfseXmlGZipB64).
        var retorno = NFSeResponse<RespostaEnvioDps>.Create(string.Empty, string.Empty, strResponse,
            httpResponse.IsSuccessStatusCode);

        var resultado = new RespostaConsultaNFSe();
        if (retorno.Resultado != null)
        {
            resultado.Mensagens.AddRange(retorno.Resultado.Erros);
            if (retorno.Sucesso && retorno.Resultado.NFSe is { } nota) resultado.NotasFiscais.Add(nota);
        }

        var sucesso = httpResponse.IsSuccessStatusCode && resultado.NotasFiscais.Count > 0;
        return NFSeResponse<RespostaConsultaNFSe>.Create(string.Empty, strResponse, sucesso, resultado);
    }

    /// <summary>
    /// Distribui os documentos vinculados à chave de acesso pelo ADN nacional, que retorna a NFS-e e os
    /// eventos compartilhados pelo município. A API da DSF não expõe a consulta de eventos.
    /// </summary>
    /// <param name="chave">Chave de acesso da NFS-e, com ou sem o prefixo <c>NFS</c>.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Resposta da consulta. <see cref="NFSeResponse{T}.Resultado"/> é preenchido mesmo quando o ADN não retorna JSON.</returns>
    public override async Task<NFSeResponse<RespostaConsultaDFe>> ConsultaChaveAsync(string chave,
        CancellationToken cancellationToken = default)
    {
        var retorno = await base.ConsultaChaveAsync(RemoverPrefixoChave(chave), cancellationToken);
        if (retorno.Resultado != null) return retorno;

        // ADN fora do ar (ex.: 503 em HTML): mantém Resultado preenchido para quem lê Lote.
        return NFSeResponse<RespostaConsultaDFe>.Create(string.Empty, retorno.JsonRetorno ?? string.Empty, false,
            new RespostaConsultaDFe { StatusProcessamento = StatusProcessamentoDistribuicao.REJEICAO });
    }

    /// <summary>
    /// Verifica se uma NFS-e foi emitida a partir do Id do DPS. Usa <c>GET</c>, pois o manual da DSF
    /// não documenta <c>HEAD</c> nesse recurso.
    /// </summary>
    /// <param name="id">Id completo com 45 posições no formato <c>DPS[0-9]{42}</c>.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>True se existir, caso contrário false.</returns>
    public override async Task<bool> ConsultaExisteDpsAsync(string id, CancellationToken cancellationToken = default)
    {
        var consulta = await ConsultaChaveDpsAsync(id, cancellationToken);
        return consulta.Sucesso;
    }

    #endregion Consultas

    #region Eventos

    /// <summary>
    /// Recepciona o pedido de registro de evento. Notas cuja chave foi obtida da consulta pelo Id do DPS
    /// podem ter sido gravadas com o prefixo <c>NFS</c>, que é removido antes da assinatura e do envio.
    /// </summary>
    /// <param name="evento">Evento a ser enviado.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Resposta do envio do evento.</returns>
    public override Task<NFSeResponse<RespostaEnvioEvento>> EnviarEventoAsync(PedidoRegistroEvento evento,
        CancellationToken cancellationToken = default)
    {
        evento.Informacoes.ChNFSe = RemoverPrefixoChave(evento.Informacoes.ChNFSe);
        evento.Informacoes.Id = evento.Informacoes.Id.Replace("PRENFS", "PRE");
        return base.EnviarEventoAsync(evento, cancellationToken);
    }

    #endregion Eventos

    #region Transporte

    /// <summary>
    /// Envia a requisição e normaliza o JSON retornado pela DSF para o formato lido pelos modelos comuns.
    /// Respostas de outros hosts (ADN nacional, incluindo o PDF do DANFSe) não são alteradas.
    /// </summary>
    /// <param name="content">Conteúdo da requisição.</param>
    /// <param name="method">Método HTTP.</param>
    /// <param name="url">URL do serviço.</param>
    /// <param name="headers">Cabeçalhos adicionais.</param>
    /// <param name="cancellationToken">Token para cancelar o envio da requisição.</param>
    /// <returns>Resposta HTTP.</returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpContent? content, HttpMethod method, string url,
        IEnumerable<KeyValuePair<string, string>>? headers = null, CancellationToken cancellationToken = default)
    {
        var resposta = await base.SendAsync(content, method, url, headers, cancellationToken);

        var urlDsf = ServiceInfo[Configuracao.WebServices.Ambiente][TipoUrl.Enviar];
        if (!url.StartsWith(urlDsf, StringComparison.OrdinalIgnoreCase)) return resposta;

        var json = await resposta.Content.ReadAsStringAsync();
        var normalizado = NormalizarRetorno(json);
        if (ReferenceEquals(normalizado, json)) return resposta;

        resposta.Content.Dispose();
        resposta.Content = new StringContent(normalizado, Encoding.UTF8, "application/json");
        return resposta;
    }

    /// <summary>
    /// Ajusta as diferenças do retorno da DSF em relação à API nacional: <c>tipoAmbiente</c> vem como
    /// texto e os campos das mensagens vêm em minúsculas (ex.: <c>codigo</c>, <c>mensagem</c>).
    /// </summary>
    /// <param name="json">Corpo da resposta.</param>
    /// <returns>O JSON ajustado ou o próprio <paramref name="json"/> quando nada precisou mudar.</returns>
    private static string NormalizarRetorno(string json)
    {
        JsonObject? raiz;
        try
        {
            raiz = JsonNode.Parse(json) as JsonObject;
        }
        catch (JsonException)
        {
            return json;
        }

        if (raiz == null) return json;

        var alterado = false;
        if (raiz["tipoAmbiente"] is JsonValue ambiente && ambiente.TryGetValue<string>(out var texto) &&
            int.TryParse(texto, NumberStyles.None, CultureInfo.InvariantCulture, out var valor))
        {
            raiz["tipoAmbiente"] = valor;
            alterado = true;
        }

        if (raiz["chaveAcesso"] is JsonValue chave && chave.TryGetValue<string>(out var textoChave) &&
            textoChave != RemoverPrefixoChave(textoChave))
        {
            raiz["chaveAcesso"] = RemoverPrefixoChave(textoChave);
            alterado = true;
        }

        foreach (var lista in ListasMensagens)
        {
            if (raiz[lista] is not JsonArray mensagens) continue;

            foreach (var mensagem in mensagens.OfType<JsonObject>())
                alterado |= NormalizarMensagem(mensagem);
        }

        return alterado ? raiz.ToJsonString(OpcoesEscrita) : json;
    }

    /// <summary>
    /// Renomeia os campos de uma mensagem para a grafia lida por <see cref="MensagemProcessamento"/>.
    /// </summary>
    /// <param name="mensagem">Mensagem retornada pela DSF.</param>
    /// <returns>True se algum campo foi renomeado.</returns>
    private static bool NormalizarMensagem(JsonObject mensagem)
    {
        var alterado = false;
        foreach (var campo in CamposMensagem)
        {
            if (!mensagem.TryGetPropertyValue(campo, out var valor)) continue;

            mensagem.Remove(campo);
            mensagem[char.ToUpperInvariant(campo[0]) + campo.Substring(1)] = valor;
            alterado = true;
        }

        return alterado;
    }

    /// <summary>
    /// Remove o prefixo <c>NFS</c> da chave de acesso. A consulta pelo Id do DPS devolve a chave com o
    /// prefixo do <c>Id</c> de <c>infNFSe</c> (53 posições), e as demais operações usam as 50 posições.
    /// </summary>
    /// <param name="chave">Chave de acesso, com ou sem o prefixo.</param>
    /// <returns>Chave de acesso com 50 posições.</returns>
    private static string RemoverPrefixoChave(string chave) =>
        chave.Length == 53 && chave.StartsWith("NFS", StringComparison.Ordinal) ? chave.Substring(3) : chave;

    #endregion Transporte

    #region Não suportadas

    /// <summary>Não suportado - não consta no manual da DSF e a Sefin Nacional não é o emissor destas notas.</summary>
    public override Task<NFSeResponse<RespostaConsultaEvento>> ConsultaEventoAsync(string chaveAcesso, string tipoEvento,
        int numSeqEvento, CancellationToken cancellationToken = default) =>
        throw OperacaoNaoSuportada(nameof(ConsultaEventoAsync));

    #endregion Não suportadas
}
