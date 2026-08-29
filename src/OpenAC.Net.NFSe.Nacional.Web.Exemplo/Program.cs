using OpenAC.Net.NFSe.Nacional;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp.Web;

var builder = WebApplication.CreateBuilder(args);

var codigoMunicipio = builder.Configuration.GetValue("NFSe:CodigoMunicipio", 5002704);
var validarSchemas = builder.Configuration.GetValue("NFSe:ValidarSchemas", true);
var certificadoPath = ResolverCaminho(builder.Environment.ContentRootPath,
    builder.Configuration["NFSe:Certificado:Caminho"]);
var certificadoBytes = LerArquivoOpcional(certificadoPath);
var senhaCertificado = builder.Configuration["NFSe:Certificado:Senha"] ?? string.Empty;
var logoPath = ResolverCaminho(builder.Environment.ContentRootPath,
    builder.Configuration["DANFSe:LogoPrestador"]);
var logoBytes = LerArquivoOpcional(logoPath);

builder.Services.AddOpenNFSeNacionalWeb(
    configuracao =>
    {
        configuracao.WebServices.CodigoMunicipio = codigoMunicipio;
        configuracao.WebServices.ValidarSchemas = validarSchemas;
        configuracao.WebServices.Tentativas = 3;
        configuracao.WebServices.IntervaloTentativas = 1000;
        configuracao.Certificados.CertificadoBytes = certificadoBytes ?? [];
        configuracao.Certificados.Senha = senhaCertificado;
    },
    infraestrutura =>
    {
        infraestrutura.TimeoutOperacao = TimeSpan.FromSeconds(60);
        infraestrutura.TimeoutTentativa = TimeSpan.FromSeconds(15);
        infraestrutura.PersistenciaHabilitada = true;
        infraestrutura.CaminhoDocumentos = "App_Data/NFSe";
    });

builder.Services.AddOpenDANFSeNacionalWeb(options =>
{
    options.Configuracoes.ExibirQRCode = true;
    options.Configuracoes.LogoPrestador = logoBytes;
    options.ForcarDownload = false;
    options.NomeArquivo = nota => $"DANFSe-{nota.Informacoes.NumeroNFSe}.pdf";
});

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new
{
    aplicacao = "OpenAC.Net.NFSe.Nacional - Exemplo Web",
    endpoints = new[]
    {
        "POST /nfse/emissoes",
        "POST /nfse/emissoes/pdf",
        "POST /danfse/pdf",
        "POST /danfse/pdf/lote",
        "GET /nfse/{chave}/danfse-oficial"
    }
}));

app.MapPost("/nfse/emissoes", async (
    Dps dps,
    IOpenNFSeNacionalClient nfse,
    CancellationToken cancellationToken) =>
{
    var resposta = await nfse.EnviarAsync(dps, cancellationToken);
    return resposta.Sucesso ? Results.Ok(resposta) : Results.BadRequest(resposta);
});

app.MapPost("/nfse/emissoes/pdf", async (
    Dps dps,
    IOpenNFSeNacionalClient nfse,
    IOpenDANFSeNacionalWeb danfse,
    CancellationToken cancellationToken) =>
{
    var resposta = await nfse.EnviarAsync(dps, cancellationToken);
    var nota = resposta.Resultado?.NFSe;
    if (!resposta.Sucesso || nota is null)
        return Results.BadRequest(resposta);

    return await danfse.GerarResultadoAsync(nota, cancellationToken: cancellationToken);
});

app.MapPost("/danfse/pdf", async (
    NotaFiscalServico nota,
    IOpenDANFSeNacionalWeb danfse,
    CancellationToken cancellationToken) =>
    await danfse.GerarResultadoAsync(nota, cancellationToken: cancellationToken));

app.MapPost("/danfse/pdf/lote", async (
    NotaFiscalServico[] notas,
    IOpenDANFSeNacionalWeb danfse,
    CancellationToken cancellationToken) =>
    await danfse.GerarResultadoAsync(notas, cancellationToken: cancellationToken));

app.MapGet("/nfse/{chave}/danfse-oficial", async (
    string chave,
    IOpenNFSeNacionalClient nfse,
    CancellationToken cancellationToken) =>
{
    var pdf = await nfse.DownloadDANFSeAsync(chave, cancellationToken);
    return Results.File(pdf, DANFSeWebDocument.TipoConteudoPdf, $"DANFSe-{chave}.pdf");
});

app.Run();

static string? ResolverCaminho(string contentRoot, string? caminho)
{
    if (string.IsNullOrWhiteSpace(caminho)) return null;
    return Path.IsPathRooted(caminho) ? caminho : Path.Combine(contentRoot, caminho);
}

static byte[]? LerArquivoOpcional(string? caminho) =>
    !string.IsNullOrWhiteSpace(caminho) && File.Exists(caminho)
        ? File.ReadAllBytes(caminho)
        : null;
