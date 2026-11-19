using System.Text;
using TaskTracker.Core.Requests;

Console.OutputEncoding = Encoding.UTF8;

TotalRequest request = BuildRequestFromInput();
TotalResult result = RequestTotalCalculator.Calculate(request);
Console.WriteLine(result.Total);

static TotalRequest BuildRequestFromInput()
{
    return new TotalRequest(
        RequestId: 1,
        Client: new ClientInfo("Іваненко", "i@ex.com", IsRegular: true),
        Lines: new List<StageLine>
        {
            new() { StageCode = "ST-1", Hours = 12, HourlyRate = 100m },
            new() { StageCode = "ST-2", Hours = 1, HourlyRate = 250m },
        },
        CreatedAt: new DateOnly(2026, 3, 10),
        CouponCode: null,
        Status: RequestStatus.New,
        TravelFee: 60m);
}
