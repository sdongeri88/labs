# Критерій якості СР3: рядкове покриття бібліотеки логіки (TaskTracker.Domain і
# TaskTracker.Application) не нижче за поріг. Межу вимірювання задає властивість
# Include у tests/TaskTracker.Tests/TaskTracker.Tests.csproj.
param(
    [string]$Report = 'tests/TaskTracker.Tests/coverage.cobertura.xml',
    [double]$Min = 70
)

$ErrorActionPreference = 'Stop'
if (-not (Test-Path $Report)) {
    Write-Host "check-coverage: файл $Report не знайдено — тести не збирали покриття" -ForegroundColor Red
    exit 1
}

[xml]$xml = Get-Content $Report -Encoding UTF8
$line = [math]::Round([double]$xml.coverage.'line-rate' * 100, 1)
$branch = [math]::Round([double]$xml.coverage.'branch-rate' * 100, 1)
Write-Host "check-coverage: рядкове $line %, гілок $branch %, поріг рядкового $Min %"
if ($line -lt $Min) {
    Write-Host "check-coverage: покриття нижче порогу" -ForegroundColor Red
    exit 1
}
