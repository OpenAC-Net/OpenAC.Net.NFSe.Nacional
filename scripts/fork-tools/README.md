# 🔧 Fork Tools - OpenAC.Net.NFSe.Nacional

Scripts automatizados para facilitar o gerenciamento do fork e contribuições ao projeto upstream.

## 📖 Índice

- [Quick Start](#-quick-start)
- [Workflows Comuns](#-workflows-comuns)
- [Referência de Scripts](#-referência-de-scripts)
- [FAQ](#-faq)
- [Troubleshooting](#-troubleshooting)

## 🚀 Quick Start

### Linux/Mac

```bash
# 1. Configuração inicial (executar uma vez)
cd scripts/fork-tools
./setup-fork.sh

# 2. (Opcional) Instalar aliases Git
./install-aliases.sh

# 3. Criar uma feature para contribuir ao upstream
./new-feature.sh minha-feature

# 4. Fazer suas alterações
git add .
git commit -m "feat: descrição da mudança"

# 5. Validar antes de abrir PR
./check-pr.sh

# 6. Push e abrir PR
git push origin feature/minha-feature
```

### Windows (PowerShell)

```powershell
# 1. Configuração inicial (executar uma vez)
cd scripts\fork-tools
.\setup-fork.ps1

# 2. (Opcional) Instalar aliases Git
bash scripts\fork-tools\install-aliases.sh
# Nota: Requer Git Bash no Windows

# 3. Criar uma feature para contribuir ao upstream
.\new-feature.ps1 minha-feature

# 4. Fazer suas alterações
git add .
git commit -m "feat: descrição da mudança"

# 5. Validar antes de abrir PR
.\check-pr.ps1

# 6. Push e abrir PR
git push origin feature/minha-feature
```

### Com Aliases Git (após instalação)

```bash
# De qualquer lugar no repositório:
git fork-setup              # Configurar fork (primeira vez)
git fork-feature minha-feat # Criar feature limpa
git fork-check              # Validar antes de PR
git fork-sync               # Sincronizar com upstream
```

## 📋 Workflows Comuns

### 1. Contribuir para o Upstream

Este é o workflow principal para enviar melhorias ao projeto original:

```bash
# Criar branch limpa baseada no upstream
cd scripts/fork-tools
./new-feature.sh corrigir-bug-123

# Fazer alterações
git add .
git commit -m "fix: corrigir bug na validação"

# Validar que não há arquivos do fork
./check-pr.sh

# Enviar
git push origin feature/corrigir-bug-123
# Abrir PR no GitHub: base=OpenAC-Net/main, compare=seu-fork/feature/corrigir-bug-123
```

### 2. Sincronizar Fork com Upstream

Manter seu fork atualizado com as últimas mudanças do projeto original:

```bash
cd scripts/fork-tools
./sync-fork.sh
```

Isso irá:
- ✅ Atualizar `main` com `upstream/main`
- ✅ Mesclar mudanças de `main` em `fork-config`
- ✅ Fazer push das atualizações

### 3. Trabalhar em Customizações do Fork

Para features específicas do seu fork que não devem ir para o upstream:

```bash
# Trabalhar na branch fork-config
git checkout fork-config

# Fazer alterações específicas do fork
# (workflows personalizados, configurações, etc.)
git add .
git commit -m "chore: adicionar workflow customizado"
git push origin fork-config
```

### 4. Validar PR Antes de Enviar

Sempre validar antes de abrir um PR para o upstream:

```bash
cd scripts/fork-tools
./check-pr.sh
```

O script irá:
- ❌ Detectar arquivos de `.github/workflows/`
- ❌ Detectar arquivos de `scripts/fork-tools/`
- ✅ Mostrar todos os arquivos que serão incluídos no PR
- ✅ Confirmar que está seguro para enviar

## 📚 Referência de Scripts

### setup-fork.sh / setup-fork.ps1

**Propósito:** Configuração inicial do fork (executar uma única vez).

**O que faz:**
- ✅ Adiciona remote `upstream` apontando para `OpenAC-Net/OpenAC.Net.NFSe.Nacional`
- ✅ Cria branch `fork-config` para customizações específicas do fork
- ✅ Faz fetch inicial do upstream

**Uso:**
```bash
# Linux/Mac
./setup-fork.sh

# Windows
.\setup-fork.ps1
```

**Quando usar:**
- Na primeira vez que clonar o fork
- Para configurar um novo ambiente de desenvolvimento

---

### new-feature.sh / new-feature.ps1

**Propósito:** Criar uma feature branch limpa baseada no `upstream/main`.

**O que faz:**
- ✅ Sincroniza `main` com `upstream/main`
- ✅ Cria nova branch `feature/<nome>` a partir de `upstream/main` (limpa)
- ✅ Garante que a branch não contém customizações do fork

**Uso:**
```bash
# Linux/Mac
./new-feature.sh nome-da-feature

# Windows
.\new-feature.ps1 nome-da-feature

# Exemplos
./new-feature.sh corrigir-validacao
./new-feature.sh adicionar-testes-unitarios
```

**Quando usar:**
- Sempre que for contribuir com código para o upstream
- Para garantir que o PR não inclui arquivos específicos do fork

---

### sync-fork.sh / sync-fork.ps1

**Propósito:** Sincronizar o fork com as últimas mudanças do upstream.

**O que faz:**
- ✅ Atualiza `main` com `upstream/main`
- ✅ Mescla mudanças de `main` em `fork-config`
- ✅ Faz push de ambas as branches

**Uso:**
```bash
# Linux/Mac
./sync-fork.sh

# Windows
.\sync-fork.ps1
```

**Quando usar:**
- Regularmente, para manter o fork atualizado
- Antes de criar uma nova feature branch
- Após mudanças significativas no upstream

**Nota:** Se houver conflitos na merge de `fork-config`, o script irá pausar para resolução manual.

---

### check-pr.sh / check-pr.ps1

**Propósito:** Validar que a branch está pronta para PR ao upstream.

**O que faz:**
- ❌ Detecta modificações em `.github/workflows/`
- ❌ Detecta modificações em `scripts/fork-tools/`
- ✅ Lista todos os arquivos que serão incluídos no PR
- ✅ Confirma se está seguro para abrir PR

**Uso:**
```bash
# Linux/Mac
./check-pr.sh

# Windows
.\check-pr.ps1
```

**Quando usar:**
- **SEMPRE** antes de abrir um PR para o upstream
- Para verificar o que será incluído no PR
- Quando tiver dúvidas sobre quais arquivos foram modificados

**Saída de Exemplo:**

✅ **Sucesso:**
```
✅ Nenhum arquivo específico do fork detectado!

📄 Arquivos que serão incluídos no PR:
   ➕ src/NovoArquivo.cs
   📝 src/ArquivoModificado.cs

✅ Seguro para abrir PR para upstream!
```

❌ **Erro:**
```
⚠️  WORKFLOWS DETECTADOS:
   .github/workflows/custom-workflow.yml

❌ ATENÇÃO: Arquivos específicos do fork detectados!
Estes arquivos NÃO devem ser incluídos no PR para upstream!
```

---

### install-aliases.sh

**Propósito:** Instalar aliases Git globais para uso conveniente.

**O que faz:**
- ✅ Detecta o sistema operacional (Linux/Mac/Windows)
- ✅ Configura aliases Git apontando para os scripts corretos
- ✅ Permite usar comandos `git fork-*` de qualquer lugar

**Uso:**
```bash
# Linux/Mac
./install-aliases.sh

# Windows (requer Git Bash)
bash install-aliases.sh
# Ou use Git Bash: ./install-aliases.sh
```

**Nota:** No Windows, este script requer Git Bash (instalado com Git for Windows).

**Aliases instalados:**
- `git fork-setup` → executa `setup-fork.sh` ou `setup-fork.ps1`
- `git fork-feature <nome>` → executa `new-feature.sh` ou `new-feature.ps1`
- `git fork-sync` → executa `sync-fork.sh` ou `sync-fork.ps1`
- `git fork-check` → executa `check-pr.sh` ou `check-pr.ps1`

**Quando usar:**
- Após o setup inicial, para maior conveniência
- Se preferir comandos `git fork-*` aos scripts diretos

## ❓ FAQ

### P: Por que não posso trabalhar direto na branch `main`?

**R:** A branch `main` deve ser um espelho limpo do `upstream/main`. Isso facilita:
- Sincronização regular com o upstream
- Criação de PRs limpos sem arquivos do fork
- Resolução de conflitos mais simples

### P: Onde devo colocar workflows personalizados?

**R:** Coloque em `.github/workflows/` na branch `fork-config`. Nunca os inclua em PRs para o upstream.

### P: E se eu quiser contribuir mudanças de `fork-config` para o upstream?

**R:** 
1. Crie uma feature branch limpa com `./new-feature.sh`
2. Cherry-pick apenas os commits relevantes
3. Valide com `./check-pr.sh`
4. Envie o PR

```bash
./new-feature.sh contribuir-feature
git cherry-pick <commit-hash>
./check-pr.sh
```

### P: Como adiciono mudanças do upstream na minha feature branch?

**R:**
```bash
# Atualizar main
git checkout main
git fetch upstream
git merge upstream/main --ff-only

# Rebase sua feature
git checkout feature/minha-feature
git rebase main
```

### P: O que é a branch `fork-config`?

**R:** É uma branch separada para customizações específicas do seu fork (workflows, configs, etc.) que não devem ir para o upstream. Ela é atualizada regularmente com mudanças do `main`.

### P: Posso usar estes scripts em outros projetos?

**R:** Sim! Os scripts são genéricos, mas você precisará ajustar:
- Nome do repositório nas verificações
- URL do upstream
- Arquivos/diretórios a serem excluídos na validação

### P: Como remover os aliases?

**R:**
```bash
git config --global --unset alias.fork-setup
git config --global --unset alias.fork-feature
git config --global --unset alias.fork-sync
git config --global --unset alias.fork-check
```

## 🔧 Troubleshooting

### Problema: `fatal: 'upstream' does not appear to be a git repository`

**Solução:** Execute o setup novamente:
```bash
cd scripts/fork-tools
./setup-fork.sh
```

---

### Problema: `error: failed to push some refs`

**Causa:** Sua branch local está desatualizada.

**Solução:**
```bash
git fetch origin
git rebase origin/main
git push origin <branch-name>
```

---

### Problema: Conflitos ao executar `sync-fork.sh`

**Causa:** Mudanças conflitantes entre `main` e `fork-config`.

**Solução:**
```bash
# O script já pausou no ponto do conflito
git status
# Resolver conflitos manualmente nos arquivos indicados
git add .
git merge --continue
git push origin fork-config
```

---

### Problema: `check-pr.sh` detectou arquivos do fork no meu PR

**Causa:** A branch foi criada a partir de `fork-config` ou `main` local com customizações.

**Solução:** Recrie a branch a partir de `upstream/main`:
```bash
cd scripts/fork-tools
./new-feature.sh minha-feature-limpa
# Cherry-pick apenas os commits relevantes
git cherry-pick <commit-hash>
./check-pr.sh
```

---

### Problema: Scripts não executam no Windows

**Causa:** PowerShell Execution Policy.

**Solução:**
```powershell
# Executar como Administrador (uma vez)
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser

# Ou executar diretamente
powershell -ExecutionPolicy Bypass -File .\script.ps1
```

---

### Problema: Alias não funcionam

**Causa:** Aliases não foram instalados ou caminho incorreto.

**Solução:**
```bash
# Reinstalar aliases
cd scripts/fork-tools
./install-aliases.sh

# Verificar instalação
git config --global --get alias.fork-setup
```

---

### Problema: `Permission denied` ao executar scripts no Linux/Mac

**Causa:** Scripts não têm permissão de execução.

**Solução:**
```bash
cd scripts/fork-tools
chmod +x *.sh
```

---

### Problema: Como desfazer mudanças na `main`?

**Solução:**
```bash
git checkout main
git fetch upstream
git reset --hard upstream/main
git push origin main --force
```

⚠️ **CUIDADO:** Isso descarta TODAS as mudanças locais em `main`.

---

## 🎯 Boas Práticas

1. **✅ Execute `check-pr.sh` SEMPRE** antes de abrir PRs para upstream
2. **✅ Mantenha `main` limpa** - apenas mirror do upstream
3. **✅ Use `fork-config`** para customizações específicas do fork
4. **✅ Sincronize regularmente** com `sync-fork.sh`
5. **✅ Use feature branches** criadas por `new-feature.sh` para PRs
6. **❌ NUNCA** faça commit direto em `main`
7. **❌ NUNCA** inclua arquivos de `.github/workflows/` ou `scripts/fork-tools/` em PRs upstream

## 📞 Suporte

- 📖 Documentação: Este README
- 🐛 Issues: Abra uma issue no repositório
- 💬 Dúvidas: Use as Discussions no GitHub

---

**Feito com ❤️ para facilitar contribuições ao OpenAC.Net.NFSe.Nacional**
