// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : OpenAC .Net
// ***********************************************************************
// <copyright file="GovBRWebService.cs" company="OpenAC .Net">
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
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.Core.Logging;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.Common.Types;

namespace OpenAC.Net.NFSe.Nacional.Webservice.GovBR;

/// <summary>
/// Comunicação com a API REST/JSON "ISS DIGITAL" da GovernancaBrasil, usada por municípios como
/// Jacarezinho/PR (ambiente de testes em <c>https://reformatributaria.cidade360.cloud/NFSe.Api.Teste</c>).
/// </summary>
/// <remarks>
/// O XML de DPS/NFS-e/Evento segue o layout nacional (mesmos models e schemas do provedor
/// <see cref="Nacional.NacionalWebservice"/>), mas o transporte HTTP é totalmente proprietário:
/// <list type="bullet">
/// <item>Todo envio (mesmo de um único documento) é feito como um "lote" JSON com uma lista de
/// XMLs compactados em GZip e codificados em Base64 (<c>loteXmlGZipB64</c>), em vez do corpo
/// <c>{"dpsXmlGZipB64": "..."}</c> usado pelo provedor Nacional.</item>
/// <item>Os endpoints ficam sob o prefixo <c>/NotaNacional/</c> e usam nomes de operação
/// (<c>EnviarSincrono</c>, <c>EnviarAssincrono</c>) em vez de rotas REST por recurso.</item>
/// <item>Não há endpoint de distribuição de DF-e por NSU/chave (<see cref="ConsultaNsuAsync"/> e
/// <see cref="ConsultaChaveAsync"/> lançam <see cref="NotSupportedException"/>).</item>
/// </list>
/// A autenticação por certificado digital (mTLS) é tratada de forma transparente pelo
/// <see cref="INFSeHttpTransport"/> configurado (o mesmo mecanismo usado pelos demais provedores),
/// não exige nenhum código específico aqui.
/// </remarks>
public class GovBRWebService : NFSeWebserviceBase
{
    #region Fields

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private const string SucessoProcessamento = "SUCESSO";

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="GovBRWebService"/>.
    /// </summary>
    /// <param name="configuracaoNFSe">Configuração da NFSe.</param>
    /// <param name="serviceInfo">Informações do serviço.</param>
    public GovBRWebService(ConfiguracaoNFSe configuracaoNFSe, NFSeServiceInfo serviceInfo) :
        base(configuracaoNFSe, serviceInfo)
    {
    }

    /// <summary>Inicializa o provedor com um transporte HTTP fornecido pelo host.</summary>
    public GovBRWebService(ConfiguracaoNFSe configuracaoNFSe, NFSeServiceInfo serviceInfo,
        INFSeHttpTransport httpTransport) : base(configuracaoNFSe, serviceInfo, httpTransport)
    {
    }

    #endregion Constructors

    #region NFS-e

    /// <summary>
    /// Recepciona a DPS e gera a NFS-e de forma síncrona (operação <c>EnviarSincrono</c>, lote com um único item).
    /// </summary>
    public override async Task<NFSeResponse<RespostaEnvioDps>> EnviarAsync(Dps dps, CancellationToken cancellationToken = default)
    {
        dps.Assinar(Configuracao);
        ValidarSchema(SchemaNFSe.DPS, dps.Xml, dps.Versao);

        var documento = dps.Informacoes.Prestador.CPF ?? dps.Informacoes.Prestador.CNPJ ??
            throw new InvalidOperationException("CPF ou CNPJ do prestador deve ser informado.");
        var prefixo = dps.Informacoes.NumeroDps.ZeroFill(6);

        await GravarDpsEmDiscoAsync(dps.Xml, $"{prefixo}_dps.xml", documento, dps.Informacoes.DhEmissao.DateTime,
            cancellationToken: cancellationToken);

        var (sucessoHttp, retorno, strEnvio, strResposta) = await PostLoteAsync(
            [ComprimirGZipBase64(dps.Xml)], sincrono: true, documento, $"Enviar-{prefixo}", cancellationToken);

        var item = retorno?.Lote.FirstOrDefault();
        var resultado = MontarRespostaEnvioDps(item);

        if (!resultado.XmlNFSe.IsEmpty())
        {
            var nomeNfse = Configuracao.Arquivos.PadronizarNomes ? resultado.ChaveAcesso : prefixo;
            await GravarNFSeEmDiscoAsync(resultado.XmlNFSe, $"{nomeNfse}_nfse.xml", documento,
                dps.Informacoes.DhEmissao.DateTime, cancellationToken: cancellationToken);
        }

        var sucesso = sucessoHttp && EhSucesso(item) && !resultado.ChaveAcesso.IsEmpty();
        return NFSeResponse<RespostaEnvioDps>.Create(strEnvio, strResposta, sucesso, resultado);
    }

