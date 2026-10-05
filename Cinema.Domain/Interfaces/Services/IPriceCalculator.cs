using Cinema.Domain.ValueObjects;

namespace Cinema.Domain.Interfaces.Services;

public interface IPriceCalculator
{
    PricingResult Calculate(decimal pricePerSeat, int seatCount);
}