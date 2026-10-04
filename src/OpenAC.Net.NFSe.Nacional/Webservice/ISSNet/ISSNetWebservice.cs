// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe.Nacional
// Author           : Adriano Trentim
// Created          : 03-01-2025
//
// Last Modified By : Adriano Trentim
// Last Modified On : 02-10-2026
// ***********************************************************************
// <copyright file="ISSNetWebService.cs" company="OpenAC .Net">
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
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using OpenAC.Net.Core.Logging;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.Common.Types;
using OpenAC.Net.NFSe.Nacional.Webservice.Nacional;
using DFeDistribuicao = OpenAC.Net.NFSe.Nacional.Common.Model.DFe;

namespace OpenAC.Net.NFSe.Nacional.Webservice.ISSNet;

/// <inheritdoc />
public class ISSNetWebService : NacionalWebservice
{
    #region Fields

    /// <summary>Namespace do padrão nacional, também usado pela ISSNet.</summary>
    private static readonly XNamespace NsSped = "http://www.sped.fazenda.gov.br/nfse";

    #endregion Fields

    #region Constructors

    /// <inheritdoc />
    public ISSNetWebService(ConfiguracaoNFSe configuracaoNFSe, NFSeServiceInfo serviceInfo) :
        base(configuracaoNFSe, serviceInfo)
    {
    }

    /// <summary>Inicializa o provedor com um transporte HTTP fornecido pelo host.</summary>
    public ISSNetWebService(ConfiguracaoNFSe configuracaoNFSe, NFSeServiceInfo serviceInfo,
        INFSeHttpTransport httpTransport) : base(configuracaoNFSe, serviceInfo, httpTransport)
    {
    }

    #endregion Constructors

    #region DANFSe

    /// <summary>
    /// A ISSNet não disponibiliza o download do DANFSe pelo webservice.
    /// </summary>
    /// <exception cref="NotSupportedException">Sempre.</exception>
    public override Task<byte[]> DownloadDANFSeAsync(string chave, CancellationToken cancellationToken = default) =>
        throw OperacaoNaoSuportada(nameof(DownloadDANFSeAsync));

    #endregion DANFSe

    #region DFe

    /// <summary>
    /// A ISSNet não possui distribuição de DF-e por NSU.
    /// </summary>
    /// <exception cref="NotSupportedException">Sempre.</exception>
    public override Task<NFSeResponse<RespostaConsultaDFe>> ConsultaNsuAsync(int nsu, CancellationToken cancellationToken = default) =>
        throw OperacaoNaoSuportada(nameof(ConsultaNsuAsync));

    /// <summary>
    /// Retorna a NFS-e e seus eventos a partir da chave de acesso. Como a ISSNet não consulta por chave,
    /// o CNPJ/CPF e o número da NFS-e são extraídos da chave e consultados pelo método <c>ConsultarNfseServicoPrestado</c>.
    /// </summary>
    /// <param name="chave">Chave de acesso da NFS-e.</param>
    /// <returns>Resposta da consulta contendo a NFS-e e seus eventos.</returns>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <remarks>Exige a inscrição municipal em <see cref="NFSeWebserviceConfig.InscricaoMunicipal"/>.</remarks>
    public override async Task<NFSeResponse<RespostaConsultaDFe>> ConsultaChaveAsync(string chave, CancellationToken cancellationToken = default)
    {
        chave = chave.Trim();
        var (xmlEnvio, strResponse, sucessoHttp, raiz) = await ConsultarPorChaveAsync(chave, "ConsultaChave", null, cancellationToken);

        var ret = new RespostaConsultaDFe
        {
            Ambiente = Configuracao.WebServices.Ambiente,
            Erros = LerMensagens(raiz, false),
            Alertas = LerMensagens(raiz, true)
        };

        foreach (var compNfse in LerCompNfse(raiz))
        {
            var nfse = compNfse.Element(NsSped + "NFSe");
            if (nfse == null || !LerChaveNFSe(nfse).Equals(chave, StringComparison.OrdinalIgnoreCase)) continue;

            ret.Lote.Add(new DFeDistribuicao
            {
                ChaveAcesso = chave,
                TipoDocumento = TipoDocumento.NFSE,
                ArquivoXml = nfse.ToString(SaveOptions.DisableFormatting),
                DataHoraGeracao = LerDataProcessamento(nfse),
                DataHoraRecebimento = LerDataProcessamento(nfse)
            });

            foreach (var evento in LerEventos(compNfse))
            {
                var (_, tipoEvento, _) = LerIdentificacaoEvento(evento);
                ret.Lote.Add(new DFeDistribuicao
                {
                    ChaveAcesso = chave,
                    TipoDocumento = TipoDocumento.EVENTO,
                    TipoEvento = TipoEventoPorCodigo(tipoEvento),
                    ArquivoXml = evento.ToString(SaveOptions.DisableFormatting),
                    DataHoraGeracao = LerDataProcessamento(evento),
                    DataHoraRecebimento = LerDataProcessamento(evento)
                });
            }
        }

        ret.StatusProcessamento = ret.Lote.Any() ? StatusProcessamentoDistribuicao.DOCUMENTOS_LOCALIZADOS
            : ret.Erros.Any() ? StatusProcessamentoDistribuicao.REJEICAO
            : StatusProcessamentoDistribuicao.NENHUM_DOCUMENTO_LOCALIZADO;
        ret.DataHoraProcessamento = ret.Lote.FirstOrDefault()?.DataHoraGeracao ?? default;

        var sucesso = sucessoHttp && !ret.Erros.Any() && ret.Lote.Any();
        return NFSeResponse<RespostaConsultaDFe>.Create(xmlEnvio, strResponse, sucesso, ret);
    }

