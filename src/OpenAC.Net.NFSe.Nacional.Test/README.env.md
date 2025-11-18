# Configuração de Variáveis de Ambiente (.env)

## 📋 Índice

- [O que é o arquivo .env?](#o-que-é-o-arquivo-env)
- [Por que usar .env?](#por-que-usar-env)
- [Como configurar](#como-configurar)
- [Estrutura do arquivo](#estrutura-do-arquivo)
- [Variáveis disponíveis](#variáveis-disponíveis)
- [Exemplos de uso](#exemplos-de-uso)
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
# Exemplo
NfseModeloAtual.InscricaoFederal=SEU_CNPJ_AQUI
NfseModeloAtual.CertificadoPath=C:\caminho\para\seu\certificado.pfx
NfseModeloAtual.CertificadoSenha=SUA_SENHA_AQUI
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

```env
Prefixo.Propriedade=Valor
```

### Convenções de nomenclatura

- **PascalCase com ponto**: `NfseModeloAtual.CodMunIbge`
- **Sem espaços**: Use apenas letras, números e pontos
- **Comentários**: Linhas iniciadas com `#` são ignoradas
- **Sem aspas**: Não use aspas nos valores (exceto se fizer parte do valor)

### Exemplo de estrutura

```env
# Comentário explicativo
Geral.PathSalvar=C:\NFSe\XML

# Configuração do prestador
NfseModeloAtual.CodMunIbge=3304557
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

### 📁 Modelo Atual (v1.00)

Configurações para testes com a **versão 1.00** do schema NFSe Nacional.

| Variável | Obrigatório | Descrição |
|----------|-------------|-----------|
| `NfseModeloAtual.CodMunIbge` | ✅ | Código IBGE do município (7 dígitos) |
| `NfseModeloAtual.TipoInscricaoFederal` | ✅ | Tipo: `1` (CPF) ou `2` (CNPJ) |
| `NfseModeloAtual.InscricaoFederal` | ✅ | CPF ou CNPJ do prestador (apenas números) |
| `NfseModeloAtual.InscricaoMunicipal` | ✅ | Inscrição Municipal do prestador |
| `NfseModeloAtual.CertificadoPath` | ✅ | Caminho completo do certificado digital (.pfx) |
| `NfseModeloAtual.CertificadoSenha` | ✅ | Senha do certificado digital |

**Uso nos testes:**
```csharp
SetupOpenNFSeNacional.ConfiguracaoModeloAtual(openNFSeNacional);
```

---

### 🆕 Modelo Novo Cenário 1 (v1.01)

Configurações para testes com a **versão 1.01** do schema NFSe Nacional.

| Variável | Obrigatório | Descrição |
|----------|-------------|-----------|
| `NfseModeloNovo1.CodMunIbge` | ✅ | Código IBGE do município |
| `NfseModeloNovo1.TipoInscricaoFederal` | ✅ | Tipo de inscrição federal |
| `NfseModeloNovo1.InscricaoFederal` | ✅ | CPF ou CNPJ do prestador |
| `NfseModeloNovo1.InscricaoMunicipal` | ✅ | Inscrição Municipal |
| `NfseModeloNovo1.CertificadoPath` | ✅ | Caminho do certificado digital |
| `NfseModeloNovo1.CertificadoSenha` | ✅ | Senha do certificado |

**Uso nos testes:**
```csharp
SetupOpenNFSeNacional.ConfiguracaoModeloNovoCenario1(openNFSeNacional);
```

---

### 🏛️ Modelo Novo Cenário 2 (v1.01 com IBS/CBS)

Configurações para testes da **Reforma Tributária** (IBS/CBS) com schema v1.01.

| Variável | Obrigatório | Descrição |
|----------|-------------|-----------|
| `NfseModeloNovo2.CodMunIbge` | ✅ | Código IBGE do município |
| `NfseModeloNovo2.TipoInscricaoFederal` | ✅ | Tipo de inscrição federal |
| `NfseModeloNovo2.InscricaoFederal` | ✅ | CPF ou CNPJ |
| `NfseModeloNovo2.InscricaoMunicipal` | ✅ | Inscrição Municipal |
| `NfseModeloNovo2.CertificadoPath` | ✅ | Caminho do certificado |
| `NfseModeloNovo2.CertificadoSenha` | ✅ | Senha do certificado |

**Uso nos testes:**
```csharp
SetupOpenNFSeNacional.ConfiguracaoModeloNovoCenario2(openNFSeNacional);
```

---

### 👥 Tomadores de Serviço

Configure múltiplos tomadores para diferentes cenários de teste.

#### Tomador 1 (Padrão)

| Variável | Obrigatório | Descrição |
|----------|-------------|-----------|
| `Tomador1.CNPJ` | ✅ | CNPJ do tomador (apenas números) |
| `Tomador1.Nome` | ✅ | Razão social ou nome do tomador |
| `Tomador1.Email` | ❌ | E-mail de contato |
| `Tomador1.Telefone` | ❌ | Telefone com DDD (ex: 2133334444) |
| `Tomador1.Logradouro` | ✅ | Nome da rua/avenida |
| `Tomador1.Numero` | ✅ | Número do endereço |
| `Tomador1.Complemento` | ❌ | Complemento (sala, bloco, etc.) |
| `Tomador1.Bairro` | ✅ | Bairro |
| `Tomador1.CEP` | ✅ | CEP (8 dígitos, sem hífen) |
| `Tomador1.CodMunicipio` | ✅ | Código IBGE do município (7 dígitos) |

#### Tomadores adicionais (2, 3, 4...)

Você pode adicionar quantos tomadores precisar, apenas alterando o número:
- `Tomador2.CNPJ`, `Tomador2.Nome`, `Tomador2.Logradouro`, etc.
- `Tomador3.CNPJ`, `Tomador3.Nome`, `Tomador3.Logradouro`, etc.

**Uso nos testes:**
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

## Exemplos de uso

### Exemplo 1: Teste básico de emissão NFSe

```csharp
[TestMethod]
public async Task EmissaoNFSe()
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

    // Obtém tomador do .env
    var tomador = SetupOpenNFSeNacional.ObterTomador("2");

    var servico = new ServicoNFSe
    {
        Localidade = new LocalidadeNFSe
        {
            // Usa o município do tomador
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

### Exemplo 2: Teste com Reforma Tributária (IBS/CBS)

```csharp
[TestMethod]
public async Task EmissaoNFSeComIBSCBS_AliquotaReduzida()
{
    var openNFSeNacional = new OpenNFSeNacional();
    
    // Carrega configurações específicas para IBS/CBS (v1.01)
    SetupOpenNFSeNacional.ConfiguracaoModeloNovoCenario2(
      openNFSeNacional,
   numDps: "10",
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
            RegimeEspecial = RegimeEspecial.SociedadeProfissionais
        }
    };

    // Obtém o Tomador1 (padrão) do .env
    var tomador = SetupOpenNFSeNacional.ObterTomadorPadrao();

    var servico = new ServicoNFSe
    {
 Localidade = new LocalidadeNFSe
        {
            // Município do tomador é usado automaticamente
      CodMunicipioPrestacao = tomador.Endereco.Municipio.CodMunicipio
        },
 Informacoes = new InformacoesServico
        {
            CodNBS = "113011000",
CodTributacaoNacional = "171401",
    CodTributacaoMunicipio = "001",
            Descricao = "Honorários advocatícios"
        }
    };

    var ibscbs = new RTCInfoIBSCBS
    {
      FinalidadeNFSe = RTCFinNFSe.Regular,
   IndicadorUsoFinal = RTCIndFinal.Nao,
        CodigoIndicadorOperacao = "100301",
        IndicadorDestinatario = RTCIndDest.ProprioTomador,
        // ... configurações IBS/CBS
    };

    // ... resto do código

    var retorno = await openNFSeNacional.EnviarAsync(dps);
    Assert.IsTrue(retorno.Sucesso, "A emissão deveria ter sucesso");
}
```

### Exemplo 3: Múltiplos tomadores no mesmo teste

```csharp
[TestMethod]
public async Task EmissaoComDestinatarioDiferente()
{
    var openNFSeNacional = new OpenNFSeNacional();
    SetupOpenNFSeNacional.ConfiguracaoModeloNovoCenario1(openNFSeNacional);

    // Tomador principal (quem contrata o serviço)
    var tomador = SetupOpenNFSeNacional.ObterTomador("2");
    
    // Destinatário diferente (quem recebe/consome o serviço)
    var destinatario = SetupOpenNFSeNacional.ObterTomador("3");

    var dps = new Dps
    {
     Versao = "1.01",
  Informacoes = new InfDps
        {
      // ... configuração do DPS
  Tomador = tomador,
   IBSCBS = new RTCInfoIBSCBS
            {
      IndicadorDestinatario = RTCIndDest.Outro,
          // Converte InfoPessoaNFSe para RTCInfoDest
       Destinatario = destinatario.ToRTCInfoDest(),
          // ... resto da configuração
            }
        }
};

    var retorno = await openNFSeNacional.EnviarAsync(dps);
Assert.IsTrue(retorno.Sucesso);
}
```

### Exemplo 4: Acessando propriedades específicas

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
NfseModeloAtual.CodMunIbge=3304557

# ❌ Errado - espaços extras
NfseModeloAtual.CodMunIbge = 3304557

# ❌ Errado - aspas desnecessárias
NfseModeloAtual.CodMunIbge="3304557"

# ❌ Errado - comentado
# NfseModeloAtual.CodMunIbge=3304557
```

---

### ❌ Erro: "Arquivo de certificado não encontrado"

**Causa**: O caminho do certificado em `CertificadoPath` está incorreto ou o arquivo não existe.

**Solução**:
1. Verifique se o caminho está correto e use caminho absoluto
2. Certifique-se de que o arquivo `.pfx` existe no local especificado
3. Verifique as permissões de leitura do arquivo

**Exemplos válidos:**
```env
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
```env
# ✅ Correto - senha simples
NfseModeloAtual.CertificadoSenha=minhaSenha123

# ✅ Correto - senha com caracteres especiais
NfseModeloAtual.CertificadoSenha=#Fix!Trutle2041

# ❌ Errado - aspas desnecessárias
NfseModeloAtual.CertificadoSenha="#Fix!Trutle2041"

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
```env
# ✅ CNPJ correto (14 dígitos)
Tomador1.CNPJ=12345678000100

# ❌ CNPJ errado (com formatação)
Tomador1.CNPJ=12.345.678/0001-00

# ✅ CEP correto (8 dígitos)
Tomador1.CEP=20220410

# ❌ CEP errado (com hífen)
Tomador1.CEP=20220-410

# ✅ Código IBGE correto (7 dígitos)
Tomador1.CodMunicipio=3304557

# ❌ Código IBGE errado (incompleto)
Tomador1.CodMunicipio=330455
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
   - Não coloque valores reais

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

- [DotNetEnv - Biblioteca usada para carregar .env](https://github.com/tonerdo/dotnet-env)
- [NFSe Nacional - Documentação oficial](https://nfse.svrs.rs.gov.br/)
- [OpenAC.Net.NFSe.Nacional - Repositório GitHub](https://github.com/OpenAC-Net/OpenAC.Net.NFSe.Nacional)
- [Especificação NFSe v1.00 - Manual técnico](https://nfse.svrs.rs.gov.br/ws/documentacao.php)
- [Especificação NFSe v1.01 - Reforma Tributária](https://nfse.svrs.rs.gov.br/ws/documentacao.php)

---

## 🤝 Contribuindo

Se encontrar problemas, tiver sugestões ou melhorias para este documento:

1. Abra uma [issue](https://github.com/OpenAC-Net/OpenAC.Net.NFSe.Nacional/issues) no repositório
2. Envie um [pull request](https://github.com/OpenAC-Net/OpenAC.Net.NFSe.Nacional/pulls) com correções
3. Entre em contato com a comunidade OpenAC.Net

---

## 📄 Licença

Este projeto é licenciado sob a [MIT License](https://github.com/OpenAC-Net/OpenAC.Net.NFSe.Nacional/blob/main/LICENSE).

---

**Última atualização:** Janeiro/2025  
**Versão do documento:** 2.0