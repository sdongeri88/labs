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
