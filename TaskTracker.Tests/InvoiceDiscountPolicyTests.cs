using TaskTracker.Core.Pricing;

namespace TaskTracker.Tests;

public class InvoiceDiscountPolicyTests
{
    [Fact]
    public void DiscountFor_NegativeAmount_Throws()
    {
        // Arrange
        var policy = new InvoiceDiscountPolicy();

        // Act
        void Act() => policy.DiscountFor(-0.01m, true);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act);
    }

    [Fact]
    public void DiscountFor_ZeroAmountNewClient_ReturnsZero()
    {
        // Arrange
        var policy = new InvoiceDiscountPolicy();

        // Act
        decimal actual = policy.DiscountFor(0m, false);

        // Assert
        Assert.Equal(0.00m, actual);
    }

    [Fact]
    public void DiscountFor_SmallInvoiceRegularClient_ReturnsFivePercent()
    {
        // Arrange
        var policy = new InvoiceDiscountPolicy();

        // Act
        decimal actual = policy.DiscountFor(400m, true);

        // Assert
        Assert.Equal(20.00m, actual);
    }

    [Fact]
    public void DiscountFor_JustBelowBigInvoiceNewClient_ReturnsZero()
    {
        // Arrange
        var policy = new InvoiceDiscountPolicy();

        // Act
        decimal actual = policy.DiscountFor(999.99m, false);

        // Assert
        Assert.Equal(0.00m, actual);
    }

    [Fact]
    public void DiscountFor_ExactlyBigInvoiceNewClient_ReturnsTenPercent()
    {
        // Arrange
        var policy = new InvoiceDiscountPolicy();

        // Act
        decimal actual = policy.DiscountFor(1000m, false);

        // Assert
        Assert.Equal(100.00m, actual);
    }

    [Fact]
    public void DiscountFor_ExactlyBigInvoiceRegularClient_ReturnsFifteenPercent()
    {
        // Arrange
        var policy = new InvoiceDiscountPolicy();

        // Act
        decimal actual = policy.DiscountFor(1000m, true);

        // Assert
        Assert.Equal(150.00m, actual);
    }
}
