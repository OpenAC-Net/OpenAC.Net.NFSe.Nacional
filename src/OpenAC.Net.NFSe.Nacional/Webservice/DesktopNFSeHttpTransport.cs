using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using OpenAC.Net.NFSe.Nacional.Common;

namespace OpenAC.Net.NFSe.Nacional.Webservice;

/// <summary>Transporte padrão para aplicações desktop sem container de serviços.</summary>
public sealed class DesktopNFSeHttpTransport : INFSeHttpTransport
{
    /// <summary>Instância compartilhada do transporte desktop.</summary>
    public static DesktopNFSeHttpTransport Instance { get; } = new();

    private DesktopNFSeHttpTransport()
    {
    }

    /// <inheritdoc />
    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, ConfiguracaoNFSe configuracao,
        CancellationToken cancellationToken = default)
    {
        var certificado = configuracao.Certificados.ObterCertificado();
        var protocolos = (System.Security.Authentication.SslProtocols)configuracao.WebServices.Protocolos;
        using var cliente = DesktopNFSeHttpClientPool.Instancia.ObterCliente(certificado, protocolos);
        var conteudo = request.Content is null
            ? null
            : await request.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        var cabecalhosConteudo = request.Content?.Headers
            .SelectMany(header => header.Value.Select(value => new KeyValuePair<string, string>(header.Key, value)))
            .ToArray();
        var tentativas = RequisicaoSegura(request.Method)
            ? Math.Max(1, configuracao.WebServices.Tentativas)
            : 1;
        var intervalo = TimeSpan.FromMilliseconds(Math.Max(0, configuracao.WebServices.IntervaloTentativas));

        for (var tentativa = 1; tentativa <= tentativas; tentativa++)
        {
            using var requisicao = ClonarRequisicao(request, conteudo, cabecalhosConteudo);
            try
            {
                var resposta = await cliente.Http.SendAsync(requisicao, cancellationToken).ConfigureAwait(false);
                if (tentativa == tentativas || !RespostaTransitoria(resposta.StatusCode))
                    return resposta;

                resposta.Dispose();
            }
            catch (HttpRequestException) when (tentativa < tentativas)
            {
                // Uma falha de transporte em operação segura pode ser repetida.
            }

            if (intervalo > TimeSpan.Zero)
                await Task.Delay(intervalo, cancellationToken).ConfigureAwait(false);
        }

        throw new InvalidOperationException("O envio HTTP terminou sem produzir uma resposta.");
    }

    private static HttpRequestMessage ClonarRequisicao(HttpRequestMessage origem, byte[]? conteudo,
        IReadOnlyCollection<KeyValuePair<string, string>>? cabecalhosConteudo)
    {
        var clone = new HttpRequestMessage(origem.Method, origem.RequestUri) { Version = origem.Version };
        foreach (var header in origem.Headers)
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

        if (conteudo is not null)
            clone.Content = new ByteArrayContent(conteudo);
        
        if (cabecalhosConteudo is null) return clone;
        
        foreach (var header in cabecalhosConteudo)
            clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);

        return clone;
    }

    private static bool RequisicaoSegura(HttpMethod method) =>
        method == HttpMethod.Get || method == HttpMethod.Head ||
        string.Equals(method.Method, "OPTIONS", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(method.Method, "TRACE", StringComparison.OrdinalIgnoreCase);

    private static bool RespostaTransitoria(HttpStatusCode statusCode) =>
        statusCode == HttpStatusCode.RequestTimeout || (int)statusCode == 429 || (int)statusCode >= 500;
}