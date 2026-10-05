using Cinema.Domain.Exceptions;

namespace Cinema.Domain.Entities;

public sealed class Hall
{
    public int HallId { get; }
    public string Name { get; private set; }
    public int Rows { get; private set; }
    public int Columns { get; private set; }
    public bool IsVip { get; private set; }

    public Hall(int hallId, string name, int rows, int columns, bool isVip)
    {
        if (hallId <= 0) throw new ValidationException("HallId must be positive.");
        if (string.IsNullOrWhiteSpace(name)) throw new ValidationException("Hall name cannot be empty.");
        if (rows <= 0) throw new ValidationException("Rows must be positive.");
        if (columns <= 0) throw new ValidationException("Columns must be positive.");

        HallId = hallId;
        Name = name.Trim();
        Rows = rows;
        Columns = columns;
        IsVip = isVip;
    }

    public int TotalSeats => Rows * Columns;

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ValidationException("Hall name cannot be empty.");
        Name = newName.Trim();
    }

    public void UpdateDimensions(int rows, int columns)
    {
        if (rows <= 0 || columns <= 0)
            throw new ValidationException("Rows and columns must be positive.");
        Rows = rows;
        Columns = columns;
    }

    public void SetVip(bool isVip) => IsVip = isVip;
}