using System.Globalization;
using Cinema.Domain.Constants;
using Cinema.Domain.Entities;
using Cinema.Domain.ValueObjects;

namespace Cinema.Infrastructure.Parsing;

public static class BookingParser
{
    public static (Booking Booking, int LinesConsumed) Parse(IReadOnlyList<string> lines, int startIndex)
    {
        if (startIndex + 1 >= lines.Count)
            throw new FormatException("Unexpected end of file while reading booking.");

        // السطر 1: التفاصيل
        var parts = lines[startIndex].Split(CinemaConstants.FieldSeparator);
        if (parts.Length < 10)
            throw new FormatException($"Invalid booking header: '{lines[startIndex]}'");

        var bookingId = int.Parse(parts[0]);
        var movieId = int.Parse(parts[1]);
        var movieName = parts[2];
        var seatCount = int.Parse(parts[3]);
        var pricePerSeat = decimal.Parse(parts[4], CultureInfo.InvariantCulture);
        var originalPrice = decimal.Parse(parts[5], CultureInfo.InvariantCulture);
        var discountAmount = decimal.Parse(parts[6], CultureInfo.InvariantCulture);
        var totalPrice = decimal.Parse(parts[7], CultureInfo.InvariantCulture);
        var isActive = parts[8] == "1";
        var isPaid = parts[9] == "1";

        // السطر 2: العميل
        var custParts = lines[startIndex + 1].Split(CinemaConstants.FieldSeparator);
        if (custParts.Length < 3)
            throw new FormatException($"Invalid customer line: '{lines[startIndex + 1]}'");

        var customer = new Customer(
            int.Parse(custParts[0]),
            custParts[1],
            custParts[2]);

        // الأسطر التالية: المقاعد
        var seats = new List<SeatPosition>(seatCount);
        for (var i = 0; i < seatCount; i++)
        {
            var idx = startIndex + 2 + i;
            if (idx >= lines.Count)
                throw new FormatException("Unexpected end of file while reading seat positions.");

            var seatParts = lines[idx].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (seatParts.Length < 2)
                throw new FormatException($"Invalid seat line: '{lines[idx]}'");

            seats.Add(new SeatPosition(
                int.Parse(seatParts[0]),
                int.Parse(seatParts[1])));
        }

        // السطر الأخير: التاريخ
        var dateIdx = startIndex + 2 + seatCount;
        if (dateIdx >= lines.Count)
            throw new FormatException("Unexpected end of file while reading booking date.");

        var dateParts = lines[dateIdx].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (dateParts.Length < 3)
            throw new FormatException($"Invalid date line: '{lines[dateIdx]}'");

        var bookingDate = new BookingDate(
            int.Parse(dateParts[0]),
            int.Parse(dateParts[1]),
            int.Parse(dateParts[2]));

        var booking = new Booking(
            bookingId,
            movieId,
            movieName,
            customer,
            seats,
            pricePerSeat,
            originalPrice,
            discountAmount,
            totalPrice,
            bookingDate,
            isActive,
            isPaid);

        return (booking, 2 + seatCount + 1);
    }

    public static IEnumerable<string> ToLines(Booking booking)
    {
        yield return string.Join(CinemaConstants.FieldSeparator,
            booking.BookingId,
            booking.MovieId,
            booking.MovieName,
            booking.SeatCount,
            booking.PricePerSeat.ToString(CultureInfo.InvariantCulture),
            booking.OriginalPrice.ToString(CultureInfo.InvariantCulture),
            booking.DiscountAmount.ToString(CultureInfo.InvariantCulture),
            booking.TotalPrice.ToString(CultureInfo.InvariantCulture),
            booking.IsActive ? "1" : "0",
            booking.IsPaid ? "1" : "0");

        yield return string.Join(CinemaConstants.FieldSeparator,
            booking.Customer.CustomerId,
            booking.Customer.FullName,
            booking.Customer.Phone);

        foreach (var seat in booking.Seats)
            yield return $"{seat.Row} {seat.Column}";

        yield return $"{booking.BookingDate.Day} {booking.BookingDate.Month} {booking.BookingDate.Year}";
    }
}