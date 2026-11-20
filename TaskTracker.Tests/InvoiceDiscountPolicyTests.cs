using TaskTracker.Core.Pricing;

namespace TaskTracker.Tests;

public class InvoiceDiscountPolicyTests
{
    [Theory]
    [InlineData(0, false, 0)]
    [InlineData(400, true, 20)]
    [InlineData(999.99, false, 0)]
    [InlineData(999.99, true, 50)]
    [InlineData(1000, false, 100)]
    [InlineData(1000, true, 150)]
    [InlineData(1500, false, 150)]
    [InlineData(1000.01, true, 150)]
    public void DiscountFor_Boundaries_MatchesTable(double amount, bool isRegular, double expected)
    {
        var policy = new InvoiceDiscountPolicy();

        decimal actual = policy.DiscountFor((decimal)amount, isRegular);

        Assert.Equal((decimal)expected, actual);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-1000)]
    public void DiscountFor_NegativeAmount_ThrowsOutOfRange(double amount)
    {
        var policy = new InvoiceDiscountPolicy();

        var ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => policy.DiscountFor((decimal)amount, true));

        Assert.Equal("amount", ex.ParamName);
    }

    [Fact]
    public void DiscountFor_HugeInvoice_IsCappedAtMax()
    {
        var policy = new InvoiceDiscountPolicy();

        decimal actual = policy.DiscountFor(5000m, true);

        Assert.Equal(150m, actual);
    }
}
