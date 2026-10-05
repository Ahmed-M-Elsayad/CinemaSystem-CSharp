namespace Cinema.Domain.Interfaces.Infrastructure;

public interface IFileStorage
{
    Task<string> ReadTextAsync(string relativePath, CancellationToken ct = default);
    Task WriteTextAsync(string relativePath, string content, CancellationToken ct = default);
    Task<IReadOnlyList<string>> ReadLinesAsync(string relativePath, CancellationToken ct = default);
    Task WriteLinesAsync(string relativePath, IEnumerable<string> lines, CancellationToken ct = default);
    bool Exists(string relativePath);
    void EnsureDirectory(string relativePath);
}