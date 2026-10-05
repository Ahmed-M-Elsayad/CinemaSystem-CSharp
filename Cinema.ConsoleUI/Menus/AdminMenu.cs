using Cinema.Domain.Interfaces.Infrastructure;
using Cinema.Domain.Interfaces.Services;
using Cinema.Domain.ValueObjects;

namespace Cinema.ConsoleUI.Menus;

public sealed class AdminMenu
{
    private readonly IInputReader _input;
    private readonly IOutputWriter _output;
    private readonly IAuthService _auth;
    private readonly IMovieService _movieService;
    private readonly IHallService _hallService;
    private readonly ISalesReportService _salesReport;
    private readonly IDataPersistenceService _persistence;

    public AdminMenu(
        IInputReader input,
        IOutputWriter output,
        IAuthService auth,
        IMovieService movieService,
        IHallService hallService,
        ISalesReportService salesReport,
        IDataPersistenceService persistence)
    {
        _input = input;
        _output = output;
        _auth = auth;
        _movieService = movieService;
        _hallService = hallService;
        _salesReport = salesReport;
        _persistence = persistence;
    }

    public async Task RunAsync()
    {
        _output.WriteHeader("Admin Login");

        var attempts = 3;
        var loggedIn = false;

        while (attempts > 0 && !loggedIn)
        {
            var password = _input.ReadString($"Enter password (attempts left: {attempts}): ");
            loggedIn = _auth.Login(password);
            if (!loggedIn)
            {
                attempts--;
                _output.WriteError(attempts > 0 ? "Incorrect password." : "Access denied.");
            }
        }

        if (!loggedIn) return;

        _output.WriteSuccess("\nLogin successful! Welcome, Admin.");
        _input.WaitForEnter();

        await ShowAdminMenuAsync();
    }

    private async Task ShowAdminMenuAsync()
    {
        while (true)
        {
            _input.Clear();
            _output.WriteHeader("ADMIN MENU");
            _output.WriteLine("1. Add Movie");
            _output.WriteLine("2. Edit Movie");
            _output.WriteLine("3. Delete Movie");
            _output.WriteLine("4. Sales Report");
            _output.WriteLine("5. Save All Data");
            _output.WriteLine("6. Load All Data");
            _output.WriteLine("7. Exit Admin Menu");
            _output.WriteLine();

            var choice = _input.ReadInt("Enter your choice: ", 1, 7);

            try
            {
                switch (choice)
                {
                    case 1: await AddMovieAsync(); break;
                    case 2: await EditMovieAsync(); break;
                    case 3: await DeleteMovieAsync(); break;
                    case 4: await ShowSalesReportAsync(); break;
                    case 5:
                        await _persistence.SaveAllAsync();
                        _output.WriteSuccess("Data saved.");
                        break;
                    case 6:
                        await _persistence.LoadAllAsync();
                        _output.WriteSuccess("Data loaded.");
                        break;
                    case 7: return;
                }
            }
            catch (Exception ex)
            {
                _output.WriteError($"Error: {ex.Message}");
            }

            _input.WaitForEnter();
        }
    }

    private async Task AddMovieAsync()
    {
        var halls = await _hallService.GetAllAsync();
        if (halls.Count == 0)
        {
            _output.WriteError("No halls available. Add a hall first.");
            return;
        }

        _output.WriteLine("\n=== Available Halls ===");
        foreach (var h in halls)
            _output.WriteLine($"  [{h.HallId}] {h.Name} ({h.Rows}x{h.Columns}){(h.IsVip ? " VIP" : "")}");

        var name = _input.ReadString("Movie Name: ");
        var genre = _input.ReadString("Genre: ");
        var showtime = _input.ReadString("Showtime: ");
        var price = _input.ReadDecimal("Ticket Price: ", 0, 10000);
        var hallId = _input.ReadInt("Hall ID: ", 1, 9999);

        _output.WriteLine("Status: 1) Now Showing  2) Coming Soon  3) Ended");
        var statusChoice = _input.ReadInt("Choose status: ", 1, 3);
        var status = statusChoice switch
        {
            1 => MovieStatus.NowShowing,
            2 => MovieStatus.ComingSoon,
            _ => MovieStatus.Ended
        };

        var movie = await _movieService.AddAsync(name, genre, showtime, price, hallId, status);
        _output.WriteSuccess($"Movie '{movie.Name}' added with ID {movie.MovieId}.");
    }

    private async Task EditMovieAsync()
    {
        var movies = await _movieService.GetAllAsync();
        if (movies.Count == 0) { _output.WriteWarning("No movies to edit."); return; }

        foreach (var m in movies)
            _output.WriteLine($"  [{m.MovieId}] {m.Name} | {m.TicketPrice} EGP | {m.Status.ToStorageString()}");

        var id = _input.ReadInt("Movie ID to edit: ", 1, 999999);

        _output.WriteLine("1. Edit Price");
        _output.WriteLine("2. Edit Status");
        var choice = _input.ReadInt("Choose: ", 1, 2);

        if (choice == 1)
        {
            var price = _input.ReadDecimal("New price: ", 0, 10000);
            await _movieService.UpdatePriceAsync(id, price);
            _output.WriteSuccess("Price updated.");
        }
        else
        {
            _output.WriteLine("1) Now Showing  2) Coming Soon  3) Ended");
            var s = _input.ReadInt("New status: ", 1, 3);
            var status = s switch
            {
                1 => MovieStatus.NowShowing,
                2 => MovieStatus.ComingSoon,
                _ => MovieStatus.Ended
            };
            await _movieService.UpdateStatusAsync(id, status);
            _output.WriteSuccess("Status updated.");
        }
    }

    private async Task DeleteMovieAsync()
    {
        var movies = await _movieService.GetAllAsync();
        if (movies.Count == 0) { _output.WriteWarning("No movies to delete."); return; }

        foreach (var m in movies)
            _output.WriteLine($"  [{m.MovieId}] {m.Name}");

        var id = _input.ReadInt("Movie ID to delete: ", 1, 999999);
        await _movieService.DeleteAsync(id);
        _output.WriteSuccess("Movie deleted.");
    }

    private async Task ShowSalesReportAsync()
    {
        var report = await _salesReport.GenerateAsync();
        _output.WriteLine($"\nTotal Revenue: {report.TotalRevenue:N2} EGP");
        if (report.TopMovie is not null)
            _output.WriteLine($"Top Movie: {report.TopMovie.MovieName} ({report.TopMovie.TotalSales:N2} EGP)");

        _output.WriteLine("\n--- Per Movie ---");
        foreach (var ms in report.PerMovie)
            _output.WriteLine($"  {ms.MovieName}: {ms.TotalSales:N2} EGP");
    }
}