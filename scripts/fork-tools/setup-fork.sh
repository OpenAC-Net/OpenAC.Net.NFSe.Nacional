#!/bin/bash
set -e

echo "🔧 Configurando fork do OpenAC.Net.NFSe.Nacional..."
echo ""

# Verificar se está no repo correto
if [ ! -d ".git" ]; then
    echo "❌ Erro: Execute este script na raiz do repositório"
    exit 1
fi

# Verificar se é o repositório correto
REPO_NAME=$(basename $(git rev-parse --show-toplevel))
if [ "$REPO_NAME" != "OpenAC.Net.NFSe.Nacional" ]; then
    echo "⚠️  Aviso: Este script foi feito para OpenAC.Net.NFSe.Nacional"
    read -p "Deseja continuar mesmo assim? (s/N) " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Ss]$ ]]; then
        exit 1
    fi
fi

# Adicionar upstream se não existir
if ! git remote | grep -q "^upstream$"; then
    echo "➕ Adicionando remote upstream..."
    git remote add upstream https://github.com/OpenAC-Net/OpenAC.Net.NFSe.Nacional.git
    echo "✅ Remote upstream adicionado"
else
    echo "✅ Remote upstream já existe"
fi

# Fetch upstream
echo "🔄 Fazendo fetch do upstream..."
git fetch upstream

# Verificar se branch fork-config existe
if ! git show-ref --verify --quiet refs/heads/fork-config; then
    echo "🌿 Criando branch fork-config..."
    git checkout -b fork-config
    git push -u origin fork-config
    echo "✅ Branch fork-config criada"
else
    echo "✅ Branch fork-config já existe"
fi

# Voltar para main
git checkout main 2>/dev/null || git checkout -b main

echo ""
echo "═══════════════════════════════════════════════════════════"
echo "✅ Setup concluído com sucesso!"
echo "═══════════════════════════════════════════════════════════"
echo ""
echo "📋 Próximos passos:"
echo ""
echo "   1. Para criar uma feature limpa para PR upstream:"
echo "      ./new-feature.sh nome-da-feature"
echo ""
echo "   2. Para sincronizar com upstream:"
echo "      ./sync-fork.sh"
echo ""
echo "   3. Para validar antes de abrir PR:"
echo "      ./check-pr.sh"
echo ""
echo "   4. (Opcional) Instalar aliases Git:"
echo "      ./install-aliases.sh"
echo ""
echo "═══════════════════════════════════════════════════════════"
