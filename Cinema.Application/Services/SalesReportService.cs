using Cinema.Domain.Interfaces.Repositories;
using Cinema.Domain.Interfaces.Services;

namespace Cinema.Application.Services;

public sealed class SalesReportService : ISalesReportService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IMovieRepository _movieRepository;

    public SalesReportService(
        IBookingRepository bookingRepository,
        IMovieRepository movieRepository)
    {
        _bookingRepository = bookingRepository
            ?? throw new ArgumentNullException(nameof(bookingRepository));
        _movieRepository = movieRepository
            ?? throw new ArgumentNullException(nameof(movieRepository));
    }

    public async Task<SalesReport> GenerateAsync(CancellationToken ct = default)
    {
        var bookings = await _bookingRepository.GetAllAsync(ct);
        var movies = await _movieRepository.GetAllAsync(ct);

        var activeBookings = bookings.Where(b => b.IsActive).ToList();
        var totalRevenue = activeBookings.Sum(b => b.TotalPrice);

        var perMovie = movies
            .Select(m => new MovieSales(
                m.Name,
                m.MovieId,
                activeBookings.Where(b => b.MovieId == m.MovieId).Sum(b => b.TotalPrice)))
            .Where(x => x.TotalSales > 0)
            .OrderByDescending(x => x.TotalSales)
            .ToList();

        var topMovie = perMovie.FirstOrDefault();
        return new SalesReport(totalRevenue, topMovie, perMovie);
    }
}