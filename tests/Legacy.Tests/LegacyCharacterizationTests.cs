using System.Globalization;

namespace TaskTracker.Legacy.Tests;

public class LegacyCharacterizationTests
{
    public LegacyCharacterizationTests()
    {
        // формат чисел залежить від культури середовища,
        // тому фіксуємо її явно
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    }

    [Theory]
    [InlineData(null, BillState.New, "a@b.c", "ERR: null")]
    [InlineData("empty", BillState.New, "a@b.c", "ERR: empty")]
    [InlineData("two", BillState.Draft, "a@b.c", "ERR: state")]
    [InlineData("two", BillState.New, "no-mail", "ERR: mail")]
    public void Handle_BadInput_ReturnsErrorCode(
        string? kindOfLines, BillState state, string mail, string expected)
    {
        List<TimeLine>? lines = kindOfLines switch
        {
            "two" => TwoLines(),
            "empty" => new List<TimeLine>(),
            _ => null,
        };
        var sut = new LegacyBillingProcessor();

        string actual = sut.Handle(Request(lines, state, mail, ClientKind.Regular, 0, false));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Handle_VipTask_ReturnsExactReport()
    {
        var sut = new LegacyBillingProcessor();

        string report = sut.Handle(
            Request(TwoLines(), BillState.New, "a@b.c", ClientKind.Vip, 3, false));

        string expected =
            "Документ #1001\n" +
            "Клієнт: Іван\n" +
            "DEV x2 = 300.00 UAH\n" +
            "REVIEW x1 = 400.00 UAH\n" +
            "Знижка: 105.00 UAH\n" +
            "Доставка: 60.00 UAH\n" +
            "Разом: 655.00 UAH\n";
        Assert.Equal(expected, report);
    }

    [Fact]
    public void Handle_SendMail_WritesOkAndMailToLog()
    {
        var sut = new LegacyBillingProcessor();

        sut.Handle(Request(TwoLines(), BillState.Paid, "a@b.c", ClientKind.Regular, 0, true));

        Assert.Equal("ok 1001\nmail -> a@b.c\n", sut.DumpLog());
    }

    [Theory]
    [InlineData(ClientKind.Regular, 0, 760)]
    [InlineData(ClientKind.Vip, 0, 655)]
    [InlineData(ClientKind.Staff, 0, 550)]
    [InlineData(ClientKind.Regular, 11, 725)]
    public void Preview_ByKind_ReturnsTotal(ClientKind kind, int closed, decimal expected)
    {
        var sut = new LegacyBillingProcessor();

        decimal total = sut.Preview(
            new TeamClient { Kind = kind, ClosedTasks = closed }, TwoLines());

        Assert.Equal(expected, total);
    }

    [Fact]
    public void Preview_PayableExactlyAtFreeShippingBoundary_ChargesNoShipping()
    {
        // 4 год × 250 = 1000 без знижки: межа «< 1000» не спрацьовує
        var lines = new List<TimeLine> { new() { Activity = "DEV", Hours = 4, Rate = 250m } };
        var sut = new LegacyBillingProcessor();

        decimal total = sut.Preview(new TeamClient { Kind = ClientKind.Regular }, lines);

        Assert.Equal(1000m, total);
    }

    [Fact]
    public void DescribeClient_Vip_BuildsCaption()
    {
        var c = new TeamClient
        {
            Name = " іван ",
            Email = "IVAN@MAIL.COM",
            Kind = ClientKind.Vip,
            ClosedTasks = 12,
            SinceUtc = new DateTime(2020, 1, 1),
        };

        // метод залежить від системного годинника,
        // тому очікування обчислюємо, а не вписуємо
        int years = DateTime.Now.Year - 2020;
        string expected = "ІВАН [VIP] [ЛОЯЛЬНИЙ] <ivan@mail.com> стаж " + years;

        string actual = c.Describe();

        Assert.Equal(expected, actual);
    }

    private static BillingRequest Request(
        List<TimeLine>? lines,
        BillState state,
        string mail,
        ClientKind kind,
        int closed,
        bool sendMail)
    {
        var client = new TeamClient
        {
            Id = 7,
            Name = "Іван",
            Email = mail,
            Kind = kind,
            ClosedTasks = closed,
        };
        return new BillingRequest(1001, client, lines, state, "UAH", sendMail);
    }

    private static List<TimeLine> TwoLines() => new()
    {
        new TimeLine { Activity = "DEV", Hours = 2, Rate = 150m },
        new TimeLine { Activity = "REVIEW", Hours = 1, Rate = 400m },
    };
}
