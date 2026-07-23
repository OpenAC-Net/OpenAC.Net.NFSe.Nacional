// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : OpenAC .Net
// ***********************************************************************
// <summary>
// Utilitários para montagem e leitura dos envelopes SOAP do provedor Fiorilli.
// </summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.NFSe.Nacional.Common.Model;

namespace OpenAC.Net.NFSe.Nacional.Webservice.Fiorilli;

/// <summary>
/// Utilitários para montagem e leitura dos envelopes SOAP do provedor Fiorilli
/// (serviço <c>IssWebWSNacional</c>).
/// </summary>
/// <remarks>
/// Os documentos assinados (DPS, pedRegEvento e NFS-e) são injetados/lidos de forma literal para
/// preservar a assinatura digital byte a byte. Por isso o envelope é montado por concatenação de
/// strings (e não via <see cref="XDocument"/>), evitando qualquer recanonização do conteúdo assinado.
/// <para/>
/// NOTA sobre namespaces: os elementos "invólucro" (RecepcionarDpsEnvio, LoteDps, IM, Protocolo, etc.)
/// ficam no namespace do provedor (<see cref="NsFiorilli"/>), enquanto os documentos DPS/pedRegEvento/NFS-e
/// carregam o próprio namespace nacional (<see cref="NsSped"/>). Caso a Fiorilli rejeite o documento por
/// namespace/schema em homologação, este é o primeiro ponto a ajustar.
/// </remarks>
internal static class FiorilliSoap
{
    #region Constants

    /// <summary>Namespace do envelope SOAP 1.1.</summary>
    public const string NsSoapEnv = "http://schemas.xmlsoap.org/soap/envelope/";

    /// <summary>Namespace do provedor Fiorilli (invólucros das operações).</summary>
    public const string NsFiorilli = "http://www.fiorilli.com.br/nfse-nacional";

    /// <summary>Namespace do padrão nacional (documentos DPS/NFS-e/eventos).</summary>
    public const string NsSped = "http://www.sped.fazenda.gov.br/nfse";

    #endregion Constants

    #region Build

    /// <summary>
    /// Monta o envelope SOAP 1.1 ao redor do corpo informado.
    /// </summary>
    /// <param name="corpo">Conteúdo já serializado do corpo (Body) da requisição.</param>
    /// <returns>Envelope SOAP completo.</returns>
    public static string Envelope(string corpo) =>
        $"<soapenv:Envelope xmlns:soapenv=\"{NsSoapEnv}\">" +
        "<soapenv:Header/>" +
        $"<soapenv:Body>{corpo}</soapenv:Body>" +
        "</soapenv:Envelope>";

    /// <summary>
    /// Remove a declaração XML (<c>&lt;?xml ... ?&gt;</c>) e a BOM de um documento, permitindo
    /// que ele seja embutido dentro de outro XML.
    /// </summary>
    /// <param name="xml">Documento XML.</param>
    /// <returns>Documento sem o prólogo.</returns>
    public static string RemoverProlog(string xml)
    {
        if (xml.IsEmpty()) return string.Empty;

        var conteudo = xml.TrimStart('﻿', ' ', '\r', '\n', '\t');
        if (!conteudo.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase)) return conteudo;

