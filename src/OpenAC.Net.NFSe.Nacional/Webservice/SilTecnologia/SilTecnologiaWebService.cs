// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : OpenAC .Net
// ***********************************************************************
// <copyright file="SilTecnologiaWebService.cs" company="OpenAC .Net">
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.Core.Logging;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.DFe.Core.Extensions;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.Common.Types;
using OpenAC.Net.NFSe.Nacional.Webservice.Nacional;

namespace OpenAC.Net.NFSe.Nacional.Webservice.SilTecnologia;

/// <summary>
/// Comunicação com o webservice SOAP <c>NotaFiscalNacional</c> da SIL Tecnologia (ex.: Bauru/SP),
/// que recebe a NFS-e no layout do padrão nacional e a compartilha com o ADN.
/// </summary>
/// <remarks>
/// Diferente do provedor Nacional, a SIL recebe a NFS-e completa (<c>infNFSe</c> com a DPS dentro),
/// assinada em <c>infNFSe</c>, e não a DPS. A montagem da NFS-e a partir da DPS fica em
/// <see cref="SilTecnologiaNFSe"/>. Emissão, cancelamento, substituição e consulta por DPS usam o webservice
/// da SIL; distribuição por NSU/chave, DANFSe e os demais eventos usam o provedor Nacional (Sefin/ADN),
/// com o qual a SIL compartilha as NFS-e.
/// </remarks>
public class SilTecnologiaWebService : NFSeWebserviceBase
{
    #region Fields

    private const string UrlIbgeMunicipio = "https://servicodados.ibge.gov.br/api/v1/localidades/municipios/";

    private static readonly ConcurrentDictionary<string, string> NomesMunicipios = new();

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="SilTecnologiaWebService"/>.
    /// </summary>
    /// <param name="configuracaoNFSe">Configuração da NFSe.</param>
    /// <param name="serviceInfo">Informações do serviço.</param>
    public SilTecnologiaWebService(ConfiguracaoNFSe configuracaoNFSe, NFSeServiceInfo serviceInfo) :
        base(configuracaoNFSe, serviceInfo)
    {
    }

    /// <summary>Inicializa o provedor com um transporte HTTP fornecido pelo host.</summary>
    public SilTecnologiaWebService(ConfiguracaoNFSe configuracaoNFSe, NFSeServiceInfo serviceInfo,
        INFSeHttpTransport httpTransport) : base(configuracaoNFSe, serviceInfo, httpTransport)
    {
    }

    #endregion Constructors

    #region NFS-e

    /// <summary>
    /// Monta a NFS-e a partir da DPS (<see cref="SilTecnologiaNFSe.Montar"/>) e envia pela operação
    /// <c>NotaFiscalNacionalGerar</c>.
    /// </summary>
    /// <param name="dps">
    /// DPS com o nome e o endereço do prestador (usados no grupo <c>emit</c>) e a alíquota do ISSQN em
    /// <c>tribMun/pAliq</c> (usada em <c>infNFSe/valores</c>) preenchidos. O município de incidência do ISSQN
    /// vem de <see cref="NFSeWebserviceConfig.CodigoMunicipioIncidencia"/>.
    /// </param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Resposta do envio.</returns>
    public override async Task<NFSeResponse<RespostaEnvioDps>> EnviarAsync(Dps dps, CancellationToken cancellationToken = default)
    {
        var nfse = await MontarNFSeAsync(dps, cancellationToken);
        return await EnviarAsync(nfse, cancellationToken);
    }