    #endregion NFS-e

    #region Eventos

    /// <summary>
    /// Recepciona o pedido de registro de evento (operação <c>EnviarSincrono</c>, lote com um único item).
    /// </summary>
    public override async Task<NFSeResponse<RespostaEnvioEvento>> EnviarEventoAsync(PedidoRegistroEvento evento, CancellationToken cancellationToken = default)
    {
        evento.Assinar(Configuracao);
        ValidarSchema(SchemaNFSe.Evento, evento.Xml, evento.Versao);

        var documento = evento.Informacoes.CPFAutor ?? evento.Informacoes.CNPJAutor ??
            throw new InvalidOperationException("CPF ou CNPJ do autor do evento deve ser informado.");
        var prefixo = $"{evento.Informacoes.ChNFSe}{evento.Informacoes.Evento}";

        await GravarDpsEmDiscoAsync(evento.Xml, $"{prefixo}_evento.xml", documento, evento.Informacoes.DhEvento.DateTime,
            cancellationToken: cancellationToken);

        var (sucessoHttp, retorno, strEnvio, strResposta) = await PostLoteAsync(
            [ComprimirGZipBase64(evento.Xml)], sincrono: true, documento, $"Evento-{prefixo}", cancellationToken);

        var item = retorno?.Lote.FirstOrDefault();
        var resultado = MontarRespostaEnvioEvento(item);

        if (!resultado.XmlEvento.IsEmpty())
            await GravarNFSeEmDiscoAsync(resultado.XmlEvento, $"{prefixo}_evento_resp.xml", documento,
                evento.Informacoes.DhEvento.DateTime, cancellationToken: cancellationToken);

        var sucesso = sucessoHttp && EhSucesso(item);
        return NFSeResponse<RespostaEnvioEvento>.Create(strEnvio, strResposta, sucesso, resultado);
    }

    /// <summary>
    /// Consulta eventos de uma NFS-e pela chave de acesso (operação <c>ConsultarEventos</c>), filtrando
    /// pelo tipo de evento e, quando informado, pelo número sequencial (lido de dentro do XML do evento -
    /// a API não expõe esse número como campo próprio).
    /// </summary>
    public override async Task<NFSeResponse<RespostaConsultaEvento>> ConsultaEventoAsync(string chaveAcesso, string tipoEvento,
        int numSeqEvento, CancellationToken cancellationToken = default)
    {
        var url = $"{ObterUrlBase()}/NotaNacional/ConsultarEventos/{Uri.EscapeDataString(chaveAcesso)}";
        using var httpResponse = await SendAsync(null, HttpMethod.Get, url, cancellationToken: cancellationToken);
        var strResposta = await httpResponse.Content.ReadAsStringAsync();

        await GravarArquivoEmDiscoAsync(strResposta, $"ConsultarEventos-{chaveAcesso}-resp.json", null, cancellationToken);

        var eventos = TryDeserialize<List<DadosEventoDto>>(strResposta) ?? [];
        var resultado = new RespostaConsultaEvento();

        foreach (var evento in eventos)
        {
            if (!tipoEvento.IsEmpty() && !string.Equals(evento.Tipo, tipoEvento, StringComparison.Ordinal))
                continue;

            var xml = DescomprimirGZipBase64(evento.XmlGZipB64);
            var nSeqEvento = ExtrairNSeqEvento(xml);
            if (numSeqEvento > 0 && nSeqEvento != numSeqEvento)
                continue;

            resultado.Eventos.Add(new RespostaConsultaEvento.Evento
            {
                ChaveAcesso = chaveAcesso,
                TipoEvento = int.TryParse(evento.Tipo, out var tipo) ? tipo : 0,
                NumeroPedidoRegistroEvento = nSeqEvento,
                DataHoraRecebimento = DateTime.TryParse(evento.DataInclusao, CultureInfo.InvariantCulture, DateTimeStyles.None, out var data) ? data : default,
                XmlEvento = xml
            });
        }

        var sucesso = httpResponse.IsSuccessStatusCode && resultado.Eventos.Count > 0;
        return NFSeResponse<RespostaConsultaEvento>.Create(string.Empty, strResposta, sucesso, resultado);
    }

