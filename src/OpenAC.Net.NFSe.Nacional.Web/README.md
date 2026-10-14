# OpenAC.Net.NFSe.Nacional.Web

Integração da `OpenAC.Net.NFSe.Nacional` com ASP.NET Core, preparada para aplicações Web, APIs,
workers e cenários multiempresa.

A biblioteca usa os serviços nativos do ASP.NET Core para gerenciar HTTP, injeção de dependência,
logs, resiliência, certificados, persistência e observabilidade. Ela não cria um `HttpClient` por
requisição e não altera configurações globais do `ServicePointManager`.

## Recursos

- `IHttpClientFactory` com reaproveitamento seguro de conexões e handlers.
- Certificado cliente configurado no handler HTTP.
- Propagação de `CancellationToken` em todas as operações.
- Timeout geral e timeout específico por operação.
- Retry somente para métodos HTTP seguros.
- Circuit breaker opcional e isolado por endpoint.
- Suporte a configuração única ou multiempresa.
- Provider genérico de configuração compartilhado com outras integrações Web do projeto.
- Isolamento do pool por empresa, certificado, protocolo TLS e endpoint.
- Persistência assíncrona e concorrente de payloads e documentos fiscais.
- Instrumentação com `ActivitySource`, `Meter` e `ILogger`.

## Requisitos

- ASP.NET Core 8 ou superior.
- Certificado digital compatível com o webservice utilizado.
- Configuração válida da NFS-e para o município e provedor.

## Configuração para uma empresa

Registre a integração no `Program.cs`:

```csharp
using OpenAC.Net.NFSe.Nacional;

builder.Services.AddOpenNFSeNacionalWeb(
    configuracao =>
    {
        configuracao.WebServices.CodigoMunicipio = 5002704;
        configuracao.WebServices.ValidarSchemas = true;
        configuracao.WebServices.Tentativas = 3;
        configuracao.WebServices.IntervaloTentativas = 1000;

        configuracao.Certificados.CertificadoBytes =
            File.ReadAllBytes(builder.Configuration["NFSe:Certificado:Caminho"]!);
        configuracao.Certificados.Senha =
            builder.Configuration["NFSe:Certificado:Senha"]!;
    },
    infraestrutura =>
    {
        infraestrutura.TimeoutOperacao = TimeSpan.FromSeconds(60);
        infraestrutura.TimeoutTentativa = TimeSpan.FromSeconds(15);
        infraestrutura.CircuitBreakerHabilitado = true;

        infraestrutura.PersistenciaHabilitada = true;
        infraestrutura.CaminhoDocumentos = "App_Data/NFSe";

        infraestrutura.TimeoutsPorOperacao[nameof(IOpenNFSeNacionalClient.EnviarAsync)] =
            TimeSpan.FromSeconds(90);
    });
```

O `IOpenNFSeNacionalClient` é registrado como `scoped` e pode ser injetado em controllers,
Minimal APIs, serviços de aplicação ou handlers.

Nesse registro, a biblioteca adiciona automaticamente um
`DefaultNFSeConfigurationProvider<ConfiguracaoNFSe>`. Ele cria uma configuração independente
para cada cliente usando a ação passada a `AddOpenNFSeNacionalWeb`. O identificador interno desse
cenário é `default`; portanto, aplicações com uma única empresa não precisam implementar um
provider próprio.

### Exemplo com Minimal API

```csharp
app.MapPost("/nfse", async (
    Dps dps,
    IOpenNFSeNacionalClient nfse,
    CancellationToken cancellationToken) =>
{
    var resposta = await nfse.EnviarAsync(dps, cancellationToken);
    return Results.Ok(resposta);
});
```

O token recebido pelo ASP.NET Core é propagado até o transporte HTTP e para a persistência dos
documentos.

## Opções de infraestrutura

| Opção | Padrão | Finalidade |
| --- | ---: | --- |
| `PersistenciaHabilitada` | `false` | Habilita a gravação de payloads e documentos fiscais. |
| `CaminhoDocumentos` | `App_Data/NFSe` | Caminho relativo ao `ContentRootPath`. |
| `TimeoutOperacao` | 100 segundos | Limite total da operação. |
| `TimeoutTentativa` | 10 segundos | Limite de cada tentativa HTTP. |
| `CircuitBreakerHabilitado` | `true` | Habilita o circuit breaker por endpoint. |
| `TempoVidaConexaoPool` | 10 minutos | Tempo máximo de uma conexão no pool. |
| `TempoVidaHandler` | 5 minutos | Tempo de vida do handler administrado pelo factory. |
| `TimeoutsPorOperacao` | vazio | Sobrescreve o timeout de uma operação. |
| `MargemExpiracaoCertificado` | 5 minutos | Recusa certificados expirados ou próximos do vencimento. |

## Resiliência e idempotência

O pipeline HTTP possui timeout por tentativa, retry e circuit breaker. O retry é desabilitado para
métodos HTTP inseguros, incluindo `POST`, evitando o reenvio automático de emissões, eventos ou
lotes.

O total de tentativas e o intervalo fixo entre elas são definidos por
`ConfiguracaoNFSe.WebServices.Tentativas` e `IntervaloTentativas`. A tentativa inicial faz parte do
total; portanto, `Tentativas = 3` permite no máximo duas repetições. As mesmas propriedades são
respeitadas no desktop e na integração Web.

A matriz semântica pode ser consultada por `NFSeOperationPolicies.IsIdempotent`. Operações de
consulta são classificadas como idempotentes; emissões e eventos não são.

O circuit breaker é separado pela autoridade do endpoint. A indisponibilidade de um provedor não
abre automaticamente o circuito de outro endpoint.

## Persistência de documentos

