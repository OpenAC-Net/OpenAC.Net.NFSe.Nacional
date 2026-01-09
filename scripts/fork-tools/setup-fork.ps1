$ErrorActionPreference = "Stop"

Write-Host "🔧 Configurando fork do OpenAC.Net.NFSe.Nacional..." -ForegroundColor Cyan
Write-Host ""

# Verificar se está no repo correto
if (-not (Test-Path ".git")) {
    Write-Host "❌ Erro: Execute este script na raiz do repositório" -ForegroundColor Red
    exit 1
}

# Verificar se é o repositório correto
$repoPath = git rev-parse --show-toplevel
$repoName = Split-Path -Leaf $repoPath
if ($repoName -ne "OpenAC.Net.NFSe.Nacional") {
    Write-Host "⚠️  Aviso: Este script foi feito para OpenAC.Net.NFSe.Nacional" -ForegroundColor Yellow
    $response = Read-Host "Deseja continuar mesmo assim? (s/N)"
    if ($response -notmatch "^[Ss]$") {
        exit 1
    }
}

# Adicionar upstream se não existir
$remotes = git remote
if ($remotes -notcontains "upstream") {
    Write-Host "➕ Adicionando remote upstream..." -ForegroundColor Yellow
    git remote add upstream https://github.com/OpenAC-Net/OpenAC.Net.NFSe.Nacional.git
    Write-Host "✅ Remote upstream adicionado" -ForegroundColor Green
} else {
    Write-Host "✅ Remote upstream já existe" -ForegroundColor Green
}

# Fetch upstream
Write-Host "🔄 Fazendo fetch do upstream..." -ForegroundColor Yellow
git fetch upstream

# Verificar se branch fork-config existe
$branches = git branch --list fork-config
if (-not $branches) {
    Write-Host "🌿 Criando branch fork-config..." -ForegroundColor Yellow
    git checkout -b fork-config
    git push -u origin fork-config
    Write-Host "✅ Branch fork-config criada" -ForegroundColor Green
} else {
    Write-Host "✅ Branch fork-config já existe" -ForegroundColor Green
}

# Voltar para main
git checkout main 2>$null
if ($LASTEXITCODE -ne 0) {
    git checkout -b main
}

Write-Host ""
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host "✅ Setup concluído com sucesso!" -ForegroundColor Green
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""
Write-Host "📋 Próximos passos:" -ForegroundColor Yellow
Write-Host ""
Write-Host "   1. Para criar uma feature limpa para PR upstream:"
Write-Host "      .\new-feature.ps1 nome-da-feature"
Write-Host ""
Write-Host "   2. Para sincronizar com upstream:"
Write-Host "      .\sync-fork.ps1"
Write-Host ""
Write-Host "   3. Para validar antes de abrir PR:"
Write-Host "      .\check-pr.ps1"
Write-Host ""
Write-Host "   4. (Opcional) Instalar aliases Git:"
Write-Host "      bash scripts\fork-tools\install-aliases.sh"
Write-Host "      # Nota: Requer Git Bash no Windows"
Write-Host ""
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
