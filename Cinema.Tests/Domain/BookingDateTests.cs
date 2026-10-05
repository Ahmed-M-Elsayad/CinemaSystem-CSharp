using Cinema.Domain.Exceptions;
using Cinema.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Cinema.Tests.Domain;

public class BookingDateTests
{
    [Fact]
    public void Constructor_WithValidDate_CreatesInstance()
    {
        var date = new BookingDate(15, 6, 2026);

        date.Day.Should().Be(15);
        date.Month.Should().Be(6);
        date.Year.Should().Be(2026);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(32)]
    [InlineData(-1)]
    public void Constructor_WithInvalidDay_Throws(int day)
    {
        Action act = () => new BookingDate(day, 6, 2026);

        act.Should().Throw<ValidationException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    [InlineData(-1)]
    public void Constructor_WithInvalidMonth_Throws(int month)
    {
        Action act = () => new BookingDate(15, month, 2026);

        act.Should().Throw<ValidationException>();
    }

    [Theory]
    [InlineData(2019)]
    [InlineData(2101)]
    public void Constructor_WithInvalidYear_Throws(int year)
    {
        Action act = () => new BookingDate(15, 6, year);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void ToString_ReturnsFormattedDate()
    {
        var date = new BookingDate(5, 10, 2026);

        date.ToString().Should().Be("5/10/2026");
    }
}