using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace OpenAC.Net.NFSe.Nacional.Webservice;

/// <summary>
/// Mantém clientes HTTP reutilizáveis para aplicações desktop, isolados por certificado e protocolo TLS.
/// </summary>
internal sealed class DesktopNFSeHttpClientPool
{
    private static readonly TimeSpan TempoVidaCliente = TimeSpan.FromMinutes(10);
    private readonly object syncRoot = new();
    private readonly Dictionary<string, EntradaCliente> clientes = new();

    /// <summary>Instância compartilhada do pool desktop.</summary>
    public static DesktopNFSeHttpClientPool Instancia { get; } = new();

    private DesktopNFSeHttpClientPool()
    {
    }

    /// <summary>Obtém um cliente associado ao certificado e aos protocolos informados.</summary>
    public EmprestimoCliente ObterCliente(X509Certificate2 certificado, SslProtocols protocolos)
    {
        var chave = CriarChave(certificado, protocolos);
        var agora = DateTimeOffset.UtcNow;

        lock (syncRoot)
        {
            RemoverClientesExpirados(agora);

            if (!clientes.TryGetValue(chave, out var entrada))
            {
                entrada = CriarEntrada(certificado, protocolos, agora);
                clientes.Add(chave, entrada);
            }

            entrada.RequisicoesAtivas++;
            return new EmprestimoCliente(this, entrada);
        }
    }

    private static EntradaCliente CriarEntrada(X509Certificate2 certificado, SslProtocols protocolos,
        DateTimeOffset agora)
    {
        var handler = new HttpClientHandler { SslProtocols = protocolos };
        handler.ClientCertificates.Add(certificado);

        var expiracaoCertificado = new DateTimeOffset(certificado.NotAfter.ToUniversalTime());
        var expiracao = agora.Add(TempoVidaCliente);
        if (expiracaoCertificado < expiracao)
            expiracao = expiracaoCertificado;

        return new EntradaCliente(new HttpClient(handler, true), expiracao);
    }

    private void RemoverClientesExpirados(DateTimeOffset agora)
    {
        var chavesExpiradas = new List<string>();
        foreach (var item in clientes)
        {
            if (item.Value.Expiracao > agora) continue;
            item.Value.Removido = true;
            chavesExpiradas.Add(item.Key);
            if (item.Value.RequisicoesAtivas == 0)
                item.Value.Http.Dispose();
        }

        foreach (var chave in chavesExpiradas)
            clientes.Remove(chave);
    }

    private void Devolver(EntradaCliente entrada)
    {
        lock (syncRoot)
        {
            entrada.RequisicoesAtivas--;
            if (entrada.Removido && entrada.RequisicoesAtivas == 0)
                entrada.Http.Dispose();
        }
    }

    private static string CriarChave(X509Certificate2 certificado, SslProtocols protocolos)
    {
        var identidade = certificado.Thumbprint;
        if (!string.IsNullOrWhiteSpace(identidade)) return $"{identidade}:{(int)protocolos}";

        using var sha256 = SHA256.Create();
        identidade = Convert.ToBase64String(sha256.ComputeHash(certificado.RawData));

        return $"{identidade}:{(int)protocolos}";
    }

    internal sealed class EntradaCliente(HttpClient http, DateTimeOffset expiracao)
    {
        public HttpClient Http { get; } = http;
        public DateTimeOffset Expiracao { get; } = expiracao;
        public int RequisicoesAtivas { get; set; }
        public bool Removido { get; set; }
    }

    /// <summary>Representa o uso temporário de um cliente mantido pelo pool.</summary>
    internal sealed class EmprestimoCliente : IDisposable
    {
        private readonly DesktopNFSeHttpClientPool pool;
        private readonly EntradaCliente entrada;
        private bool devolvido;

        internal EmprestimoCliente(DesktopNFSeHttpClientPool pool, EntradaCliente entrada)
        {
            this.pool = pool;
            this.entrada = entrada;
        }

        public HttpClient Http => entrada.Http;

        public void Dispose()
        {
            if (devolvido) return;
            devolvido = true;
            pool.Devolver(entrada);
        }
    }
}
