using System.Globalization;
using TaskTracker.Legacy;
using Xunit;

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
    [InlineData(null, "new", "a@b.c", "ERR: null")]
    [InlineData("empty", "new", "a@b.c", "ERR: empty")]
    [InlineData("two", "draft", "a@b.c", "ERR: state")]
    [InlineData("two", "new", "no-mail", "ERR: mail")]
    public void Handle_BadInput_ReturnsErrorCode(
        string? kindOfLines, string state, string mail, string expected)
    {
        List<TimeLine>? lines = kindOfLines switch
        {
            "two" => TwoLines(),
            "empty" => new List<TimeLine>(),
            _ => null,
        };
        var sut = new LegacyBillingProcessor();

        string actual = sut.Handle(
            1001, 7, "Іван", mail, "regular", 0, lines, state, "UAH",
            new DateTime(2026, 3, 1), false);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Handle_VipTask_ReturnsExactReport()
    {
        var sut = new LegacyBillingProcessor();

        string report = sut.Handle(
            1001, 7, "Іван", "a@b.c", "vip", 3, TwoLines(), "new", "UAH",
            new DateTime(2026, 3, 1), false);

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

        sut.Handle(
            1001, 7, "Іван", "a@b.c", "regular", 0, TwoLines(), "paid", "UAH",
            new DateTime(2026, 3, 1), true);

        Assert.Equal("ok 1001\nmail -> a@b.c\n", sut.DumpLog());
    }

    [Theory]
    [InlineData("regular", 0, 760)]
    [InlineData("vip", 0, 655)]
    [InlineData("staff", 0, 550)]
    [InlineData("regular", 11, 725)]
    public void Preview_ByKind_ReturnsTotal(string kind, int closed, decimal expected)
    {
        var sut = new LegacyBillingProcessor();

        decimal total = sut.Preview(kind, closed, TwoLines());

        Assert.Equal(expected, total);
    }

    [Fact]
    public void Preview_PayableExactlyAtFreeShippingBoundary_ChargesNoShipping()
    {
        // 4 год × 250 = 1000 без знижки: межа «< 1000» не спрацьовує
        var lines = new List<TimeLine> { new() { Activity = "DEV", Hours = 4, Rate = 250m } };
        var sut = new LegacyBillingProcessor();

        decimal total = sut.Preview("regular", 0, lines);

        Assert.Equal(1000m, total);
    }

    [Fact]
    public void DescribeClient_Vip_BuildsCaption()
    {
        var c = new TeamClient
        {
            Name = " іван ",
            Email = "IVAN@MAIL.COM",
            Kind = "vip",
            ClosedTasks = 12,
            SinceUtc = new DateTime(2020, 1, 1),
        };

        // метод залежить від системного годинника,
        // тому очікування обчислюємо, а не вписуємо
        int years = DateTime.Now.Year - 2020;
        string expected = "ІВАН [VIP] [ЛОЯЛЬНИЙ] <ivan@mail.com> стаж " + years;

        string actual = new LegacyBillingProcessor().DescribeClient(c);

        Assert.Equal(expected, actual);
    }

    private static List<TimeLine> TwoLines() => new()
    {
        new TimeLine { Activity = "DEV", Hours = 2, Rate = 150m },
        new TimeLine { Activity = "REVIEW", Hours = 1, Rate = 400m },
    };
}
