namespace Cinema.Domain.Interfaces.Services;

public sealed record MovieSales(string MovieName, int MovieId, decimal TotalSales);
public sealed record SalesReport(decimal TotalRevenue, MovieSales? TopMovie, IReadOnlyList<MovieSales> PerMovie);

public interface ISalesReportService
{
    Task<SalesReport> GenerateAsync(CancellationToken ct = default);
}