using Cinema.Domain.Exceptions;
using Cinema.Domain.ValueObjects;

namespace Cinema.Domain.Entities;

public sealed class Booking
{
    private readonly List<SeatPosition> _seats = new();

    public int BookingId { get; }
    public int MovieId { get; }
    public string MovieName { get; }
    public Customer Customer { get; }
    public IReadOnlyList<SeatPosition> Seats => _seats;
    public int SeatCount => _seats.Count;
    public decimal PricePerSeat { get; }
    public decimal OriginalPrice { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TotalPrice { get; private set; }
    public BookingDate BookingDate { get; }
    public bool IsActive { get; private set; }
    public bool IsPaid { get; private set; }

    public Booking(
        int bookingId,
        int movieId,
        string movieName,
        Customer customer,
        IEnumerable<SeatPosition> seats,
        decimal pricePerSeat,
        decimal originalPrice,
        decimal discountAmount,
        decimal totalPrice,
        BookingDate bookingDate,
        bool isActive = true,
        bool isPaid = false)
    {
        if (bookingId <= 0) throw new ValidationException("BookingId must be positive.");
        if (movieId <= 0) throw new ValidationException("MovieId must be positive.");
        if (pricePerSeat < 0) throw new ValidationException("Price per seat cannot be negative.");

        BookingId = bookingId;
        MovieId = movieId;
        MovieName = movieName ?? string.Empty;
        Customer = customer ?? throw new ArgumentNullException(nameof(customer));
        _seats.AddRange(seats);
        PricePerSeat = pricePerSeat;
        OriginalPrice = originalPrice;
        DiscountAmount = discountAmount;
        TotalPrice = totalPrice;
        BookingDate = bookingDate;
        IsActive = isActive;
        IsPaid = isPaid;
    }

    public void AddSeat(SeatPosition seat)
    {
        if (_seats.Contains(seat))
            throw new BusinessRuleException($"Seat {seat} is already part of this booking.");
        _seats.Add(seat);
    }

    public void RemoveSeat(SeatPosition seat)
    {
        if (!_seats.Remove(seat))
            throw new BusinessRuleException($"Seat {seat} is not part of this booking.");
    }

    public void Cancel()
    {
        if (!IsActive)
            throw new BusinessRuleException("Booking is already cancelled.");
        IsActive = false;
    }

    public void MarkAsPaid() => IsPaid = true;

    public void UpdatePricing(PricingResult pricing)
    {
        OriginalPrice = pricing.OriginalPrice;
        DiscountAmount = pricing.DiscountAmount;
        TotalPrice = pricing.TotalPrice;
    }
}