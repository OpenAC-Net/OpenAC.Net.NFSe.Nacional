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
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
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
/// carregam o próprio namespace nacional (<see cref="NsSped"/>). Documentos assinados com canonicalização
/// inclusiva, como a DPS, são protegidos de namespaces ancestrais pelo formato de <see cref="Envelope"/>.
/// O pedido de cancelamento possui ainda um perfil específico da Fiorilli, aplicado em
/// <see cref="ReassinarEvento"/>.
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
    /// <remarks>
    /// O namespace do SOAP é declarado como PADRÃO (sem prefixo) de propósito. Os documentos assinados
    /// como a DPS usam assinatura com canonicalização inclusiva; um prefixo declarado em ancestral
    /// (ex.: <c>soapenv</c>) entraria no escopo do elemento assinado e seria incorporado à sua forma
    /// canônica. Como o namespace padrão é sempre redeclarado/sombreado nos níveis abaixo (Fiorilli,
    /// depois SPED), nada vaza para o conteúdo assinado. Não troque para um prefixo aqui.
    /// </remarks>
    public static string Envelope(string corpo) =>
        $"<Envelope xmlns=\"{NsSoapEnv}\">" +
        "<Header/>" +
        $"<Body>{corpo}</Body>" +
        "</Envelope>";

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
        if (!conteudo.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase)) return conteudo.TrimEnd();

        var fim = conteudo.IndexOf("?>", StringComparison.Ordinal);
        return (fim < 0 ? conteudo : conteudo.Substring(fim + 2).TrimStart()).TrimEnd();
    }

    /// <summary>
    /// Garante que o namespace do padrão nacional esteja declarado diretamente no elemento assinado.
    /// </summary>
    /// <param name="xml">Documento nacional já assinado.</param>
    /// <param name="elementoAssinado">Nome local do elemento referenciado pela assinatura.</param>
    /// <returns>Documento sem prólogo e com o namespace explícito no elemento assinado.</returns>
    /// <remarks>
    /// O namespace já é herdado normalmente do elemento raiz, mas o validador da Fiorilli exige a
    /// declaração também na própria tag assinada. A inserção é textual para não reserializar o XML.
    /// Como a ligação de namespace efetiva não muda, o digest calculado antes da inserção permanece válido.
    /// </remarks>
    public static string GarantirNamespaceExplicito(string xml, string elementoAssinado)
    {
        var conteudo = RemoverProlog(xml);
        if (conteudo.IsEmpty() || elementoAssinado.IsEmpty()) return conteudo;

        var inicioTag = conteudo.IndexOf($"<{elementoAssinado}", StringComparison.Ordinal);
        if (inicioTag < 0) return conteudo;

        var fimNome = inicioTag + elementoAssinado.Length + 1;
        if (fimNome >= conteudo.Length || conteudo[fimNome] is not (' ' or '\t' or '\r' or '\n' or '>'))
            return conteudo;

        var fimTag = conteudo.IndexOf('>', fimNome);
        if (fimTag < 0) return conteudo;

        var tagInicial = conteudo.Substring(inicioTag, fimTag - inicioTag);
        if (tagInicial.IndexOf("xmlns=", StringComparison.Ordinal) >= 0 ||
            tagInicial.IndexOf("xmlns:", StringComparison.Ordinal) >= 0)
            return conteudo;

        return conteudo.Insert(fimNome, $" xmlns=\"{NsSped}\"");
    }

    /// <summary>
    /// Reassina o pedido de evento com o perfil exigido pelo cancelamento da Fiorilli.
    /// </summary>
    /// <param name="xml">Pedido de evento já serializado.</param>
    /// <param name="certificado">Certificado com chave privada.</param>
    /// <returns>Pedido de evento assinado em uma única linha.</returns>
    /// <remarks>
    /// Apesar de a API 1.01 usar C14N inclusiva nos demais documentos, o método
    /// <c>cancelarNFSe</c> da Fiorilli 3.10.2 retornou E172 com C14N inclusiva e aceitou
    /// C14N exclusiva, RSA-SHA1 e SHA1 em uma chamada real ao provedor.
    /// </remarks>
    public static string ReassinarEvento(string xml, X509Certificate2 certificado)
    {
        var conteudo = GarantirNamespaceExplicito(xml, "infPedReg");
        var documento = new XmlDocument { PreserveWhitespace = true };
        documento.LoadXml(conteudo);

        var assinaturaAnterior = documento
            .GetElementsByTagName("Signature", SignedXml.XmlDsigNamespaceUrl)
            .OfType<XmlElement>()
            .SingleOrDefault();
        assinaturaAnterior?.ParentNode?.RemoveChild(assinaturaAnterior);

        var informacoes = documento
            .GetElementsByTagName("infPedReg", NsSped)
            .OfType<XmlElement>()
            .Single();
        var id = informacoes.GetAttribute("Id");
        if (id.IsEmpty()) throw new InvalidOperationException("O elemento infPedReg não possui o atributo Id.");

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
            Uri = $"#{id}",
            DigestMethod = SignedXml.XmlDsigSHA1Url
        };
        referencia.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        referencia.AddTransform(new XmlDsigExcC14NTransform());
        signedXml.AddReference(referencia);
        signedXml.ComputeSignature();

        documento.DocumentElement!.AppendChild(
            documento.ImportNode(signedXml.GetXml(), true));
        return documento.OuterXml.TrimEnd();
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
    /// Indica se a mensagem representa um erro. Segue a convenção da Fiorilli/padrão nacional,
    /// em que códigos iniciados por "E" são erros (ex.: E172) e por "A" são alertas.
    /// </summary>
    /// <param name="mensagem">Mensagem de processamento.</param>
    /// <returns><c>true</c> quando a mensagem é um erro.</returns>
    public static bool EhErro(MensagemProcessamento mensagem) =>
        mensagem.Codigo.StartsWith("E", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Indica se a coleção contém ao menos uma mensagem de erro.
    /// </summary>
    /// <param name="mensagens">Mensagens de processamento.</param>
    /// <returns><c>true</c> quando há pelo menos um erro.</returns>
    public static bool TemErro(IEnumerable<MensagemProcessamento> mensagens) => mensagens.Any(EhErro);

    /// <summary>
    /// Distribui as mensagens entre <see cref="RespostaBase.Erros"/> e <see cref="RespostaBase.Alertas"/>
    /// de acordo com o prefixo do código.
    /// </summary>
    /// <param name="resposta">Resposta a ser preenchida.</param>
    /// <param name="mensagens">Mensagens de processamento.</param>
    public static void DistribuirMensagens(RespostaBase resposta, IEnumerable<MensagemProcessamento> mensagens)
    {
        foreach (var mensagem in mensagens)
        {
            if (EhErro(mensagem)) resposta.Erros.Add(mensagem);
            else resposta.Alertas.Add(mensagem);
        }
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
