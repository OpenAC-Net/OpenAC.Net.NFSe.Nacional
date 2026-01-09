#!/bin/bash
set -e

FEATURE_NAME=$1

if [ -z "$FEATURE_NAME" ]; then
    echo "❌ Uso: ./new-feature.sh <nome-da-feature>"
    echo ""
    echo "Exemplo:"
    echo "   ./new-feature.sh corrigir-bug-123"
    echo "   ./new-feature.sh adicionar-validacao"
    exit 1
fi

echo "🌟 Criando feature branch limpa para '$FEATURE_NAME'..."
echo ""

# Atualizar main com upstream
echo "🔄 Sincronizando main com upstream..."
git checkout main
git fetch upstream
git merge upstream/main --ff-only || {
    echo "⚠️  Não foi possível fazer fast-forward. Fazendo reset..."
    git reset --hard upstream/main
}
git push origin main

# Criar feature branch a partir de upstream/main
BRANCH_NAME="feature/$FEATURE_NAME"
echo "🌿 Criando branch '$BRANCH_NAME' a partir de upstream/main..."
git checkout -b "$BRANCH_NAME" upstream/main

echo ""
echo "═══════════════════════════════════════════════════════════"
echo "✅ Branch criada com sucesso!"
echo "═══════════════════════════════════════════════════════════"
echo ""
echo "📋 Branch: $BRANCH_NAME"
echo "📍 Base:   upstream/main (limpa para PR)"
echo ""
echo "📋 Próximos passos:"
echo ""
echo "   1. Faça suas alterações e commits:"
echo "      git add ."
echo "      git commit -m 'feat: descrição'"
echo ""
echo "   2. Valide antes de enviar:"
echo "      cd scripts/fork-tools && ./check-pr.sh"
echo ""
echo "   3. Push para seu fork:"
echo "      git push origin $BRANCH_NAME"
echo ""
echo "   4. Abra PR no GitHub:"
echo "      Base: OpenAC-Net/OpenAC.Net.NFSe.Nacional (main)"
echo "      Compare: renatoguarilha/OpenAC.Net.NFSe.Nacional ($BRANCH_NAME)"
echo ""
echo "═══════════════════════════════════════════════════════════"