    #endregion DFe

    #region DPS

    /// <summary>
    /// Retorna a chave de acesso da NFS-e a partir do identificador do DPS, pelo método <c>ConsultarNfseDps</c>.
    /// </summary>
    /// <param name="id">Identificação do DPS.</param>
    /// <returns>Resposta da consulta contendo a chave de acesso.</returns>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <remarks>Exige a inscrição municipal em <see cref="NFSeWebserviceConfig.InscricaoMunicipal"/>.</remarks>
    public override async Task<NFSeResponse<RespostaConsultaChaveDps>> ConsultaChaveDpsAsync(string id, CancellationToken cancellationToken = default)
    {
        id = id.Trim();
        var (cnpj, cpf, serie, numero) = DecomporIdDps(id);
        var xmlEnvio = MontarConsultarNfseDpsEnvio(numero, serie, cnpj, cpf, ObterInscricaoMunicipal(null));

        var (sucessoHttp, strResponse) = await EnviarSoapAsync("ConsultarNfseDps", "ConsultaChaveDps", xmlEnvio,
            $"ConsultaChaveDps-{id}", cnpj ?? cpf, true, cancellationToken);

        var raiz = LerCorpoResposta(strResponse, "ConsultarNfseDpsResposta");
        var nfse = LerCompNfse(raiz).Select(x => x.Element(NsSped + "NFSe")).FirstOrDefault(x => x != null);

        var ret = new RespostaConsultaChaveDps
        {
            Ambiente = Configuracao.WebServices.Ambiente,
            DataHoraProcessamento = LerDataProcessamento(nfse),
            IdDps = id,
            Chave = LerChaveNFSe(nfse),
            Erros = LerMensagens(raiz, false),
            Alertas = LerMensagens(raiz, true)
        };

        var sucesso = sucessoHttp && !ret.Erros.Any() && !string.IsNullOrEmpty(ret.Chave);
        return NFSeResponse<RespostaConsultaChaveDps>.Create(xmlEnvio, strResponse, sucesso, ret);
    }

    /// <summary>
    /// Verifica se uma NFS-e foi emitida a partir do Id do DPS (via <see cref="ConsultaChaveDpsAsync"/>).
    /// </summary>
    /// <param name="id">Identificação do DPS.</param>
    /// <returns>True se existir, caso contrário false.</returns>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    public override async Task<bool> ConsultaExisteDpsAsync(string id, CancellationToken cancellationToken = default)
    {
        var consulta = await ConsultaChaveDpsAsync(id, cancellationToken);
        return consulta.Sucesso;
    }

    #endregion DPS

    #region Eventos

    /// <inheritdoc />
    public override async Task<NFSeResponse<RespostaEnvioEvento>> EnviarEventoAsync(PedidoRegistroEvento evento, CancellationToken cancellationToken = default)
    {
        var options = DFeSaveOptions.DisableFormatting;
        if (Configuracao.Geral.RetirarAcentos)
            options |= DFeSaveOptions.RemoveAccents;

        options |= DFeSaveOptions.OmitDeclaration;
        evento.Assinar(Configuracao, options);

        ValidarSchema(SchemaNFSe.Evento, evento.Xml, evento.Versao);

        var documento = evento.Informacoes.CPFAutor ?? evento.Informacoes.CNPJAutor ?? throw new InvalidOperationException("CPF ou CNPJ do autor do evento deve ser informado.");

        await GravarDpsEmDiscoAsync(evento.Xml, $"{evento.Informacoes.ChNFSe}{evento.Informacoes.Evento}_evento.xml",
            documento, evento.Informacoes.DhEvento.DateTime);

        var xmlEnvio = $@"<nfse:CancelarNfseEnvio >{evento.Xml}</nfse:CancelarNfseEnvio >";

        var (success, strResponse) = await EnviarSoapAsync("CancelarNfse", "Evento", xmlEnvio,
            $"Evento-{evento.Informacoes.ChNFSe}{evento.Informacoes.Evento}", documento, false, cancellationToken);

        var ret = await TrataRetorno<RespostaEnvioEvento>(strResponse, "evento");

        if (ret.Erros.Any())
            success = false;

        return NFSeResponse<RespostaEnvioEvento>.Create(evento.Xml, strResponse, success, ret);
    }

