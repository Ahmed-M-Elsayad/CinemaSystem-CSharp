using Cinema.Domain.Exceptions;

namespace Cinema.Domain.ValueObjects;

public sealed class SeatMap
{
    private readonly bool[,] _seats;

    public int Rows { get; }
    public int Columns { get; }
    public int TotalSeats => Rows * Columns;

    public SeatMap(int rows, int columns)
    {
        if (rows <= 0) throw new ValidationException("Rows must be positive.");
        if (columns <= 0) throw new ValidationException("Columns must be positive.");

        Rows = rows;
        Columns = columns;
        _seats = new bool[rows, columns];
    }

    public int AvailableCount
    {
        get
        {
            var count = 0;
            foreach (var s in _seats) if (!s) count++;
            return count;
        }
    }

    public bool IsWithinBounds(SeatPosition position) =>
        position.Row >= 0 && position.Row < Rows &&
        position.Column >= 0 && position.Column < Columns;

    public bool IsAvailable(SeatPosition position)
    {
        if (!IsWithinBounds(position))
            throw new ValidationException($"Seat {position} is outside the seat map.");

        return !_seats[position.Row, position.Column];
    }

    public void Reserve(SeatPosition position)
    {
        if (!IsAvailable(position))
            throw new BusinessRuleException($"Seat {position} is not available.");
        _seats[position.Row, position.Column] = true;
    }

    public void Release(SeatPosition position)
    {
        if (!IsWithinBounds(position))
            throw new ValidationException($"Seat {position} is outside the seat map.");
        _seats[position.Row, position.Column] = false;
    }

    public void ReserveAll(IEnumerable<SeatPosition> positions)
    {
        var reserved = new List<SeatPosition>();
        try
        {
            foreach (var pos in positions)
            {
                Reserve(pos);
                reserved.Add(pos);
            }
        }
        catch
        {
            foreach (var pos in reserved) Release(pos);
            throw;
        }
    }

    public static SeatMap FromRows(IReadOnlyList<string> rows)
    {
        if (rows.Count == 0)
            throw new ValidationException("Seat map cannot be empty.");

        var columns = rows[0].Length;
        var map = new SeatMap(rows.Count, columns);

        for (var r = 0; r < rows.Count; r++)
        {
            if (rows[r].Length != columns)
                throw new ValidationException("All seat rows must have the same length.");

            for (var c = 0; c < columns; c++)
            {
                if (rows[r][c] == 'X')
                    map._seats[r, c] = true;
            }
        }
        return map;
    }

    public IReadOnlyList<string> ToRows()
    {
        var result = new string[Rows];
        for (var r = 0; r < Rows; r++)
        {
            var chars = new char[Columns];
            for (var c = 0; c < Columns; c++)
                chars[c] = _seats[r, c] ? 'X' : 'O';
            result[r] = new string(chars);
        }
        return result;
    }
}