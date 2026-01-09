$ErrorActionPreference = "Stop"

Write-Host "🔄 Sincronizando fork com upstream..." -ForegroundColor Cyan
Write-Host ""

# Atualizar main
Write-Host "📥 Atualizando main com upstream/main..." -ForegroundColor Yellow
git checkout main
git fetch upstream
git merge upstream/main --ff-only
if ($LASTEXITCODE -ne 0) {
    Write-Host "⚠️  Não foi possível fazer fast-forward. Fazendo reset..." -ForegroundColor Yellow
    git reset --hard upstream/main
}
git push origin main
Write-Host "✅ main atualizada" -ForegroundColor Green

# Atualizar fork-config
Write-Host ""
Write-Host "📥 Atualizando fork-config com mudanças de main..." -ForegroundColor Yellow
git checkout fork-config
git merge main --no-edit
if ($LASTEXITCODE -ne 0) {
    Write-Host "⚠️  Conflitos detectados. Resolva manualmente e execute:" -ForegroundColor Yellow
    Write-Host "   git merge --continue"
    Write-Host "   git push origin fork-config"
    exit 1
}
git push origin fork-config
Write-Host "✅ fork-config atualizada" -ForegroundColor Green

# Voltar para main
git checkout main

Write-Host ""
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host "✅ Sincronização concluída!" -ForegroundColor Green
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""
Write-Host "📊 Status das branches:" -ForegroundColor Yellow
Write-Host ""
git branch -vv | Select-String -Pattern "(main|fork-config)"
Write-Host ""
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
