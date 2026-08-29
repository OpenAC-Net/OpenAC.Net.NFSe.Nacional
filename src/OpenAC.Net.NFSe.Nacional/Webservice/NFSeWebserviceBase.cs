using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;
using OpenAC.Net.Core.Logging;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.Common.Types;
using OpenAC.Net.NFSe.Nacional.Storage;

namespace OpenAC.Net.NFSe.Nacional.Webservice;

/// <summary>
/// Classe base para comunicação com os webservices de NFS-e (Nacional).
/// Contém funcionalidades comuns usadas pelos webservices, como envio HTTP,
/// validação de XML via schema e gravação de arquivos em disco.
/// Implementa <see cref="IOpenLog"/> para integração com o sistema de logging.
/// </summary>
public abstract class NFSeWebserviceBase : IOpenLog
{
    #region Internal Types

    /// <summary>
    /// Tipos de arquivos manipulados pelo webservice.
    /// </summary>
    protected enum TipoArquivo
    {
        /// <summary>
        /// Arquivo de comunicação com o webservice.
        /// </summary>
        Webservice,
        /// <summary>
        /// Arquivo de RPS (Recibo Provisório de Serviços).
        /// </summary>
        Rps,
        /// <summary>
        /// Arquivo de NFS-e (Nota Fiscal de Serviço eletrônica).
        /// </summary>
        NFSe
    }

    #endregion Internal Types