    #endregion Eventos

    #region Lote

    /// <summary>
    /// Recepciona um lote de DPS de forma assíncrona (operação <c>EnviarAssincrono</c>), retornando o
    /// protocolo (UUID) para consulta posterior via <see cref="ConsultarLoteAsync"/>.
    /// </summary>
    public override async Task<NFSeResponse<RespostaRecepcaoLote>> EnviarLoteAsync(LoteDps lote, CancellationToken cancellationToken = default)
    {
        var (xmls, documento) = PrepararLote(lote);

        var (sucessoHttp, retorno, strEnvio, strResposta) = await PostLoteAsync(
            xmls, sincrono: false, documento, $"EnviarLote-{lote.NumeroLote}", cancellationToken);

        var resultado = new RespostaRecepcaoLote
        {
            NumeroLote = lote.NumeroLote,
            Protocolo = retorno?.Protocolo ?? string.Empty,
            DataRecebimento = ParseData(retorno?.DataHoraProcessamento)
        };
        PreencherMensagensDoLote(resultado.Mensagens, retorno);

        var sucesso = sucessoHttp && !resultado.Protocolo.IsEmpty();
        return NFSeResponse<RespostaRecepcaoLote>.Create(strEnvio, strResposta, sucesso, resultado);
    }

    /// <summary>
    /// Recepciona um lote de DPS de forma síncrona (operação <c>EnviarSincrono</c>), retornando as
    /// NFS-e já geradas.
    /// </summary>
    public override async Task<NFSeResponse<RespostaRecepcaoLoteSincrono>> EnviarLoteSincronoAsync(LoteDps lote, CancellationToken cancellationToken = default)
    {
        var (xmls, documento) = PrepararLote(lote);

        var (sucessoHttp, retorno, strEnvio, strResposta) = await PostLoteAsync(
            xmls, sincrono: true, documento, $"EnviarLoteSincrono-{lote.NumeroLote}", cancellationToken);

        var resultado = new RespostaRecepcaoLoteSincrono
        {
            NumeroLote = lote.NumeroLote,
            Protocolo = retorno?.Protocolo ?? string.Empty,
            DataRecebimento = ParseData(retorno?.DataHoraProcessamento)
        };
        PreencherMensagensDoLote(resultado.Mensagens, retorno);
        resultado.NotasFiscais.AddRange(CarregarNotas(retorno));

        var sucesso = sucessoHttp && resultado.NotasFiscais.Count > 0;
        return NFSeResponse<RespostaRecepcaoLoteSincrono>.Create(strEnvio, strResposta, sucesso, resultado);
    }

