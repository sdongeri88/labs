using System.Text;
using TaskTracker.Core.Domain;

Console.OutputEncoding = Encoding.UTF8;

var request = new WorkRequest("R-1001", "Іваненко");
request.AddStage("ST-1", 3, 250m);
request.AddStage("ST-2", 12, 90m);

Console.WriteLine(request.CalculateCost(false));
Console.WriteLine(request.CalculateCost(true));
Console.WriteLine(request.IsValid());
Console.WriteLine(request.TryChangeStatus(WorkRequest.StatusInProgress));
Console.Write(request.BuildReport());
