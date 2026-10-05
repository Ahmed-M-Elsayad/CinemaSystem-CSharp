using Cinema.Application.Pricing;
using Cinema.Application.Services;
using Cinema.Application.Strategies;
using Cinema.ConsoleUI.Adapters;
using Cinema.ConsoleUI.Menus;
using Cinema.Domain.Constants;
using Cinema.Domain.Interfaces.Infrastructure;
using Cinema.Domain.Interfaces.Repositories;
using Cinema.Domain.Interfaces.Services;
using Cinema.Infrastructure.FileSystem;
using Cinema.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Cinema.ConsoleUI.Setup;

public static class DependencyInjectionSetup
{
    public static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();

        // Infrastructure
        services.AddSingleton<IFileStorage>(_ =>
            new LocalFileStorage(AppContext.BaseDirectory));

        services.AddSingleton<IHallRepository, FileHallRepository>();
        services.AddSingleton<IMovieRepository, FileMovieRepository>();
        services.AddSingleton<IBookingRepository, FileBookingRepository>();

        services.AddSingleton<IInputReader, ConsoleInputReader>();
        services.AddSingleton<IOutputWriter, ConsoleOutputWriter>();

        // Application - Strategies
        services.AddSingleton<IDiscountStrategy, BulkTicketDiscountStrategy>();
        services.AddSingleton<IPriceCalculator, StandardPriceCalculator>();

        // Application - Services
        services.AddSingleton<IAuthService>(_ =>
            new AuthService(CinemaConstants.AdminPassword));

        services.AddSingleton<IHallService, HallService>();
        services.AddSingleton<IMovieService, MovieService>();
        services.AddSingleton<IBookingService, BookingService>();
        services.AddSingleton<ISalesReportService, SalesReportService>();
        services.AddSingleton<ITicketExporter, TicketExporter>();
        services.AddSingleton<IDataPersistenceService, DataPersistenceService>();

        // UI - Menus
        services.AddSingleton<AdminMenu>();
        services.AddSingleton<CustomerMenu>();
        services.AddSingleton<MainMenu>();

        return services.BuildServiceProvider();
    }
}