using System.Text;
using Microsoft.Extensions.Logging;
using TaskTracker.Application.Services;
using TaskTracker.Cli;
using TaskTracker.Domain;
using TaskTracker.Infrastructure;
using TaskTracker.Infrastructure.Logging;
using TaskTracker.Infrastructure.Storage;

Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;
const string boardPath = "data/board.json";
const string logPath = "logs/tasktracker.log";

// Композиційний корінь: лише тут створюються конкретні реалізації.
var clock = new SystemClock();
using var fileLog = new FileLoggerProvider(logPath, clock);
using ILoggerFactory logs = LoggerFactory.Create(b => b.AddProvider(fileLog));
try
{
    JsonBoardRepository board = JsonBoardRepository.Open(boardPath, clock.Today);
    var policy = new WipMovePolicy(WipLimits.Default);
    var ui = new BoardConsole(
        new TaskService(board, board, clock, logs.CreateLogger<TaskService>()),
        new MoveService(board, policy, clock, logs.CreateLogger<MoveService>()),
        new WorkloadReportService(board, board, clock),
        new ConsoleIo(Console.In, Console.Out, Console.IsInputRedirected));
    ui.Run();
    return 0;
}
catch (Exception ex)
{
    logs.CreateLogger("TaskTracker.Cli").LogCritical(ex, "Непередбачений збій застосунку");
    Console.Error.WriteLine("Непередбачений збій: " + ex.Message);
    return 2;
}