    /// <summary>
    /// Consulta o processamento de um lote assíncrono pelo protocolo (operação <c>ConsultarProtocolo</c>).
    /// </summary>
    /// <remarks>O protocolo é temporário: a própria API o descarta 10 dias após o envio.</remarks>
    public override async Task<NFSeResponse<RespostaConsultaLote>> ConsultarLoteAsync(ConsultaLoteFiltro filtro, CancellationToken cancellationToken = default)
    {
        if (filtro.Protocolo.IsEmpty())
            throw new InvalidOperationException("Informe o protocolo (UUID) retornado pelo envio assíncrono para consultar o lote no provedor GovernancaBrasil.");

        var documento = filtro.CNPJ ?? filtro.CPF;
        var url = $"{ObterUrlBase()}/NotaNacional/ConsultarProtocolo/{Uri.EscapeDataString(filtro.Protocolo)}";

        using var httpResponse = await SendAsync(null, HttpMethod.Get, url, cancellationToken: cancellationToken);
        var strResposta = await httpResponse.Content.ReadAsStringAsync();
        await GravarArquivoEmDiscoAsync(strResposta, $"ConsultarProtocolo-{filtro.Protocolo}-resp.json", documento, cancellationToken);

        var retorno = TryDeserialize<RetornoLoteDto>(strResposta);

        var resultado = new RespostaConsultaLote
        {
            Situacao = retorno?.Processado == true ? 1 : 0
        };
        PreencherMensagensDoLote(resultado.Mensagens, retorno);
        resultado.NotasFiscais.AddRange(CarregarNotas(retorno));

        var sucesso = httpResponse.IsSuccessStatusCode && retorno is { Processado: true };
        return NFSeResponse<RespostaConsultaLote>.Create(string.Empty, strResposta, sucesso, resultado);
    }

    #endregion Lote

    #region Consultas

    /// <summary>
    /// Consulta NFS-e por chave de acesso (operação <c>ConsultarNFSe</c>) ou, na ausência dela, por
    /// CNPJ/CPF + série + número do DPS (operação <c>ConsultarDPS</c>).
    /// </summary>
    public override async Task<NFSeResponse<RespostaConsultaNFSe>> ConsultarNFSeAsync(ConsultaNFSeFiltro filtro, CancellationToken cancellationToken = default)
    {
        string url;
        string identificador;

        if (!string.IsNullOrEmpty(filtro.ChaveNFSe))
        {
            identificador = filtro.ChaveNFSe!;
            url = $"{ObterUrlBase()}/NotaNacional/ConsultarNFSe/{Uri.EscapeDataString(identificador)}";
        }
        else
        {
            var documento = filtro.CNPJ ?? filtro.CPF ??
                throw new InvalidOperationException("Informe a ChaveNFSe ou o CNPJ/CPF junto com Série/Número do DPS para consultar no provedor GovernancaBrasil.");
            if (string.IsNullOrEmpty(filtro.SerieDPS) || string.IsNullOrEmpty(filtro.NumeroDPS))
                throw new InvalidOperationException("Informe a Série e o Número do DPS para consulta por CNPJ/CPF no provedor GovernancaBrasil.");

            identificador = $"{documento}-{filtro.SerieDPS}-{filtro.NumeroDPS}";
            url = $"{ObterUrlBase()}/NotaNacional/ConsultarDPS/{documento}/{int.Parse(filtro.SerieDPS!, CultureInfo.InvariantCulture)}/{long.Parse(filtro.NumeroDPS!, CultureInfo.InvariantCulture)}";
        }

        using var httpResponse = await SendAsync(null, HttpMethod.Get, url, cancellationToken: cancellationToken);
        var strResposta = await httpResponse.Content.ReadAsStringAsync();
        await GravarArquivoEmDiscoAsync(strResposta, $"ConsultarNFSe-{identificador}-resp.json", filtro.CNPJ ?? filtro.CPF, cancellationToken);

        var retorno = TryDeserialize<RetornoConsultaDto>(strResposta);
        var resultado = new RespostaConsultaNFSe();
        resultado.NotasFiscais.AddRange(CarregarNotas(retorno?.Notas));

        var sucesso = httpResponse.IsSuccessStatusCode && resultado.NotasFiscais.Count > 0;
        return NFSeResponse<RespostaConsultaNFSe>.Create(string.Empty, strResposta, sucesso, resultado);
    }