    #region Constructors

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="NFSeWebserviceBase"/>.
    /// </summary>
    /// <param name="configuracaoNFSe">Configuração da NFSe.</param>
    /// <param name="serviceInfo"></param>
    /// <param name="httpTransport">Transporte HTTP fornecido pelo host ou <see langword="null"/> para usar o transporte desktop.</param>
    /// <param name="documentStore">Armazenamento de documentos fornecido pelo host ou <see langword="null"/> para usar o armazenamento desktop.</param>
    protected NFSeWebserviceBase(ConfiguracaoNFSe configuracaoNFSe, NFSeServiceInfo serviceInfo,
        INFSeHttpTransport? httpTransport = null, INFSeDocumentStore? documentStore = null)
    {
        Configuracao = configuracaoNFSe;
        ServiceInfo = serviceInfo;
        HttpTransport = httpTransport ?? DesktopNFSeHttpTransport.Instance;
        DocumentStore = documentStore ?? DesktopNFSeDocumentStore.Instancia;
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Configuração da NFSe utilizada pelo webservice.
    /// </summary>
    protected ConfiguracaoNFSe Configuracao { get; }

    /// <summary>
    /// Informações do serviço (endpoints, timeout e metadados) utilizadas pelo webservice.
    /// </summary>
    public NFSeServiceInfo ServiceInfo { get; }

    /// <summary>
    /// Transporte HTTP fornecido pelo host. Quando nulo, mantém o transporte legado para aplicações desktop.
    /// </summary>
    protected INFSeHttpTransport HttpTransport { get; }

    /// <summary>Obtém o armazenamento usado para persistir payloads técnicos e documentos fiscais.</summary>
    protected internal INFSeDocumentStore DocumentStore { get; internal set; }

    #endregion Properties

    #region Methods

    // Sobrecargas mantidas para compatibilidade com consumidores desktop existentes.
    /// <inheritdoc cref="DownloadDANFSeAsync(string, CancellationToken)" />
    public virtual Task<byte[]> DownloadDANFSeAsync(string chave) => DownloadDANFSeAsync(chave, CancellationToken.None);
    /// <inheritdoc cref="ConsultaNsuAsync(int, CancellationToken)" />
    public virtual Task<NFSeResponse<RespostaConsultaDFe>> ConsultaNsuAsync(int nsu) => ConsultaNsuAsync(nsu, CancellationToken.None);
    /// <inheritdoc cref="ConsultaChaveAsync(string, CancellationToken)" />
    public virtual Task<NFSeResponse<RespostaConsultaDFe>> ConsultaChaveAsync(string chave) => ConsultaChaveAsync(chave, CancellationToken.None);
    /// <inheritdoc cref="ConsultaChaveDpsAsync(string, CancellationToken)" />
    public virtual Task<NFSeResponse<RespostaConsultaChaveDps>> ConsultaChaveDpsAsync(string id) => ConsultaChaveDpsAsync(id, CancellationToken.None);
    /// <inheritdoc cref="ConsultaExisteDpsAsync(string, CancellationToken)" />
    public virtual Task<bool> ConsultaExisteDpsAsync(string id) => ConsultaExisteDpsAsync(id, CancellationToken.None);
    /// <inheritdoc cref="EnviarEventoAsync(PedidoRegistroEvento, CancellationToken)" />
    public virtual Task<NFSeResponse<RespostaEnvioEvento>> EnviarEventoAsync(PedidoRegistroEvento evento) => EnviarEventoAsync(evento, CancellationToken.None);
    /// <inheritdoc cref="EnviarAsync(Dps, CancellationToken)" />
    public virtual Task<NFSeResponse<RespostaEnvioDps>> EnviarAsync(Dps dps) => EnviarAsync(dps, CancellationToken.None);
    /// <inheritdoc cref="EnviarLoteAsync(LoteDps, CancellationToken)" />
    public virtual Task<NFSeResponse<RespostaRecepcaoLote>> EnviarLoteAsync(LoteDps lote) => EnviarLoteAsync(lote, CancellationToken.None);
    /// <inheritdoc cref="EnviarLoteSincronoAsync(LoteDps, CancellationToken)" />
    public virtual Task<NFSeResponse<RespostaRecepcaoLoteSincrono>> EnviarLoteSincronoAsync(LoteDps lote) => EnviarLoteSincronoAsync(lote, CancellationToken.None);
    /// <inheritdoc cref="ConsultarLoteAsync(ConsultaLoteFiltro, CancellationToken)" />
    public virtual Task<NFSeResponse<RespostaConsultaLote>> ConsultarLoteAsync(ConsultaLoteFiltro filtro) => ConsultarLoteAsync(filtro, CancellationToken.None);
    /// <inheritdoc cref="ConsultarNFSeAsync(ConsultaNFSeFiltro, CancellationToken)" />
    public virtual Task<NFSeResponse<RespostaConsultaNFSe>> ConsultarNFSeAsync(ConsultaNFSeFiltro filtro) => ConsultarNFSeAsync(filtro, CancellationToken.None);

    /// <summary>
    /// Retorna o DANFSe de uma NFS-e a partir de sua chave de acesso.
    /// </summary>
    /// <param name="chave">Chave de acesso da NFS-e.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Array de bytes contendo o DANFSe.</returns>
    public abstract Task<byte[]> DownloadDANFSeAsync(string chave, CancellationToken cancellationToken);

    /// <summary>
    /// Distribui os DF-e para contribuintes relacionados à NFS-e.
    /// </summary>
    /// <param name="nsu">Número NSU.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Resposta da consulta contendo os DF-e.</returns>
    public abstract Task<NFSeResponse<RespostaConsultaDFe>> ConsultaNsuAsync(int nsu, CancellationToken cancellationToken);

    /// <summary>
    /// Distribui os DF-e vinculados à chave de acesso informada.
    /// </summary>
    /// <param name="chave">Chave de acesso da NFS-e.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Resposta da consulta contendo os DF-e.</returns>
    public abstract Task<NFSeResponse<RespostaConsultaDFe>> ConsultaChaveAsync(string chave, CancellationToken cancellationToken);

    /// <summary>
    /// Retorna a chave de acesso da NFS-e a partir do identificador do DPS.
    /// </summary>
    /// <param name="id">Identificação do DPS.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Resposta da consulta contendo a chave de acesso.</returns>
    public abstract Task<NFSeResponse<RespostaConsultaChaveDps>> ConsultaChaveDpsAsync(string id, CancellationToken cancellationToken);

    /// <summary>
    /// Verifica se uma NFS-e foi emitida a partir do Id do DPS.
    /// </summary>
    /// <param name="id">Identificação do DPS.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>True se existir, caso contrário false.</returns>
    public abstract Task<bool> ConsultaExisteDpsAsync(string id, CancellationToken cancellationToken);

    /// <summary>
    /// Recepciona o Pedido de Registro de Evento e gera Eventos de NFS-e, crédito, débito e apuração.
    /// </summary>
    /// <param name="evento">Evento a ser enviado.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Resposta do envio do evento.</returns>
    public abstract Task<NFSeResponse<RespostaEnvioEvento>> EnviarEventoAsync(PedidoRegistroEvento evento, CancellationToken cancellationToken);

    /// <summary>
    /// Recepciona a DPS e gera a NFS-e de forma síncrona.
    /// </summary>
    /// <param name="dps">DPS a ser enviada.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Resposta do envio da DPS.</returns>
    public abstract Task<NFSeResponse<RespostaEnvioDps>> EnviarAsync(Dps dps, CancellationToken cancellationToken);

    /// <summary>
    /// Recepciona um lote de DPS de forma assíncrona, retornando o protocolo para acompanhamento.
    /// </summary>
    /// <param name="lote">Lote de DPS a ser enviado.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Resposta contendo o protocolo do lote.</returns>
    /// <remarks>Suportado apenas por provedores que expõem envio em lote (ex.: Fiorilli).</remarks>
    public virtual Task<NFSeResponse<RespostaRecepcaoLote>> EnviarLoteAsync(LoteDps lote, CancellationToken cancellationToken) =>
        throw OperacaoNaoSuportada(nameof(EnviarLoteAsync));

    /// <summary>
    /// Recepciona um lote de DPS de forma síncrona, retornando as NFS-e geradas.
    /// </summary>
    /// <param name="lote">Lote de DPS a ser enviado.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Resposta contendo o protocolo e as NFS-e geradas.</returns>
    /// <remarks>Suportado apenas por provedores que expõem envio em lote síncrono (ex.: Fiorilli).</remarks>
    public virtual Task<NFSeResponse<RespostaRecepcaoLoteSincrono>> EnviarLoteSincronoAsync(LoteDps lote, CancellationToken cancellationToken) =>
        throw OperacaoNaoSuportada(nameof(EnviarLoteSincronoAsync));

    /// <summary>
    /// Consulta o resultado do processamento de um lote de DPS a partir do protocolo.
    /// </summary>
    /// <param name="filtro">Filtro contendo o protocolo e a identificação do transmissor.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Resposta contendo a situação e as NFS-e do lote.</returns>
    /// <remarks>Suportado apenas por provedores que expõem consulta de lote (ex.: Fiorilli).</remarks>
    public virtual Task<NFSeResponse<RespostaConsultaLote>> ConsultarLoteAsync(ConsultaLoteFiltro filtro, CancellationToken cancellationToken) =>
        throw OperacaoNaoSuportada(nameof(ConsultarLoteAsync));

    /// <summary>
    /// Consulta NFS-e por chave, Id do DPS ou número/série do DPS.
    /// </summary>
    /// <param name="filtro">Filtro da consulta.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Resposta contendo as NFS-e encontradas.</returns>
    /// <remarks>Suportado apenas por provedores que expõem consulta de NFS-e (ex.: Fiorilli).</remarks>
    public virtual Task<NFSeResponse<RespostaConsultaNFSe>> ConsultarNFSeAsync(ConsultaNFSeFiltro filtro, CancellationToken cancellationToken) =>
        throw OperacaoNaoSuportada(nameof(ConsultarNFSeAsync));

    /// <summary>
    /// Cria a exceção padrão para operações não suportadas pelo provedor atual.
    /// </summary>
    /// <param name="operacao">Nome da operação não suportada.</param>
    /// <returns>Instância de <see cref="NotSupportedException"/>.</returns>
    protected NotSupportedException OperacaoNaoSuportada(string operacao) =>
        new($"A operação '{operacao}' não é suportada pelo provedor '{ServiceInfo.Provider}'.");

    /// <summary>
    /// Envia uma requisição HTTP para o webservice.
    /// </summary>
    /// <param name="content">Conteúdo da requisição.</param>
    /// <param name="method">Método HTTP.</param>
    /// <param name="url">URL do serviço.</param>
    /// <param name="headers">Cabeçalhos adicionais a serem incluídos na requisição (ex.: SOAPAction).</param>
    /// <param name="cancellationToken">Token para cancelar o envio da requisição.</param>
    /// <returns>Resposta HTTP.</returns>
    protected virtual async Task<HttpResponseMessage> SendAsync(HttpContent? content, HttpMethod method, string url,
        IEnumerable<KeyValuePair<string, string>>? headers = null, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(method, url);

        var assemblyName = GetType().Assembly.GetName();
        var productValue = new ProductInfoHeaderValue("OpenAC.Net.NFSe.Nacional", assemblyName!.Version!.ToString());
        var commentValue = new ProductInfoHeaderValue("(+https://github.com/OpenAC-Net/OpenAC.Net.NFSe.Nacional)");

        request.Headers.UserAgent.Add(productValue);
        request.Headers.UserAgent.Add(commentValue);

        if (headers != null)
            foreach (var header in headers)
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);

        request.Content = content;

        return await HttpTransport.SendAsync(request, Configuracao, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Valida o XML de acordo com o schema.
    /// </summary>
    /// <param name="schema">O schema que será usado na verificação.</param>
    /// <param name="xml">Conteúdo XML a ser validado.</param>
    /// <param name="versao">Versão do Schema a ser carregado</param>
    /// <exception cref="XmlSchemaException">Lançada se o arquivo de schema não for encontrado.</exception>
    /// <exception cref="XmlSchemaValidationException">Lançada se houver erros de validação.</exception>
    protected virtual void ValidarSchema(SchemaNFSe schema, string xml, VersaoNFSe versao = VersaoNFSe.Ve100)
    {
        if (!Configuracao.WebServices.ValidarSchemas)
            return;

        Configuracao.Arquivos.VersaoSchema = versao;
        var schemaFile = Configuracao.Arquivos.GetSchema(schema);
        if (!File.Exists(schemaFile))
            throw new XmlSchemaException("Nao encontrou o arquivo schema do xml => " + schemaFile);

        if (XmlSchemaValidation.ValidarXml(xml, schemaFile, out var errosSchema, out _)) return;

        throw new XmlSchemaValidationException("Erros gerado ao validar o schema do xml" + Environment.NewLine +
                                               string.Join(Environment.NewLine, errosSchema));
    }

    /// <summary>
    /// Valida o XML de acordo com o schema.
    /// </summary>
    /// <param name="schemaFile">Arquivo do schema.</param>
    /// <param name="xml">Conteúdo XML a ser validado.</param>
    /// <param name="versao">Versão do Schema a ser carregado</param>
    /// <exception cref="XmlSchemaException">Lançada se o arquivo de schema não for encontrado.</exception>
    /// <exception cref="XmlSchemaValidationException">Lançada se houver erros de validação.</exception>
    protected virtual void ValidarSchema(string schemaFile, string xml, VersaoNFSe versao = VersaoNFSe.Ve100)
    {
        if (!Configuracao.WebServices.ValidarSchemas)
            return;

        Configuracao.Arquivos.VersaoSchema = versao;

        if (!File.Exists(schemaFile))
            throw new XmlSchemaException("Nao encontrou o arquivo schema do xml => " + schemaFile);

        if (XmlSchemaValidation.ValidarXml(xml, schemaFile, out var errosSchema, out _)) return;

        throw new XmlSchemaValidationException("Erros gerado ao validar o schema do xml" + Environment.NewLine +
                                               string.Join(Environment.NewLine, errosSchema));
    }

    /// <summary>
    /// Grava o xml da Dps no disco.
    /// </summary>
    /// <param name="conteudoArquivo">Conteúdo do arquivo.</param>
    /// <param name="nomeArquivo">Nome do arquivo.</param>
    /// <param name="documento">Documento do prestador.</param>
    /// <param name="data">Data de emissão.</param>
    /// <param name="incrementarNome">Indica se o nome do arquivo deve ser incrementado caso já exista.</param>
    /// <param name="cancellationToken">Token para cancelar a gravação.</param>
    protected virtual Task GravarDpsEmDiscoAsync(string conteudoArquivo, string nomeArquivo, string? documento,
        DateTime data, bool incrementarNome = false, CancellationToken cancellationToken = default)
    {
        if (!Configuracao.Arquivos.Salvar) return Task.CompletedTask;
        return DocumentStore.SaveAsync(new NFSeDocument(NFSeDocumentType.Dps, conteudoArquivo, nomeArquivo,
            documento, data, incrementarNome), Configuracao, cancellationToken);
    }

    /// <summary>
    /// Grava o xml da NFSe no disco.
    /// </summary>
    /// <param name="conteudoArquivo">Conteúdo do arquivo.</param>
    /// <param name="nomeArquivo">Nome do arquivo.</param>
    /// <param name="documento">Documento do prestador.</param>
    /// <param name="data">Data de emissão.</param>
    /// <param name="incrementarNome">Indica se o nome do arquivo deve ser incrementado caso já exista.</param>
    /// <param name="cancellationToken">Token para cancelar a gravação.</param>
    protected virtual Task GravarNFSeEmDiscoAsync(string conteudoArquivo, string nomeArquivo, string? documento,
        DateTime data, bool incrementarNome = false, CancellationToken cancellationToken = default)
    {
        if (!Configuracao.Arquivos.Salvar) return Task.CompletedTask;
        return DocumentStore.SaveAsync(new NFSeDocument(NFSeDocumentType.NFSe, conteudoArquivo, nomeArquivo,
            documento, data, incrementarNome), Configuracao, cancellationToken);
    }

    /// <summary>
    /// Grava o xml de comunicação com o webservice no disco.
    /// </summary>
    /// <param name="conteudoArquivo">Conteúdo do arquivo.</param>
    /// <param name="nomeArquivo">Nome do arquivo.</param>
    /// <param name="documento">Documento do prestador.</param>
    /// <param name="cancellationToken">Token para cancelar a gravação.</param>
    protected virtual Task GravarArquivoEmDiscoAsync(string conteudoArquivo, string nomeArquivo, string? documento,
        CancellationToken cancellationToken = default)
    {
        if (!Configuracao.Geral.Salvar) return Task.CompletedTask;
        var type = nomeArquivo.Contains("-resp.", StringComparison.OrdinalIgnoreCase)
            ? NFSeDocumentType.RespostaTecnica
            : NFSeDocumentType.SolicitacaoTecnica;
        return DocumentStore.SaveAsync(new NFSeDocument(type, conteudoArquivo, nomeArquivo, documento,
            DateTime.Today), Configuracao, cancellationToken);
    }

    #endregion Methods
}
