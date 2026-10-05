using Cinema.Domain.Entities;
using Cinema.Domain.ValueObjects;

namespace Cinema.Domain.Interfaces.Services;

public interface IBookingService
{
    Task<Booking> CreateAsync(
        int movieId,
        IEnumerable<SeatPosition> seats,
        Customer customer,
        BookingDate bookingDate,
        CancellationToken ct = default);

    Task AddSeatsAsync(int bookingId, IEnumerable<SeatPosition> seats, CancellationToken ct = default);
    Task RemoveSeatsAsync(int bookingId, IEnumerable<SeatPosition> seats, CancellationToken ct = default);
    Task CancelAsync(int bookingId, CancellationToken ct = default);

    Task<Booking> GetByIdAsync(int bookingId, CancellationToken ct = default);
    Task<IReadOnlyList<Booking>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<SeatPosition>> GetAvailableSeatsAsync(int movieId, CancellationToken ct = default);
    Task<PricingResult> PreviewPriceAsync(int movieId, int seatCount, CancellationToken ct = default);
}