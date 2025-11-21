# Configuração de Variáveis de Ambiente (.env)

## 📋 Índice

- [O que é o arquivo .env?](#o-que-é-o-arquivo-env)
- [Por que usar .env?](#por-que-usar-env)
- [Como configurar](#como-configurar)
- [Estrutura do arquivo](#estrutura-do-arquivo)
- [Variáveis disponíveis](#variáveis-disponíveis)
- [Configurações de modelos](#configurações-de-modelos)
- [Tomadores de serviço](#tomadores-de-serviço)
- [Exemplos de uso](#exemplos-de-uso)
- [Tomadores estrangeiros](#tomadores-estrangeiros)
- [Solução de problemas](#solução-de-problemas)
- [Segurança](#segurança)
- [Referências](#referências)

---

## O que é o arquivo .env?

O arquivo `.env` é usado para armazenar **configurações sensíveis** dos testes da biblioteca OpenAC.Net.NFSe.Nacional, como:

- Dados de certificados digitais
- Informações de prestadores de serviço (CNPJ, Inscrição Municipal)
- Dados de tomadores/clientes (endereços, contatos)
- Caminhos de diretórios para salvamento de arquivos

Essas informações **não devem ser commitadas no repositório Git** para proteger dados confidenciais.

---

## Por que usar .env?

✅ **Segurança**: Dados sensíveis ficam fora do código-fonte  
✅ **Flexibilidade**: Cada desenvolvedor pode ter suas próprias configurações locais  
✅ **Organização**: Centraliza todas as configurações em um único arquivo  
✅ **Praticidade**: Facilita a manutenção e troca entre ambientes (dev/homologação/produção)  
✅ **Versionamento**: Apenas o `.env.example` é versionado, nunca os dados reais  

---

## Como configurar

### Passo 1: Criar o arquivo .env

1. Copie o arquivo `.env.example` para `.env`:
```bash
copy .env.example .env
```

2. Ou crie um novo arquivo `.env` na raiz do projeto de testes:
```
OpenAC.Net.NFSe.Nacional.Test\.env
```

### Passo 2: Preencher as variáveis

Edite o arquivo `.env` e substitua os valores de exemplo pelos seus dados reais:

```env
# Exemplo - SUBSTITUA PELOS SEUS DADOS REAIS
NfseModeloAtual.InscricaoFederal=12345678000100
NfseModeloAtual.CertificadoPath=C:\caminho\para\seu\certificado.pfx
NfseModeloAtual.CertificadoSenha=SUA_SENHA_REAL_AQUI
```

### Passo 3: Verificar o .gitignore

Certifique-se de que o arquivo `.env` está no `.gitignore`:

```gitignore
# Environment files
.env
*.env
!.env.example

# Certificados
*.pfx
*.p12
certificados/
```

---

## Estrutura do arquivo

O arquivo `.env` usa a notação **hierárquica com ponto** para organizar as configurações:

```
Prefixo.Propriedade=Valor
```

### Convenções de nomenclatura

- **PascalCase com ponto**: `NfseModeloAtual.CodMunIbge`
- **Sem espaços**: Use apenas letras, números e pontos
- **Comentários**: Linhas iniciadas com `#` são ignoradas
- **Sem aspas desnecessárias**: Não use aspas nos valores
- **Caracteres especiais em senhas**: Não precisa de aspas, ex: `#Fix!Trutle2041`

### Exemplo de estrutura

```env
# Comentário explicativo
Geral.PathSalvar=C:\NFSe\XML

# Configuração do prestador
NfseModeloAtual.CodMunIbge=3550308
NfseModeloAtual.InscricaoFederal=12345678000100

# Dados do tomador
Tomador1.CNPJ=98765432000100
Tomador1.Nome=Empresa Exemplo Ltda
```

---

## Variáveis disponíveis

### 🔧 Configurações Gerais

| Variável | Descrição | Exemplo |
|----------|-----------|---------|
| `Geral.PathSalvar` | Diretório para salvar XMLs gerados pelos testes | `C:\NFSe\XML` |

---

## Configurações de modelos

### 📁 Modelo Atual (v1.00) - Hotelaria

Configurações para testes com a **versão 1.00** do schema NFSe Nacional (setor hoteleiro).

**Usar nos testes:**
```csharp
SetupOpenNFSeNacional.ConfiguracaoModeloAtual(openNFSeNacional);
```

| Variável | Obrigatório | Descrição |
|----------|-------------|-----------|
| `NfseModeloAtual.CodMunIbge` | ✅ | Código IBGE do município (7 dígitos) |
| `NfseModeloAtual.TipoInscricaoFederal` | ✅ | Tipo: `1` (CPF) ou `2` (CNPJ) |
| `NfseModeloAtual.InscricaoFederal` | ✅ | CPF ou CNPJ do prestador (apenas números) |
| `NfseModeloAtual.InscricaoMunicipal` | ✅ | Inscrição Municipal do prestador |
| `NfseModeloAtual.CertificadoPath` | ✅ | Caminho completo do certificado digital (.pfx) |
| `NfseModeloAtual.CertificadoSenha` | ✅ | Senha do certificado digital |

---

### 📁 Modelo Atual 2 (v1.00) - Advocacia

Configurações alternativas para testes com a **versão 1.00** do schema (setor de advocacia).

**Usar nos testes:**
```csharp
SetupOpenNFSeNacional.ConfiguracaoModeloAtual2(openNFSeNacional);
```

| Variável | Obrigatório | Descrição |
|----------|-------------|-----------|
| `NfseModeloAtual2.CodMunIbge` | ✅ | Código IBGE do município |
| `NfseModeloAtual2.TipoInscricaoFederal` | ✅ | Tipo de inscrição federal |
| `NfseModeloAtual2.InscricaoFederal` | ✅ | CPF ou CNPJ do prestador |
| `NfseModeloAtual2.InscricaoMunicipal` | ✅ | Inscrição Municipal |
| `NfseModeloAtual2.CertificadoPath` | ✅ | Caminho do certificado digital |
| `NfseModeloAtual2.CertificadoSenha` | ✅ | Senha do certificado |

---

### 🆕 Modelo Novo Cenário 1 (v1.01)

Configurações para testes com a **versão 1.01** do schema NFSe Nacional.

**Usar nos testes:**
```csharp
SetupOpenNFSeNacional.ConfiguracaoModeloNovoCenario1(openNFSeNacional);
```

| Variável | Obrigatório | Descrição |
|----------|-------------|-----------|
| `NfseModeloNovo1.CodMunIbge` | ✅ | Código IBGE do município |
| `NfseModeloNovo1.TipoInscricaoFederal` | ✅ | Tipo de inscrição federal |
| `NfseModeloNovo1.InscricaoFederal` | ✅ | CPF ou CNPJ do prestador |
| `NfseModeloNovo1.InscricaoMunicipal` | ✅ | Inscrição Municipal |
| `NfseModeloNovo1.CertificadoPath` | ✅ | Caminho do certificado digital |
| `NfseModeloNovo1.CertificadoSenha` | ✅ | Senha do certificado |

---

### 🏛️ Modelo Novo Cenário 2 (v1.01 com IBS/CBS)

Configurações para testes da **Reforma Tributária** (IBS/CBS) com schema v1.01.

**Usar nos testes:**
```csharp
SetupOpenNFSeNacional.ConfiguracaoModeloNovoCenario2(openNFSeNacional);
```

| Variável | Obrigatório | Descrição |
|----------|-------------|-----------|
| `NfseModeloNovo2.CodMunIbge` | ✅ | Código IBGE do município |
| `NfseModeloNovo2.TipoInscricaoFederal` | ✅ | Tipo de inscrição federal |
| `NfseModeloNovo2.InscricaoFederal` | ✅ | CPF ou CNPJ |
| `NfseModeloNovo2.InscricaoMunicipal` | ✅ | Inscrição Municipal |
| `NfseModeloNovo2.CertificadoPath` | ✅ | Caminho do certificado |
| `NfseModeloNovo2.CertificadoSenha` | ✅ | Senha do certificado |

---

## Tomadores de serviço

Configure múltiplos tomadores para diferentes cenários de teste. Existem dois tipos:

1. **Tomadores Brasileiros** (CNPJ): Usam `CodMunicipio` com código IBGE (7 dígitos)
2. **Tomadores Estrangeiros** (NIF): Usam `CodMunicipio=9999999` (código especial)

### 👥 Tomador Padrão (CNPJ Brasil)

Para tomadores brasileiros, use os campos CNPJ e informações de endereço no Brasil.

| Variável | Obrigatório | Descrição |
|----------|-------------|-----------|
| `TomadorN.CNPJ` | ✅ | CNPJ do tomador (apenas números) |
| `TomadorN.Nome` | ✅ | Razão social ou nome do tomador |
| `TomadorN.Email` | ❌ | E-mail de contato |
| `TomadorN.Telefone` | ❌ | Telefone com DDD (ex: 1133334444) |
| `TomadorN.Logradouro` | ✅ | Nome da rua/avenida |
| `TomadorN.Numero` | ✅ | Número do endereço |
| `TomadorN.Complemento` | ❌ | Complemento (sala, bloco, etc.) |
| `TomadorN.Bairro` | ✅ | Bairro |
| `TomadorN.CEP` | ✅ | CEP (8 dígitos, sem hífen) |
| `TomadorN.CodMunicipio` | ✅ | Código IBGE do município (7 dígitos) |

**Exemplo de uso:**
```csharp
// Tomador padrão (Tomador1)
var tomador = SetupOpenNFSeNacional.ObterTomadorPadrao();

// Tomador específico
var tomador2 = SetupOpenNFSeNacional.ObterTomador("2");
var tomador3 = SetupOpenNFSeNacional.ObterTomador("3");

// Obtém apenas o código do município do tomador
var codMunicipio = SetupOpenNFSeNacional.ObterCodMunicipioTomador("2");
```

---

## Tomadores estrangeiros

### 🌍 Configuração de Tomador Estrangeiro

Para tomadores estrangeiros (pessoas jurídicas do exterior), use os campos NIF e informações de endereço no exterior.

**IMPORTANTE:** Tomadores estrangeiros devem ter `CodMunicipio=9999999` (código especial para identificar registros de exterior).

| Variável | Obrigatório | Descrição |
|----------|-------------|-----------|
| `TomadorN.Nif` | ✅ | Número de Identificação Fiscal estrangeiro (sem formatação) |
| `TomadorN.Nome` | ✅ | Razão social da empresa estrangeira |
| `TomadorN.Email` | ❌ | E-mail de contato |
| `TomadorN.Telefone` | ❌ | Telefone internacional |
| `TomadorN.Logradouro` | ✅ | Rua/avenida no exterior |
| `TomadorN.Numero` | ✅ | Número do endereço |
| `TomadorN.Complemento` | ❌ | Complemento (suite, andar, etc.) |
| `TomadorN.Bairro` | ✅ | Bairro/Distrito |
| `TomadorN.CEP` | ✅ | Código postal do país estrangeiro |
| `TomadorN.NomeMunicipio` | ✅ | Cidade/Município no exterior |
| `TomadorN.ISO` | ✅ | Código ISO 3166-1 alpha-2 do país |
| `TomadorN.State` | ✅ | Estado/Província conforme padrão do país |
| `TomadorN.CodMunicipio` | ✅ | Deve ser sempre `9999999` |

### 📍 Códigos ISO Comuns

```
US = Estados Unidos         CA = CanadáFR = França
DE = Alemanha           GB = Reino Unido         IT = Itália
ES = Espanha    NL = Holanda     AU = Austrália
JP = Japão                 CN = China        IN = Índia
MX = México       AR = Argentina           BR = Brasil
```

[Complete list](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2) de códigos ISO.

### ✅ Exemplo Completo: Tomador Estrangeiro

```env
# Tomador 4 - Empresa nos EUA
Tomador4.Nif=12345678
Tomador4.Nome=International Services Corporation
Tomador4.Email=contact@intlservices.com
Tomador4.Telefone=+1-555-0100
Tomador4.Logradouro=Business Avenue
Tomador4.Numero=456
Tomador4.Complemento=Suite 300
Tomador4.Bairro=Financial District
Tomador4.CEP=10001
Tomador4.NomeMunicipio=New York
Tomador4.ISO=US
Tomador4.State=NY
Tomador4.CodMunicipio=9999999

# Tomador 5 - Empresa na Alemanha
Tomador5.Nif=87654321
Tomador5.Nome=European Trade GmbH
Tomador5.Email=info@europtrade.de
Tomador5.Telefone=+49-30-1234567
Tomador5.Logradouro=Handelsstrasse
Tomador5.Numero=123
Tomador5.Complemento=
Tomador5.Bairro=Charlottenburg
Tomador5.CEP=10115
Tomador5.NomeMunicipio=Berlin
Tomador5.ISO=DE
Tomador5.State=Berlin
Tomador5.CodMunicipio=9999999
```

### 🔗 Usando Tomador Estrangeiro nos Testes

```csharp
[TestMethod]
public async Task EmissaoNFSeTomadorEstrangeiro()
{
    var openNFSeNacional = new OpenNFSeNacional();
    SetupOpenNFSeNacional.ConfiguracaoModeloAtual(openNFSeNacional);

    // Obtém tomador estrangeiro (Tomador4)
    var tomadorEstrangeiro = SetupOpenNFSeNacional.ObterTomador("4");

    // Verifica se é estrangeiro
    if (tomadorEstrangeiro.Endereco.Municipio is MunicipioExterior municipioExt)
    {
 // Dados do tomador estrangeiro
        var nif = tomadorEstrangeiro.Nif;
        var pais = municipioExt.CodigoPais;      // ISO
        var estado = municipioExt.EstadoProvincia;
 var cidade = municipioExt.Cidade;
    }

    // Resto do código do teste...
}
```

---

## Exemplos de uso

### Exemplo 1: Teste básico de emissão NFSe com tomador brasileiro

```csharp
[TestMethod]
public async Task EmissaoNFSeBasico()
{
  var openNFSeNacional = new OpenNFSeNacional();
  
    // Carrega configurações do .env (NfseModeloAtual)
    SetupOpenNFSeNacional.ConfiguracaoModeloAtual(
        openNFSeNacional,
 numDps: "1",
      serieDps: "1",
        numEvento: "1"
    );

    var prest = new PrestadorDps
    {
        CNPJ = SetupOpenNFSeNacional.InscricaoFederal,
        Email = "teste@teste.com",
      Regime = new RegimeTributario
   {
            OptanteSimplesNacional = OptanteSimplesNacional.NaoOptante,
            RegimeEspecial = RegimeEspecial.Nenhum
        }
    };

    // Obtém tomador brasileiro do .env
    var tomador = SetupOpenNFSeNacional.ObterTomador("2");

    var servico = new ServicoNFSe
    {
        Localidade = new LocalidadeNFSe
    {
        CodMunicipioPrestacao = tomador.Endereco.Municipio.CodMunicipio
        },
        Informacoes = new InformacoesServico
        {
  CodTributacaoNacional = "010101",
            CodTributacaoMunicipio = "002",
            Descricao = "Serviço de desenvolvimento de software"
  }
    };

    // ... resto do código
    
    var retorno = await openNFSeNacional.EnviarAsync(dps);
    Assert.IsTrue(retorno.Sucesso);
}
```

### Exemplo 2: Teste com tomador estrangeiro

```csharp
[TestMethod]
public async Task EmissaoNFSeComTomadorEstrangeiro()
{
    var openNFSeNacional = new OpenNFSeNacional();
    SetupOpenNFSeNacional.ConfiguracaoModeloAtual(openNFSeNacional, "43", "1", "1");

    var prest = new PrestadorDps
    {
        CNPJ = SetupOpenNFSeNacional.InscricaoFederal,
        Email = "teste@teste.com",
        Regime = new RegimeTributario
   {
       OptanteSimplesNacional = OptanteSimplesNacional.NaoOptante,
    RegimeEspecial = RegimeEspecial.Nenhum
        }
    };

 // Obtém tomador estrangeiro (Tomador4) do .env
    var tomadorExt = SetupOpenNFSeNacional.ObterTomador("4");

    // O tipo pode ser MunicipioExterior para estrangeiros
    if (tomadorExt?.Endereco?.Municipio is MunicipioExterior municipioExt)
    {
        Debug.WriteLine($"Tomador estrangeiro: {tomadorExt.Nome}");
        Debug.WriteLine($"País (ISO): {municipioExt.CodigoPais}");
        Debug.WriteLine($"Estado: {municipioExt.EstadoProvincia}");
        Debug.WriteLine($"Cidade: {municipioExt.Cidade}");
    }

    var servico = new ServicoNFSe
    {
        Localidade = new LocalidadeNFSe
    {
            CodMunicipioPrestacao = SetupOpenNFSeNacional.CodMunIBGE
   },
 Informacoes = new InformacoesServico
        {
 CodTributacaoNacional = "090101",
     CodTributacaoMunicipio = "001",
            Descricao = "Hospedagem internacional"
        },
        ServicoExterior = new ServicoExterior
        {
            Modo = ModoPrestacao.ConsumoBrasil,
            Vinculo = VinculoPrestador.SemVinculo,
     CodMoeda = 986,
         ValorServico = 1000
     }
  };

    // ... resto do código
}
```

### Exemplo 3: Teste com Reforma Tributária (IBS/CBS)

```csharp
[TestMethod]
public async Task EmissaoNFSeComIBSCBS()
{
    var openNFSeNacional = new OpenNFSeNacional();
    
    // Carrega configurações específicas para IBS/CBS (v1.01)
    SetupOpenNFSeNacional.ConfiguracaoModeloNovoCenario2(
 openNFSeNacional,
        numDps: "10",
        serieDps: "1",
        numEvento: "1"
    );

    // ... resto do código
}
```

### Exemplo 4: Acesso direto às propriedades configuradas

```csharp
[TestMethod]
public void TestePropriedadesSetup()
{
 // Após chamar qualquer método de configuração, as propriedades ficam disponíveis:
    SetupOpenNFSeNacional.ConfiguracaoModeloAtual(new OpenNFSeNacional());

    // Acessa propriedades estáticas
    var codMunicipio = SetupOpenNFSeNacional.CodMunIBGE;
    var cnpj = SetupOpenNFSeNacional.InscricaoFederal;
    var inscricaoMunicipal = SetupOpenNFSeNacional.InscricaoMunicipal;
    var numDps = SetupOpenNFSeNacional.NumDPS;
    var serieDps = SetupOpenNFSeNacional.SerieDPS;

    Assert.IsNotNull(cnpj);
    Assert.IsTrue(cnpj.Length == 14 || cnpj.Length == 11);
}
```

---

## Solução de problemas

### ❌ Erro: "Variável de ambiente não encontrada"

**Mensagem completa:**
```
InvalidOperationException: Variável de ambiente 'NfseModeloAtual.CodMunIbge' não encontrada.
Certifique-se de que o arquivo .env existe e contém esta configuração.
```

**Causa**: A variável não existe no arquivo `.env` ou está com nome incorreto.

**Solução**:
1. Verifique se o arquivo `.env` existe em `OpenAC.Net.NFSe.Nacional.Test\.env`
2. Certifique-se de que a variável está escrita corretamente (case-sensitive)
3. Verifique se não há espaços extras antes/depois do sinal de igual `=`
4. Confirme que a linha não está comentada com `#`

**Exemplo correto:**
```env
# ✅ Correto
NfseModeloAtual.CodMunIbge=3550308

# ❌ Errado - espaços extras
NfseModeloAtual.CodMunIbge = 3550308

# ❌ Errado - aspas desnecessárias
NfseModeloAtual.CodMunIbge="3550308"

# ❌ Errado - comentado
# NfseModeloAtual.CodMunIbge=3550308
```

---

### ❌ Erro: "Arquivo de certificado não encontrado"

**Causa**: O caminho do certificado em `CertificadoPath` está incorreto ou o arquivo não existe.

**Solução**:
1. Verifique se o caminho está correto e use caminho absoluto
2. Certifique-se de que o arquivo `.pfx` existe no local especificado
3. Verifique as permissões de leitura do arquivo

**Exemplos válidos:**
```
# Windows - barra invertida simples
NfseModeloAtual.CertificadoPath=C:\certificados\meu_cert.pfx

# Windows - barra invertida dupla (também funciona)
NfseModeloAtual.CertificadoPath=C:\\certificados\\meu_cert.pfx

# Caminho relativo (não recomendado)
NfseModeloAtual.CertificadoPath=..\..\certificados\meu_cert.pfx
```

---

### ❌ Erro: "Senha do certificado inválida"

**Causa**: A senha em `CertificadoSenha` está incorreta.

**Solução**:
1. Verifique a senha - ela é case-sensitive
2. Certifique-se de que não há espaços extras no início ou fim
3. Se a senha tiver caracteres especiais, não use aspas

**Exemplos:**
```
# ✅ Correto - senha simples
NfseModeloAtual.CertificadoSenha=minhaSenha123

# ✅ Correto - senha com caracteres especiais
NfseModeloAtual.CertificadoSenha=Senh@_#2024!Especial

# ❌ Errado - aspas desnecessárias
NfseModeloAtual.CertificadoSenha="minhaSenha123"

# ❌ Errado - espaços extras
NfseModeloAtual.CertificadoSenha= minhaSenha123 
```

---

### ⚠️ Arquivo .env não está sendo carregado

**Causa**: O arquivo não está no diretório correto ou não foi copiado para o diretório de build.

**Solução**:

O arquivo `.env` deve estar em um destes locais:

1. **Raiz do projeto de testes** (recomendado):
```
   OpenAC.Net.NFSe.Nacional.Test\.env
```

2. **Diretório de saída da build**:
```
   OpenAC.Net.NFSe.Nacional.Test\bin\Debug\net9.0\.env
```

**Como garantir que o arquivo seja copiado:**

Edite o `.csproj` e adicione:
```xml
<ItemGroup>
  <None Update=".env">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

---

### ❌ Erro: "CNPJ inválido" ou "CEP inválido"

**Causa**: Formato incorreto de CNPJ, CPF ou CEP.

**Solução**:
Todos os campos numéricos devem conter **apenas números**, sem pontos, hífens ou barras.

**Exemplos corretos:**
```
# ✅ CNPJ correto (14 dígitos)
Tomador1.CNPJ=12345678000100

# ❌ CNPJ errado (com formatação)
Tomador1.CNPJ=12.345.678/0001-00

# ✅ CEP correto (8 dígitos)
Tomador1.CEP=01310100

# ❌ CEP errado (com hífen)
Tomador1.CEP=01310-100

# ✅ Código IBGE correto (7 dígitos)
Tomador1.CodMunicipio=3550308

# ❌ Código IBGE errado (incompleto)
Tomador1.CodMunicipio=355030
```

---

### ❌ Erro ao usar tomador estrangeiro

**Causa**: Configuração incorreta do tomador estrangeiro.

**Solução**:
```env
# ✅ CORRETO - Para estrangeiro, SEMPRE use CodMunicipio=9999999
Tomador4.CodMunicipio=9999999

# ✅ Use NIF em vez de CNPJ
Tomador4.Nif=12345678

# ✅ Configure ISO e State
Tomador4.ISO=US
Tomador4.State=NY

# ✅ Use NomeMunicipio em vez de deixar vazio
Tomador4.NomeMunicipio=New York

# ❌ ERRADO - Usar código IBGE para estrangeiro
Tomador4.CodMunicipio=3550308

# ❌ ERRADO - Misturar CNPJ e NIF
Tomador4.CNPJ=12345678000100
Tomador4.Nif=12345678
```

---

### 🔍 Como debugar problemas com .env

1. **Verificar se o arquivo existe:**
```csharp
   var envPath = Path.Combine(AppContext.BaseDirectory, ".env");
   Console.WriteLine($"Procurando .env em: {envPath}");
   Console.WriteLine($"Arquivo existe: {File.Exists(envPath)}");
```

2. **Listar variáveis carregadas:**
```csharp
   static SetupOpenNFSeNacional()
   {
       var envPath = Path.Combine(AppContext.BaseDirectory, ".env");
       if (File.Exists(envPath))
       {
           Env.Load(envPath);
           Console.WriteLine("Arquivo .env carregado com sucesso!");
  }
       else
    {
       Console.WriteLine($"AVISO: Arquivo .env não encontrado em: {envPath}");
       }
   }
```

3. **Verificar valor específico:**
```csharp
   var valor = Env.GetString("NfseModeloAtual.CodMunIbge");
   Console.WriteLine($"Valor lido: '{valor}'");
   Console.WriteLine($"É nulo ou vazio: {string.IsNullOrWhiteSpace(valor)}");
```

---

## 🔒 Segurança

### ⚠️ REGRAS IMPORTANTES

1. **NUNCA** commite o arquivo `.env` no Git
2. **NUNCA** compartilhe senhas de certificados
3. **SEMPRE** use certificados de teste/homologação em desenvolvimento
4. **SEMPRE** mantenha o `.env` no `.gitignore`
5. **NUNCA** exponha dados reais em capturas de tela ou logs públicos
6. **USE** dados fictícios no `.env.example`

### Verificando segurança

```bash
# Verifica se .env está no .gitignore
git check-ignore .env

# Deve retornar: .env (se estiver correto)

# Verifica se .env está sendo rastreado (não deveria estar)
git ls-files | findstr ".env"

# Não deve retornar nada (exceto .env.example)
```

### Removendo .env do histórico do Git (se foi commitado por engano)

```bash
# Remove do histórico
git filter-branch --force --index-filter \
  "git rm --cached --ignore-unmatch .env" \
  --prune-empty --tag-name-filter cat -- --all

# Force push (CUIDADO: isso reescreve o histórico!)
git push origin --force --all
```

### Boas práticas

1. **Use .env.example como template**
   - Mantenha `.env.example` atualizado
   - Documente novas variáveis
   - **NÃO coloque valores reais**

2. **Rotacione credenciais regularmente**
   - Troque senhas periodicamente
   - Revogue certificados antigos
- Atualize o `.env` local

3. **Ambiente de CI/CD**
   - Use secrets do GitHub/Azure DevOps
   - Não coloque `.env` no repositório
   - Configure variáveis de ambiente direto no CI

---

## 📚 Referências

- [DotNetEnv - Biblioteca usado