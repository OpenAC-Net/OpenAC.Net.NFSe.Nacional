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

namespace OpenAC.Net.NFSe.Nacional.Webservice.SilTecnologia;

/// <summary>
/// Comunicação com o webservice SOAP <c>NotaFiscalNacional</c> da SIL Tecnologia (ex.: Bauru/SP),
/// que recebe a NFS-e no layout do padrão nacional e a compartilha com o ADN.
/// </summary>
/// <remarks>
/// Diferente do provedor Nacional, a SIL recebe a NFS-e completa (<c>infNFSe</c> com a DPS dentro),
/// assinada em <c>infNFSe</c>, e não a DPS. A montagem da NFS-e a partir da DPS fica em
/// <see cref="SilTecnologiaNFSe"/>. Implementa apenas a emissão (<c>NotaFiscalNacionalGerar</c>);
/// as demais operações lançam <see cref="NotSupportedException"/>.
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
    /// <param name="dps">DPS com o nome e o endereço do prestador preenchidos (usados no grupo <c>emit</c>).</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Resposta do envio.</returns>
    /// <remarks>
    /// Os nomes dos municípios (<c>xLocEmi</c>, <c>xLocPrestacao</c>, <c>xLocIncid</c>) vêm do cadastro do
    /// município do provedor ou, para outros códigos, da API de localidades do IBGE.
    /// </remarks>
    public override async Task<NFSeResponse<RespostaEnvioDps>> EnviarAsync(Dps dps, CancellationToken cancellationToken = default)
    {
        var inf = dps.Informacoes;
        var nomes = new Dictionary<string, string>();
        foreach (var codigo in new[] { inf.LocalidadeEmitente, inf.Servico.Localidade.CodMunicipioPrestacao })
        {
            if (string.IsNullOrEmpty(codigo) || nomes.ContainsKey(codigo!)) continue;
            nomes[codigo!] = await ObterNomeMunicipioAsync(codigo!, cancellationToken);
        }

        var dataProcessamento = DateTimeOffset.Now.ToOffset(inf.DhEmissao.Offset);
        var nfse = SilTecnologiaNFSe.Montar(dps, codigo => nomes[codigo], dataProcessamento);
        return await EnviarAsync(nfse, cancellationToken);
    }

    /// <summary>
    /// Assina a NFS-e em <c>infNFSe</c> e envia pela operação <c>NotaFiscalNacionalGerar</c>.
    /// </summary>
    /// <param name="nfse">NFS-e montada (ex.: por <see cref="SilTecnologiaNFSe.Montar"/>).</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Resposta do envio.</returns>
    public async Task<NFSeResponse<RespostaEnvioDps>> EnviarAsync(NotaFiscalServico nfse, CancellationToken cancellationToken = default)
    {
        var dps = nfse.Informacoes.Dps;
        var options = DFeSaveOptions.DisableFormatting;
        if (Configuracao.Geral.RetirarAcentos) options |= DFeSaveOptions.RemoveAccents;

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
            XmlNFSe = SilTecnologiaSoap.NotaXml(retorno),
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

    #region Não suportadas

    /// <summary>Não implementado para a SIL Tecnologia.</summary>
    public override Task<NFSeResponse<RespostaEnvioEvento>> EnviarEventoAsync(PedidoRegistroEvento evento,
        CancellationToken cancellationToken = default) =>
        throw OperacaoNaoSuportada(nameof(EnviarEventoAsync));

    /// <summary>Não implementado para a SIL Tecnologia.</summary>
    public override Task<NFSeResponse<RespostaConsultaChaveDps>> ConsultaChaveDpsAsync(string id,
        CancellationToken cancellationToken = default) =>
        throw OperacaoNaoSuportada(nameof(ConsultaChaveDpsAsync));

    /// <summary>Não implementado para a SIL Tecnologia.</summary>
    public override Task<bool> ConsultaExisteDpsAsync(string id, CancellationToken cancellationToken = default) =>
        throw OperacaoNaoSuportada(nameof(ConsultaExisteDpsAsync));

    /// <summary>Não suportado pela SIL Tecnologia.</summary>
    public override Task<byte[]> DownloadDANFSeAsync(string chave, CancellationToken cancellationToken = default) =>
        throw OperacaoNaoSuportada(nameof(DownloadDANFSeAsync));

    /// <summary>Não suportado pela SIL Tecnologia (não há distribuição por NSU no WSDL).</summary>
    public override Task<NFSeResponse<RespostaConsultaDFe>> ConsultaNsuAsync(int nsu, CancellationToken cancellationToken = default) =>
        throw OperacaoNaoSuportada(nameof(ConsultaNsuAsync));

    /// <summary>Não suportado pela SIL Tecnologia (não há distribuição por chave no WSDL).</summary>
    public override Task<NFSeResponse<RespostaConsultaDFe>> ConsultaChaveAsync(string chave, CancellationToken cancellationToken = default) =>
        throw OperacaoNaoSuportada(nameof(ConsultaChaveAsync));

    #endregion Não suportadas

    #region Helpers

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
