using Cinema.Domain.Entities;
using Cinema.Domain.Interfaces.Infrastructure;
using Cinema.Domain.Interfaces.Repositories;
using Cinema.Infrastructure.FileSystem;
using Cinema.Infrastructure.Parsing;

namespace Cinema.Infrastructure.Repositories;

public sealed class FileMovieRepository : IMovieRepository
{
    private readonly IFileStorage _fileStorage;
    private readonly List<Movie> _cache = new();

    public FileMovieRepository(IFileStorage fileStorage)
    {
        _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
    }

    public Task<IReadOnlyList<Movie>> GetAllAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Movie>>(_cache.ToList());

    public Task<Movie?> GetByIdAsync(int id, CancellationToken ct = default)
        => Task.FromResult(_cache.FirstOrDefault(m => m.MovieId == id));

    public Task AddAsync(Movie movie, CancellationToken ct = default)
    {
        _cache.Add(movie);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Movie movie, CancellationToken ct = default)
        => Task.CompletedTask;

    public Task DeleteAsync(int id, CancellationToken ct = default)
    {
        _cache.RemoveAll(m => m.MovieId == id);
        return Task.CompletedTask;
    }

    public Task<int> GetNextIdAsync(CancellationToken ct = default)
        => Task.FromResult(_cache.Count == 0 ? 1 : _cache.Max(m => m.MovieId) + 1);

    public async Task LoadAsync(CancellationToken ct = default)
    {
        _cache.Clear();
        var lines = await _fileStorage.ReadLinesAsync(DataPaths.MoviesFile, ct);

        var i = 0;
        while (i < lines.Count)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) { i++; continue; }

            var (movie, consumed) = MovieParser.Parse(lines, i);
            _cache.Add(movie);
            i += consumed;
        }
    }

    public async Task SaveAsync(CancellationToken ct = default)
    {
        var allLines = _cache.SelectMany(MovieParser.ToLines);
        await _fileStorage.WriteLinesAsync(DataPaths.MoviesFile, allLines, ct);
    }
}