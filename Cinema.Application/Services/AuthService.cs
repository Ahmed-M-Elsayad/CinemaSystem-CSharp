using Cinema.Domain.Interfaces.Services;

namespace Cinema.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly string _correctPassword;

    public AuthService(string correctPassword)
    {
        if (string.IsNullOrWhiteSpace(correctPassword))
            throw new ArgumentException("Correct password cannot be empty.", nameof(correctPassword));

        _correctPassword = correctPassword;
    }

    public bool Login(string password)
    {
        if (string.IsNullOrEmpty(password)) return false;
        return string.Equals(password, _correctPassword, StringComparison.Ordinal);
    }
}