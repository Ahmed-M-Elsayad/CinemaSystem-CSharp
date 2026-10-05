using Cinema.Application.Strategies;
using FluentAssertions;
using Xunit;

namespace Cinema.Tests.Application;

public class BulkTicketDiscountStrategyTests
{
    private readonly BulkTicketDiscountStrategy _strategy = new(); // default: threshold=4, rate=0.10

    [Theory]
    [InlineData(1, 0)]       // 100 * 0
    [InlineData(4, 0)]       // 400 * 0
    [InlineData(5, 50)]      // 500 * 0.10
    [InlineData(10, 100)]    // 1000 * 0.10
    public void CalculateDiscount_ReturnsExpectedDiscount(int seatCount, decimal expected)
    {
        var discount = _strategy.CalculateDiscount(100m * seatCount, seatCount);

        discount.Should().Be(expected);
    }

    [Fact]
    public void Constructor_WithInvalidThreshold_Throws()
    {
        Action act = () => new BulkTicketDiscountStrategy(0, 0.10m);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.5)]
    public void Constructor_WithInvalidRate_Throws(decimal rate)
    {
        Action act = () => new BulkTicketDiscountStrategy(4, rate);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void CalculateDiscount_WithCustomThreshold_UsesCustomValue()
    {
        var strategy = new BulkTicketDiscountStrategy(threshold: 2, rate: 0.20m);

        var discount = strategy.CalculateDiscount(100m, 3);

        discount.Should().Be(20m); // 100 * 0.20
    }
}