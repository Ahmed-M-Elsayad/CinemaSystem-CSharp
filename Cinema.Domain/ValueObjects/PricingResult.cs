namespace Cinema.Domain.ValueObjects;

public readonly record struct PricingResult(
    decimal OriginalPrice,
    decimal DiscountAmount,
    decimal TotalPrice);