using Cinema.Application.Services;
using Cinema.Domain.Entities;
using Cinema.Domain.Exceptions;
using Cinema.Domain.Interfaces.Repositories;
using Cinema.Domain.Interfaces.Services;
using Cinema.Domain.ValueObjects;
using FluentAssertions;
using Moq;
using System.Timers;
using Xunit;

namespace Cinema.Tests.Application;

public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _bookingRepo = new();
    private readonly Mock<IMovieRepository> _movieRepo = new();
    private readonly Mock<IPriceCalculator> _pricingMock = new();
    private readonly BookingService _service;

    public BookingServiceTests()
    {
        _service = new BookingService(
            _bookingRepo.Object,
            _movieRepo.Object,
            _pricingMock.Object);
    }

    private static Movie CreateMovie(int id = 1, decimal price = 100m)
    {
        var seats = new SeatMap(3, 3);
        return new Movie(id, "Test Movie", "Action", "8 PM", price, 1, MovieStatus.NowShowing, seats);
    }

    private static Customer CreateCustomer() => new(1, "Test User", "0123");
    private static BookingDate CreateDate() => new(1, 1, 2026);

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsBookingAndSaves()
    {
        var movie = CreateMovie();
        _movieRepo
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(movie);
        _bookingRepo
            .Setup(r => r.GetNextIdAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _pricingMock
            .Setup(p => p.Calculate(100m, 2))
            .Returns(new PricingResult(200m, 0m, 200m));

        var booking = await _service.CreateAsync(
            1,
            new[] { new SeatPosition(0, 0), new SeatPosition(0, 1) },
            CreateCustomer(),
            CreateDate());

        booking.BookingId.Should().Be(1);
        booking.SeatCount.Should().Be(2);
        booking.TotalPrice.Should().Be(200m);
        booking.IsActive.Should().BeTrue();

        _bookingRepo.Verify(
            r => r.AddAsync(booking, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenMovieNotFound_Throws()
    {
        _movieRepo
            .Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Movie?)null);

        Func<Task> act = () => _service.CreateAsync(
            99,
            new[] { new SeatPosition(0, 0) },
            CreateCustomer(),
            CreateDate());

        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_WithEmptySeats_Throws()
    {
        var movie = CreateMovie();
        _movieRepo
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(movie);

        Func<Task> act = () => _service.CreateAsync(
            1,
            Array.Empty<SeatPosition>(),
            CreateCustomer(),
            CreateDate());

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateAsync_WhenSeatAlreadyBooked_Throws()
    {
        var movie = CreateMovie();
        movie.Seats.Reserve(new SeatPosition(0, 0));
        _movieRepo
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(movie);

        Func<Task> act = () => _service.CreateAsync(
            1,
            new[] { new SeatPosition(0, 0) },
            CreateCustomer(),
            CreateDate());

        await act.Should().ThrowAsync<BusinessRuleException>();
    }

    [Fact]
    public async Task CancelAsync_WhenBookingActive_CancelsAndReleasesSeats()
    {
        var movie = CreateMovie();
        var seat = new SeatPosition(0, 0);
        movie.Seats.Reserve(seat);

        var booking = new Booking(
            1, 1, "Test Movie", CreateCustomer(),
            new[] { seat }, 100m, 100m, 0m, 100m,
            CreateDate(), isActive: true);

        _bookingRepo
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        _movieRepo
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(movie);

        await _service.CancelAsync(1);

        booking.IsActive.Should().BeFalse();
        movie.Seats.IsAvailable(seat).Should().BeTrue();
        _bookingRepo.Verify(
            r => r.UpdateAsync(booking, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CancelAsync_WhenBookingAlreadyCancelled_Throws()
    {
        var booking = new Booking(
            1, 1, "Test Movie", CreateCustomer(),
            new[] { new SeatPosition(0, 0) }, 100m, 100m, 0m, 100m,
            CreateDate(), isActive: false);

        _bookingRepo
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        Func<Task> act = () => _service.CancelAsync(1);

        await act.Should().ThrowAsync<BusinessRuleException>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_Throws()
    {
        _bookingRepo
            .Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        Func<Task> act = () => _service.GetByIdAsync(99);

        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task PreviewPriceAsync_ReturnsCalculatedPrice()
    {
        var movie = CreateMovie(price: 150m);
        _movieRepo
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(movie);
        _pricingMock
            .Setup(p => p.Calculate(150m, 3))
            .Returns(new PricingResult(450m, 45m, 405m));

        var result = await _service.PreviewPriceAsync(1, 3);

        result.OriginalPrice.Should().Be(450m);
        result.DiscountAmount.Should().Be(45m);
        result.TotalPrice.Should().Be(405m);
    }
}