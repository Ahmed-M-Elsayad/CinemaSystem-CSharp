using Cinema.Domain.Constants;

namespace Cinema.Infrastructure.FileSystem;

/// <summary>
/// يحسب مسارات الملفات بشكل موحد. يعتمد على AppContext.BaseDirectory.
/// </summary>
public static class DataPaths
{
    public static string DataFolder =>
        Path.Combine(AppContext.BaseDirectory, CinemaConstants.DataFolder);

    public static string ReceiptsFolder =>
        Path.Combine(AppContext.BaseDirectory, CinemaConstants.ReceiptsFolder);

    public static string HallsFile => Path.Combine(DataFolder, "halls.txt");
    public static string MoviesFile => Path.Combine(DataFolder, "movies.txt");
    public static string BookingsFile => Path.Combine(DataFolder, "bookings.txt");
}