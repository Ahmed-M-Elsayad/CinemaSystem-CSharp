using Cinema.Domain.Entities;
using Cinema.Domain.Exceptions;
using Cinema.Domain.Interfaces.Repositories;
using Cinema.Domain.Interfaces.Services;
using Cinema.Domain.ValueObjects;

namespace Cinema.Application.Services;

public sealed class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;
    private readonly IHallRepository _hallRepository;
    private readonly IBookingRepository _bookingRepository;

    public MovieService(
        IMovieRepository movieRepository,
        IHallRepository hallRepository,
        IBookingRepository bookingRepository)
    {
        _movieRepository = movieRepository
            ?? throw new ArgumentNullException(nameof(movieRepository));
        _hallRepository = hallRepository
            ?? throw new ArgumentNullException(nameof(hallRepository));
        _bookingRepository = bookingRepository
            ?? throw new ArgumentNullException(nameof(bookingRepository));
    }

    public Task<IReadOnlyList<Movie>> GetAllAsync(CancellationToken ct = default)
        => _movieRepository.GetAllAsync(ct);

    public async Task<IReadOnlyList<Movie>> GetNowShowingAsync(CancellationToken ct = default)
    {
        var all = await _movieRepository.GetAllAsync(ct);
        return all.Where(m => m.IsNowShowing).ToList();
    }

    public async Task<Movie> GetByIdAsync(int movieId, CancellationToken ct = default)
    {
        var movie = await _movieRepository.GetByIdAsync(movieId, ct);
        if (movie is null)
            throw new EntityNotFoundException(nameof(Movie), movieId);
        return movie;
    }

    public async Task<Movie> AddAsync(
        string name, string genre, string showtime,
        decimal price, int hallId, MovieStatus status,
        CancellationToken ct = default)
    {
        var hall = await _hallRepository.GetByIdAsync(hallId, ct)
            ?? throw new EntityNotFoundException(nameof(Hall), hallId);

        var nextId = await _movieRepository.GetNextIdAsync(ct);
        var seats = new SeatMap(hall.Rows, hall.Columns);

        var movie = new Movie(nextId, name, genre, showtime, price, hallId, status, seats);
        await _movieRepository.AddAsync(movie, ct);
        return movie;
    }

    public async Task UpdatePriceAsync(int movieId, decimal newPrice, CancellationToken ct = default)
    {
        var movie = await GetByIdAsync(movieId, ct);
        movie.UpdatePrice(newPrice);
        await _movieRepository.UpdateAsync(movie, ct);
    }

    public async Task UpdateStatusAsync(int movieId, MovieStatus newStatus, CancellationToken ct = default)
    {
        var movie = await GetByIdAsync(movieId, ct);
        movie.UpdateStatus(newStatus);
        await _movieRepository.UpdateAsync(movie, ct);
    }

    public async Task UpdateDetailsAsync(
        int movieId, string name, string genre, string showtime,
        CancellationToken ct = default)
    {
        var movie = await GetByIdAsync(movieId, ct);
        movie.UpdateDetails(name, genre, showtime);
        await _movieRepository.UpdateAsync(movie, ct);
    }

    public async Task DeleteAsync(int movieId, CancellationToken ct = default)
    {
        var movie = await _movieRepository.GetByIdAsync(movieId, ct)
            ?? throw new EntityNotFoundException(nameof(Movie), movieId);

        if (await _bookingRepository.HasActiveBookingsForMovieAsync(movieId, ct))
            throw new BusinessRuleException(
                $"Cannot delete movie '{movie.Name}' because it has active bookings.");

        await _movieRepository.DeleteAsync(movieId, ct);
    }
}