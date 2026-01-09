#!/bin/bash

echo "⚡ Instalando aliases Git para ferramentas do fork..."
echo ""

# Obter diretório do repositório
REPO_DIR=$(git rev-parse --show-toplevel 2>/dev/null)

if [ -z "$REPO_DIR" ]; then
    echo "❌ Erro: Execute este script dentro do repositório Git"
    exit 1
fi

SCRIPT_DIR="$REPO_DIR/scripts/fork-tools"

# Detectar OS e escolher extensão
if [[ "$OSTYPE" == "msys" ]] || [[ "$OSTYPE" == "win32" ]] || [[ "$OSTYPE" == "cygwin" ]]; then
    EXT=".ps1"
    echo "🪟 Windows detectado - usando scripts PowerShell"
else
    EXT=".sh"
    echo "🐧 Linux/Mac detectado - usando scripts Bash"
fi

echo ""
echo "📁 Scripts em: $SCRIPT_DIR"
echo ""

# Adicionar aliases
echo "Instalando aliases..."

git config --global alias.fork-setup "!bash '$SCRIPT_DIR/setup-fork.sh'" 2>/dev/null || \
git config --global alias.fork-setup "!powershell -ExecutionPolicy Bypass -File '$SCRIPT_DIR/setup-fork.ps1'"

git config --global alias.fork-feature "!bash '$SCRIPT_DIR/new-feature.sh'" 2>/dev/null || \
git config --global alias.fork-feature "!powershell -ExecutionPolicy Bypass -File '$SCRIPT_DIR/new-feature.ps1'"

git config --global alias.fork-sync "!bash '$SCRIPT_DIR/sync-fork.sh'" 2>/dev/null || \
git config --global alias.fork-sync "!powershell -ExecutionPolicy Bypass -File '$SCRIPT_DIR/sync-fork.ps1'"

git config --global alias.fork-check "!bash '$SCRIPT_DIR/check-pr.sh'" 2>/dev/null || \
git config --global alias.fork-check "!powershell -ExecutionPolicy Bypass -File '$SCRIPT_DIR/check-pr.ps1'"

echo ""
echo "═══════════════════════════════════════════════════════════"
echo "✅ Aliases instalados com sucesso!"
echo "═══════════════════════════════════════════════════════════"
echo ""
echo "📋 Comandos disponíveis:"
echo ""
echo "   git fork-setup              # Configurar fork (primeira vez)"
echo "   git fork-feature <nome>     # Criar feature limpa para PR"
echo "   git fork-sync               # Sincronizar com upstream"
echo "   git fork-check              # Validar antes de abrir PR"
echo ""
echo "═══════════════════════════════════════════════════════════"
echo ""
echo "💡 Dica: Agora você pode usar estes comandos de qualquer lugar!"
echo ""
