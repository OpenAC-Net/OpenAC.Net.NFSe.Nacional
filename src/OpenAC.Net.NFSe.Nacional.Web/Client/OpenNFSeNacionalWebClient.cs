using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAC.Net.NFSe.Nacional.Common;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.Storage;
using OpenAC.Net.NFSe.Nacional.Web.Configuration;
using OpenAC.Net.NFSe.Nacional.Web.Exception;
using OpenAC.Net.NFSe.Nacional.Webservice;

namespace OpenAC.Net.NFSe.Nacional.Web.Client;

internal sealed class OpenNFSeNacionalWebClient(
    ConfiguracaoNFSe configuracao,
    INFSeHttpTransport transport,
    INFSeDocumentStore documentStore,
    ILogger<OpenNFSeNacionalWebClient> logger,
    IOptions<OpenNFSeNacionalWebOptions> options,
    string? tenantId = null)
    : IOpenNFSeNacionalClient
{
    private const string InstrumentationName = "OpenAC.Net.NFSe.Nacional.Web";
    private static readonly ActivitySource ActivitySource = new(InstrumentationName);
    private static readonly Meter Meter = new(InstrumentationName);
    private static readonly Counter<long> Operations = Meter.CreateCounter<long>("openac.nfse.operations");

    private static readonly Histogram<double> Duration =
        Meter.CreateHistogram<double>("openac.nfse.operation.duration", "ms");

    private readonly OpenNFSeNacionalWebOptions options = options.Value;

    private NFSeWebserviceBase Provider() =>
        NFSeServiceManager.Instance.GetProvider(configuracao, transport, documentStore);

    private async Task<T> ExecuteAsync<T>(string operation, CancellationToken cancellationToken,
        Func<NFSeWebserviceBase, CancellationToken, Task<T>> action)
    {
        using var activity = ActivitySource.StartActivity(operation, ActivityKind.Client);
        activity?.SetTag("nfse.operation", operation);
        activity?.SetTag("nfse.tenant.id", tenantId);
        activity?.SetTag("nfse.provider", configuracao.WebServices.Provedor.ToString());
        var started = Stopwatch.GetTimestamp();
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(options.TimeoutsPorOperacao.TryGetValue(operation, out var operationTimeout)
            ? operationTimeout
            : options.TimeoutOperacao);
        using var scope = logger.BeginScope(new Dictionary<string, object?>
        {
            ["NFSe.Operation"] = operation,
            ["NFSe.Municipio"] = configuracao.WebServices.CodigoMunicipio,
            ["NFSe.Provider"] = configuracao.WebServices.Provedor,
            ["NFSe.TenantId"] = tenantId
        });
        try
        {
            var result = await action(Provider(), timeout.Token).ConfigureAwait(false);
            activity?.SetStatus(ActivityStatusCode.Ok);
            Operations.Add(1, new KeyValuePair<string, object?>("operation", operation),
                new KeyValuePair<string, object?>("outcome", "success"));
            return result;
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Operação {Operation} cancelada ou expirada", operation);
            activity?.SetStatus(ActivityStatusCode.Error, "cancelled_or_timeout");
            Operations.Add(1, new KeyValuePair<string, object?>("operation", operation),
                new KeyValuePair<string, object?>("outcome", "cancelled"));
            throw;
        }
        catch (System.Exception exception) when (exception is HttpRequestException
                                                     or Polly.CircuitBreaker.BrokenCircuitException
                                                     or Polly.Timeout.TimeoutRejectedException)
        {
            logger.LogError(exception, "Falha na operação {Operation}", operation);
            activity?.SetStatus(ActivityStatusCode.Error, exception.Message);
            Operations.Add(1, new KeyValuePair<string, object?>("operation", operation),
                new KeyValuePair<string, object?>("outcome", "technical_error"));
            throw new NFSeTechnicalException(operation, $"Falha técnica na operação '{operation}'.", exception);
        }
        catch (System.Exception exception)
        {
            logger.LogError(exception, "Falha na operação {Operation}", operation);
            activity?.SetStatus(ActivityStatusCode.Error, exception.Message);
            Operations.Add(1, new KeyValuePair<string, object?>("operation", operation),
                new KeyValuePair<string, object?>("outcome", "error"));
            throw;
        }
        finally
        {
            Duration.Record(Stopwatch.GetElapsedTime(started).TotalMilliseconds,
                new KeyValuePair<string, object?>("operation", operation));
        }
    }

    public Task<NFSeResponse<RespostaEnvioDps>> EnviarAsync(Dps dps, CancellationToken cancellationToken = default) =>
        ExecuteAsync(nameof(EnviarAsync), cancellationToken, (p, ct) => p.EnviarAsync(dps, ct));

    public Task<NFSeResponse<RespostaEnvioEvento>> EnviarEventoAsync(PedidoRegistroEvento evento,
        CancellationToken cancellationToken = default) =>
        ExecuteAsync(nameof(EnviarEventoAsync), cancellationToken, (p, ct) => p.EnviarEventoAsync(evento, ct));

    public Task<NFSeResponse<RespostaConsultaDFe>> ConsultaNsuAsync(int nsu,
        CancellationToken cancellationToken = default) =>
        ExecuteAsync(nameof(ConsultaNsuAsync), cancellationToken, (p, ct) => p.ConsultaNsuAsync(nsu, ct));

    public Task<NFSeResponse<RespostaConsultaDFe>> ConsultaChaveAsync(string chave,
        CancellationToken cancellationToken = default) =>
        ExecuteAsync(nameof(ConsultaChaveAsync), cancellationToken, (p, ct) => p.ConsultaChaveAsync(chave, ct));

    public Task<byte[]> DownloadDANFSeAsync(string chave, CancellationToken cancellationToken = default) =>
        ExecuteAsync(nameof(DownloadDANFSeAsync), cancellationToken, (p, ct) => p.DownloadDANFSeAsync(chave, ct));

    public Task<NFSeResponse<RespostaConsultaChaveDps>> ConsultaChaveDpsAsync(string id,
        CancellationToken cancellationToken = default) =>
        ExecuteAsync(nameof(ConsultaChaveDpsAsync), cancellationToken, (p, ct) => p.ConsultaChaveDpsAsync(id, ct));

    public Task<bool> ConsultaExisteDpsAsync(string id, CancellationToken cancellationToken = default) =>
        ExecuteAsync(nameof(ConsultaExisteDpsAsync), cancellationToken, (p, ct) => p.ConsultaExisteDpsAsync(id, ct));

    public Task<NFSeResponse<RespostaRecepcaoLote>> EnviarLoteAsync(LoteDps lote,
        CancellationToken cancellationToken = default) =>
        ExecuteAsync(nameof(EnviarLoteAsync), cancellationToken, (p, ct) => p.EnviarLoteAsync(lote, ct));

    public Task<NFSeResponse<RespostaRecepcaoLoteSincrono>> EnviarLoteSincronoAsync(LoteDps lote,
        CancellationToken cancellationToken = default) =>
        ExecuteAsync(nameof(EnviarLoteSincronoAsync), cancellationToken,
            (p, ct) => p.EnviarLoteSincronoAsync(lote, ct));

    public Task<NFSeResponse<RespostaConsultaLote>> ConsultarLoteAsync(ConsultaLoteFiltro filtro,
        CancellationToken cancellationToken = default) =>
        ExecuteAsync(nameof(ConsultarLoteAsync), cancellationToken, (p, ct) => p.ConsultarLoteAsync(filtro, ct));

    public Task<NFSeResponse<RespostaConsultaNFSe>> ConsultarNFSeAsync(ConsultaNFSeFiltro filtro,
        CancellationToken cancellationToken = default) =>
        ExecuteAsync(nameof(ConsultarNFSeAsync), cancellationToken, (p, ct) => p.ConsultarNFSeAsync(filtro, ct));

    public Task<NFSeResponse<RespostaConsultaEvento>> ConsultaEventoAsync(string chaveAcesso, string tipoEvento,
        int numSeqEvento, CancellationToken cancellationToken = default) =>
        ExecuteAsync(nameof(ConsultaEventoAsync), cancellationToken,
            (p, ct) => p.ConsultaEventoAsync(chaveAcesso, tipoEvento, numSeqEvento, ct));
}