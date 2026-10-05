using Cinema.Domain.Exceptions;

namespace Cinema.Domain.ValueObjects;

public readonly record struct BookingDate
{
    public int Day { get; }
    public int Month { get; }
    public int Year { get; }

    public BookingDate(int day, int month, int year)
    {
        if (day is < 1 or > 31)
            throw new ValidationException("Day must be between 1 and 31.");
        if (month is < 1 or > 12)
            throw new ValidationException("Month must be between 1 and 12.");
        if (year is < 2020 or > 2100)
            throw new ValidationException("Year must be between 2020 and 2100.");

        Day = day;
        Month = month;
        Year = year;
    }

    public override string ToString() => $"{Day}/{Month}/{Year}";
}