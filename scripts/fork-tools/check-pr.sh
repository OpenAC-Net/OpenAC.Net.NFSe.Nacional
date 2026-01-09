#!/bin/bash

CURRENT_BRANCH=$(git branch --show-current)

echo "🔍 Verificando branch '$CURRENT_BRANCH' antes de PR para upstream..."
echo ""

# Verificar se não está em main ou fork-config
if [ "$CURRENT_BRANCH" = "main" ] || [ "$CURRENT_BRANCH" = "fork-config" ]; then
    echo "⚠️  Você está em '$CURRENT_BRANCH'."
    echo "❌ Crie uma feature branch primeiro usando:"
    echo "   ./new-feature.sh nome-da-feature"
    exit 1
fi

# Fetch upstream
git fetch upstream --quiet

# Verificar diferenças em arquivos sensíveis
echo "📋 Verificando arquivos modificados..."
echo ""

WORKFLOW_CHANGES=$(git diff upstream/main...HEAD --name-only -- .github/workflows/ 2>/dev/null || true)
SCRIPT_CHANGES=$(git diff upstream/main...HEAD --name-only -- scripts/fork-tools/ 2>/dev/null || true)

HAS_ISSUES=0

if [ -n "$WORKFLOW_CHANGES" ]; then
    echo "⚠️  WORKFLOWS DETECTADOS:" 
    echo "$WORKFLOW_CHANGES" | sed 's/^/   /'
    echo ""
    HAS_ISSUES=1
fi

if [ -n "$SCRIPT_CHANGES" ]; then
    echo "⚠️  SCRIPTS DO FORK DETECTADOS:"
    echo "$SCRIPT_CHANGES" | sed 's/^/   /'
    echo ""
    HAS_ISSUES=1
fi

if [ $HAS_ISSUES -eq 1 ]; then
    echo "═══════════════════════════════════════════════════════════"
    echo "❌ ATENÇÃO: Arquivos específicos do fork detectados!"
    echo "═══════════════════════════════════════════════════════════"
    echo ""
    echo "Estes arquivos NÃO devem ser incluídos no PR para upstream!"
    echo ""
    echo "💡 Solução: Recrie a branch a partir do upstream/main:"
    echo "   cd scripts/fork-tools"
    echo "   ./new-feature.sh ${CURRENT_BRANCH#feature/}-clean"
    echo "   # Cherry-pick apenas os commits relevantes"
    echo ""
    exit 1
fi

# Mostrar todos os arquivos modificados
echo "✅ Nenhum arquivo específico do fork detectado!"
echo ""
echo "═══════════════════════════════════════════════════════════"
echo "📄 Arquivos que serão incluídos no PR:"
echo "═══════════════════════════════════════════════════════════"
echo ""
git diff upstream/main...HEAD --name-status | while read status file; do
    case $status in
        A) echo "   ➕ $file" ;;
        M) echo "   📝 $file" ;;
        D) echo "   ❌ $file" ;;
        *) echo "   $status $file" ;;
    esac
done

echo ""
echo "═══════════════════════════════════════════════════════════"
echo "✅ Seguro para abrir PR para upstream!"
echo "═══════════════════════════════════════════════════════════"
echo ""
echo "📋 Próximos passos:"
echo "   1. git push origin $CURRENT_BRANCH"
echo "   2. Abra PR no GitHub:"
echo "      Base: OpenAC-Net/OpenAC.Net.NFSe.Nacional (main)"
echo "      Compare: renatoguarilha/OpenAC.Net.NFSe.Nacional ($CURRENT_BRANCH)"
echo ""
