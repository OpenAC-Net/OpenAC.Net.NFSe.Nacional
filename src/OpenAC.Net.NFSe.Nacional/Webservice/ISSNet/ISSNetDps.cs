// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : Adriano Trentim
// Created          : 01-10-2026
//
// Last Modified By : Adriano Trentim
// Last Modified On : 01-10-2026
// ***********************************************************************
// <copyright file="ISSNetDps.cs" company="OpenAC .Net">
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
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using OpenAC.Net.DFe.Core;
using OpenAC.Net.NFSe.Nacional.Common.Model;

namespace OpenAC.Net.NFSe.Nacional.Webservice.ISSNet;

/// <summary>
/// Adequa a DPS gerada no layout nacional ao layout da ISSNet (manual de integração v1.01).
/// </summary>
/// <remarks>
/// O schema da ISSNet diverge do nacional nos grupos <c>obra</c>, <c>atvEvento</c> e <c>IBSCBS/imovel</c>:
/// <list type="bullet">
/// <item>o <c>end</c> é do tipo <c>TCEndereco</c> (<c>endNac</c> com cMun + CEP, ou <c>endExt</c> com cPais),
/// e não <c>TCEnderObraEvento</c>/<c>TCEnderecoSimples</c> (CEP direto, <c>endExt</c> sem cPais);</item>
/// <item>os campos <c>cObra</c>, <c>cCIB</c> e <c>idAtvEvt</c> não existem;</item>
/// <item>a obra possui o campo opcional <c>nProcessoObra</c>.</item>
/// </list>
/// Como a transformação altera o conteúdo assinado, a DPS é reassinada quando houver alteração.
/// </remarks>
public static class ISSNetDps
{
    #region Constants

    /// <summary>Namespace do padrão nacional, também usado pela ISSNet.</summary>
    private const string NsSped = "http://www.sped.fazenda.gov.br/nfse";

    /// <summary>Pasta do provedor dentro da pasta de schemas (<c>Schemas/ISSNet</c>).</summary>
    public const string PastaSchema = "ISSNet";

    /// <summary>Versão do schema da ISSNet (subpasta de <see cref="PastaSchema"/>).</summary>
    public const string VersaoSchema = "1.01";

    /// <summary>Nome do arquivo de schema da ISSNet, em <c>Schemas/ISSNet/1.01</c>.</summary>
    public const string ArquivoSchema = "ISSNet_v1.01.xsd";

    #endregion Constants

    #region Methods

    /// <summary>
    /// Adequa o XML da DPS ao layout da ISSNet, reassinando-o quando necessário.
    /// </summary>
    /// <param name="xml">XML da DPS gerado no layout nacional (assinado ou não).</param>
    /// <param name="dps">DPS de origem, usada para obter os dados que não existem no layout nacional.</param>
    /// <param name="certificado">Certificado para reassinar a DPS. Só é obtido se a DPS estava assinada e foi alterada.</param>
    /// <returns>O XML no layout da ISSNet. Se nada precisou ser alterado, retorna o próprio <paramref name="xml"/>.</returns>
    /// <exception cref="InvalidOperationException">Quando faltar um dado obrigatório para o layout da ISSNet.</exception>
    public static string Adequar(string xml, Dps dps, Func<X509Certificate2> certificado)
    {
        var documento = new XmlDocument { PreserveWhitespace = true };
        documento.LoadXml(xml);

        if (!Adequar(documento, dps)) return xml;

        var assinatura = documento.DocumentElement?
            .ChildNodes.OfType<XmlElement>()
            .SingleOrDefault(x => x.LocalName == "Signature" && x.NamespaceURI == SignedXml.XmlDsigNamespaceUrl);
        var assinado = assinatura?.GetElementsByTagName("SignatureValue", SignedXml.XmlDsigNamespaceUrl)
            .OfType<XmlElement>().Any(x => !string.IsNullOrWhiteSpace(x.InnerText)) == true;
        if (!assinado) return documento.OuterXml;

        assinatura!.ParentNode!.RemoveChild(assinatura);
        return XmlSigning.AssinarXml(documento.OuterXml, "DPS", "infDPS", certificado(), false, false, false, SignDigest.SHA1);
    }

    /// <summary>
    /// Monta o XML de <c>GerarNfseEnvio</c> usado para validar a DPS contra o schema da ISSNet,
    /// já que a DPS não é declarada como elemento global nesse schema.
    /// </summary>
    /// <param name="xmlDps">XML da DPS.</param>
    /// <returns>O XML para validação.</returns>
    public static string EnvelopeValidacao(string xmlDps) => $"<GerarNfseEnvio xmlns=\"{NsSped}\">{xmlDps}</GerarNfseEnvio>";

