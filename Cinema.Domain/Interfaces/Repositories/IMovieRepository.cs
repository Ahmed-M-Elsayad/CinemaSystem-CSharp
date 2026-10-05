using Cinema.Domain.Entities;

namespace Cinema.Domain.Interfaces.Repositories;

public interface IMovieRepository
{
    Task<IReadOnlyList<Movie>> GetAllAsync(CancellationToken ct = default);
    Task<Movie?> GetByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(Movie movie, CancellationToken ct = default);
    Task UpdateAsync(Movie movie, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task<int> GetNextIdAsync(CancellationToken ct = default);
    Task LoadAsync(CancellationToken ct = default);
    Task SaveAsync(CancellationToken ct = default);
}