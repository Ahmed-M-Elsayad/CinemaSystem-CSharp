using Cinema.Domain.Constants;
using Cinema.Domain.Entities;

namespace Cinema.Infrastructure.Parsing;

/// <summary>
/// Parser/S serializer لصالة بصيغة: hallId|name|rows|cols|isVip
/// </summary>
public static class HallParser
{
    public static Hall Parse(string line)
    {
        var parts = line.Split(CinemaConstants.FieldSeparator);
        if (parts.Length < 5)
            throw new FormatException($"Invalid hall line: '{line}'");

        return new Hall(
            hallId: int.Parse(parts[0]),
            name: parts[1],
            rows: int.Parse(parts[2]),
            columns: int.Parse(parts[3]),
            isVip: parts[4] == "1");
    }

    public static string ToLine(Hall hall)
        => string.Join(CinemaConstants.FieldSeparator,
            hall.HallId,
            hall.Name,
            hall.Rows,
            hall.Columns,
            hall.IsVip ? "1" : "0");
}