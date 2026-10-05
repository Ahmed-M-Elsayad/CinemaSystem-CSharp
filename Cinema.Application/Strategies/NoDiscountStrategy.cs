using Cinema.Domain.Interfaces.Services;

namespace Cinema.Application.Strategies;

public sealed class NoDiscountStrategy : IDiscountStrategy
{
    public decimal CalculateDiscount(decimal originalPrice, int seatCount) => 0m;
}