    /// <summary>
    /// Monta a NFS-e a partir da DPS, resolvendo os nomes dos municípios (<c>xLocEmi</c>, <c>xLocPrestacao</c>,
    /// <c>xLocIncid</c>) pelo cadastro do município do provedor ou pela API de localidades do IBGE.
    /// O município de incidência vem de <see cref="NFSeWebserviceConfig.CodigoMunicipioIncidencia"/>.
    /// </summary>
    /// <param name="dps">DPS de origem. Não é alterada.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>NFS-e pronta para <see cref="EnviarAsync(NotaFiscalServico, CancellationToken)"/>.</returns>
    public async Task<NotaFiscalServico> MontarNFSeAsync(Dps dps, CancellationToken cancellationToken = default)
    {
        var inf = dps.Informacoes;
        var codIncidencia = Configuracao.WebServices.CodigoMunicipioIncidencia;
        var nomes = new Dictionary<string, string>();
        foreach (var codigo in new[] { inf.LocalidadeEmitente, inf.Servico.Localidade.CodMunicipioPrestacao, codIncidencia })
        {
            if (string.IsNullOrEmpty(codigo) || nomes.ContainsKey(codigo!)) continue;
            nomes[codigo!] = await ObterNomeMunicipioAsync(codigo!, cancellationToken);
        }

        var dataProcessamento = DateTimeOffset.Now.ToOffset(inf.DhEmissao.Offset);
        return SilTecnologiaNFSe.Montar(dps, codigo => nomes[codigo], dataProcessamento, codIncidencia);
    }

    /// <summary>
    /// Assina a NFS-e em <c>infNFSe</c> e envia pela operação <c>NotaFiscalNacionalGerar</c>.
    /// </summary>
    /// <param name="nfse">NFS-e montada (ex.: por <see cref="MontarNFSeAsync"/>).</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Resposta do envio.</returns>
    public async Task<NFSeResponse<RespostaEnvioDps>> EnviarAsync(NotaFiscalServico nfse, CancellationToken cancellationToken = default)
    {
        var dps = nfse.Informacoes.Dps;
        var options = OpcoesXml();

        // Valida apenas a DPS: o NFSe_v1.01.xsd exige nNFSe >= 1, mas a SIL exige nNFSe = 0.
        ValidarSchema(SchemaNFSe.DPS, dps.GetXml(options), dps.Versao);
        var xml = SilTecnologiaSoap.Assinar(nfse.GetXml(options), "infNFSe", Configuracao.Certificados.ObterCertificado());

        var documento = dps.Informacoes.Prestador.CPF ?? dps.Informacoes.Prestador.CNPJ;
        var prefixo = dps.Informacoes.NumeroDps.ZeroFill(6);
        await GravarDpsEmDiscoAsync(xml, $"{prefixo}_dps.xml", documento, dps.Informacoes.DhEmissao.DateTime,
            cancellationToken: cancellationToken);

        var envelope = SilTecnologiaSoap.Envelope("NotaFiscalNacionalGerar", xml);
        var (sucessoHttp, resposta) = await EnviarSoapAsync("NotaFiscalNacionalGerar", envelope,
            $"Enviar-{prefixo}", documento, cancellationToken);

        var retorno = SilTecnologiaSoap.Retorno(resposta);
        var resultado = new RespostaEnvioDps
        {
            IdDps = dps.Informacoes.Id,
            ChaveAcesso = SilTecnologiaSoap.ChaveAcesso(retorno),
            XmlNFSe = SilTecnologiaSoap.ElementoXml(retorno, "NFSe"),
            Erros = SilTecnologiaSoap.Erros(retorno)
        };

        if (!resultado.XmlNFSe.IsEmpty())
        {
            var nome = Configuracao.Arquivos.PadronizarNomes && !resultado.ChaveAcesso.IsEmpty() ? resultado.ChaveAcesso : prefixo;
            await GravarNFSeEmDiscoAsync(resultado.XmlNFSe, $"{nome}_nfse.xml", documento,
                dps.Informacoes.DhEmissao.DateTime, cancellationToken: cancellationToken);
        }

        var sucesso = sucessoHttp && retorno != null && resultado.Erros.Count == 0;
        return NFSeResponse<RespostaEnvioDps>.Create(envelope, resposta, sucesso, resultado);
    }

    #endregion NFS-e

    #region Eventos

