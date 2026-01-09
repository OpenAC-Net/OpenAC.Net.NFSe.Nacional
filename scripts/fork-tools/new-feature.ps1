$ErrorActionPreference = "Stop"

$featureName = $args[0]

if (-not $featureName) {
    Write-Host "❌ Uso: .\new-feature.ps1 <nome-da-feature>" -ForegroundColor Red
    Write-Host ""
    Write-Host "Exemplo:"
    Write-Host "   .\new-feature.ps1 corrigir-bug-123"
    Write-Host "   .\new-feature.ps1 adicionar-validacao"
    exit 1
}

Write-Host "🌟 Criando feature branch limpa para '$featureName'..." -ForegroundColor Cyan
Write-Host ""

# Atualizar main com upstream
Write-Host "🔄 Sincronizando main com upstream..." -ForegroundColor Yellow
git checkout main
git fetch upstream
git merge upstream/main --ff-only
if ($LASTEXITCODE -ne 0) {
    Write-Host "⚠️  Não foi possível fazer fast-forward. Fazendo reset..." -ForegroundColor Yellow
    git reset --hard upstream/main
}
git push origin main

# Criar feature branch a partir de upstream/main
$branchName = "feature/$featureName"
Write-Host "🌿 Criando branch '$branchName' a partir de upstream/main..." -ForegroundColor Yellow
git checkout -b $branchName upstream/main

Write-Host ""
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host "✅ Branch criada com sucesso!" -ForegroundColor Green
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""
Write-Host "📋 Branch: $branchName" -ForegroundColor White
Write-Host "📍 Base:   upstream/main (limpa para PR)" -ForegroundColor White
Write-Host ""
Write-Host "📋 Próximos passos:" -ForegroundColor Yellow
Write-Host ""
Write-Host "   1. Faça suas alterações e commits:"
Write-Host "      git add ."
Write-Host "      git commit -m 'feat: descrição'"
Write-Host ""
Write-Host "   2. Valide antes de enviar:"
Write-Host "      cd scripts\fork-tools; .\check-pr.ps1"
Write-Host ""
Write-Host "   3. Push para seu fork:"
Write-Host "      git push origin $branchName"
Write-Host ""
Write-Host "   4. Abra PR no GitHub:"
Write-Host "      Base: OpenAC-Net/OpenAC.Net.NFSe.Nacional (main)"
Write-Host "      Compare: renatoguarilha/OpenAC.Net.NFSe.Nacional ($branchName)"
Write-Host ""
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
