using Cinema.Domain.Exceptions;
using Cinema.Domain.Interfaces.Services;
using Cinema.Domain.ValueObjects;

namespace Cinema.Application.Pricing;

public sealed class StandardPriceCalculator : IPriceCalculator
{
    private readonly IDiscountStrategy _discountStrategy;

    public StandardPriceCalculator(IDiscountStrategy discountStrategy)
    {
        _discountStrategy = discountStrategy
            ?? throw new ArgumentNullException(nameof(discountStrategy));
    }

    public PricingResult Calculate(decimal pricePerSeat, int seatCount)
    {
        if (pricePerSeat < 0)
            throw new ValidationException("Price per seat cannot be negative.");
        if (seatCount <= 0)
            throw new ValidationException("Seat count must be greater than zero.");

        var originalPrice = pricePerSeat * seatCount;
        var discount = _discountStrategy.CalculateDiscount(originalPrice, seatCount);
        var total = originalPrice - discount;

        return new PricingResult(originalPrice, discount, total);
    }
}