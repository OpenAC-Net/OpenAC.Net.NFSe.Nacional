// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : OpenAC .Net
// ***********************************************************************
// <summary>
// Utilitários para montagem e leitura dos envelopes SOAP do provedor SIL Tecnologia.
// </summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.NFSe.Nacional.Common.Model;

namespace OpenAC.Net.NFSe.Nacional.Webservice.SilTecnologia;

/// <summary>
/// Utilitários para montagem e leitura dos envelopes SOAP do serviço <c>NotaFiscalNacional</c>
/// da SIL Tecnologia (namespace <c>http://webservices.sil.com/</c>).
/// </summary>
/// <remarks>
/// Todas as operações recebem um único parâmetro <c>xml</c> do tipo string e retornam uma string
/// (<c>return</c>) com o XML de retorno escapado. Por isso o documento assinado é enviado dentro de
/// uma seção CDATA e não como nós do envelope.
/// </remarks>
internal static class SilTecnologiaSoap
{
    #region Constants

    /// <summary>Namespace das operações do serviço.</summary>
    public const string NsSil = "http://webservices.sil.com/";

    /// <summary>Namespace do padrão nacional (documentos DPS/NFS-e/eventos).</summary>
    public const string NsSped = "http://www.sped.fazenda.gov.br/nfse";

    #endregion Constants

    #region Build

    /// <summary>
    /// Monta o envelope SOAP 1.1 da operação informada com o documento em CDATA.
    /// </summary>
    /// <param name="operacao">Nome da operação (ex.: <c>NotaFiscalNacionalGerar</c>).</param>
    /// <param name="xml">Documento a ser enviado no parâmetro <c>xml</c>.</param>
    /// <returns>Envelope SOAP completo.</returns>
    public static string Envelope(string operacao, string xml) =>
        "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" " +
        $"xmlns:web=\"{NsSil}\">" +
        "<soapenv:Header/>" +
        "<soapenv:Body>" +
        $"<web:{operacao}><xml><![CDATA[{RemoverProlog(xml)}]]></xml></web:{operacao}>" +
        "</soapenv:Body>" +
        "</soapenv:Envelope>";

    /// <summary>
    /// Remove a declaração XML (<c>&lt;?xml ... ?&gt;</c>) e a BOM de um documento.
    /// </summary>
    /// <param name="xml">Documento XML.</param>
    /// <returns>Documento sem o prólogo.</returns>
    public static string RemoverProlog(string xml)
    {
        if (xml.IsEmpty()) return string.Empty;

        var conteudo = xml.TrimStart('﻿', ' ', '\r', '\n', '\t');
        if (!conteudo.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase)) return conteudo.TrimEnd();

