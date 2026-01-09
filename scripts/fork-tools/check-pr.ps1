$ErrorActionPreference = "Stop"

$currentBranch = git branch --show-current

Write-Host "🔍 Verificando branch '$currentBranch' antes de PR para upstream..." -ForegroundColor Cyan
Write-Host ""

# Verificar se não está em main ou fork-config
if ($currentBranch -eq "main" -or $currentBranch -eq "fork-config") {
    Write-Host "⚠️  Você está em '$currentBranch'." -ForegroundColor Yellow
    Write-Host "❌ Crie uma feature branch primeiro usando:" -ForegroundColor Red
    Write-Host "   .\new-feature.ps1 nome-da-feature"
    exit 1
}

# Fetch upstream
git fetch upstream --quiet

# Verificar diferenças em arquivos sensíveis
Write-Host "📋 Verificando arquivos modificados..." -ForegroundColor Yellow
Write-Host ""

$workflowChanges = git diff upstream/main...HEAD --name-only -- .github/workflows/ 2>$null
$scriptChanges = git diff upstream/main...HEAD --name-only -- scripts/fork-tools/ 2>$null

$hasIssues = $false

if ($workflowChanges) {
    Write-Host "⚠️  WORKFLOWS DETECTADOS:" -ForegroundColor Yellow
    $workflowChanges | ForEach-Object { Write-Host "   $_" }
    Write-Host ""
    $hasIssues = $true
}

if ($scriptChanges) {
    Write-Host "⚠️  SCRIPTS DO FORK DETECTADOS:" -ForegroundColor Yellow
    $scriptChanges | ForEach-Object { Write-Host "   $_" }
    Write-Host ""
    $hasIssues = $true
}

if ($hasIssues) {
    Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Red
    Write-Host "❌ ATENÇÃO: Arquivos específicos do fork detectados!" -ForegroundColor Red
    Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Red
    Write-Host ""
    Write-Host "Estes arquivos NÃO devem ser incluídos no PR para upstream!"
    Write-Host ""
    Write-Host "💡 Solução: Recrie a branch a partir do upstream/main:" -ForegroundColor Yellow
    $cleanName = $currentBranch -replace "^feature/", ""
    Write-Host "   cd scripts\fork-tools"
    Write-Host "   .\new-feature.ps1 $cleanName-clean"
    Write-Host "   # Cherry-pick apenas os commits relevantes"
    Write-Host ""
    exit 1
}

# Mostrar todos os arquivos modificados
Write-Host "✅ Nenhum arquivo específico do fork detectado!" -ForegroundColor Green
Write-Host ""
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host "📄 Arquivos que serão incluídos no PR:" -ForegroundColor Yellow
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""

git diff upstream/main...HEAD --name-status | ForEach-Object {
    $parts = $_ -split "`t"
    $status = $parts[0]
    $file = $parts[1]
    
    switch ($status) {
        "A" { Write-Host "   ➕ $file" -ForegroundColor Green }
        "M" { Write-Host "   📝 $file" -ForegroundColor Yellow }
        "D" { Write-Host "   ❌ $file" -ForegroundColor Red }
        default { Write-Host "   $status $file" }
    }
}

Write-Host ""
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host "✅ Seguro para abrir PR para upstream!" -ForegroundColor Green
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""
Write-Host "📋 Próximos passos:" -ForegroundColor Yellow
Write-Host "   1. git push origin $currentBranch"
Write-Host "   2. Abra PR no GitHub:"
Write-Host "      Base: OpenAC-Net/OpenAC.Net.NFSe.Nacional (main)"
Write-Host "      Compare: renatoguarilha/OpenAC.Net.NFSe.Nacional ($currentBranch)"
Write-Host ""