    /// <summary>
    /// Consulta um evento da NFS-e. A ISSNet não possui consulta de evento, então a NFS-e é consultada
    /// (como em <see cref="ConsultaChaveAsync"/>) e os eventos retornados em <c>ListaEvento</c> são filtrados
    /// pelo tipo e pelo número do pedido de registro.
    /// </summary>
    /// <param name="chaveAcesso">Chave de acesso da NFS-e</param>
    /// <param name="tipoEvento">Tipo de evento (ex.: <see cref="TipoEventoCod.Cancelamento"/>)</param>
    /// <param name="numSeqEvento">Número do pedido de registro do evento</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Resposta da consulta contendo os eventos encontrados.</returns>
    /// <remarks>Exige a inscrição municipal em <see cref="NFSeWebserviceConfig.InscricaoMunicipal"/>.</remarks>
    public override async Task<NFSeResponse<RespostaConsultaEvento>> ConsultaEventoAsync(string chaveAcesso, string tipoEvento, int numSeqEvento, CancellationToken cancellationToken = default)
    {
        chaveAcesso = chaveAcesso.Trim();
        tipoEvento = tipoEvento.Trim().TrimStart('e', 'E');

        var (xmlEnvio, strResponse, sucessoHttp, raiz) = await ConsultarPorChaveAsync(chaveAcesso, "ConsultaEvento", null, cancellationToken);

        var ret = new RespostaConsultaEvento
        {
            Ambiente = Configuracao.WebServices.Ambiente,
            Erros = LerMensagens(raiz, false),
            Alertas = LerMensagens(raiz, true)
        };

        foreach (var evento in LerCompNfse(raiz).SelectMany(LerEventos))
        {
            var (chave, tipo, numeroPedido) = LerIdentificacaoEvento(evento);
            if (!chave.Equals(chaveAcesso, StringComparison.OrdinalIgnoreCase) || tipo != tipoEvento || numeroPedido != numSeqEvento)
                continue;

            ret.Eventos.Add(new RespostaConsultaEvento.Evento
            {
                ChaveAcesso = chave,
                TipoEvento = int.Parse(tipo),
                NumeroPedidoRegistroEvento = numeroPedido,
                DataHoraRecebimento = LerDataProcessamento(evento).DateTime,
                XmlEvento = evento.ToString(SaveOptions.DisableFormatting)
            });
        }

        var sucesso = sucessoHttp && !ret.Erros.Any() && ret.Eventos.Any();
        return NFSeResponse<RespostaConsultaEvento>.Create(xmlEnvio, strResponse, sucesso, ret);
    }

    #endregion Eventos

    #region Lote

    /// <summary>
    /// Consulta o processamento de um lote de DPS pelo protocolo (método <c>ConsultarLoteDps</c>).
    /// </summary>
    /// <param name="filtro">Filtro com o protocolo e a identificação do prestador.</param>
    /// <returns>Resposta contendo a situação e as NFS-e do lote.</returns>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <remarks>Quando <see cref="ConsultaLoteFiltro.IM"/> não for informada, usa <see cref="NFSeWebserviceConfig.InscricaoMunicipal"/>.</remarks>
    public override async Task<NFSeResponse<RespostaConsultaLote>> ConsultarLoteAsync(ConsultaLoteFiltro filtro, CancellationToken cancellationToken = default)
    {
        var xmlEnvio = $"<ConsultarLoteDpsEnvio xmlns=\"{NsSped.NamespaceName}\">" +
                       MontarPrestador(filtro.CNPJ, filtro.CPF, ObterInscricaoMunicipal(filtro.IM)) +
                       MontarElemento("Protocolo", filtro.Protocolo) +
                       "</ConsultarLoteDpsEnvio>";

        var (sucessoHttp, strResponse) = await EnviarSoapAsync("ConsultarLoteDps", "ConsultarLote", xmlEnvio,
            $"ConsultarLote-{filtro.Protocolo}", filtro.CNPJ ?? filtro.CPF, true, cancellationToken);

        var raiz = LerCorpoResposta(strResponse, "ConsultarLoteDpsResposta");
        var erros = LerMensagens(raiz, false);

        var ret = new RespostaConsultaLote
        {
            Situacao = int.TryParse(raiz?.Element(NsSped + "Situacao")?.Value, out var situacao) ? situacao : 0,
            NotasFiscais = CarregarNotas(raiz),
            Mensagens = erros.Concat(LerMensagens(raiz, true)).ToList()
        };

        var sucesso = sucessoHttp && !erros.Any();
        return NFSeResponse<RespostaConsultaLote>.Create(xmlEnvio, strResponse, sucesso, ret);
    }

    #endregion Lote

    #region NFS-e

