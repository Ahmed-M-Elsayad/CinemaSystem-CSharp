using Cinema.Domain.Exceptions;
using Cinema.Domain.ValueObjects;

namespace Cinema.Domain.Entities;

public sealed class Movie
{
    public int MovieId { get; }
    public string Name { get; private set; }
    public string Genre { get; private set; }
    public string Showtime { get; private set; }
    public decimal TicketPrice { get; private set; }
    public int HallId { get; private set; }
    public MovieStatus Status { get; private set; }
    public SeatMap Seats { get; private set; }

    public Movie(
        int movieId,
        string name,
        string genre,
        string showtime,
        decimal ticketPrice,
        int hallId,
        MovieStatus status,
        SeatMap seats)
    {
        if (movieId <= 0) throw new ValidationException("MovieId must be positive.");
        if (string.IsNullOrWhiteSpace(name)) throw new ValidationException("Movie name cannot be empty.");
        if (ticketPrice < 0) throw new ValidationException("Ticket price cannot be negative.");
        if (hallId <= 0) throw new ValidationException("HallId must be positive.");

        MovieId = movieId;
        Name = name.Trim();
        Genre = genre?.Trim() ?? string.Empty;
        Showtime = showtime?.Trim() ?? string.Empty;
        TicketPrice = ticketPrice;
        HallId = hallId;
        Status = status;
        Seats = seats ?? throw new ArgumentNullException(nameof(seats));
    }

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ValidationException("Movie name cannot be empty.");
        Name = newName.Trim();
    }

    public void UpdateDetails(string name, string genre, string showtime)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Movie name cannot be empty.");
        Name = name.Trim();
        Genre = genre?.Trim() ?? string.Empty;
        Showtime = showtime?.Trim() ?? string.Empty;
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0) throw new ValidationException("Ticket price cannot be negative.");
        TicketPrice = newPrice;
    }

    public void UpdateStatus(MovieStatus newStatus) => Status = newStatus;

    public void AssignToHall(int hallId, SeatMap newSeats)
    {
        if (hallId <= 0) throw new ValidationException("HallId must be positive.");
        HallId = hallId;
        Seats = newSeats ?? throw new ArgumentNullException(nameof(newSeats));
    }

    public bool IsNowShowing => Status == MovieStatus.NowShowing;
}