    /// <summary>
    /// Cancelamento (101101) e cancelamento por substituição (105102) vão para a SIL pelas operações
    /// <c>NotaFiscalNacionalCancelar</c> e <c>NotaFiscalNacionalSubstituir</c>. Os demais eventos
    /// (confirmação, rejeição etc.) vão para a Sefin Nacional.
    /// </summary>
    /// <param name="evento">Pedido de registro de evento.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Resposta do envio do evento.</returns>
    /// <remarks>
    /// A SIL recebe o <c>evento</c> completo (<c>infEvento</c> com o <c>pedRegEvento</c> dentro), assinado em
    /// <c>infEvento</c>. Quando o retorno não traz o XML do evento, <see cref="RespostaEnvioEvento.XmlEvento"/>
    /// recebe o evento enviado.
    /// </remarks>
    public override async Task<NFSeResponse<RespostaEnvioEvento>> EnviarEventoAsync(PedidoRegistroEvento evento,
        CancellationToken cancellationToken = default)
    {
        var (operacao, codigoEvento) = evento.Informacoes.Evento switch
        {
            EventoCancelamento => ("NotaFiscalNacionalCancelar", TipoEventoCod.Cancelamento),
            EventoCancelamentoPorSubstituicao => ("NotaFiscalNacionalSubstituir", TipoEventoCod.CancelamentoPorSubstituicao),
            _ => (null, null)
        };
        if (operacao == null || codigoEvento == null) return await Nacional().EnviarEventoAsync(evento, cancellationToken);

        var inf = evento.Informacoes;
        var dataProcessamento = DateTimeOffset.Now.ToOffset(inf.DhEvento.Offset);
        var eventoXml = SilTecnologiaSoap.Evento(evento.GetXml(OpcoesXml()), inf.ChNFSe, codigoEvento,
            evento.Versao.GetDFeValue(), dataProcessamento);
        var xml = SilTecnologiaSoap.Assinar(eventoXml, "infEvento", Configuracao.Certificados.ObterCertificado());

        var documento = inf.CPFAutor ?? inf.CNPJAutor;
        await GravarDpsEmDiscoAsync(xml, $"{inf.ChNFSe}{codigoEvento}_evento.xml", documento,
            inf.DhEvento.DateTime, true, cancellationToken);

        var envelope = SilTecnologiaSoap.Envelope(operacao, xml);
        var (sucessoHttp, resposta) = await EnviarSoapAsync(operacao, envelope,
            $"Evento-{inf.ChNFSe}{codigoEvento}", documento, cancellationToken);

        var retorno = SilTecnologiaSoap.Retorno(resposta);
        var xmlEvento = SilTecnologiaSoap.ElementoXml(retorno, "evento");
        var resultado = new RespostaEnvioEvento
        {
            XmlEvento = xmlEvento.IsEmpty() ? xml : xmlEvento,
            DataHoraProcessamento = dataProcessamento,
            Erros = SilTecnologiaSoap.Erros(retorno)
        };

        var sucesso = sucessoHttp && retorno != null && resultado.Erros.Count == 0;
        return NFSeResponse<RespostaEnvioEvento>.Create(envelope, resposta, sucesso, resultado);
    }

    /// <summary>Consulta o evento na Sefin Nacional.</summary>
    public override Task<NFSeResponse<RespostaConsultaEvento>> ConsultaEventoAsync(string chaveAcesso, string tipoEvento,
        int numSeqEvento, CancellationToken cancellationToken = default) =>
        Nacional().ConsultaEventoAsync(chaveAcesso, tipoEvento, numSeqEvento, cancellationToken);

    #endregion Eventos

    #region Consultas

