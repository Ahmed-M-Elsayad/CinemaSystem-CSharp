using Cinema.Domain.Entities;

namespace Cinema.Domain.Interfaces.Repositories;

public interface IBookingRepository
{
    Task<IReadOnlyList<Booking>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Booking>> GetByMovieIdAsync(int movieId, CancellationToken ct = default);
    Task<Booking?> GetByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(Booking booking, CancellationToken ct = default);
    Task UpdateAsync(Booking booking, CancellationToken ct = default);
    Task<int> GetNextIdAsync(CancellationToken ct = default);
    Task<bool> HasActiveBookingsForMovieAsync(int movieId, CancellationToken ct = default);
    Task LoadAsync(CancellationToken ct = default);
    Task SaveAsync(CancellationToken ct = default);
}