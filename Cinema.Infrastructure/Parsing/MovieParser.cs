using Cinema.Domain.Constants;
using Cinema.Domain.Entities;
using Cinema.Domain.ValueObjects;

namespace Cinema.Infrastructure.Parsing;

public static class MovieParser
{
    /// <summary>
    /// يحلل فيلم من عدة أسطر: 
    /// - السطر 1: movieId|name|genre|showtime|price|hallId|status
    /// - السطر 2: rows|cols
    /// - الأسطر التالية: صفوف المقاعد (X/O)
    /// </summary>
    public static (Movie Movie, int LinesConsumed) Parse(IReadOnlyList<string> lines, int startIndex)
    {
        if (startIndex + 1 >= lines.Count)
            throw new FormatException("Unexpected end of file while reading movie.");

        // السطر 1: التفاصيل
        var parts = lines[startIndex].Split(CinemaConstants.FieldSeparator);
        if (parts.Length < 7)
            throw new FormatException($"Invalid movie header: '{lines[startIndex]}'");

        var movieId = int.Parse(parts[0]);
        var name = parts[1];
        var genre = parts[2];
        var showtime = parts[3];
        var price = decimal.Parse(parts[4]);
        var hallId = int.Parse(parts[5]);
        var status = MovieStatusExtensions.Parse(parts[6]);

        // السطر 2: الأبعاد
        var dimParts = lines[startIndex + 1].Split(CinemaConstants.FieldSeparator);
        if (dimParts.Length < 2)
            throw new FormatException($"Invalid movie dimensions: '{lines[startIndex + 1]}'");

        var rows = int.Parse(dimParts[0]);
        var cols = int.Parse(dimParts[1]);

        // الأسطر التالية: المقاعد
        var seatRows = new List<string>(rows);
        for (var i = 0; i < rows; i++)
        {
            var idx = startIndex + 2 + i;
            if (idx >= lines.Count)
                throw new FormatException("Unexpected end of file while reading seat rows.");
            seatRows.Add(lines[idx]);
        }

        var seatMap = SeatMap.FromRows(seatRows);
        var movie = new Movie(movieId, name, genre, showtime, price, hallId, status, seatMap);

        return (movie, 2 + rows);
    }

    public static IEnumerable<string> ToLines(Movie movie)
    {
        yield return string.Join(CinemaConstants.FieldSeparator,
            movie.MovieId,
            movie.Name,
            movie.Genre,
            movie.Showtime,
            movie.TicketPrice.ToString(System.Globalization.CultureInfo.InvariantCulture),
            movie.HallId,
            movie.Status.ToStorageString());

        yield return string.Join(CinemaConstants.FieldSeparator,
            movie.Seats.Rows,
            movie.Seats.Columns);

        foreach (var row in movie.Seats.ToRows())
            yield return row;
    }
}