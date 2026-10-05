using System.Text;
using Cinema.Domain.Entities;
using Cinema.Domain.Interfaces.Infrastructure;
using Cinema.Domain.Interfaces.Services;

namespace Cinema.Application.Services;

public sealed class TicketExporter : ITicketExporter
{
    private readonly IFileStorage _fileStorage;
    private readonly IOutputWriter _output;

    public TicketExporter(IFileStorage fileStorage, IOutputWriter output)
    {
        _fileStorage = fileStorage
            ?? throw new ArgumentNullException(nameof(fileStorage));
        _output = output
            ?? throw new ArgumentNullException(nameof(output));
    }

    public async Task<string> ExportToFileAsync(
        Booking booking, Movie movie,
        CancellationToken ct = default)
    {
        var fileName = $"receipts/Ticket_{booking.BookingId}.txt";
        var content = BuildTicketText(booking, movie);

        await _fileStorage.WriteTextAsync(fileName, content, ct);
        _output.WriteSuccess($"Ticket exported to: {fileName}");

        return fileName;
    }

    public void PrintToConsole(Booking booking, Movie movie)
    {
        _output.WriteLine(BuildTicketText(booking, movie));
    }

    private static string BuildTicketText(Booking booking, Movie movie)
    {
        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine("==============================");
        sb.AppendLine("         CINEMA TICKET        ");
        sb.AppendLine("==============================");
        sb.AppendLine($"Booking ID   : {booking.BookingId}");
        sb.AppendLine($"Customer ID  : {booking.Customer.CustomerId}");
        sb.AppendLine($"Customer     : {booking.Customer.FullName}");
        sb.AppendLine($"Phone        : {booking.Customer.Phone}");
        sb.AppendLine($"Movie        : {movie.Name}");
        sb.AppendLine($"Showtime     : {movie.Showtime}");
        sb.AppendLine($"Hall         : {movie.HallId}");
        sb.AppendLine($"Seats        : {string.Join(", ", booking.Seats)}");
        sb.AppendLine($"Seat Count   : {booking.SeatCount}");
        sb.AppendLine($"Booking Date : {booking.BookingDate}");
        sb.AppendLine($"Original     : {booking.OriginalPrice:N2} EGP");
        sb.AppendLine($"Discount     : {booking.DiscountAmount:N2} EGP");
        sb.AppendLine($"Total        : {booking.TotalPrice:N2} EGP");
        sb.AppendLine("==============================");
        return sb.ToString();
    }
}