    /// <summary>
    /// Retorna a chave de acesso da NFS-e a partir do Id do DPS, decompondo o próprio Id (layout
    /// <c>TSIdDPS</c>: documento + série + número) e consultando via <c>ConsultarDPS</c> - esta API
    /// não expõe consulta direta pelo Id do DPS.
    /// </summary>
    public override async Task<NFSeResponse<RespostaConsultaChaveDps>> ConsultaChaveDpsAsync(string id, CancellationToken cancellationToken = default)
    {
        var (documento, serie, numero) = ExtrairChaveDoIdDps(id);
        var url = $"{ObterUrlBase()}/NotaNacional/ConsultarDPS/{documento}/{serie}/{numero}";

        using var httpResponse = await SendAsync(null, HttpMethod.Get, url, cancellationToken: cancellationToken);
        var strResposta = await httpResponse.Content.ReadAsStringAsync();
        await GravarArquivoEmDiscoAsync(strResposta, $"ConsultaChaveDps-{id}-resp.json", documento, cancellationToken);

        var retorno = TryDeserialize<RetornoConsultaDto>(strResposta);
        var primeira = retorno?.Notas.FirstOrDefault();

        var resultado = new RespostaConsultaChaveDps
        {
            IdDps = id,
            Chave = primeira?.Chave ?? string.Empty
        };

        var sucesso = httpResponse.IsSuccessStatusCode && !resultado.Chave.IsEmpty();
        return NFSeResponse<RespostaConsultaChaveDps>.Create(string.Empty, strResposta, sucesso, resultado);
    }

    /// <summary>
    /// Verifica se uma NFS-e foi emitida a partir do Id do DPS (via <see cref="ConsultaChaveDpsAsync"/>).
    /// </summary>
    public override async Task<bool> ConsultaExisteDpsAsync(string id, CancellationToken cancellationToken = default)
    {
        var consulta = await ConsultaChaveDpsAsync(id, cancellationToken);
        return consulta.Sucesso;
    }

    /// <summary>
    /// Retorna o DANFSe de uma NFS-e a partir de sua chave de acesso (operação <c>DownloadPDFChave</c>).
    /// </summary>
    public override async Task<byte[]> DownloadDANFSeAsync(string chave, CancellationToken cancellationToken = default)
    {
        var url = $"{ObterUrlBase()}/NotaNacional/DownloadPDFChave/{Uri.EscapeDataString(chave)}";
        using var httpResponse = await SendAsync(null, HttpMethod.Get, url, cancellationToken: cancellationToken);
        httpResponse.EnsureSuccessStatusCode();

        var strResposta = await httpResponse.Content.ReadAsStringAsync();
        var retorno = TryDeserialize<RetornoPdfDto>(strResposta) ??
            throw new InvalidOperationException("Resposta inválida ao baixar o DANFSe do provedor GovernancaBrasil.");

        return DescomprimirGZipBase64Bytes(retorno.PdfGZipB64);
    }

    #endregion Consultas

    #region Não suportadas

    /// <summary>Não suportado - a API não expõe distribuição de DF-e por NSU.</summary>
    public override Task<NFSeResponse<RespostaConsultaDFe>> ConsultaNsuAsync(int nsu, CancellationToken cancellationToken = default) =>
        throw OperacaoNaoSuportada(nameof(ConsultaNsuAsync));

    /// <summary>Não suportado - a API não expõe distribuição de DF-e por chave de acesso.</summary>
    public override Task<NFSeResponse<RespostaConsultaDFe>> ConsultaChaveAsync(string chave, CancellationToken cancellationToken = default) =>
        throw OperacaoNaoSuportada(nameof(ConsultaChaveAsync));

    #endregion Não suportadas

    #region Helpers

    /// <summary>Obtém a URL base do provedor para o ambiente atual (todas as operações usam o mesmo host).</summary>
    /// <exception cref="InvalidOperationException">Quando o endpoint não está configurado.</exception>
    private string ObterUrlBase()
    {
        var ambiente = ServiceInfo[Configuracao.WebServices.Ambiente];
        if (!ambiente.Enderecos.TryGetValue(TipoUrl.Enviar, out var url) || string.IsNullOrEmpty(url))
            throw new InvalidOperationException("Endpoint do provedor GovernancaBrasil (ISS Digital) não configurado (TipoUrl.Enviar).");

        return url.TrimEnd('/');
    }

