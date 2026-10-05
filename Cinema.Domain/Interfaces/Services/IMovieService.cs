using Cinema.Domain.Entities;
using Cinema.Domain.ValueObjects;

namespace Cinema.Domain.Interfaces.Services;

public interface IMovieService
{
    Task<IReadOnlyList<Movie>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Movie>> GetNowShowingAsync(CancellationToken ct = default);
    Task<Movie> GetByIdAsync(int movieId, CancellationToken ct = default);

    Task<Movie> AddAsync(
        string name, string genre, string showtime,
        decimal price, int hallId, MovieStatus status,
        CancellationToken ct = default);

    Task UpdatePriceAsync(int movieId, decimal newPrice, CancellationToken ct = default);
    Task UpdateStatusAsync(int movieId, MovieStatus newStatus, CancellationToken ct = default);
    Task UpdateDetailsAsync(int movieId, string name, string genre, string showtime, CancellationToken ct = default);
    Task DeleteAsync(int movieId, CancellationToken ct = default);
}