A persistência fica desabilitada por padrão. Quando habilitada, o `FileSystemNFSeDocumentStore`
resolve a raiz através do `IHostEnvironment.ContentRootPath`.

```text
App_Data/NFSe/
└── Tenants/{identificador-hash}/
    ├── Technical/
    └── Fiscal/
        ├── DPS/
        └── NFSe/
```

As gravações são assíncronas. Arquivos concorrentes com o mesmo nome recebem um sufixo, evitando
sobrescrita. Para substituir o armazenamento padrão, registre uma implementação de
`INFSeDocumentStore` antes de chamar `AddOpenNFSeNacionalWeb`:

```csharp
builder.Services.AddSingleton<INFSeDocumentStore, MeuArmazenamentoNFSe>();
builder.Services.AddOpenNFSeNacionalWeb(configuracao =>
{
    // Configuração fiscal.
});
```

## Multiempresa

O contrato `INFSeConfigurationProvider<TConfiguration>` pertence a esta biblioteca Web e pode ser
reutilizado por integrações que precisam resolver configurações single-tenant ou multiempresa,
como `OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp.Web`.

Implemente os contratos:

- `INFSeConfigurationProvider<ConfiguracaoNFSe>`: retorna uma nova configuração para a empresa.
- `INFSeCertificateProvider`: carrega ou renova o certificado da empresa.

```csharp
public sealed class ConfiguracaoNFSePorEmpresa : INFSeConfigurationProvider<ConfiguracaoNFSe>
{
    public async ValueTask<ConfiguracaoNFSe> GetConfigurationAsync(
        string tenantId,
        CancellationToken cancellationToken = default)
    {
        var empresa = await CarregarEmpresaAsync(tenantId, cancellationToken);
        var configuracao = new ConfiguracaoNFSe();
        configuracao.WebServices.CodigoMunicipio = empresa.CodigoMunicipio;
        return configuracao;
    }
}

public sealed class CertificadoNFSePorEmpresa : INFSeCertificateProvider
{
    public async ValueTask ConfigureAsync(
        string tenantId,
        NFSeCertificadoConfig certificateConfiguration,
        CancellationToken cancellationToken = default)
    {
        var certificado = await CarregarCertificadoAsync(tenantId, cancellationToken);
        certificateConfiguration.CertificadoBytes = certificado.Conteudo;
        certificateConfiguration.Senha = certificado.Senha;
    }
}
```

O provider deve devolver uma nova `ConfiguracaoNFSe` por chamada. A configuração é mutável durante
a preparação do certificado e do cliente; por isso, não compartilhe a mesma instância entre
empresas ou requisições concorrentes.

Registre os providers:

```csharp
builder.Services
    .AddOpenNFSeNacionalWebMultiTenant<
        ConfiguracaoNFSePorEmpresa,
        CertificadoNFSePorEmpresa>(opcoes =>
    {
        opcoes.TimeoutOperacao = TimeSpan.FromSeconds(60);
        opcoes.PersistenciaHabilitada = true;
    });
```

Injete a fábrica e informe a empresa:

```csharp
app.MapPost("/empresas/{empresaId}/nfse", async (
    string empresaId,
    Dps dps,
    IOpenNFSeNacionalClientFactory fabrica,
    CancellationToken cancellationToken) =>
{
    var cliente = await fabrica.CreateAsync(empresaId, cancellationToken);
    var resposta = await cliente.EnviarAsync(dps, cancellationToken);
    return Results.Ok(resposta);
});
```

Cada criação consulta novamente o provider de certificado. Quando o certificado é renovado e seu
thumbprint muda, um novo pool HTTP é criado. Certificados dentro da margem de expiração são
rejeitados antes da chamada ao webservice.

## Provider genérico de configuração

O contrato é genérico para que cada integração mantenha seu próprio tipo de configuração:

```csharp
public interface INFSeConfigurationProvider<TConfiguration>
{
    ValueTask<TConfiguration> GetConfigurationAsync(
        string tenantId = NFSeTenant.Padrao,
        CancellationToken cancellationToken = default);
}
```

O `tenantId` é opcional nas APIs públicas. Quando omitido, o valor
`NFSeTenant.Padrao` é utilizado. Providers e componentes internos continuam recebendo uma
identidade não nula e validada.

Para criar integrações single-tenant manualmente, use o provider padrão:

```csharp
var provider = new DefaultNFSeConfigurationProvider<MinhaConfiguracao>(() =>
    new MinhaConfiguracao());
```

A função é executada a cada consulta. Isso permite retornar configurações independentes e evita o
compartilhamento acidental de estado mutável.

## Observabilidade

A biblioteca cria logs estruturados com operação, município, provedor e empresa. Também expõe:

- `ActivitySource`: `OpenAC.Net.NFSe.Nacional.Web`.
- `Meter`: `OpenAC.Net.NFSe.Nacional.Web`.
- Contador: `openac.nfse.operations`.
- Histograma: `openac.nfse.operation.duration`.

Exemplo com OpenTelemetry:

```csharp
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddSource("OpenAC.Net.NFSe.Nacional.Web"))
    .WithMetrics(metrics => metrics
        .AddMeter("OpenAC.Net.NFSe.Nacional.Web"));
```

## Tratamento de falhas

- Falhas de transporte, timeout do pipeline e circuit breaker geram `NFSeTechnicalException`.
- Cancelamentos continuam sendo representados por `OperationCanceledException`.
- Rejeições fiscais são retornadas nos objetos `NFSeResponse` e não são tratadas como falha de
  infraestrutura.

Assim, a aplicação consegue diferenciar indisponibilidade técnica, cancelamento da requisição e
rejeição fiscal sem depender da mensagem textual da exceção.