    /// <inheritdoc />
    public override async Task<NFSeResponse<RespostaEnvioDps>> EnviarAsync(Dps dps, CancellationToken cancellationToken = default)
    {
        var options = DFeSaveOptions.DisableFormatting;
        if (Configuracao.Geral.RetirarAcentos)
            options |= DFeSaveOptions.RemoveAccents;

        options |= DFeSaveOptions.OmitDeclaration;
        dps.Assinar(Configuracao, options);

        // O layout da ISSNet diverge do nacional (ex.: endereço da obra), por isso a DPS é adequada
        // e validada contra o schema da própria ISSNet.
        var xmlDps = ISSNetDps.Adequar(dps.Xml, dps, () => Configuracao.Certificados.ObterCertificado());
        ValidarSchemaISSNet(ISSNetDps.EnvelopeValidacao(xmlDps), dps.Versao);

        var documento = dps.Informacoes.Prestador.CPF ?? dps.Informacoes.Prestador.CNPJ ?? throw new InvalidOperationException("CPF ou CNPJ do prestador deve ser informado.");

        await GravarDpsEmDiscoAsync(xmlDps, $"{dps.Informacoes.NumeroDps:000000}_dps.xml",
            documento, dps.Informacoes.DhEmissao.DateTime, cancellationToken: cancellationToken);

        var xmlEnvio = $@"<nfse:GerarNfseEnvio>{xmlDps}</nfse:GerarNfseEnvio>";

        var (success, strResponse) = await EnviarSoapAsync("GerarNfse", "Enviar", xmlEnvio,
            $"Enviar-{dps.Informacoes.NumeroDps:000000}", documento, false, cancellationToken);

        var ret = await TrataRetorno<RespostaEnvioDps>(strResponse, "NFSe");

        if (ret.Erros.Any())
            success = false;

        return NFSeResponse<RespostaEnvioDps>.Create(xmlDps, strResponse, success, ret);
    }

    /// <summary>
    /// Consulta NFS-e na ISSNet. A consulta é feita conforme o filtro informado:
    /// <list type="bullet">
    /// <item><see cref="ConsultaNFSeFiltro.ChaveNFSe"/>: pelo número da NFS-e contido na chave (método <c>ConsultarNfseServicoPrestado</c>);</item>
    /// <item><see cref="ConsultaNFSeFiltro.IdDPS"/>: pela série/número contidos no Id (método <c>ConsultarNfseDps</c>);</item>
    /// <item><see cref="ConsultaNFSeFiltro.NumeroDPS"/> e <see cref="ConsultaNFSeFiltro.SerieDPS"/>: pela DPS informada (método <c>ConsultarNfseDps</c>).</item>
    /// </list>
    /// </summary>
    /// <param name="filtro">Filtro da consulta.</param>
    /// <returns>Resposta contendo as NFS-e encontradas.</returns>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <remarks>Quando <see cref="ConsultaNFSeFiltro.IM"/> não for informada, usa <see cref="NFSeWebserviceConfig.InscricaoMunicipal"/>.</remarks>
    /// <exception cref="ArgumentException">Quando nenhum critério de consulta for informado.</exception>
    public override async Task<NFSeResponse<RespostaConsultaNFSe>> ConsultarNFSeAsync(ConsultaNFSeFiltro filtro, CancellationToken cancellationToken = default)
    {
        string xmlEnvio, strResponse;
        bool sucessoHttp;
        XElement? raiz;

        if (!string.IsNullOrWhiteSpace(filtro.ChaveNFSe))
        {
            (xmlEnvio, strResponse, sucessoHttp, raiz) = await ConsultarPorChaveAsync(filtro.ChaveNFSe!.Trim(), "ConsultarNfse", filtro.IM, cancellationToken);
        }
        else
        {
            string? cnpj, cpf;
            string serie, numero;
            if (!string.IsNullOrWhiteSpace(filtro.IdDPS))
                (cnpj, cpf, serie, numero) = DecomporIdDps(filtro.IdDPS!.Trim());
            else if (!string.IsNullOrWhiteSpace(filtro.NumeroDPS) && !string.IsNullOrWhiteSpace(filtro.SerieDPS))
                (cnpj, cpf, serie, numero) = (filtro.CNPJ, filtro.CPF, filtro.SerieDPS!, filtro.NumeroDPS!);
            else
                throw new ArgumentException("ISSNet: informe a chave da NFS-e, o Id da DPS ou o número e a série da DPS.", nameof(filtro));

            xmlEnvio = MontarConsultarNfseDpsEnvio(numero, serie, cnpj, cpf, ObterInscricaoMunicipal(filtro.IM));
            (sucessoHttp, strResponse) = await EnviarSoapAsync("ConsultarNfseDps", "ConsultarNfse", xmlEnvio,
                $"ConsultarNfse-{filtro.IdDPS?.Trim() ?? $"{serie}-{numero}"}", cnpj ?? cpf, true, cancellationToken);
            raiz = LerCorpoResposta(strResponse, "ConsultarNfseDpsResposta");
        }

        var erros = LerMensagens(raiz, false);
        var ret = new RespostaConsultaNFSe
        {
            NotasFiscais = CarregarNotas(raiz),
            Mensagens = erros.Concat(LerMensagens(raiz, true)).ToList()
        };

        var sucesso = sucessoHttp && !erros.Any();
        return NFSeResponse<RespostaConsultaNFSe>.Create(xmlEnvio, strResponse, sucesso, ret);
    }

    #endregion NFS-e

