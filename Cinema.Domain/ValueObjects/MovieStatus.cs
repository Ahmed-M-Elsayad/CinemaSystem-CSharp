using Cinema.Domain.Exceptions;

namespace Cinema.Domain.ValueObjects;

public enum MovieStatus
{
    ComingSoon,
    NowShowing,
    Ended
}

public static class MovieStatusExtensions
{
    public const string NowShowingText = "Now Showing";
    public const string ComingSoonText = "Coming Soon";
    public const string EndedText = "Ended";

    public static string ToStorageString(this MovieStatus status) => status switch
    {
        MovieStatus.NowShowing => NowShowingText,
        MovieStatus.ComingSoon => ComingSoonText,
        MovieStatus.Ended => EndedText,
        _ => throw new ValidationException($"Unknown status: {status}")
    };

    public static MovieStatus Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return MovieStatus.ComingSoon;

        return value.Trim() switch
        {
            NowShowingText => MovieStatus.NowShowing,
            ComingSoonText => MovieStatus.ComingSoon,
            EndedText => MovieStatus.Ended,
            _ => throw new ValidationException($"Invalid movie status: '{value}'")
        };
    }
}