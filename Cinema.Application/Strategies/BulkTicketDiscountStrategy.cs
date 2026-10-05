using Cinema.Domain.Constants;
using Cinema.Domain.Interfaces.Services;

namespace Cinema.Application.Strategies;

public sealed class BulkTicketDiscountStrategy : IDiscountStrategy
{
    private readonly int _threshold;
    private readonly decimal _rate;

    public BulkTicketDiscountStrategy(
        int threshold = CinemaConstants.DiscountThreshold,
        decimal rate = CinemaConstants.DiscountRate)
    {
        if (threshold < 1)
            throw new ArgumentOutOfRangeException(nameof(threshold));
        if (rate < 0 || rate > 1)
            throw new ArgumentOutOfRangeException(nameof(rate));

        _threshold = threshold;
        _rate = rate;
    }

    public decimal CalculateDiscount(decimal originalPrice, int seatCount)
    {
        return seatCount > _threshold
            ? originalPrice * _rate
            : 0m;
    }
}