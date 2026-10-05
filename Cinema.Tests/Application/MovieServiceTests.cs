using Cinema.Application.Services;
using Cinema.Domain.Entities;
using Cinema.Domain.Exceptions;
using Cinema.Domain.Interfaces.Repositories;
using Cinema.Domain.ValueObjects;
using FluentAssertions;
using Moq;
using System.Timers;
using Xunit;

namespace Cinema.Tests.Application;

public class MovieServiceTests
{
    private readonly Mock<IMovieRepository> _movieRepo = new();
    private readonly Mock<IHallRepository> _hallRepo = new();
    private readonly Mock<IBookingRepository> _bookingRepo = new();
    private readonly MovieService _service;

    public MovieServiceTests()
    {
        _service = new MovieService(
            _movieRepo.Object,
            _hallRepo.Object,
            _bookingRepo.Object);
    }

    private static Movie CreateMovie() =>
        new(1, "Test Movie", "Action", "8 PM", 100m, 1, MovieStatus.NowShowing, new SeatMap(3, 3));

    private static Hall CreateHall() =>
        new(1, "Main Hall", 5, 6, false);

    [Fact]
    public async Task DeleteAsync_WhenMovieHasActiveBookings_Throws()
    {
        var movie = CreateMovie();
        _movieRepo
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(movie);
        _bookingRepo
            .Setup(r => r.HasActiveBookingsForMovieAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        Func<Task> act = () => _service.DeleteAsync(1);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*active bookings*");
    }

    [Fact]
    public async Task DeleteAsync_WhenNoActiveBookings_DeletesMovie()
    {
        var movie = CreateMovie();
        _movieRepo
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(movie);
        _bookingRepo
            .Setup(r => r.HasActiveBookingsForMovieAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await _service.DeleteAsync(1);

        _movieRepo.Verify(
            r => r.DeleteAsync(1, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdatePriceAsync_WithValidPrice_UpdatesAndSaves()
    {
        var movie = CreateMovie();
        _movieRepo
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(movie);

        await _service.UpdatePriceAsync(1, 150m);

        movie.TicketPrice.Should().Be(150m);
        _movieRepo.Verify(
            r => r.UpdateAsync(movie, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateStatusAsync_UpdatesMovieStatus()
    {
        var movie = CreateMovie();
        _movieRepo
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(movie);

        await _service.UpdateStatusAsync(1, MovieStatus.Ended);

        movie.Status.Should().Be(MovieStatus.Ended);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_Throws()
    {
        _movieRepo
            .Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Movie?)null);

        Func<Task> act = () => _service.GetByIdAsync(99);

        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task AddAsync_WithValidData_CreatesAndSaves()
    {
        var hall = CreateHall();
        _hallRepo
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(hall);
        _movieRepo
            .Setup(r => r.GetNextIdAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        var movie = await _service.AddAsync(
            "New Movie", "Drama", "10 PM", 120m, 1, MovieStatus.ComingSoon);

        movie.MovieId.Should().Be(5);
        movie.Name.Should().Be("New Movie");
        movie.Seats.Rows.Should().Be(5);
        movie.Seats.Columns.Should().Be(6);
        _movieRepo.Verify(
            r => r.AddAsync(movie, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task AddAsync_WhenHallNotFound_Throws()
    {
        _hallRepo
            .Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Hall?)null);

        Func<Task> act = () => _service.AddAsync(
            "New Movie", "Drama", "10 PM", 120m, 99, MovieStatus.ComingSoon);

        await act.Should().ThrowAsync<EntityNotFoundException>();
    }
}