using Cinema.Domain.Interfaces.Infrastructure;

namespace Cinema.Infrastructure.FileSystem;

public sealed class LocalFileStorage : IFileStorage
{
    private readonly string _baseDirectory;

    public LocalFileStorage(string baseDirectory)
    {
        _baseDirectory = baseDirectory ?? throw new ArgumentNullException(nameof(baseDirectory));
    }

    public bool Exists(string relativePath)
        => File.Exists(Resolve(relativePath));

    public void EnsureDirectory(string relativePath)
    {
        var fullPath = Resolve(relativePath);
        if (!Directory.Exists(fullPath))
            Directory.CreateDirectory(fullPath);
    }

    public async Task<string> ReadTextAsync(string relativePath, CancellationToken ct = default)
    {
        var fullPath = Resolve(relativePath);
        if (!File.Exists(fullPath))
            return string.Empty;
        return await File.ReadAllTextAsync(fullPath, ct);
    }

    public async Task WriteTextAsync(string relativePath, string content, CancellationToken ct = default)
    {
        var fullPath = Resolve(relativePath);
        EnsureDirectoryForFile(fullPath);
        await File.WriteAllTextAsync(fullPath, content, ct);
    }

    public async Task<IReadOnlyList<string>> ReadLinesAsync(string relativePath, CancellationToken ct = default)
    {
        var fullPath = Resolve(relativePath);
        if (!File.Exists(fullPath))
            return Array.Empty<string>();
        return await File.ReadAllLinesAsync(fullPath, ct);
    }

    public async Task WriteLinesAsync(string relativePath, IEnumerable<string> lines, CancellationToken ct = default)
    {
        var fullPath = Resolve(relativePath);
        EnsureDirectoryForFile(fullPath);
        await File.WriteAllLinesAsync(fullPath, lines, ct);
    }

    private string Resolve(string relativePath)
        => Path.IsPathRooted(relativePath)
            ? relativePath
            : Path.Combine(_baseDirectory, relativePath);

    private static void EnsureDirectoryForFile(string fullPath)
    {
        var dir = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);
    }
}