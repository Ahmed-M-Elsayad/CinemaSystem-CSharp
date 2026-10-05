using Cinema.Domain.Exceptions;
using Cinema.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Cinema.Tests.Domain;

public class SeatMapTests
{
    [Fact]
    public void Constructor_WithValidDimensions_CreatesMap()
    {
        var map = new SeatMap(5, 6);

        map.Rows.Should().Be(5);
        map.Columns.Should().Be(6);
        map.TotalSeats.Should().Be(30);
        map.AvailableCount.Should().Be(30);
    }

    [Theory]
    [InlineData(0, 5)]
    [InlineData(5, 0)]
    [InlineData(-1, 5)]
    [InlineData(5, -1)]
    public void Constructor_WithInvalidDimensions_Throws(int rows, int cols)
    {
        Action act = () => new SeatMap(rows, cols);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void IsAvailable_ForNewSeat_ReturnsTrue()
    {
        var map = new SeatMap(5, 6);

        map.IsAvailable(new SeatPosition(0, 0)).Should().BeTrue();
    }

    [Fact]
    public void Reserve_AvailableSeat_MarksAsUnavailable()
    {
        var map = new SeatMap(5, 6);
        var seat = new SeatPosition(0, 0);

        map.Reserve(seat);

        map.IsAvailable(seat).Should().BeFalse();
        map.AvailableCount.Should().Be(29);
    }

    [Fact]
    public void Reserve_AlreadyReservedSeat_Throws()
    {
        var map = new SeatMap(5, 6);
        var seat = new SeatPosition(0, 0);
        map.Reserve(seat);

        Action act = () => map.Reserve(seat);

        act.Should().Throw<BusinessRuleException>()
            .WithMessage("*not available*");
    }

    [Fact]
    public void Release_ReservedSeat_MarksAsAvailable()
    {
        var map = new SeatMap(5, 6);
        var seat = new SeatPosition(0, 0);
        map.Reserve(seat);

        map.Release(seat);

        map.IsAvailable(seat).Should().BeTrue();
    }

    [Fact]
    public void ReserveAll_WithValidSeats_ReservesAll()
    {
        var map = new SeatMap(5, 6);
        var seats = new[]
        {
            new SeatPosition(0, 0),
            new SeatPosition(0, 1),
            new SeatPosition(1, 2)
        };

        map.ReserveAll(seats);

        seats.Should().AllSatisfy(s => map.IsAvailable(s).Should().BeFalse());
    }

    [Fact]
    public void ReserveAll_WhenOneSeatFails_RollsBackPreviousReservations()
    {
        var map = new SeatMap(5, 6);
        var alreadyReserved = new SeatPosition(0, 1);
        map.Reserve(alreadyReserved);

        var seats = new[]
        {
            new SeatPosition(0, 0),
            alreadyReserved  // will fail
        };

        Action act = () => map.ReserveAll(seats);

        act.Should().Throw<BusinessRuleException>();
        // First seat should be rolled back
        map.IsAvailable(new SeatPosition(0, 0)).Should().BeTrue();
    }

    [Fact]
    public void IsWithinBounds_ForOutOfRangeSeat_ReturnsFalse()
    {
        var map = new SeatMap(5, 6);

        // Row out of range (valid rows: 0..4)
        map.IsWithinBounds(new SeatPosition(5, 0)).Should().BeFalse();

        // Column out of range (valid cols: 0..5)
        map.IsWithinBounds(new SeatPosition(0, 6)).Should().BeFalse();
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    [InlineData(-1, -1)]
    public void SeatPosition_Constructor_WithNegativeValues_Throws(int row, int col)
    {
        Action act = () => new SeatPosition(row, col);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void FromRows_WithValidRows_CreatesMapWithCorrectState()
    {
        var rows = new[] { "OXO", "OOO", "XXO" };

        var map = SeatMap.FromRows(rows);

        map.Rows.Should().Be(3);
        map.Columns.Should().Be(3);
        map.IsAvailable(new SeatPosition(0, 0)).Should().BeTrue();
        map.IsAvailable(new SeatPosition(0, 1)).Should().BeFalse();
        map.IsAvailable(new SeatPosition(2, 0)).Should().BeFalse();
        map.IsAvailable(new SeatPosition(2, 2)).Should().BeTrue();
    }

    [Fact]
    public void ToRows_AfterReservations_ReturnsCorrectFormat()
    {
        var map = new SeatMap(2, 3);
        map.Reserve(new SeatPosition(0, 1));
        map.Reserve(new SeatPosition(1, 0));

        var rows = map.ToRows();

        rows.Should().BeEquivalentTo(new[] { "OXO", "XOO" });
    }
}