    /// <summary>
    /// Envia um lote (síncrono ou assíncrono) já com os XMLs compactados, grava requisição/resposta
    /// em disco e desserializa o retorno.
    /// </summary>
    private async Task<(bool sucessoHttp, RetornoLoteDto? retorno, string strEnvio, string strResposta)> PostLoteAsync(
        List<string> xmlsGZipB64, bool sincrono, string? documento, string nomeArquivo, CancellationToken cancellationToken)
    {
        var envioDto = new EnvioLoteDto { LoteXmlGZipB64 = xmlsGZipB64 };
        using var content = JsonContent.Create(envioDto, options: JsonOptions);
        var strEnvio = await content.ReadAsStringAsync();

        var operacao = sincrono ? "EnviarSincrono" : "EnviarAssincrono";
        this.Log().Debug($"GovBR: [{operacao}][Envio] - {strEnvio}");
        await GravarArquivoEmDiscoAsync(strEnvio, $"{nomeArquivo}-env.json", documento, cancellationToken);

        var url = $"{ObterUrlBase()}/NotaNacional/{operacao}";
        using var httpResponse = await SendAsync(content, HttpMethod.Post, url, cancellationToken: cancellationToken);
        var strResposta = await httpResponse.Content.ReadAsStringAsync();

        this.Log().Debug($"GovBR: [{operacao}][Resposta] - {strResposta}");
        await GravarArquivoEmDiscoAsync(strResposta, $"{nomeArquivo}-resp.json", documento, cancellationToken);

        var retorno = TryDeserialize<RetornoLoteDto>(strResposta);
        return (httpResponse.IsSuccessStatusCode, retorno, strEnvio, strResposta);
    }

    /// <summary>Assina e valida cada DPS do lote, retornando os XMLs já compactados e o documento do transmissor.</summary>
    private (List<string> xmls, string? documento) PrepararLote(LoteDps lote)
    {
        var xmls = new List<string>();
        foreach (var dps in lote.Documentos)
        {
            dps.Assinar(Configuracao);
            ValidarSchema(SchemaNFSe.DPS, dps.Xml, dps.Versao);
            xmls.Add(ComprimirGZipBase64(dps.Xml));
        }

        return (xmls, lote.CNPJ ?? lote.CPF);
    }

    /// <summary>Carrega como <see cref="NotaFiscalServico"/> os itens de lote processados com sucesso.</summary>
    private static IEnumerable<NotaFiscalServico> CarregarNotas(RetornoLoteDto? retorno) =>
        CarregarNotas((retorno?.Lote ?? []).Where(item => EhSucesso(item)).Select(item => item.XmlGZipB64));

    /// <summary>Carrega como <see cref="NotaFiscalServico"/> os itens retornados por uma consulta de NFS-e/DPS.</summary>
    private static IEnumerable<NotaFiscalServico> CarregarNotas(IEnumerable<DadosNotaDto>? notas) =>
        CarregarNotas((notas ?? []).Select(nota => nota.XmlGZipB64));

    private static IEnumerable<NotaFiscalServico> CarregarNotas(IEnumerable<string> xmlsGZipB64)
    {
        foreach (var xmlGZipB64 in xmlsGZipB64)
        {
            if (xmlGZipB64.IsEmpty()) continue;

            var xml = DescomprimirGZipBase64(xmlGZipB64);
            if (xml.IndexOf("<NFSe", StringComparison.OrdinalIgnoreCase) < 0) continue; // ignora eventos misturados no lote

            var nota = NotaFiscalServico.Load(xml);
            if (nota != null) yield return nota;
        }
    }

    private static RespostaEnvioDps MontarRespostaEnvioDps(RetornoItemLoteDto? item)
    {
        var resultado = new RespostaEnvioDps
        {
            IdDps = item?.Id ?? string.Empty,
            ChaveAcesso = item?.ChaveAcesso ?? string.Empty
        };

        if (item is not null && !item.XmlGZipB64.IsEmpty())
            resultado.XmlNFSe = DescomprimirGZipBase64(item.XmlGZipB64);

        AdicionarErros(resultado.Erros, item);
        return resultado;
    }

