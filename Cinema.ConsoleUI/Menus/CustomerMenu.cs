using Cinema.Domain.Interfaces.Infrastructure;
using Cinema.Domain.Interfaces.Services;
using Cinema.Domain.ValueObjects;

namespace Cinema.ConsoleUI.Menus;

public sealed class CustomerMenu
{
    private readonly IInputReader _input;
    private readonly IOutputWriter _output;
    private readonly IMovieService _movieService;
    private readonly IBookingService _bookingService;
    private readonly ITicketExporter _ticketExporter;

    public CustomerMenu(
        IInputReader input,
        IOutputWriter output,
        IMovieService movieService,
        IBookingService bookingService,
        ITicketExporter ticketExporter)
    {
        _input = input;
        _output = output;
        _movieService = movieService;
        _bookingService = bookingService;
        _ticketExporter = ticketExporter;
    }

    public async Task RunAsync()
    {
        try
        {
            await CreateBookingAsync();
        }
        catch (Exception ex)
        {
            _output.WriteError($"Error: {ex.Message}");
        }
        _input.WaitForEnter();
    }

    private async Task CreateBookingAsync()
    {
        _input.Clear();
        var movies = (await _movieService.GetAllAsync()).ToList();
        if (movies.Count == 0)
        {
            _output.WriteWarning("No movies available.");
            return;
        }

        _output.WriteHeader("Movies");
        for (var i = 0; i < movies.Count; i++)
            _output.WriteLine($"{i + 1}. {movies[i].Name} - {movies[i].TicketPrice:N2} EGP");

        var choice = _input.ReadInt($"\nChoose a movie (1-{movies.Count}): ", 1, movies.Count);
        var movie = movies[choice - 1];
        _output.WriteLine($"\nYou selected: {movie.Name}");

        DisplaySeatMap(movie.Seats);

        var seatCount = _input.ReadInt("\nHow many seats? ", 1, 10);

        var selectedSeats = new List<SeatPosition>();
        while (selectedSeats.Count < seatCount)
        {
            _output.WriteLine($"\n--- Seat {selectedSeats.Count + 1} of {seatCount} ---");
            var row = _input.ReadInt($"Row (1-{movie.Seats.Rows}): ", 1, movie.Seats.Rows) - 1;
            var col = _input.ReadInt($"Col (1-{movie.Seats.Columns}): ", 1, movie.Seats.Columns) - 1;
            var pos = new SeatPosition(row, col);

            if (!movie.Seats.IsAvailable(pos))
            {
                _output.WriteWarning("Seat already booked. Choose another.");
                continue;
            }
            if (selectedSeats.Contains(pos))
            {
                _output.WriteWarning("You already selected this seat.");
                continue;
            }

            selectedSeats.Add(pos);
            _output.WriteSuccess("Seat added.");
        }

        // Preview price
        var preview = await _bookingService.PreviewPriceAsync(movie.MovieId, selectedSeats.Count);
        _output.WriteLine($"\nOriginal: {preview.OriginalPrice:N2} EGP");
        _output.WriteLine($"Discount: {preview.DiscountAmount:N2} EGP");
        _output.WriteLine($"Total:    {preview.TotalPrice:N2} EGP");

        // Customer info
        _output.WriteHeader("Customer Information");
        var customerId = _input.ReadInt("Customer ID: ", 1, 999999);
        var fullName = _input.ReadString("Full name: ");
        var phone = _input.ReadString("Phone: ");
        var customer = new Customer(customerId, fullName, phone);

        // Booking date
        _output.WriteHeader("Booking Date");
        var day = _input.ReadInt("Day (1-31): ", 1, 31);
        var month = _input.ReadInt("Month (1-12): ", 1, 12);
        var year = _input.ReadInt("Year (2020-2100): ", 2020, 2100);
        var date = new BookingDate(day, month, year);

        var booking = await _bookingService.CreateAsync(
            movie.MovieId, selectedSeats, customer, date);

        _output.WriteLine("\n========================================");
        _output.WriteSuccess("   Booking completed successfully!");
        _output.WriteLine("========================================");
        _output.WriteLine($"Booking ID : {booking.BookingId}");
        _output.WriteLine($"Movie      : {booking.MovieName}");
        _output.WriteLine($"Customer   : {booking.Customer.FullName}");
        _output.WriteLine($"Seats      : {booking.SeatCount}");
        _output.WriteLine($"Total      : {booking.TotalPrice:N2} EGP");

        _ticketExporter.PrintToConsole(booking, movie);
    }

    private void DisplaySeatMap(SeatMap seats)
    {
        _output.WriteLine("\n===== Seat Map =====");
        for (var r = 0; r < seats.Rows; r++)
        {
            _output.Write($"Row {r + 1}: ");
            for (var c = 0; c < seats.Columns; c++)
            {
                var pos = new SeatPosition(r, c);
                _output.Write(seats.IsAvailable(pos) ? "O " : "X ");
            }
            _output.WriteLine();
        }
    }
}