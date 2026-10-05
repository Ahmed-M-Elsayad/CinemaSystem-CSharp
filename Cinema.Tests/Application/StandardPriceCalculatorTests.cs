using Cinema.Application.Pricing;
using Cinema.Domain.Exceptions;
using Cinema.Domain.Interfaces.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace Cinema.Tests.Application;

public class StandardPriceCalculatorTests
{
    private readonly Mock<IDiscountStrategy> _discountMock = new();
    private readonly StandardPriceCalculator _calculator;

    public StandardPriceCalculatorTests()
    {
        _calculator = new StandardPriceCalculator(_discountMock.Object);
    }

    [Fact]
    public void Calculate_WithNoDiscount_ReturnsOriginalPrice()
    {
        _discountMock
            .Setup(d => d.CalculateDiscount(It.IsAny<decimal>(), It.IsAny<int>()))
            .Returns(0m);

        var result = _calculator.Calculate(100m, 2);

        result.OriginalPrice.Should().Be(200m);
        result.DiscountAmount.Should().Be(0m);
        result.TotalPrice.Should().Be(200m);
    }

    [Fact]
    public void Calculate_WithDiscount_SubtractsFromTotal()
    {
        _discountMock
            .Setup(d => d.CalculateDiscount(500m, 5))
            .Returns(50m);

        var result = _calculator.Calculate(100m, 5);

        result.OriginalPrice.Should().Be(500m);
        result.DiscountAmount.Should().Be(50m);
        result.TotalPrice.Should().Be(450m);
    }

    [Fact]
    public void Calculate_WithNegativePrice_Throws()
    {
        Action act = () => _calculator.Calculate(-10m, 2);

        act.Should().Throw<ValidationException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Calculate_WithInvalidSeatCount_Throws(int seatCount)
    {
        Action act = () => _calculator.Calculate(100m, seatCount);

        act.Should().Throw<ValidationException>();
    }
}