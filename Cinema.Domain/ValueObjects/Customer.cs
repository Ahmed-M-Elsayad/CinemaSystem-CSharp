using Cinema.Domain.Exceptions;

namespace Cinema.Domain.ValueObjects;

public sealed record Customer
{
    public int CustomerId { get; }
    public string FullName { get; }
    public string Phone { get; }

    public Customer(int customerId, string fullName, string phone)
    {
        if (customerId <= 0)
            throw new ValidationException("CustomerId must be positive.");
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ValidationException("Customer full name cannot be empty.");

        CustomerId = customerId;
        FullName = fullName.Trim();
        Phone = phone?.Trim() ?? string.Empty;
    }
}