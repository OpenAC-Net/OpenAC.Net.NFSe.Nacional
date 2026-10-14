# OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp.Web

Integração ASP.NET Core para gerar o DANFSe Nacional em PDF usando a biblioteca
`OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp`.

A biblioteca depende de `OpenAC.Net.NFSe.Nacional.Web` para reutilizar o contrato
`INFSeConfigurationProvider<TConfiguration>`. A configuração da impressão não usa
`IOptions`/`IOptionsMonitor`: ela é obtida pelo provider single-tenant ou multiempresa em cada
operação.

## Recursos

- Registro no container de serviços do ASP.NET Core.
- Configuração single-tenant pronta para uso.
- Configuração e isolamento por empresa.
- Geração individual ou em lote, inteiramente em memória.
- Retorno em `byte[]`, `Stream` ou `IResult`.
- Exibição no navegador ou download com nome configurável.
- Propagação de `CancellationToken` ao provider de configuração.
- Compatibilidade com .NET 8, 9 e 10.

## Registro

```csharp
builder.Services.AddOpenDANFSeNacionalWeb(opcoes =>
{
    opcoes.Configuracoes.ExibirQRCode = true;
    opcoes.Configuracoes.LogoPrestador = File.ReadAllBytes("logo.png");
    opcoes.NomeArquivo = nota => $"DANFSe-{nota.Informacoes.NumeroNFSe}.pdf";
});
```

Esse registro cria automaticamente um
`DefaultNFSeConfigurationProvider<DANFSeNacionalWebOptions>`. A função de configuração é
aplicada a uma nova instância sempre que o serviço precisa gerar um documento. As sobrecargas sem
`tenantId` usam o identificador `default`.

Também é possível registrar apenas os valores padrão:

```csharp
builder.Services.AddOpenDANFSeNacionalWeb();
```

## Minimal API

```csharp
app.MapGet("/danfse/{chave}", async (string chave, IOpenDANFSeNacionalWeb danfse,
    CancellationToken cancellationToken) =>
{
    NotaFiscalServico nota = ObterNota(chave);
    return await danfse.GerarResultadoAsync(nota, cancellationToken: cancellationToken);
});
```

Por padrão, o PDF é exibido no navegador. Defina `ForcarDownload = true` para enviar
o nome do arquivo no cabeçalho `Content-Disposition` e sugerir o download.

A geração do PDF é síncrona internamente porque PDFSharp executa trabalho de CPU. Os métodos Web
são assíncronos para permitir que o provider carregue configurações, logotipos ou regras da empresa
em banco de dados ou serviço externo sem bloquear a requisição.

Também é possível obter o documento em memória:

```csharp
DANFSeWebDocument documento = await danfse.GerarAsync(nota);
byte[] pdf = documento.Conteudo;
using Stream stream = documento.AbrirLeitura();
```

## Multiempresa

Implemente `INFSeConfigurationProvider<DANFSeNacionalWebOptions>` para carregar logo,
layout, segurança e nomes de arquivo por empresa:

```csharp
public sealed class ConfiguracaoDANFSePorEmpresa
    : INFSeConfigurationProvider<DANFSeNacionalWebOptions>
{
    public async ValueTask<DANFSeNacionalWebOptions> GetConfigurationAsync(
        string tenantId,
        CancellationToken cancellationToken = default)
    {
        var empresa = await CarregarEmpresaAsync(tenantId, cancellationToken);

        return new DANFSeNacionalWebOptions
        {
            ForcarDownload = empresa.ForcarDownload,
            NomeArquivoLote = $"DANFSe-{empresa.Documento}-Lote.pdf",
            NomeArquivo = nota => $"DANFSe-{nota.Informacoes.NumeroNFSe}.pdf"
        };
    }
}
```

Registre o provider e informe o identificador da empresa durante a geração:

```csharp
builder.Services.AddOpenDANFSeNacionalWeb<ConfiguracaoDANFSePorEmpresa>();

app.MapGet("/empresas/{empresaId}/danfse/{chave}", async (
    string empresaId,
    string chave,
    IOpenDANFSeNacionalWeb danfse,
    CancellationToken cancellationToken) =>
{
    NotaFiscalServico nota = ObterNota(chave);
    return await danfse.GerarResultadoAsync(
        nota,
        tenantId: empresaId,
        cancellationToken: cancellationToken);
});
```

O provider deve retornar uma configuração independente para cada chamada. Não reutilize uma mesma
instância mutável entre empresas, especialmente quando os logotipos ou as opções de segurança forem
diferentes.

## Configurações disponíveis

| Propriedade | Padrão | Finalidade |
| --- | --- | --- |
| `Configuracoes` | nova configuração PDFSharp | Layout, logotipos, QR Code, margens e segurança do PDF. |
| `NomeArquivo` | `DANFSe-{numero}.pdf` | Define o nome do documento individual. |
| `NomeArquivoLote` | `DANFSe-Lote.pdf` | Define o nome do documento com várias notas. |
| `ForcarDownload` | `false` | Envia `Content-Disposition` para sugerir o download. |

## Geração em lote

```csharp
NotaFiscalServico[] notas = await ObterNotasAsync(cancellationToken);

DANFSeWebDocument documento = await danfse.GerarAsync(
    notas,
    tenantId: empresaId,
    cancellationToken: cancellationToken);
```
