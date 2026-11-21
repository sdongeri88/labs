# Локальний еквівалент конвеєра .github/workflows/ci.yml (ті самі кроки 2–6).
$ErrorActionPreference = 'Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)
function Step([string]$name, [scriptblock]$body) {
    Write-Host "== $name"
    & $body
    if ($LASTEXITCODE -ne 0) { Write-Host "КРОК НЕ ПРОЙДЕНО: $name" -ForegroundColor Red; exit 1 }
}
Step 'Відновлення' { dotnet restore }
Step 'Форматування' { dotnet format --verify-no-changes --no-restore }
Step 'Правила поза аналізаторами' { pwsh ./tools/check-style.ps1 }
Step 'Складання Release' { dotnet build -c Release --no-restore -warnaserror }
Step 'Тести й покриття' { dotnet test -c Release --no-build /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura }
Step 'Поріг покриття' { pwsh ./tools/check-coverage.ps1 -Min 70 }
Write-Host 'Усі критерії якості пройдено.' -ForegroundColor Green
