namespace Cinema.Domain.Interfaces.Services;

/// <summary>Open/Closed Principle: عايز خصم جديد؟ اعمل implementation جديد.</summary>
public interface IDiscountStrategy
{
    decimal CalculateDiscount(decimal originalPrice, int seatCount);
}