    private static RespostaEnvioEvento MontarRespostaEnvioEvento(RetornoItemLoteDto? item)
    {
        var resultado = new RespostaEnvioEvento();

        if (item is not null && !item.XmlGZipB64.IsEmpty())
            resultado.XmlEvento = DescomprimirGZipBase64(item.XmlGZipB64);

        AdicionarErros(resultado.Erros, item);
        return resultado;
    }

    private static void PreencherMensagensDoLote(List<MensagemProcessamento> mensagens, RetornoLoteDto? retorno)
    {
        foreach (var item in retorno?.Lote ?? [])
            AdicionarErros(mensagens, item, item.Id);
    }

    private static void AdicionarErros(List<MensagemProcessamento> mensagens, RetornoItemLoteDto? item, string? prefixo = null)
    {
        foreach (var erro in item?.Erros ?? [])
        {
            mensagens.Add(new MensagemProcessamento
            {
                Codigo = erro.Codigo,
                Descricao = erro.Descricao,
                Mensagem = string.IsNullOrEmpty(prefixo) ? erro.Descricao : $"{prefixo}: {erro.Descricao}"
            });
        }
    }

    private static bool EhSucesso(RetornoItemLoteDto? item) =>
        item is not null && string.Equals(item.StatusProcessamento, SucessoProcessamento, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Decompõe o Id do DPS (layout <c>TSIdDPS</c>, 45 posições: "DPS" + Cód.Mun.(7) + Tipo Insc.(1) +
    /// Insc.Federal(14) + Série(5) + Núm.DPS(15)) nos parâmetros exigidos por <c>ConsultarDPS</c>.
    /// </summary>
    /// <seealso cref="Dps.GerarId"/>
    private static (string documento, int serie, long numero) ExtrairChaveDoIdDps(string id)
    {
        if (id.Length != 45 || !id.StartsWith("DPS", StringComparison.Ordinal))
            throw new ArgumentException("Id do DPS em formato inesperado (esperado 45 caracteres iniciando com \"DPS\").", nameof(id));

        var tipoInsc = id[10];
        var campoDocumento = id.Substring(11, 14);
        var documento = tipoInsc == '1' ? campoDocumento.Substring(3) : campoDocumento;
        var serie = int.Parse(id.Substring(25, 5), CultureInfo.InvariantCulture);
        var numero = long.Parse(id.Substring(30, 15), CultureInfo.InvariantCulture);

        return (documento, serie, numero);
    }

    private static int ExtrairNSeqEvento(string xmlEvento)
    {
        if (xmlEvento.IsEmpty()) return 0;

        var valor = XDocument.Parse(xmlEvento).Descendants()
            .FirstOrDefault(x => x.Name.LocalName == "nSeqEvento")?.Value;
        return int.TryParse(valor, out var nSeqEvento) ? nSeqEvento : 0;
    }

    private static DateTimeOffset ParseData(string? valor) =>
        DateTimeOffset.TryParse(valor, CultureInfo.InvariantCulture, DateTimeStyles.None, out var data) ? data : default;

    private static string ComprimirGZipBase64(string xml)
    {
        var bytes = Encoding.UTF8.GetBytes(xml);
        using var memoryStream = new MemoryStream();
        using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress))
            gZipStream.Write(bytes, 0, bytes.Length);

        return Convert.ToBase64String(memoryStream.ToArray());
    }

    private static string DescomprimirGZipBase64(string base64) =>
        base64.IsEmpty() ? string.Empty : Encoding.UTF8.GetString(DescomprimirGZipBase64Bytes(base64));

    private static byte[] DescomprimirGZipBase64Bytes(string base64)
    {
        var gzipBytes = Convert.FromBase64String(base64);
        using var memoryStream = new MemoryStream(gzipBytes);
        using var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
        using var memoryStreamOutput = new MemoryStream();
        gZipStream.CopyTo(memoryStreamOutput);

        return memoryStreamOutput.ToArray();
    }

    private T? TryDeserialize<T>(string json) where T : class
    {
        try
        {
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }
        catch (Exception e)
        {
            this.Log().Error(e);
            return null;
        }
    }

    #endregion Helpers
}
