using Cinema.Domain.Entities;
using Cinema.Domain.Interfaces.Infrastructure;
using Cinema.Domain.Interfaces.Repositories;
using Cinema.Infrastructure.FileSystem;
using Cinema.Infrastructure.Parsing;

namespace Cinema.Infrastructure.Repositories;

public sealed class FileHallRepository : IHallRepository
{
    private readonly IFileStorage _fileStorage;
    private readonly List<Hall> _cache = new();

    public FileHallRepository(IFileStorage fileStorage)
    {
        _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
    }

    public Task<IReadOnlyList<Hall>> GetAllAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Hall>>(_cache.ToList());

    public Task<Hall?> GetByIdAsync(int id, CancellationToken ct = default)
        => Task.FromResult(_cache.FirstOrDefault(h => h.HallId == id));

    public Task AddAsync(Hall hall, CancellationToken ct = default)
    {
        _cache.Add(hall);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Hall hall, CancellationToken ct = default)
    {
        // Hall is reference type so already updated in cache.
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id, CancellationToken ct = default)
    {
        _cache.RemoveAll(h => h.HallId == id);
        return Task.CompletedTask;
    }

    public Task<int> GetNextIdAsync(CancellationToken ct = default)
        => Task.FromResult(_cache.Count == 0 ? 1 : _cache.Max(h => h.HallId) + 1);

    public async Task LoadAsync(CancellationToken ct = default)
    {
        _cache.Clear();
        var lines = await _fileStorage.ReadLinesAsync(DataPaths.HallsFile, ct);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            _cache.Add(HallParser.Parse(line));
        }
    }

    public async Task SaveAsync(CancellationToken ct = default)
    {
        var lines = _cache.Select(HallParser.ToLine);
        await _fileStorage.WriteLinesAsync(DataPaths.HallsFile, lines, ct);
    }
}