using Cinema.Domain.Entities;
using Cinema.Domain.Exceptions;
using Cinema.Domain.Interfaces.Repositories;
using Cinema.Domain.Interfaces.Services;
using Cinema.Domain.ValueObjects;

namespace Cinema.Application.Services;

public sealed class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IMovieRepository _movieRepository;
    private readonly IPriceCalculator _priceCalculator;

    public BookingService(
        IBookingRepository bookingRepository,
        IMovieRepository movieRepository,
        IPriceCalculator priceCalculator)
    {
        _bookingRepository = bookingRepository
            ?? throw new ArgumentNullException(nameof(bookingRepository));
        _movieRepository = movieRepository
            ?? throw new ArgumentNullException(nameof(movieRepository));
        _priceCalculator = priceCalculator
            ?? throw new ArgumentNullException(nameof(priceCalculator));
    }

    public async Task<Booking> CreateAsync(
        int movieId,
        IEnumerable<SeatPosition> seats,
        Customer customer,
        BookingDate bookingDate,
        CancellationToken ct = default)
    {
        var movie = await _movieRepository.GetByIdAsync(movieId, ct)
            ?? throw new EntityNotFoundException(nameof(Movie), movieId);

        var seatList = seats.ToList();
        if (seatList.Count == 0)
            throw new ValidationException("Booking must contain at least one seat.");

        foreach (var seat in seatList)
        {
            if (!movie.Seats.IsWithinBounds(seat))
                throw new ValidationException($"Seat {seat} is outside the seat map.");
            if (!movie.Seats.IsAvailable(seat))
                throw new BusinessRuleException($"Seat {seat} is already booked.");
        }

        movie.Seats.ReserveAll(seatList);
        var pricing = _priceCalculator.Calculate(movie.TicketPrice, seatList.Count);
        var nextId = await _bookingRepository.GetNextIdAsync(ct);

        var booking = new Booking(
            nextId, movieId, movie.Name, customer, seatList,
            movie.TicketPrice,
            pricing.OriginalPrice, pricing.DiscountAmount, pricing.TotalPrice,
            bookingDate);

        await _bookingRepository.AddAsync(booking, ct);
        await _movieRepository.UpdateAsync(movie, ct);

        return booking;
    }

    public async Task AddSeatsAsync(
        int bookingId, IEnumerable<SeatPosition> seats,
        CancellationToken ct = default)
    {
        var booking = await GetByIdAsync(bookingId, ct);
        EnsureBookingActive(booking);

        var movie = await _movieRepository.GetByIdAsync(booking.MovieId, ct)
            ?? throw new EntityNotFoundException(nameof(Movie), booking.MovieId);

        foreach (var seat in seats.ToList())
        {
            if (!movie.Seats.IsAvailable(seat))
                throw new BusinessRuleException($"Seat {seat} is not available.");

            movie.Seats.Reserve(seat);
            booking.AddSeat(seat);
        }

        Recalculate(booking, movie.TicketPrice);
        await _bookingRepository.UpdateAsync(booking, ct);
        await _movieRepository.UpdateAsync(movie, ct);
    }

    public async Task RemoveSeatsAsync(
        int bookingId, IEnumerable<SeatPosition> seats,
        CancellationToken ct = default)
    {
        var booking = await GetByIdAsync(bookingId, ct);
        EnsureBookingActive(booking);

        var movie = await _movieRepository.GetByIdAsync(booking.MovieId, ct)
            ?? throw new EntityNotFoundException(nameof(Movie), booking.MovieId);

        foreach (var seat in seats.ToList())
        {
            booking.RemoveSeat(seat);
            movie.Seats.Release(seat);
        }

        Recalculate(booking, movie.TicketPrice);
        await _bookingRepository.UpdateAsync(booking, ct);
        await _movieRepository.UpdateAsync(movie, ct);
    }

    public async Task CancelAsync(int bookingId, CancellationToken ct = default)
    {
        var booking = await GetByIdAsync(bookingId, ct);
        EnsureBookingActive(booking);

        var movie = await _movieRepository.GetByIdAsync(booking.MovieId, ct);
        if (movie is not null)
        {
            foreach (var seat in booking.Seats)
                movie.Seats.Release(seat);
            await _movieRepository.UpdateAsync(movie, ct);
        }

        booking.Cancel();
        await _bookingRepository.UpdateAsync(booking, ct);
    }

    public async Task<Booking> GetByIdAsync(int bookingId, CancellationToken ct = default)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId, ct);
        if (booking is null)
            throw new EntityNotFoundException(nameof(Booking), bookingId);
        return booking;
    }

    public Task<IReadOnlyList<Booking>> GetAllAsync(CancellationToken ct = default)
        => _bookingRepository.GetAllAsync(ct);

    public async Task<IReadOnlyList<SeatPosition>> GetAvailableSeatsAsync(
        int movieId, CancellationToken ct = default)
    {
        var movie = await _movieRepository.GetByIdAsync(movieId, ct)
            ?? throw new EntityNotFoundException(nameof(Movie), movieId);

        var result = new List<SeatPosition>();
        for (var r = 0; r < movie.Seats.Rows; r++)
            for (var c = 0; c < movie.Seats.Columns; c++)
            {
                var pos = new SeatPosition(r, c);
                if (movie.Seats.IsAvailable(pos))
                    result.Add(pos);
            }
        return result;
    }

    public async Task<PricingResult> PreviewPriceAsync(
        int movieId, int seatCount, CancellationToken ct = default)
    {
        var movie = await _movieRepository.GetByIdAsync(movieId, ct)
            ?? throw new EntityNotFoundException(nameof(Movie), movieId);
        return _priceCalculator.Calculate(movie.TicketPrice, seatCount);
    }

    private static void EnsureBookingActive(Booking booking)
    {
        if (!booking.IsActive)
            throw new BusinessRuleException("Cannot modify a cancelled booking.");
    }

    private void Recalculate(Booking booking, decimal pricePerSeat)
    {
        var pricing = _priceCalculator.Calculate(pricePerSeat, booking.SeatCount);
        booking.UpdatePricing(pricing);
    }
}