    /// <summary>
    /// Retorna a chave de acesso da NFS-e a partir do Id da DPS (operação <c>NotaFiscalNacionalConsultar</c>).
    /// </summary>
    /// <param name="id">Id da DPS com 45 posições (<c>DPS</c> + município + tipo de inscrição + inscrição + série + número).</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Resposta da consulta contendo a chave de acesso.</returns>
    public override async Task<NFSeResponse<RespostaConsultaChaveDps>> ConsultaChaveDpsAsync(string id,
        CancellationToken cancellationToken = default)
    {
        if (id.Length != 45 || !id.StartsWith("DPS", StringComparison.Ordinal))
            throw new ArgumentException("Id da DPS inválido: esperado DPS + 42 posições.", nameof(id));

        // Posição 10: 1 = CPF (inscrição com 3 zeros à esquerda), 2 = CNPJ.
        var inscricao = id.Substring(11, 14);
        var documento = id[10] == '1' ? inscricao.Substring(3) : inscricao;

        var xml = SilTecnologiaSoap.Assinar(SilTecnologiaSoap.Consulta(documento, null, id), null,
            Configuracao.Certificados.ObterCertificado());
        var envelope = SilTecnologiaSoap.Envelope("NotaFiscalNacionalConsultar", xml);
        var (sucessoHttp, resposta) = await EnviarSoapAsync("NotaFiscalNacionalConsultar", envelope,
            $"ConsultarDps-{id}", documento, cancellationToken);

        var retorno = SilTecnologiaSoap.Retorno(resposta);
        var resultado = new RespostaConsultaChaveDps
        {
            IdDps = id,
            Chave = SilTecnologiaSoap.ChaveAcesso(retorno),
            Erros = SilTecnologiaSoap.Erros(retorno)
        };

        var sucesso = sucessoHttp && retorno != null && resultado.Erros.Count == 0 && !resultado.Chave.IsEmpty();
        return NFSeResponse<RespostaConsultaChaveDps>.Create(envelope, resposta, sucesso, resultado);
    }

    /// <summary>
    /// Verifica se uma NFS-e foi emitida a partir do Id da DPS (via <see cref="ConsultaChaveDpsAsync"/>).
    /// </summary>
    /// <param name="id">Id da DPS com 45 posições.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>True se existir, caso contrário false.</returns>
    public override async Task<bool> ConsultaExisteDpsAsync(string id, CancellationToken cancellationToken = default)
    {
        var consulta = await ConsultaChaveDpsAsync(id, cancellationToken);
        return consulta.Sucesso;
    }

    /// <summary>
    /// Distribui os DF-e por NSU pelo ADN. A SIL compartilha as NFS-e com o ADN, então a distribuição
    /// é a mesma do provedor Nacional.
    /// </summary>
    public override Task<NFSeResponse<RespostaConsultaDFe>> ConsultaNsuAsync(int nsu, CancellationToken cancellationToken = default) =>
        Nacional().ConsultaNsuAsync(nsu, cancellationToken);

    /// <summary>Distribui os DF-e vinculados à chave pelo ADN.</summary>
    public override Task<NFSeResponse<RespostaConsultaDFe>> ConsultaChaveAsync(string chave, CancellationToken cancellationToken = default) =>
        Nacional().ConsultaChaveAsync(chave, cancellationToken);

    /// <summary>Baixa o DANFSe pelo ADN.</summary>
    public override Task<byte[]> DownloadDANFSeAsync(string chave, CancellationToken cancellationToken = default) =>
        Nacional().DownloadDANFSeAsync(chave, cancellationToken);

    #endregion Consultas

    #region Helpers

    /// <summary>
    /// Cria o provedor Nacional (Sefin/ADN) com a mesma configuração, transporte e armazenamento.
    /// </summary>
    /// <returns>Provedor Nacional.</returns>
    /// <exception cref="InvalidOperationException">Quando o serviço Nacional não está cadastrado.</exception>
    private NacionalWebservice Nacional()
    {
        var servico = NFSeServiceManager.Instance.Services.Webservices.FirstOrDefault(x => x.Provider == NFSeProvider.Nacional)
                      ?? throw new InvalidOperationException("Serviço do provedor Nacional não cadastrado.");
        return new NacionalWebservice(Configuracao, servico, HttpTransport) { DocumentStore = DocumentStore };
    }

    private DFeSaveOptions OpcoesXml() =>
        Configuracao.Geral.RetirarAcentos
            ? DFeSaveOptions.DisableFormatting | DFeSaveOptions.RemoveAccents
            : DFeSaveOptions.DisableFormatting;

