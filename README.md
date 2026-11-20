# TaskTracker

![CI](https://github.com/sdongeri88/labs/actions/workflows/ci.yml/badge.svg)

Навчальний проєкт з дисципліни «Конструювання програмного забезпечення».
Варіант № 15: облік робочих завдань команди — завдання, виконавці, проєкти
та записи витраченого часу.

## Склад рішення

- `TaskTracker.Core` — логіка предметної області (бібліотека класів)
- `TaskTracker.App` — консольний застосунок (введення, виведення)

## Вимоги

.NET SDK 8.0 або новіший, Git.

## Складання та запуск

    dotnet build
    dotnet run --project TaskTracker.App

## Стандарт кодування

Правила описано у файлі [CODING_STANDARD.md](CODING_STANDARD.md) і закріплено
в `.editorconfig` та `Directory.Build.props`.
Перевірка: `dotnet format --verify-no-changes`

## Автор

Решетнік Костянтин, група 472

## ЛР5: обробка помилок і логування

- класифікація помилок — docs/errors.md;
- барикада: TaskTracker.App/WorkLogParser.cs повертає Result, ядро приймає WorkLog;
- DomainRuleException і Result — TaskTracker.Core/Errors;
- журнал Microsoft.Extensions.Logging (Debug/Information/Warning/Error, BeginScope);
- аудит у audit.log через FileAuditLog (IDisposable).

Запуск із демонстраційним вводом: dotnet run --project TaskTracker.App < docs/lab05/demo-input.txt

## ЛР8: складання та безперервна інтеграція

Версія збірки задається в `Directory.Build.props` (`VersionPrefix` 0.8.0); застосунок
друкує її першим рядком. Конвеєр `.github/workflows/ci.yml` запускається на push у `main`
і `feature/**` та на кожен pull request у `main`. Критерії якості:

| Крок | Команда | Що ламає збірку |
|---|---|---|
| Формат | `dotnet format --verify-no-changes` | зайвий пробіл, відступ, порядок using |
| Складання | `dotnet build -c Release` | будь-яке попередження CS, CA, SA, IDE, NU |
| Тести | `dotnet test -c Release` | хоча б один червоний тест |
| Покриття | `/p:Threshold=60 /p:ThresholdType=line` | покриття рядків нижче за 60 % |

Секрети в репозиторії не зберігаються: локальні налаштування (`appsettings.Local.json`,
`*.secrets.json`, `.env`) виключено в `.gitignore`, у конвеєрі — лише через GitHub Secrets.