        var fim = conteudo.IndexOf("?>", StringComparison.Ordinal);
        return (fim < 0 ? conteudo : conteudo.Substring(fim + 2).TrimStart()).TrimEnd();
    }

    /// <summary>
    /// Assina o elemento informado com C14N exclusiva, RSA-SHA1 e SHA1, anexando a assinatura
    /// ao elemento raiz do documento.
    /// </summary>
    /// <param name="xml">Documento a ser assinado.</param>
    /// <param name="elementoAssinado">Nome local do elemento referenciado pela assinatura (ex.: <c>infNFSe</c>) ou <c>null</c> para o documento inteiro.</param>
    /// <param name="certificado">Certificado com chave privada.</param>
    /// <returns>Documento assinado em uma única linha, sem prólogo.</returns>
    /// <remarks>
    /// O validador da SIL usa canonicalização exclusiva (mesma configuração adotada pela Unimake para
    /// os municípios da SIL). Assinaturas existentes no documento são removidas antes de assinar.
    /// </remarks>
    public static string Assinar(string xml, string? elementoAssinado, X509Certificate2 certificado)
    {
        var documento = new XmlDocument { PreserveWhitespace = true };
        documento.LoadXml(RemoverProlog(xml));

        foreach (var assinatura in documento.GetElementsByTagName("Signature", SignedXml.XmlDsigNamespaceUrl)
                     .OfType<XmlElement>().ToList())
            assinatura.ParentNode?.RemoveChild(assinatura);

        // Sem elemento (ex.: ConsultarNFE, que não tem Id), a assinatura referencia o documento inteiro (URI="").
        var uri = string.Empty;
        if (elementoAssinado != null)
        {
            var elemento = documento
                .GetElementsByTagName(elementoAssinado, NsSped)
                .OfType<XmlElement>()
                .FirstOrDefault() ?? throw new InvalidOperationException($"Elemento {elementoAssinado} não encontrado.");
            var id = elemento.GetAttribute("Id");
            if (id.IsEmpty()) throw new InvalidOperationException($"O elemento {elementoAssinado} não possui o atributo Id.");
            uri = $"#{id}";
        }

        using var chavePrivada = certificado.GetRSAPrivateKey()
            ?? throw new InvalidOperationException("O certificado não possui uma chave privada RSA.");
        var signedXml = new SignedXml(documento)
        {
            SigningKey = chavePrivada,
            KeyInfo = new KeyInfo()
        };
        signedXml.KeyInfo.AddClause(new KeyInfoX509Data(certificado));
        signedXml.SignedInfo!.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;
        signedXml.SignedInfo.SignatureMethod = SignedXml.XmlDsigRSASHA1Url;

        var referencia = new Reference
        {
            Uri = uri,
            DigestMethod = SignedXml.XmlDsigSHA1Url
        };
        referencia.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        referencia.AddTransform(new XmlDsigExcC14NTransform());
        signedXml.AddReference(referencia);
        signedXml.ComputeSignature();

        documento.DocumentElement!.AppendChild(documento.ImportNode(signedXml.GetXml(), true));
        return documento.OuterXml.TrimEnd();
    }

    /// <summary>
    /// Monta o <c>evento</c> (sem assinatura) que a SIL recebe no cancelamento e na substituição:
    /// <c>infEvento</c> com o <c>pedRegEvento</c> dentro, conforme os modelos da SIL.
    /// </summary>
    /// <param name="pedidoXml">Pedido de registro de evento serializado.</param>
    /// <param name="chave">Chave de acesso da NFS-e.</param>
    /// <param name="codigoEvento">Código do evento (ex.: 101101).</param>
    /// <param name="versao">Versão do layout (atributo <c>versao</c>).</param>
    /// <param name="dataProcessamento">Data/hora informada em <c>dhProc</c>.</param>
    /// <returns>XML do evento sem prólogo.</returns>
    public static string Evento(string pedidoXml, string chave, string codigoEvento, string versao, DateTimeOffset dataProcessamento)
    {
        // O pedido vai sem assinatura própria: a SIL assina só infEvento.
        var pedido = XElement.Parse(RemoverProlog(pedidoXml));
        pedido.Descendants().Where(x => x.Name.LocalName == "Signature").ToList().ForEach(x => x.Remove());

        XNamespace ns = NsSped;
        var verAplic = pedido.Descendants(ns + "verAplic").FirstOrDefault();
        if (verAplic != null) verAplic.Value = SilTecnologiaNFSe.VersaoAplicacao;

        var evento = new XElement(ns + "evento",
            new XAttribute("versao", versao),
            new XElement(ns + "infEvento",
                new XAttribute("Id", $"EVT{chave}{codigoEvento}001"),
                new XElement(ns + "verAplic", SilTecnologiaNFSe.VersaoAplicacao),
                new XElement(ns + "ambGer", "1"),
                new XElement(ns + "nSeqEvento", "1"),
                new XElement(ns + "dhProc", dataProcessamento.ToString("yyyy-MM-dd'T'HH:mm:sszzz")),
                new XElement(ns + "nDFSe", "0"),
                pedido));

        return evento.ToString(SaveOptions.DisableFormatting);
    }

    /// <summary>
    /// Monta o <c>ConsultarNFE</c> (sem assinatura) da operação <c>NotaFiscalNacionalConsultar</c>.
    /// </summary>
    /// <param name="cnpj">CNPJ (ou CPF) do prestador, tomador ou intermediário da nota.</param>
    /// <param name="chaveAcesso">Chave de acesso da NFS-e (ou vazio).</param>
    /// <param name="idDps">Id da DPS enviada na emissão (ou vazio).</param>
    /// <returns>XML da consulta.</returns>
    public static string Consulta(string cnpj, string? chaveAcesso, string? idDps) =>
        new XElement("ConsultarNFE",
            new XElement("consul", new XElement("CNPJ", cnpj)),
            new XElement("chaveAcesso", chaveAcesso ?? string.Empty),
            new XElement("DPS", idDps ?? string.Empty)).ToString(SaveOptions.DisableFormatting);

    #endregion Build

    #region Parse

    /// <summary>
    /// Verifica se o envelope contém um <c>soap:Fault</c> e extrai a mensagem correspondente.
    /// </summary>
    /// <param name="xml">Envelope SOAP de resposta.</param>
    /// <param name="mensagem">Mensagem do fault, quando existente.</param>
    /// <returns><c>true</c> se houver fault.</returns>
    public static bool PossuiFault(string xml, out string mensagem)
    {
        mensagem = string.Empty;
        var doc = TentarCarregar(xml);
        var fault = doc?.Descendants().FirstOrDefault(x => x.Name.LocalName == "Fault");
        if (fault == null) return false;

        mensagem = fault.Descendants().FirstOrDefault(x => x.Name.LocalName is "faultstring" or "Text")?.Value
                   ?? fault.Value;
        return true;
    }

    /// <summary>
    /// Extrai o XML de retorno da operação (conteúdo do elemento <c>return</c>).
    /// </summary>
    /// <param name="envelope">Envelope SOAP de resposta.</param>
    /// <returns>Elemento raiz do retorno (normalmente <c>Retorno</c>) ou <c>null</c>.</returns>
    public static XElement? Retorno(string envelope)
    {
        var doc = TentarCarregar(envelope);
        var conteudo = doc?.Descendants().FirstOrDefault(x => x.Name.LocalName == "return")?.Value;
        return TentarCarregar(conteudo ?? string.Empty)?.Root;
    }

    /// <summary>
    /// Obtém o valor do primeiro elemento com o nome local informado (ignorando maiúsculas).
    /// </summary>
    /// <param name="root">Elemento de retorno.</param>
    /// <param name="nomes">Nomes locais aceitos.</param>
    /// <returns>Valor do elemento ou <c>null</c>.</returns>
    public static string? Valor(XElement? root, params string[] nomes) =>
        root?.DescendantsAndSelf()
            .FirstOrDefault(x => nomes.Any(n => string.Equals(x.Name.LocalName, n, StringComparison.OrdinalIgnoreCase)) &&
                                 !x.HasElements)?.Value.Trim();

    /// <summary>
    /// Lê as mensagens de erro do retorno. A SIL informa <c>Status</c> = <c>ERRO</c> e o texto em
    /// <c>MensagemErro</c>; quando o texto começa por "Rejeição do Emissor Nacional", o código da
    /// rejeição (ex.: E1235) é extraído para <see cref="MensagemProcessamento.Codigo"/>.
    /// </summary>
    /// <param name="root">Elemento de retorno.</param>
    /// <returns>Lista de erros (vazia quando não houver).</returns>
    public static List<MensagemProcessamento> Erros(XElement? root)
    {
        var resultado = new List<MensagemProcessamento>();
        if (root == null) return resultado;

        var status = Valor(root, "Status") ?? string.Empty;
        var mensagens = root.DescendantsAndSelf()
            .Where(x => string.Equals(x.Name.LocalName, "MensagemErro", StringComparison.OrdinalIgnoreCase))
            .Select(x => x.Value.Trim())
            .Where(x => !x.IsEmpty())
            .ToList();

        if (mensagens.Count == 0 && string.Equals(status, "ERRO", StringComparison.OrdinalIgnoreCase))
            mensagens.Add("Erro não informado pelo provedor.");

        foreach (var mensagem in mensagens)
        {
            var codigo = Regex.Match(mensagem, @"\b([A-Z]{1,3}\d{2,5})\s*-");
            resultado.Add(new MensagemProcessamento
            {
                Codigo = codigo.Success ? codigo.Groups[1].Value : status,
                Mensagem = mensagem
            });
        }

        return resultado;
    }

    /// <summary>
    /// Procura a chave de acesso (50 dígitos) no retorno: em elementos de chave, no <c>Id</c> do
    /// <c>infNFSe</c> ou em qualquer texto com 50 dígitos.
    /// </summary>
    /// <param name="root">Elemento de retorno.</param>
    /// <returns>Chave de acesso ou string vazia.</returns>
    public static string ChaveAcesso(XElement? root)
    {
        if (root == null) return string.Empty;

        var id = root.Descendants().FirstOrDefault(x => x.Name.LocalName == "infNFSe")?.Attribute("Id")?.Value;
        if (id != null && id.Length == 53 && id.StartsWith("NFS", StringComparison.Ordinal))
            return id.Substring(3);

        var candidato = Valor(root, "chaveAcesso", "chNFSe", "chave", "ChaveNFSe");
        if (candidato != null && Regex.IsMatch(candidato, @"^\d{50}$")) return candidato;

        return root.DescendantsAndSelf()
            .Where(x => !x.HasElements)
            .Select(x => x.Value.Trim())
            .FirstOrDefault(x => Regex.IsMatch(x, @"^\d{50}$")) ?? string.Empty;
    }

    /// <summary>
    /// Retorna o XML (sem formatação) do primeiro elemento com o nome informado no retorno
    /// (ex.: <c>NFSe</c> ou <c>evento</c>), se houver.
    /// </summary>
    /// <param name="root">Elemento de retorno.</param>
    /// <param name="nomeLocal">Nome local do elemento.</param>
    /// <returns>XML do elemento ou string vazia.</returns>
    public static string ElementoXml(XElement? root, string nomeLocal)
    {
        var elemento = root?.DescendantsAndSelf().FirstOrDefault(x => x.Name.LocalName == nomeLocal && x.HasElements);
        return elemento?.ToString(SaveOptions.DisableFormatting) ?? string.Empty;
    }

    private static XDocument? TentarCarregar(string xml)
    {
        if (xml.IsEmpty()) return null;

        try
        {
            return XDocument.Parse(xml);
        }
        catch (XmlException)
        {
            return null;
        }
    }

    #endregion Parse
}
