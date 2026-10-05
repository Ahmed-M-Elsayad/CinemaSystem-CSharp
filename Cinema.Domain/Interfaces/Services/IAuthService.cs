namespace Cinema.Domain.Interfaces.Services;

public interface IAuthService
{
    bool Login(string password);
}