using Cinema.Domain.Interfaces.Repositories;
using Cinema.Domain.Interfaces.Services;

namespace Cinema.Application.Services;

public sealed class DataPersistenceService : IDataPersistenceService
{
    private readonly IHallRepository _halls;
    private readonly IMovieRepository _movies;
    private readonly IBookingRepository _bookings;

    public DataPersistenceService(
        IHallRepository halls,
        IMovieRepository movies,
        IBookingRepository bookings)
    {
        _halls = halls ?? throw new ArgumentNullException(nameof(halls));
        _movies = movies ?? throw new ArgumentNullException(nameof(movies));
        _bookings = bookings ?? throw new ArgumentNullException(nameof(bookings));
    }

    public async Task LoadAllAsync(CancellationToken ct = default)
    {
        await _halls.LoadAsync(ct);
        await _movies.LoadAsync(ct);
        await _bookings.LoadAsync(ct);
    }

    public async Task SaveAllAsync(CancellationToken ct = default)
    {
        await _halls.SaveAsync(ct);
        await _movies.SaveAsync(ct);
        await _bookings.SaveAsync(ct);
    }
}