# Стандарт кодування TaskTracker v2.0, розділ 6.6: правила, яких не перевіряють аналізатори.
# Запуск: pwsh tools/check-style.ps1 (або powershell -File tools/check-style.ps1 у Windows).
# Код завершення 1, якщо знайдено порушення, яких немає в базовому списку tools/style-baseline.txt.

$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$maxLineLength = 100
$maxFileLines = 500
$logicDirs = @('TaskTracker.Core', 'src/TaskTracker.Domain', 'src/TaskTracker.Application')
$scopeDirs = @('TaskTracker.Core', 'TaskTracker.App', 'TaskTracker.Tests',
    'src/TaskTracker.Domain', 'src/TaskTracker.Application', 'src/TaskTracker.Infrastructure',
    'src/TaskTracker.Cli', 'tests/TaskTracker.Tests')

# Кожна перевірка: ідентифікатор правила, теки, шаблон порушення, пояснення.
$checks = @(
    @{ Rule = 'A-06'; Dirs = $scopeDirs; Pattern = '#pragma\s+warning\s+disable';
       Text = 'точкове придушення #pragma заборонено, лише .editorconfig або SuppressMessage' },
    @{ Rule = 'F-11'; Dirs = $scopeDirs; Pattern = '#region';
       Text = '#region заборонено' },
    @{ Rule = 'C-02'; Dirs = $scopeDirs; Pattern = '^\s*//(?!/).*[;{}]\s*$';
       Text = 'закоментований код (для історії є Git)' },
    @{ Rule = 'S-05'; Dirs = $scopeDirs; Pattern = '\bgoto\b';
       Text = 'goto заборонено' },
    @{ Rule = 'C-04'; Dirs = $scopeDirs; Pattern = '\bTODO\b(?!\(TD-\d+, [^)]+\):)';
       Text = 'TODO без номера в реєстрі та автора: TODO(TD-12, Прізвище): ...' },
    @{ Rule = 'D-17'; Dirs = $logicDirs; Pattern = 'DateTime\.(Now|UtcNow|Today)\b';
       Text = 'системний годинник у бібліотеці логіки, час передають ззовні (TimeProvider)' },
    @{ Rule = 'D-14'; Dirs = $logicDirs; Pattern = '\b(double|float)\b';
       Text = 'double/float у бібліотеці логіки, для сум і годин — decimal' }
)

$baselineFile = Join-Path $PSScriptRoot 'style-baseline.txt'
$baseline = @()
if (Test-Path $baselineFile) {
    $baseline = Get-Content $baselineFile -Encoding UTF8 |
        Where-Object { $_ -and -not $_.StartsWith('#') } |
        ForEach-Object { ($_ -split '\s+#')[0].Trim() }
}

$found = New-Object System.Collections.Generic.List[string]
$known = 0

function Add-Violation([string]$file, [int]$line, [string]$rule, [string]$text) {
    $relative = $file.Substring($root.Length + 1).Replace('\', '/')
    $key = "$relative|$rule"
    if ($baseline -contains $key) {
        $script:known++
        return
    }

    $script:found.Add("${relative}(${line}): $rule $text")
}

foreach ($dir in $scopeDirs) {
    $files = Get-ChildItem -Path (Join-Path $root $dir) -Recurse -Filter *.cs |
        Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' }
    foreach ($file in $files) {
        $lines = @(Get-Content $file.FullName -Encoding UTF8)
        if ($lines.Count -gt $maxFileLines) {
            Add-Violation $file.FullName $lines.Count 'S-13' "файл $($lines.Count) рядків (межа $maxFileLines)"
        }

        for ($i = 0; $i -lt $lines.Count; $i++) {
            $text = $lines[$i]
            $number = $i + 1
            if ($text.Length -gt $maxLineLength) {
                Add-Violation $file.FullName $number 'F-02' "рядок $($text.Length) символів (межа $maxLineLength)"
            }

            # D-15: округлення сум лише з явним MidpointRounding (виклик може тривати 3 рядки)
            if ($text -cmatch '\b(Math|decimal)\.Round\(' -and $dir -notlike '*Tests') {
                $call = ($lines[$i..([Math]::Min($i + 3, $lines.Count - 1))]) -join ' '
                if ($call -cnotmatch 'MidpointRounding\.') {
                    Add-Violation $file.FullName $number 'D-15' 'Round без MidpointRounding.AwayFromZero'
                }
            }

            foreach ($check in $checks) {
                if ($check.Dirs -contains $dir -and $text -cmatch $check.Pattern) {
                    Add-Violation $file.FullName $number $check.Rule $check.Text
                }
            }
        }
    }
}

Write-Host "check-style: відомий борг із базового списку — $known, нових порушень — $($found.Count)"
foreach ($item in $found) {
    Write-Host "  error $item" -ForegroundColor Red
}

if ($found.Count -gt 0) {
    exit 1
}
