using Cinema.Domain.Entities;

namespace Cinema.Domain.Interfaces.Services;

public interface ITicketExporter
{
    Task<string> ExportToFileAsync(Booking booking, Movie movie, CancellationToken ct = default);
    void PrintToConsole(Booking booking, Movie movie);
}