#!/bin/bash
set -e

echo "🔄 Sincronizando fork com upstream..."
echo ""

# Atualizar main
echo "📥 Atualizando main com upstream/main..."
git checkout main
git fetch upstream
git merge upstream/main --ff-only || {
    echo "⚠️  Não foi possível fazer fast-forward. Fazendo reset..."
    git reset --hard upstream/main
}
git push origin main
echo "✅ main atualizada"

# Atualizar fork-config
echo ""
echo "📥 Atualizando fork-config com mudanças de main..."
git checkout fork-config
git merge main --no-edit || {
    echo "⚠️  Conflitos detectados. Resolva manualmente e execute:"
    echo "   git merge --continue"
    echo "   git push origin fork-config"
    exit 1
}
git push origin fork-config
echo "✅ fork-config atualizada"

# Voltar para main
git checkout main

echo ""
echo "═══════════════════════════════════════════════════════════"
echo "✅ Sincronização concluída!"
echo "═══════════════════════════════════════════════════════════"
echo ""
echo "📊 Status das branches:"
echo ""
git branch -vv | grep -E "(main|fork-config)"
echo ""
echo "═══════════════════════════════════════════════════════════"
