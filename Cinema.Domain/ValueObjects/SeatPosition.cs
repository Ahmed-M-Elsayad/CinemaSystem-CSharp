using Cinema.Domain.Exceptions;

namespace Cinema.Domain.ValueObjects;

public readonly record struct SeatPosition
{
    public int Row { get; }
    public int Column { get; }

    public SeatPosition(int row, int column)
    {
        if (row < 0) throw new ValidationException("Row cannot be negative.");
        if (column < 0) throw new ValidationException("Column cannot be negative.");

        Row = row;
        Column = column;
    }

    public override string ToString() => $"(Row {Row + 1}, Col {Column + 1})";
}