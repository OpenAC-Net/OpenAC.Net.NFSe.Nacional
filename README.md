# OpenAC.Net.NFSe.Nacional

[![NuGet Version](https://img.shields.io/nuget/v/OpenAC.Net.NFSe.Nacional.svg)](https://www.nuget.org/packages/OpenAC.Net.NFSe.Nacional/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/OpenAC.Net.NFSe.Nacional.svg)](https://www.nuget.org/packages/OpenAC.Net.NFSe.Nacional/)
[![Discord](https://img.shields.io/badge/Chat%20on-Discord-purple.svg)](https://discord.com/invite/brdmJ7Yv6w)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![Target Frameworks](https://img.shields.io/badge/.NET%20Framework-4.6.2%20%7C%204.8-blue.svg)](https://dotnet.microsoft.com/)
[![Target Frameworks](https://img.shields.io/badge/.NET%20Core%20%2F%20Standard-netstandard2.0%20%7C%20net8.0%20%7C%20net9.0%20%7C%20net10.0-blue.svg)](https://dotnet.microsoft.com/)

Biblioteca .NET multiplataforma para geração, assinatura, validação, transmissão e consulta de **NFS-e (Nota Fiscal de Serviço Eletrônica) Padrão Nacional**.

Desenvolvida pelo time **OpenAC .Net**, a biblioteca adere às especificações oficiais do Sistema Nacional da NFS-e (Sefin / Receita Federal) e suporta múltiplos provedores e layouts.

---

## 🚀 Funcionalidades

- **Emissão e Transmissão de DPS** (Documento de Prestação de Serviços):
  - Emissão Síncrona e Assíncrona.
  - Envio e consulta de lote de DPS (quando suportado pelo provedor).
- **Eventos da NFS-e**:
  - Cancelamento e Solicitação de Cancelamento.
  - Cancelamento por Substituição.
  - Confirmação pelo Prestador, Tomador e Intermediário.
  - Rejeição pelo Prestador, Tomador e Intermediário.
  - Bloqueio e Desbloqueio de Ofício.
- **Consultas e Distribuição (ADN)**:
  - Consulta de DF-e por NSU (*Número Sequencial Único*).
  - Consulta de DF-e por Chave de Acesso.
  - Consulta de Chave de Acesso por Id da DPS.
  - Verificação de existência da DPS.
  - Consulta de situação de lote de DPS.
- **Download do DANFSe**:
  - Obtenção do DANFSe em formato PDF (via webservice).
- **Assinatura e Validação Digital**:
  - Assinatura digital XML com suporte a certificados A1 e A3 (`DFeSignature`).
  - Validação contra os schemas oficiais XSD.
- **Alta Performance**:
  - Serialização e desserialização XML baseada em *Source Generators* (`OpenAC.Net.DFe.Core`).
- **Suporte aos novos layouts**:
  - Schemas **v1.00** e **v1.01**.
  - Suporte ao formato de **CNPJ Alfanumérico**.
  - Campos da **Reforma Tributária (IBS / CBS)**.

---

## 🏛️ Provedores Suportados

A biblioteca abstrai a comunicação com diferentes provedores e sistemas de integração da NFS-e Nacional:

- **Nacional** (Ambiente Nacional / API REST & ADN)
- **Fiorilli**
- **ISSNet**
- **SimplISS**
- **Tiplan**

---

## 📦 Instalação

Adicione o pacote via NuGet Package Manager ou CLI:

### .NET CLI:
```bash
dotnet add package OpenAC.Net.NFSe.Nacional
```

### Package Manager Console:
```powershell
Install-Package OpenAC.Net.NFSe.Nacional
```

### PackageReference:
```xml
<PackageReference Include="OpenAC.Net.NFSe.Nacional" Version="1.4.8" />
```

---

## 🛠️ Como Usar

### 1. Inicializando e Configurando o Componente

```csharp
using System.Security.Cryptography.X509Certificates;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Nacional;
using OpenAC.Net.NFSe.Nacional.Common.Types;

var nfse = new OpenNFSeNacional();

// Configurações Gerais
nfse.Configuracoes.Geral.Ambiente = DFeTipoAmbiente.Homologacao;
nfse.Configuracoes.Geral.VersaoDps = VersaoNFSe.Ve101; // ou Ve100
nfse.Configuracoes.Geral.RetirarAcentos = true;
nfse.Configuracoes.Geral.Salvar = true;
nfse.Configuracoes.Geral.CodigoMunicipio = 3550308; // Código IBGE do município

// Configurações de Arquivos/Diretórios
nfse.Configuracoes.Arquivos.PathSalvar = @"C:\NFSe\Salvar";
nfse.Configuracoes.Arquivos.PathSchemas = @"C:\NFSe\Schemas";

// Configurações do Certificado Digital (exemplo com arquivo PFX)
nfse.Configuracoes.Certificados.Certificado = @"C:\Certificados\certificado.pfx";
nfse.Configuracoes.Certificados.Senha = "123456";
```

---

### 2. Criando e Emitindo uma DPS

```csharp
using OpenAC.Net.NFSe.Nacional.Common.Model;
using OpenAC.Net.NFSe.Nacional.Common.Types;

var dps = new Dps
{
    Versao = VersaoNFSe.Ve101,
    Informacoes = new InfDps
    {
        TipoAmbiente = DFeTipoAmbiente.Homologacao,
        DhEmissao = DateTime.Now,
        LocalidadeEmitente = "3550308",
        Serie = "1",
        NumeroDps = "100",
        Competencia = DateTime.Today,
        TipoEmitente = EmitenteDps.Prestador,
        Prestador = new PrestadorDps
        {
            CNPJ = "12345678000195",
            Email = "prestador@empresa.com",
            Regime = new RegimeTributario
            {
                OptanteSimplesNacional = OptanteSimplesNacional.NaoOptante,
                RegimeEspecial = RegimeEspecial.Nenhum
            }
        },
        Tomador = new InfoPessoaNFSe
        {
            CNPJ = "98765432000100",
            Nome = "Cliente Tomador de Servicos Ltda",
            Endereco = new EnderecoNFSe
            {
                Logradouro = "Avenida Brasil",
                Numero = "1500",
                Bairro = "Centro",
                Municipio = new MunicipioNacional
                {
                    CodMunicipio = "3550308",
                    CEP = "01001000"
                }
            }
        },
        Servico = new ServicoNFSe
        {
            Localidade = new LocalidadeNFSe
            {
                CodMunicipioPrestacao = "3550308"
            },
            Informacoes = new InformacoesServico
            {
                CodTributacaoNacional = "010101",
                CodTributacaoMunicipio = "001",
                Descricao = "Prestação de serviços de desenvolvimento de software."
            }
        },
        Valores = new ValoresDps
        {
            ValoresServico = new ValoresServico
            {
                Valor = 1500.00m
            },
            Tributos = new TributosNFSe
            {
                Municipal = new TributoMunicipal
                {
                    ISSQN = TributoISSQN.OperacaoTributavel,
                    TipoRetencaoISSQN = TipoRetencaoISSQN.NaoRetido
                }
            }
        }
    }
};

// Assinar e Enviar
dps.Assinar(nfse.Configuracoes);
var retorno = await nfse.EnviarAsync(dps);

if (retorno.Sucesso)
{
    Console.WriteLine($"NFS-e emitida com sucesso! Chave: {retorno.Resultado?.ChaveAcesso}");
}
else
{
    Console.WriteLine($"Erro ao emitir NFS-e: {retorno.Erros}");
}
```

---

### 3. Consultas e Download do DANFSe

```csharp
// Consulta de DF-e pela chave de acesso
var respostaConsulta = await nfse.ConsultaChaveAsync("35230912345678000195560010000000010012345678");

// Download do DANFSe em PDF
byte[] pdfBytes = await nfse.DownloadDANFSeAsync("35230912345678000195560010000000010012345678");
await File.WriteAllBytesAsync(@"C:\NFSe\Danfse.pdf", pdfBytes);

// Consulta por NSU
var respostaNsu = await nfse.ConsultaNsuAsync(1234);
```

---

### 4. Cancelamento de NFS-e (Registro de Evento)

```csharp
var evento = new PedidoRegistroEvento
{
    Versao = VersaoNFSe.Ve100,
    Informacoes = new InfPedReg
    {
        TipoAmbiente = DFeTipoAmbiente.Homologacao,
        DhEvento = DateTime.Now,
        ChaveAcesso = "35230912345678000195560010000000010012345678",
        Autor = new AutorEvento { CNPJ = "12345678000195" },
        Evento = new EventoCancelamento
        {
            CodigoMotivo = MotivoCancelamento.ErroEmissao,
            DescricaoMotivo = "Erro na descrição do serviço."
        }
    }
};

evento.Assinar(nfse.Configuracoes);
var retornoEvento = await nfse.EnviarEventoAsync(evento);
```

---

## ⚙️ Gerenciador de Municípios

A biblioteca conta com uma base interna de municípios integrados (`Municipios.nfse`), gerenciada pela classe `NFSeServiceManager`. É possível consultar ou registrar novos provedores e URLs em tempo de execução:

```csharp
var manager = NFSeServiceManager.Instance;
var provedor = manager.GetProvider(nfse.Configuracoes);
```

---

## 🤝 Como Contribuir

Contribuições são muito bem-vindas! Se você deseja relatar um problema, sugerir melhorias ou enviar uma contribuição:

1. Faça um Fork do repositório.
2. Crie uma Branch para a sua feature (`git checkout -b feature/nova-funcionalidade`).
3. Faça o commit das suas alterações (`git commit -m 'Adiciona suporte a novo provedor'`).
4. Envie para o GitHub (`git push origin feature/nova-funcionalidade`).
5. Abra um Pull Request.

---

## 💖 Ajude o Projeto OpenAC .Net

Se o **OpenAC.Net.NFSe.Nacional** é útil para o seu negócio ou projeto open source e você deseja apoiar o desenvolvimento contínuo, considere fazer uma contribuição:

- Participe da nossa comunidade no [Discord](https://discord.com/invite/brdmJ7Yv6w).
- Contribua com código, testes e documentação no [GitHub](https://github.com/OpenAC-Net/OpenAC.Net.NFSe.Nacional).

---

## 📄 Licença

Este projeto é distribuído sob a licença **MIT**. Consulte o arquivo [LICENSE](LICENSE) para mais detalhes.