    private static bool Adequar(XmlDocument documento, Dps dps)
    {
        var infDps = Filho(documento.DocumentElement, "infDPS");
        var servico = dps.Informacoes.Servico;
        var local = servico.Localidade;
        var alterado = false;

        var serv = Filho(infDps, "serv");
        var obra = Filho(serv, "obra");
        if (obra != null)
        {
            alterado |= Remover(obra, "cObra") | Remover(obra, "cCIB");
            alterado |= AdequarEndereco(Filho(obra, "end"), servico.Obra?.Endereco, local, "obra");

            var processo = servico.Obra?.NumeroProcesso;
            if (!string.IsNullOrWhiteSpace(processo) && Filho(obra, "nProcessoObra") == null)
            {
                var elemento = Criar(documento, "nProcessoObra", processo!.Trim());
                var inscricao = Filho(obra, "inscImobFisc");
                if (inscricao != null)
                    obra.InsertAfter(elemento, inscricao);
                else
                    obra.PrependChild(elemento);

                alterado = true;
            }
        }

        var evento = Filho(serv, "atvEvento");
        if (evento != null)
        {
            alterado |= Remover(evento, "idAtvEvt");
            alterado |= AdequarEndereco(Filho(evento, "end"), servico.Evento?.Endereco, local, "atvEvento");
        }

        var imovel = Filho(Filho(infDps, "IBSCBS"), "imovel");
        if (imovel != null)
        {
            alterado |= Remover(imovel, "cCIB");
            alterado |= AdequarEndereco(Filho(imovel, "end"), dps.Informacoes.IBSCBS?.Imovel?.Endereco, local, "imovel");
        }

        return alterado;
    }

    /// <summary>
    /// Converte o endereço simples do layout nacional (CEP | endExt) para o <c>TCEndereco</c> da ISSNet
    /// (endNac{cMun, CEP} | endExt{cPais, ...}).
    /// </summary>
    private static bool AdequarEndereco(XmlElement? end, EnderecoSimplesNFSe? endereco, LocalidadeNFSe local, string grupo)
    {
        if (end == null) return false;

        var documento = end.OwnerDocument;
        var cep = Filho(end, "CEP");
        if (cep != null)
        {
            var cMun = Primeiro(endereco?.CodMunicipio, local.CodMunicipioPrestacao) ??
                       throw new InvalidOperationException(
                           $"ISSNet: o endereço do grupo '{grupo}' exige o código do município (cMun). " +
                           $"Informe {nameof(EnderecoSimplesNFSe)}.{nameof(EnderecoSimplesNFSe.CodMunicipio)} " +
                           "ou o município de prestação (cLocPrestacao).");

            var endNac = Criar(documento, "endNac");
            end.ReplaceChild(endNac, cep);
            endNac.AppendChild(Criar(documento, "cMun", cMun));
            endNac.AppendChild(cep);
            return true;
        }

        var endExt = Filho(end, "endExt");
        if (endExt == null || Filho(endExt, "cPais") != null) return false;

        var cPais = Primeiro(endereco?.EnderecoExterior?.CodPais, local.CodPaisPrestacao) ??
                    throw new InvalidOperationException(
                        $"ISSNet: o endereço no exterior do grupo '{grupo}' exige o código do país (cPais). " +
                        $"Informe {nameof(EnderecoExterior)}.{nameof(EnderecoExterior.CodPais)} " +
                        "ou o país de prestação (cPaisPrestacao).");

        endExt.PrependChild(Criar(documento, "cPais", cPais));
        return true;
    }

    private static string? Primeiro(params string?[] valores) =>
        valores.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x))?.Trim();

    private static XmlElement? Filho(XmlNode? pai, string nome) =>
        pai?.ChildNodes.OfType<XmlElement>().FirstOrDefault(x => x.LocalName == nome && x.NamespaceURI == NsSped);

    private static bool Remover(XmlElement pai, string nome)
    {
        var elemento = Filho(pai, nome);
        if (elemento == null) return false;

        pai.RemoveChild(elemento);
        return true;
    }

    private static XmlElement Criar(XmlDocument documento, string nome, string? valor = null)
    {
        var elemento = documento.CreateElement(nome, NsSped);
        if (valor != null) elemento.InnerText = valor;
        return elemento;
    }

    #endregion Methods
}
