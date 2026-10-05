using Cinema.Domain.Entities;
using Cinema.Domain.Interfaces.Infrastructure;
using Cinema.Domain.Interfaces.Repositories;
using Cinema.Infrastructure.FileSystem;
using Cinema.Infrastructure.Parsing;

namespace Cinema.Infrastructure.Repositories;

public sealed class FileBookingRepository : IBookingRepository
{
    private readonly IFileStorage _fileStorage;
    private readonly List<Booking> _cache = new();

    public FileBookingRepository(IFileStorage fileStorage)
    {
        _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
    }

    public Task<IReadOnlyList<Booking>> GetAllAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Booking>>(_cache.ToList());

    public Task<IReadOnlyList<Booking>> GetByMovieIdAsync(int movieId, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Booking>>(
            _cache.Where(b => b.MovieId == movieId).ToList());

    public Task<Booking?> GetByIdAsync(int id, CancellationToken ct = default)
        => Task.FromResult(_cache.FirstOrDefault(b => b.BookingId == id));

    public Task AddAsync(Booking booking, CancellationToken ct = default)
    {
        _cache.Add(booking);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Booking booking, CancellationToken ct = default)
        => Task.CompletedTask;

    public Task<int> GetNextIdAsync(CancellationToken ct = default)
    {
        var nextId = _cache.Count == 0
            ? Cinema.Domain.Constants.CinemaConstants.StartingBookingId
            : _cache.Max(b => b.BookingId) + 1;
        return Task.FromResult(nextId);
    }

    public Task<bool> HasActiveBookingsForMovieAsync(int movieId, CancellationToken ct = default)
        => Task.FromResult(_cache.Any(b => b.MovieId == movieId && b.IsActive));

    public async Task LoadAsync(CancellationToken ct = default)
    {
        _cache.Clear();
        var lines = await _fileStorage.ReadLinesAsync(DataPaths.BookingsFile, ct);

        var i = 0;
        while (i < lines.Count)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) { i++; continue; }

            var (booking, consumed) = BookingParser.Parse(lines, i);
            _cache.Add(booking);
            i += consumed;
        }
    }

    public async Task SaveAsync(CancellationToken ct = default)
    {
        var allLines = _cache.SelectMany(BookingParser.ToLines);
        await _fileStorage.WriteLinesAsync(DataPaths.BookingsFile, allLines, ct);
    }
}