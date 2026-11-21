# TaskTracker

![CI](https://github.com/sdongeri88/labs/actions/workflows/ci.yml/badge.svg)

Консольний застосунок для дошки завдань команди: створює завдання, переміщує їх колонками з
контролем лімітів незавершеної роботи й показує завантаження виконавців.

## Автор і варіант

Решетнік Костянтин Валентинович, група 472. Дисципліна «Конструювання програмного забезпечення»,
самостійна робота № 3, варіант № 15 — TaskTracker.

## Швидкий старт

Потрібен .NET SDK 8.0 і Git.

    git clone https://github.com/sdongeri88/labs.git && cd labs
    dotnet build TaskTracker.sln
    dotnet run --project src/TaskTracker.Cli

Під час першого запуску створюється `data/board.json` з умовними даними (8 вигаданих учасників,
60 завдань), журнал пишеться в `logs/tasktracker.log`.

## Структура рішення

- `src/TaskTracker.Domain` — завдання, колонки, таблиця переходів, ліміти WIP, профільне правило; не залежить ні від чого.
- `src/TaskTracker.Application` — сценарії UC-01…UC-06, барикада валідації, інтерфейси сховища й годинника.
- `src/TaskTracker.Infrastructure` — сховище JSON, умовні дані, системний годинник, журнал у файл.
- `src/TaskTracker.Cli` — меню консолі й композиційний корінь.
- `tests/TaskTracker.Tests` — модульні тести домену й прикладного шару (xUnit, тестові дублери).
- `TaskTracker.Core`, `TaskTracker.App`, `TaskTracker.Tests`, `src/Legacy`, `tests/Legacy.Tests` — код
  лабораторних робіт 1–8 (історія курсу, див. [docs/labs.md](docs/labs.md)).

Шари й правило залежностей — [docs/architecture.md](docs/architecture.md).

## Можливості

Сценарії описано в [docs/spec.md](docs/spec.md), прохід із вводом і виводом — у
[docs/user-guide.md](docs/user-guide.md).

- UC-01 — створити завдання (ядро варіанта);
- UC-02 — перемістити завдання дошкою за таблицею переходів і лімітами WIP (ядро варіанта);
- UC-03 — призначити виконавця;
- UC-04 — знайти завдання за виконавцем, колонкою і фрагментом назви;
- UC-05 — звіт «завантаження виконавців»;
- UC-06 — зберегти дошку у файл JSON (і завантажити під час запуску).

## Тести й покриття

    dotnet test tests/TaskTracker.Tests /p:CollectCoverage=true

Межу вимірювання (лише `TaskTracker.Domain` і `TaskTracker.Application`) задає властивість `Include`
у `tests/TaskTracker.Tests/TaskTracker.Tests.csproj`. Поточне покриття бібліотеки логіки:
рядкове 94,9 %, гілок 89,7 % (107 тестів).

Усі перевірки конвеєра локально однією командою (PowerShell):

    pwsh tools/ci-local.ps1

## Технології

.NET 8, C# 12, xUnit 2.9, coverlet, Microsoft.Extensions.Logging, System.Text.Json, аналізатори
Roslyn (AnalysisMode Recommended) і StyleCop.Analyzers, GitHub Actions.

## Обмеження й відомий борг

- Файл дошки читається й пишеться цілком; з ним не можна працювати з двох копій застосунку одночасно.
- Немає облікових записів і прав: будь-хто, хто запустив застосунок, може змінити дошку.
- Журнал у файлі не ротується й росте без обмежень (борг TD-12, `docs/tech-debt.md`).
- Ліміти WIP задано в коді (`WipLimits.Default`), а не у файлі налаштувань.
- Код лабораторних робіт лежить у тому самому рішенні; він не використовується застосунком СР3.