        var fim = conteudo.IndexOf("?>", StringComparison.Ordinal);
        return fim < 0 ? conteudo : conteudo.Substring(fim + 2).TrimStart();
    }

    /// <summary>
    /// Escapa um valor para uso como texto de um elemento XML.
    /// </summary>
    /// <param name="valor">Valor a ser escapado.</param>
    /// <returns>Valor escapado (string vazia quando nulo).</returns>
    public static string Escape(string? valor)
    {
        if (string.IsNullOrEmpty(valor)) return string.Empty;

        return valor!
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&apos;");
    }

    /// <summary>
    /// Gera um elemento XML simples quando o valor está preenchido; caso contrário retorna vazio.
    /// </summary>
    /// <param name="nome">Nome do elemento.</param>
    /// <param name="valor">Valor do elemento.</param>
    /// <returns>Elemento serializado ou string vazia.</returns>
    public static string Elemento(string nome, string? valor) =>
        string.IsNullOrEmpty(valor) ? string.Empty : $"<{nome}>{Escape(valor)}</{nome}>";

    #endregion Build

    #region Parse

    /// <summary>
    /// Retorna o elemento invólucro da resposta (primeiro filho do Body do envelope).
    /// </summary>
    /// <param name="xml">Envelope SOAP de resposta.</param>
    /// <returns>Elemento da resposta ou <c>null</c> quando não encontrado.</returns>
    public static XElement? CorpoResposta(string xml)
    {
        if (xml.IsEmpty()) return null;

        var doc = XDocument.Parse(xml);
        var body = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "Body");
        return body?.Elements().FirstOrDefault();
    }

    /// <summary>
    /// Verifica se o envelope contém um <c>soap:Fault</c> e extrai a mensagem correspondente.
    /// </summary>
    /// <param name="xml">Envelope SOAP de resposta.</param>
    /// <param name="mensagem">Mensagem do fault, quando existente.</param>
    /// <returns><c>true</c> se houver fault.</returns>
    public static bool PossuiFault(string xml, out string mensagem)
    {
        mensagem = string.Empty;
        if (xml.IsEmpty()) return false;

        var doc = XDocument.Parse(xml);
        var fault = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "Fault");
        if (fault == null) return false;

        mensagem = fault.Descendants().FirstOrDefault(x => x.Name.LocalName is "faultstring" or "Text")?.Value
                   ?? fault.Value;
        return true;
    }

    /// <summary>
    /// Obtém o valor de um elemento filho direto do invólucro (busca por nome local).
    /// </summary>
    /// <param name="root">Elemento invólucro.</param>
    /// <param name="nomeLocal">Nome local do elemento.</param>
    /// <returns>Valor do elemento ou <c>null</c>.</returns>
    public static string? Valor(XElement? root, string nomeLocal) =>
        root?.Elements().FirstOrDefault(x => x.Name.LocalName == nomeLocal)?.Value;

    /// <summary>
    /// Lê a lista de mensagens (<c>ListaMensagens/mensagem</c>) presente na resposta.
    /// </summary>
    /// <param name="root">Elemento invólucro da resposta.</param>
    /// <returns>Lista de mensagens de processamento.</returns>
    public static List<MensagemProcessamento> Mensagens(XElement? root)
    {
        var resultado = new List<MensagemProcessamento>();
        if (root == null) return resultado;

        foreach (var msg in root.Descendants().Where(x => x.Name.LocalName == "mensagem"))
        {
            resultado.Add(new MensagemProcessamento
            {
                Codigo = msg.Elements().FirstOrDefault(x => x.Name.LocalName == "Codigo")?.Value ?? string.Empty,
                Mensagem = msg.Elements().FirstOrDefault(x => x.Name.LocalName == "Mensagem")?.Value ?? string.Empty,
                Complemento = msg.Elements().FirstOrDefault(x => x.Name.LocalName == "Correcao")?.Value ?? string.Empty,
                Descricao = msg.Elements().FirstOrDefault(x => x.Name.LocalName == "IdDPS")?.Value ?? string.Empty
            });
        }

        return resultado;
    }

    /// <summary>
    /// Extrai os elementos <c>NFSe</c> da resposta e os carrega como <see cref="NotaFiscalServico"/>.
    /// </summary>
    /// <param name="root">Elemento invólucro da resposta.</param>
    /// <returns>Lista de NFS-e.</returns>
    public static List<NotaFiscalServico> Notas(XElement? root)
    {
        var resultado = new List<NotaFiscalServico>();
        if (root == null) return resultado;

        foreach (var nfse in root.Descendants().Where(x => x.Name.LocalName == "NFSe"))
        {
            var xml = nfse.ToString(SaveOptions.DisableFormatting);
            var nota = NotaFiscalServico.Load(xml);
            if (nota != null) resultado.Add(nota);
        }

        return resultado;
    }

    /// <summary>
    /// Retorna o XML (sem formatação) do primeiro elemento <c>NFSe</c> encontrado na resposta.
    /// </summary>
    /// <param name="root">Elemento invólucro da resposta.</param>
    /// <returns>XML da NFS-e ou string vazia.</returns>
    public static string NotaXml(XElement? root)
    {
        var nfse = root?.Descendants().FirstOrDefault(x => x.Name.LocalName == "NFSe");
        return nfse?.ToString(SaveOptions.DisableFormatting) ?? string.Empty;
    }

    /// <summary>
    /// Tenta extrair a chave de acesso (50 dígitos) do atributo <c>Id</c> do elemento <c>infNFSe</c>.
    /// </summary>
    /// <param name="notaXml">XML da NFS-e.</param>
    /// <returns>Chave de acesso ou string vazia.</returns>
    public static string ChaveDaNota(string notaXml)
    {
        if (notaXml.IsEmpty()) return string.Empty;

        var inf = XDocument.Parse(notaXml).Descendants().FirstOrDefault(x => x.Name.LocalName == "infNFSe");
        var id = inf?.Attribute("Id")?.Value ?? string.Empty;

        // O Id costuma ter o formato "NFS" + chave(50).
        if (id.StartsWith("NFS", StringComparison.OrdinalIgnoreCase) && id.Length >= 53)
            return id.Substring(3);

        return id.Length == 50 ? id : string.Empty;
    }

    #endregion Parse
}