    /// <summary>
    /// Retorna o nome do município: do cadastro do provedor quando for o próprio município, ou da
    /// API de localidades do IBGE (com cache) nos demais casos.
    /// </summary>
    /// <param name="codigo">Código IBGE do município (7 dígitos).</param>
    /// <param name="cancellationToken">Token para cancelar a consulta.</param>
    /// <returns>Nome do município.</returns>
    /// <exception cref="InvalidOperationException">Quando o nome não puder ser obtido.</exception>
    private async Task<string> ObterNomeMunicipioAsync(string codigo, CancellationToken cancellationToken)
    {
        if (codigo == ServiceInfo.Codigo.ToString() && !ServiceInfo.Nome.IsEmpty()) return ServiceInfo.Nome;
        if (NomesMunicipios.TryGetValue(codigo, out var nome)) return nome;

        try
        {
            using var resposta = await SendAsync(null, HttpMethod.Get, UrlIbgeMunicipio + codigo, cancellationToken: cancellationToken);
            var json = await resposta.Content.ReadAsStringAsync();
            using var documento = JsonDocument.Parse(json);
            if (resposta.IsSuccessStatusCode && documento.RootElement.ValueKind == JsonValueKind.Object &&
                documento.RootElement.TryGetProperty("nome", out var propriedade) && !string.IsNullOrEmpty(propriedade.GetString()))
                return NomesMunicipios.GetOrAdd(codigo, propriedade.GetString()!);
        }
        catch (Exception ex) when (ex is HttpRequestException or JsonException or TaskCanceledException &&
                                   !cancellationToken.IsCancellationRequested)
        {
            this.Log().Debug($"SilTecnologia: falha ao consultar o município {codigo} no IBGE - {ex.Message}");
        }

        throw new InvalidOperationException(
            $"Não foi possível obter o nome do município {codigo} na API do IBGE. Monte a NFS-e com " +
            "SilTecnologiaNFSe.Montar informando os nomes e envie com SilTecnologiaWebService.EnviarAsync(NotaFiscalServico).");
    }

    /// <summary>
    /// Envia o envelope SOAP, grava requisição/resposta em disco e retorna o status e o corpo da resposta.
    /// </summary>
    /// <param name="operacao">Nome da operação (usado no log).</param>
    /// <param name="envelope">Envelope SOAP.</param>
    /// <param name="nomeArquivo">Prefixo do nome dos arquivos gravados em disco.</param>
    /// <param name="documento">Documento do prestador para composição do caminho.</param>
    /// <param name="cancellationToken">Token para cancelar o envio.</param>
    /// <returns>Indicador de sucesso HTTP (sem fault) e o corpo da resposta.</returns>
    private async Task<(bool sucesso, string resposta)> EnviarSoapAsync(string operacao, string envelope,
        string nomeArquivo, string? documento, CancellationToken cancellationToken)
    {
        this.Log().Debug($"SilTecnologia: [{operacao}][Envio] - {envelope}");
        await GravarArquivoEmDiscoAsync(envelope, $"{nomeArquivo}-env.xml", documento, cancellationToken);

        var content = new StringContent(envelope, Encoding.UTF8, "text/xml");
        var headers = new[] { new KeyValuePair<string, string>("SOAPAction", "\"\"") };

        using var httpResponse = await SendAsync(content, HttpMethod.Post, ObterUrl(), headers, cancellationToken);
        var resposta = await httpResponse.Content.ReadAsStringAsync();

        this.Log().Debug($"SilTecnologia: [{operacao}][Resposta] - {resposta}");
        await GravarArquivoEmDiscoAsync(resposta, $"{nomeArquivo}-resp.xml", documento, cancellationToken);

        var possuiFault = SilTecnologiaSoap.PossuiFault(resposta, out var faultMsg);
        if (possuiFault) this.Log().Debug($"SilTecnologia: [{operacao}][Fault] - {faultMsg}");

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
        if (!ambiente.Enderecos.TryGetValue(TipoUrl.Enviar, out var url) || url.IsEmpty())
            throw new InvalidOperationException("Endpoint SOAP do provedor SIL Tecnologia não configurado (TipoUrl.Enviar).");

        return url;
    }

    #endregion Helpers
}
