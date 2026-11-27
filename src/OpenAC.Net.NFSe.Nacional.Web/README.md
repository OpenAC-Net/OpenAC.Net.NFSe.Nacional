# OpenAC.Net.NFSe.Nacional.Web

[![NuGet Version](https://img.shields.io/nuget/v/OpenAC.Net.NFSe.Nacional.Web.svg)](https://www.nuget.org/packages/OpenAC.Net.NFSe.Nacional.Web/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/OpenAC.Net.NFSe.Nacional.Web.svg)](https://www.nuget.org/packages/OpenAC.Net.NFSe.Nacional.Web/)
[![Discord](https://img.shields.io/badge/Chat%20on-Discord-purple.svg)](https://discord.com/invite/brdmJ7Yv6w)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![Target Frameworks](https://img.shields.io/badge/.NET-net8.0%20%7C%20net9.0%20%7C%20net10.0%20%7C%20net11.0-blue.svg)](https://dotnet.microsoft.com/)

Biblioteca .NET para integração da **NFS-e (Nota Fiscal de Serviço Eletrônica) Padrão Nacional** com aplicações **ASP.NET Core**.

Desenvolvida pelo time **OpenAC .Net**, a biblioteca estende a `OpenAC.Net.NFSe.Nacional` com injeção de dependências, transporte HTTP, configuração por empresa e observabilidade.

---

## 🚀 Funcionalidades

- **Integração com ASP.NET Core**:
  - Registro dos serviços com `AddNFSeNacional`.
  - Cliente `IOpenNFSeNacionalClient` disponível por injeção de dependências.
- **Emissão, Eventos e Consultas**:
  - Envio de DPS, lotes e eventos conforme o suporte do provedor.
  - Consulta por chave de acesso, NSU e Id da DPS.
  - Download do DANFSe oficial em PDF via webservice.
- **Configuração Multiempresa**:
  - Configuração fiscal e certificado digital por empresa.
  - Criação de clientes com `IOpenNFSeNacionalClientFactory`.
  - Isolamento do transporte HTTP por empresa.
- **Transporte e Resiliência**:
  - Integração com `IHttpClientFactory`.
  - Configuração de tentativas, intervalos e timeouts.
  - Propagação de `CancellationToken`.
- **Persistência e Observabilidade**:
  - Gravação de DPS, NFS-e e arquivos de comunicação.
  - Logging estruturado, métricas e rastreamento com `ActivitySource`.
- **Compatibilidade**:
  - .NET **8**, **9**, **10** e **11**.

---

## 📦 Instalação

Adicione o pacote via NuGet Package Manager ou CLI:

### .NET CLI:

```bash
dotnet add package OpenAC.Net.NFSe.Nacional.Web
```

### Package Manager Console:

```powershell
Install-Package OpenAC.Net.NFSe.Nacional.Web
```

### PackageReference:

```xml
<PackageReference Include="OpenAC.Net.NFSe.Nacional.Web" Version="1.6.1" />
```

---

## 🛠️ Como Usar

### 1. Registrando e Configurando os Serviços

No `Program.cs` da aplicação ASP.NET Core:

```csharp
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Nacional;
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.Common.Types;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddNFSeNacional(configuracao =>
{
    configuracao.Geral.Versao = VersaoNFSe.Ve101;
    configuracao.WebServices.Ambiente = DFeTipoAmbiente.Homologacao;
    configuracao.WebServices.CodigoMunicipio = 3550308;
    configuracao.WebServices.ValidarSchemas = true;
    configuracao.WebServices.TimeoutOperacao = TimeSpan.FromSeconds(60);
    configuracao.WebServices.TimeoutTentativa = TimeSpan.FromSeconds(15);
    configuracao.WebServices.Tentativas = 3;
    configuracao.WebServices.IntervaloTentativas = 1000;

    configuracao.Arquivos.PathSchemas = Path.Combine(
        builder.Environment.ContentRootPath, "Schemas", "1.01", "Homologacao");
    configuracao.Certificados.CertificadoBytes = File.ReadAllBytes(
        builder.Configuration["NFSe:Certificado:Caminho"]!);
    configuracao.Certificados.Senha = builder.Configuration["NFSe:Certificado:Senha"]!;
});

var app = builder.Build();

app.MapPost("/nfse/emissoes", async (
    Dps dps,
    IOpenNFSeNacionalClient nfse,
    CancellationToken cancellationToken) =>
{
    var resposta = await nfse.EnviarAsync(dps, cancellationToken);
    return resposta.Sucesso ? Results.Ok(resposta) : Results.BadRequest(resposta);
});

app.Run();
```

A DPS recebida deve conter os dados fiscais exigidos pelo layout e pelo provedor. Os arquivos XSD devem estar disponíveis na pasta configurada em `PathSchemas`.

---

### 2. Consultas e Download do DANFSe

Adicione as rotas antes de `app.Run()`:

```csharp
app.MapGet("/nfse/{chave}", async (
    string chave,
    IOpenNFSeNacionalClient nfse,
    CancellationToken cancellationToken) =>
{
    var resposta = await nfse.ConsultaChaveAsync(chave, cancellationToken);
    return resposta.Sucesso ? Results.Ok(resposta) : Results.BadRequest(resposta);
});

app.MapGet("/nfse/{chave}/danfse", async (
    string chave,
    IOpenNFSeNacionalClient nfse,
    CancellationToken cancellationToken) =>
{
    var pdf = await nfse.DownloadDANFSeAsync(chave, cancellationToken);
    return Results.File(pdf, "application/pdf", $"DANFSe-{chave}.pdf");
});
```

Para gerar o DANFSe localmente a partir do XML da NFS-e, utilize o pacote [OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp.Web](https://www.nuget.org/packages/OpenAC.Net.NFSe.Nacional.DANFSe.PDFSharp.Web/).

---

### 3. Configuração Multiempresa

Implemente os contratos `INFSeConfigurationProvider<ConfiguracaoNFSe>` e `INFSeCertificateProvider`, disponíveis no namespace `OpenAC.Net.NFSe.Nacional.Web.Provider`, para obter a configuração fiscal e o certificado de cada empresa.

Registre suas implementações pela sobrecarga genérica de `AddNFSeNacional`. No exemplo abaixo, `ConfiguracaoEmpresaProvider` e `CertificadoEmpresaProvider` são classes da aplicação que implementam esses contratos:

```csharp
builder.Services.AddNFSeNacional<ConfiguracaoEmpresaProvider, CertificadoEmpresaProvider>();
```

Utilize a fábrica para criar o cliente da empresa em cada operação:

```csharp
using OpenAC.Net.NFSe.Nacional.Web.Client;

app.MapGet("/empresas/{empresaId}/nfse/{chave}", async (
    string empresaId,
    string chave,
    IOpenNFSeNacionalClientFactory factory,
    CancellationToken cancellationToken) =>
{
    var nfse = await factory.CreateAsync(empresaId, cancellationToken);
    var resposta = await nfse.ConsultaChaveAsync(chave, cancellationToken);
    return resposta.Sucesso ? Results.Ok(resposta) : Results.BadRequest(resposta);
});
```

Cada configuração retornada pelo provider deve ser independente, com os dados fiscais da empresa solicitada. O provider de certificado preenche a configuração com o certificado vigente dessa mesma empresa.

---

## 📚 Projeto de Exemplo

O projeto [OpenAC.Net.NFSe.Nacional.Web.Exemplo](https://github.com/OpenAC-Net/OpenAC.Net.NFSe.Nacional/tree/main/src/OpenAC.Net.NFSe.Nacional.Web.Exemplo) demonstra o uso em uma Minimal API, incluindo emissão, download do DANFSe oficial e geração local de PDF.

---

## 🤝 Como Contribuir

Contribuições são muito bem-vindas! Se você deseja relatar um problema, sugerir melhorias ou enviar uma contribuição:

1. Faça um Fork do repositório.
2. Crie uma Branch para a sua feature (`git checkout -b feature/nova-funcionalidade`).
3. Faça o commit das suas alterações (`git commit -m 'Adiciona melhoria na integração Web'`).
4. Envie para o GitHub (`git push origin feature/nova-funcionalidade`).
5. Abra um Pull Request.

---

## 💖 Ajude o Projeto OpenAC .Net

Se o **OpenAC.Net.NFSe.Nacional.Web** é útil para o seu negócio ou projeto open source e você deseja apoiar o desenvolvimento contínuo, considere fazer uma contribuição:

- Participe da nossa comunidade no [Discord](https://discord.com/invite/brdmJ7Yv6w).
- Contribua com código, testes e documentação no [GitHub](https://github.com/OpenAC-Net/OpenAC.Net.NFSe.Nacional).

---

## 📄 Licença

Este projeto é distribuído sob a licença **MIT**. Consulte o arquivo [LICENSE](https://github.com/OpenAC-Net/OpenAC.Net.NFSe.Nacional/blob/main/LICENSE) para mais detalhes.
