namespace Cinema.Domain.Constants;

public static class CinemaConstants
{
    public const int DefaultRows = 5;
    public const int DefaultColumns = 6;
    public const int StartingBookingId = 1;
    public const int MaxSeatsPerBooking = 10;
    public const int DiscountThreshold = 4;
    public const decimal DiscountRate = 0.10m;
    public const int MaxLoginAttempts = 3;
    public const char FieldSeparator = '|';
    public const string DataFolder = "data";
    public const string ReceiptsFolder = "receipts";
    public const string AdminPassword = "admin123";
}