    #region Comunicação

    /// <summary>
    /// Consulta a NFS-e pelo número contido na chave de acesso (método <c>ConsultarNfseServicoPrestado</c>).
    /// </summary>
    private async Task<(string XmlEnvio, string Resposta, bool SucessoHttp, XElement? Raiz)> ConsultarPorChaveAsync(
        string chave, string operacao, string? inscricaoMunicipal, CancellationToken cancellationToken)
    {
        var (cnpj, cpf, numeroNfse) = DecomporChave(chave);

        var xmlEnvio = $"<ConsultarNfseServicoPrestadoEnvio xmlns=\"{NsSped.NamespaceName}\">" +
                       MontarPrestador(cnpj, cpf, ObterInscricaoMunicipal(inscricaoMunicipal)) +
                       MontarElemento("NumeroNfse", numeroNfse) +
                       MontarElemento("Pagina", "1") +
                       "</ConsultarNfseServicoPrestadoEnvio>";

        var (sucessoHttp, strResponse) = await EnviarSoapAsync("ConsultarNfseServicoPrestado", operacao, xmlEnvio,
            $"{operacao}-{chave}", cnpj ?? cpf, true, cancellationToken);

        return (xmlEnvio, strResponse, sucessoHttp, LerCorpoResposta(strResponse, "ConsultarNfseServicoPrestadoResposta"));
    }

    /// <summary>
    /// Envia a mensagem ao webservice SOAP da ISSNet (<c>nfse.asmx</c>), gravando o envio e a resposta.
    /// </summary>
    /// <param name="metodo">Método do webservice (ex.: <c>GerarNfse</c>).</param>
    /// <param name="operacao">Nome da operação usado no log.</param>
    /// <param name="xmlEnvio">XML enviado em <c>nfseDadosMsg</c>.</param>
    /// <param name="nomeArquivo">Prefixo dos arquivos de envio (<c>-env.xml</c>) e resposta (<c>-resp.xml</c>).</param>
    /// <param name="documento">Documento do prestador.</param>
    /// <param name="validarSchema">Indica se o <paramref name="xmlEnvio"/> deve ser validado contra o schema da ISSNet.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Se o status HTTP foi de sucesso e o conteúdo da resposta.</returns>
    private async Task<(bool SucessoHttp, string Resposta)> EnviarSoapAsync(string metodo, string operacao, string xmlEnvio,
        string nomeArquivo, string? documento, bool validarSchema, CancellationToken cancellationToken)
    {
        if (validarSchema)
            ValidarSchemaISSNet(xmlEnvio, Configuracao.Arquivos.VersaoSchema);

        this.Log().Debug($"ISSNet: [{operacao}][Envio] - {xmlEnvio}");

        var xmlCabecalho = "<cabecalho versao=\"1.01\"><versaoDados>1.01</versaoDados></cabecalho>";

        var soapEnvelope = $@"
            <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:nfse=""{NsSped.NamespaceName}"">
                <soapenv:Header/>
                <soapenv:Body>
                    <nfse:{metodo}>
                        <nfseCabecMsg>{xmlCabecalho}</nfseCabecMsg>
                        <nfseDadosMsg>{xmlEnvio}</nfseDadosMsg>
                    </nfse:{metodo}>
                </soapenv:Body>
            </soapenv:Envelope>";

        var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");
        content.Headers.Add("SOAPAction", $"\"{NsSped.NamespaceName}/{metodo}\"");

        await GravarArquivoEmDiscoAsync(xmlEnvio, $"{nomeArquivo}-env.xml", documento, cancellationToken);

        var url = ServiceInfo[Configuracao.WebServices.Ambiente][TipoUrl.Enviar] ?? throw new InvalidOperationException("URL de envio não encontrada na configuração do serviço.");
        using var httpResponse = await SendAsync(content, HttpMethod.Post, $"{url}/nfse.asmx", cancellationToken: cancellationToken);

        var strResponse = await httpResponse.Content.ReadAsStringAsync();

        this.Log().Debug($"ISSNet: [{operacao}][Resposta] - {strResponse}");

        await GravarArquivoEmDiscoAsync(strResponse, $"{nomeArquivo}-resp.xml", documento, cancellationToken);

        return (httpResponse.IsSuccessStatusCode, strResponse);
    }

    /// <summary>
    /// Valida o XML de envio contra o schema da ISSNet (<c>Schemas/ISSNet/1.01/ISSNet_v1.01.xsd</c>).
    /// </summary>
    /// <param name="xmlEnvio">XML de envio (elemento global do schema, ex.: <c>GerarNfseEnvio</c>).</param>
    /// <param name="versao">Versão do schema.</param>
    private void ValidarSchemaISSNet(string xmlEnvio, VersaoNFSe versao)
    {
        if (!Configuracao.WebServices.ValidarSchemas) return;

        ValidarSchema(ObterSchemaISSNet(), xmlEnvio, versao);
    }

    /// <summary>
    /// Localiza o schema da ISSNet. O <c>PathSchemas</c> aponta para a pasta da versão nacional
    /// (<c>Schemas/1.0x</c>), então a pasta do provedor é procurada ao lado dela
    /// (<c>Schemas/ISSNet/1.01</c>) e, em seguida, na pasta da aplicação.
    /// </summary>
    /// <returns>O caminho do schema (o primeiro existente, ou o primeiro candidato se nenhum existir).</returns>
    private string ObterSchemaISSNet()
    {
        var relativo = Path.Combine(ISSNetDps.PastaSchema, ISSNetDps.VersaoSchema, ISSNetDps.ArquivoSchema);
        var pathSchemas = Configuracao.Arquivos.PathSchemas?.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        var candidatos = new List<string>();
        if (!string.IsNullOrEmpty(pathSchemas))
        {
            candidatos.Add(Path.Combine(pathSchemas, relativo));

            var pastaSchemas = Path.GetDirectoryName(pathSchemas);
            if (!string.IsNullOrEmpty(pastaSchemas))
                candidatos.Add(Path.Combine(pastaSchemas, relativo));
        }

        candidatos.Add(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Schemas", relativo));

        return candidatos.FirstOrDefault(File.Exists) ?? candidatos[0];
    }

    /// <summary>
    /// Retorna a inscrição municipal informada ou, na ausência dela, a configurada em <see cref="NFSeWebserviceConfig.InscricaoMunicipal"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Quando nenhuma inscrição municipal estiver disponível.</exception>
    private string ObterInscricaoMunicipal(string? informada)
    {
        var inscricao = string.IsNullOrWhiteSpace(informada) ? Configuracao.WebServices.InscricaoMunicipal : informada;
        if (string.IsNullOrWhiteSpace(inscricao))
            throw new InvalidOperationException(
                "Informe a inscrição municipal em Configuracoes.WebServices.InscricaoMunicipal para realizar consultas no provedor ISSNet.");

        return inscricao!.Trim();
    }

    #endregion Comunicação

    #region Montagem

    /// <summary>
    /// Decompõe o identificador da DPS (<c>TSIdDPS</c>, 45 posições): "DPS" + Cód.Mun.(7) + Tipo Insc.(1) +
    /// Insc.Federal(14) + Série(5) + Núm.DPS(15).
    /// </summary>
    /// <exception cref="ArgumentException">Quando o identificador não estiver no formato esperado.</exception>
    private static (string? CNPJ, string? CPF, string Serie, string Numero) DecomporIdDps(string id)
    {
        if (id.Length != 45 || !id.StartsWith("DPS", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException($"Identificador da DPS inválido: '{id}'. Esperado \"DPS\" + 42 posições.", nameof(id));

        var (cnpj, cpf) = DecomporInscricaoFederal(id.Substring(10, 1), id.Substring(11, 14));
        return (cnpj, cpf, RemoverZerosEsquerda(id.Substring(25, 5)), RemoverZerosEsquerda(id.Substring(30, 15)));
    }

    /// <summary>
    /// Decompõe a chave de acesso da NFS-e (50 posições): Cód.Mun.(7) + Amb.Ger.(1) + Tipo Insc.(1) +
    /// Insc.Federal(14) + nNFSe(13) + AnoMes(4) + Cód.Num.(9) + DV(1).
    /// </summary>
    /// <exception cref="ArgumentException">Quando a chave não estiver no formato esperado.</exception>
    private static (string? CNPJ, string? CPF, string NumeroNfse) DecomporChave(string chave)
    {
        if (chave.Length != 50)
            throw new ArgumentException($"Chave de acesso da NFS-e inválida: '{chave}'. Esperado 50 posições.", nameof(chave));

        var (cnpj, cpf) = DecomporInscricaoFederal(chave.Substring(8, 1), chave.Substring(9, 14));
        return (cnpj, cpf, RemoverZerosEsquerda(chave.Substring(23, 13)));
    }

    /// <summary>Tipo de inscrição 1 = CPF (completado com 000 à esquerda), 2 = CNPJ.</summary>
    private static (string? CNPJ, string? CPF) DecomporInscricaoFederal(string tipo, string inscricao) =>
        tipo == "1" ? (null, inscricao.Substring(3)) : (inscricao, null);

    private static string RemoverZerosEsquerda(string valor)
    {
        var resultado = valor.TrimStart('0');
        return resultado.Length == 0 ? "0" : resultado;
    }

    /// <summary>Monta o <c>ConsultarNfseDpsEnvio</c> (método <c>ConsultarNfseDps</c>).</summary>
    private static string MontarConsultarNfseDpsEnvio(string numeroDps, string serieDps, string? cnpj, string? cpf, string inscricaoMunicipal) =>
        $"<ConsultarNfseDpsEnvio xmlns=\"{NsSped.NamespaceName}\">" +
        "<IdentificacaoDps>" + MontarElemento("NumDPS", numeroDps) + MontarElemento("SerieDPS", serieDps) + "</IdentificacaoDps>" +
        MontarPrestador(cnpj, cpf, inscricaoMunicipal) +
        "</ConsultarNfseDpsEnvio>";

    /// <summary>Monta o grupo <c>Prestador</c> (<c>tcIdentificacaoPessoaEmpresaComIM</c>).</summary>
    private static string MontarPrestador(string? cnpj, string? cpf, string inscricaoMunicipal) =>
        "<Prestador>" +
        (!string.IsNullOrWhiteSpace(cnpj) ? MontarElemento("CNPJ", cnpj!) : MontarElemento("CPF", cpf ?? string.Empty)) +
        MontarElemento("IM", inscricaoMunicipal) +
        "</Prestador>";

    private static string MontarElemento(string nome, string valor) => $"<{nome}>{SecurityElement.Escape(valor.Trim())}</{nome}>";

    #endregion Montagem

    #region Leitura

    /// <summary>
    /// Retorna o elemento de resposta (ex.: <c>ConsultarNfseDpsResposta</c>) contido no envelope SOAP,
    /// tratando tanto o XML embutido quanto o XML retornado como texto escapado.
    /// </summary>
    private XElement? LerCorpoResposta(string xmlResposta, string nomeResposta)
    {
        try
        {
            var documento = XDocument.Parse(xmlResposta);
            var resposta = documento.Descendants().FirstOrDefault(x => x.Name.LocalName == nomeResposta);
            if (resposta != null) return resposta;

            var texto = documento.Descendants()
                .Where(x => !x.HasElements)
                .Select(x => x.Value.Trim())
                .FirstOrDefault(x => x.StartsWith("<") && x.Contains(nomeResposta));

            return texto == null ? null : XDocument.Parse(texto).Descendants().FirstOrDefault(x => x.Name.LocalName == nomeResposta);
        }
        catch (Exception ex)
        {
            this.Log().Error(ex);
            return null;
        }
    }

    /// <summary>
    /// Lê as mensagens (<c>MensagemRetorno</c>) da resposta. Os alertas são os da <c>ListaMensagemAlertaRetorno</c>;
    /// os demais (<c>ListaMensagemRetorno</c> e <c>ListaMensagemRetornoLote</c>) são erros. O mapeamento dos campos
    /// segue o da emissão: <c>Mensagem</c> → Descricao e <c>Correcao</c> → Mensagem.
    /// </summary>
    private static List<MensagemProcessamento> LerMensagens(XElement? raiz, bool alertas)
    {
        if (raiz == null) return [];

        return raiz.Descendants(NsSped + "MensagemRetorno")
            .Where(x => (x.Parent?.Name.LocalName == "ListaMensagemAlertaRetorno") == alertas)
            .Select(x =>
            {
                var dps = x.Element(NsSped + "IdentificacaoDps");
                return new MensagemProcessamento
                {
                    Codigo = x.Element(NsSped + "Codigo")?.Value ?? string.Empty,
                    Descricao = x.Element(NsSped + "Mensagem")?.Value ?? string.Empty,
                    Mensagem = x.Element(NsSped + "Correcao")?.Value ?? string.Empty,
                    Complemento = dps == null
                        ? string.Empty
                        : $"DPS {dps.Element(NsSped + "NumDPS")?.Value} série {dps.Element(NsSped + "SerieDPS")?.Value}"
                };
            })
            .ToList();
    }

    private static List<XElement> LerCompNfse(XElement? raiz) =>
        raiz?.Descendants(NsSped + "CompNfse").ToList() ?? [];

    private static IEnumerable<XElement> LerEventos(XElement compNfse) =>
        compNfse.Element(NsSped + "ListaEvento")?.Elements(NsSped + "evento") ?? [];

    /// <summary>Extrai a chave de acesso (50 posições) do atributo <c>Id</c> do <c>infNFSe</c> ("NFS" + chave).</summary>
    private static string LerChaveNFSe(XElement? nfse)
    {
        var id = nfse?.Element(NsSped + "infNFSe")?.Attribute("Id")?.Value ?? string.Empty;
        return id.StartsWith("NFS", StringComparison.OrdinalIgnoreCase) ? id.Substring(3) : id;
    }

    /// <summary>
    /// Extrai a chave, o tipo do evento (6) e o número do pedido de registro (3) do atributo <c>Id</c>
    /// do <c>infEvento</c> ("EVT" + chave(50) + tipo(6) + nPedRegEvento(3)).
    /// </summary>
    private static (string Chave, string TipoEvento, int NumeroPedido) LerIdentificacaoEvento(XElement evento)
    {
        var id = evento.Element(NsSped + "infEvento")?.Attribute("Id")?.Value ?? string.Empty;
        if (id.Length != 62 || !id.StartsWith("EVT", StringComparison.OrdinalIgnoreCase))
            return (string.Empty, string.Empty, 0);

        return (id.Substring(3, 50), id.Substring(53, 6), int.TryParse(id.Substring(59, 3), out var numero) ? numero : 0);
    }

    /// <summary>Lê a data/hora de processamento (<c>dhProc</c>) do <c>infNFSe</c> ou do <c>infEvento</c>.</summary>
    private static DateTimeOffset LerDataProcessamento(XElement? documento)
    {
        var valor = documento?.Elements().FirstOrDefault()?.Element(NsSped + "dhProc")?.Value;
        return DateTimeOffset.TryParse(valor, out var data) ? data : default;
    }

    /// <summary>Converte as NFS-e retornadas no modelo nacional; as que não puderem ser carregadas são registradas no log.</summary>
    private List<NotaFiscalServico> CarregarNotas(XElement? raiz)
    {
        var notas = new List<NotaFiscalServico>();

        foreach (var nfse in LerCompNfse(raiz).Select(x => x.Element(NsSped + "NFSe")).Where(x => x != null))
        {
            try
            {
                var nota = NotaFiscalServico.Load(nfse!.ToString(SaveOptions.DisableFormatting));
                if (nota != null) notas.Add(nota);
            }
            catch (Exception ex)
            {
                this.Log().Error("ISSNet: não foi possível carregar a NFS-e retornada.", ex);
            }
        }

        return notas;
    }

    /// <summary>Converte o código do evento (ex.: <c>101101</c>) no <see cref="TipoEvento"/> correspondente.</summary>
    private static TipoEvento? TipoEventoPorCodigo(string codigo) => codigo switch
    {
        TipoEventoCod.Cancelamento => TipoEvento.CANCELAMENTO,
        TipoEventoCod.SolicitacaoCancelamento => TipoEvento.SOLICITACAO_CANCELAMENTO_ANALISE_FISCAL,
        TipoEventoCod.CancelamentoPorSubstituicao => TipoEvento.CANCELAMENTO_POR_SUBSTITUICAO,
        TipoEventoCod.CancelamentoDeferido => TipoEvento.CANCELAMENTO_DEFERIDO_ANALISE_FISCAL,
        TipoEventoCod.CancelamentoIndeferido => TipoEvento.CANCELAMENTO_INDEFERIDO_ANALISE_FISCAL,
        TipoEventoCod.ConfirmacaoPrestador => TipoEvento.CONFIRMACAO_PRESTADOR,
        TipoEventoCod.RejeicaoPrestador => TipoEvento.REJEICAO_PRESTADOR,
        TipoEventoCod.ConfirmacaoTomador => TipoEvento.CONFIRMACAO_TOMADOR,
        TipoEventoCod.RejeicaoTomador => TipoEvento.REJEICAO_TOMADOR,
        TipoEventoCod.ConfirmacaoIntermediario => TipoEvento.CONFIRMACAO_INTERMEDIARIO,
        TipoEventoCod.RejeicaoIntermediario => TipoEvento.REJEICAO_INTERMEDIARIO,
        TipoEventoCod.ConfirmacaoTacita => TipoEvento.CONFIRMACAO_TACITA,
        TipoEventoCod.AnulacaoRejeicao => TipoEvento.ANULACAO_REJEICAO,
        TipoEventoCod.CancelamentoOficio => TipoEvento.CANCELAMENTO_POR_OFICIO,
        TipoEventoCod.BloqueioOficio => TipoEvento.BLOQUEIO_POR_OFICIO,
        TipoEventoCod.DesbloqueioOficio => TipoEvento.DESBLOQUEIO_POR_OFICIO,
        _ => null
    };

    private Task<T> TrataRetorno<T>(string xmlResposta, string xmlRootTag) where T : RespostaBase, new()
    {
        var resultado = new T();

        try
        {
            var doc = XDocument.Parse(xmlResposta);

            var elementosMensagem = doc.Descendants(NsSped + "MensagemRetorno");

            var listaMensagensErros = elementosMensagem
                .Select(elemento =>
                    new MensagemProcessamento
                    {
                        Codigo = elemento.Element(NsSped + "Codigo")?.Value ?? string.Empty,
                        Descricao = elemento.Element(NsSped + "Mensagem")?.Value ?? string.Empty,
                        Mensagem = elemento.Element(NsSped + "Correcao")?.Value ?? string.Empty,
                        Complemento = string.Empty,
                        Parametros = new List<string>()
                    })
                .ToList();

            if (listaMensagensErros.Any())
                resultado.Erros = listaMensagensErros;

            var elementNfse = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == xmlRootTag);

            if (elementNfse != null)
            {
                if (resultado is RespostaEnvioDps respostaNFSe)
                {
                    respostaNFSe.XmlNFSe = elementNfse.ToString();

                    var chaveAcesso = doc.Descendants()
                        .FirstOrDefault(x => x.Name.LocalName == "infNFSe")?
                        .Attribute("Id")?.Value.Replace("NFS", "") ?? string.Empty;

                    respostaNFSe.IdDps = $"DPS{chaveAcesso}";
                    respostaNFSe.ChaveAcesso = chaveAcesso;
                }
                else if (resultado is RespostaEnvioEvento respostaEvento)
                {
                    respostaEvento.XmlEvento = elementNfse.ToString();
                }
            }
        }
        catch (Exception ex)
        {
            this.Log().Error(ex);
        }

        return Task.FromResult(resultado);
    }

    #endregion Leitura
}
