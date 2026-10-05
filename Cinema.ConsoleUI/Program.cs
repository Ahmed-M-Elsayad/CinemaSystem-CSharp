using Cinema.ConsoleUI.Menus;
using Cinema.ConsoleUI.Setup;
using Cinema.Domain.Interfaces.Services;

namespace Cinema.ConsoleUI;

public static class Program
{
    public static async Task Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var provider = DependencyInjectionSetup.BuildProvider();

        var persistence = provider.GetService(typeof(IDataPersistenceService))
            as IDataPersistenceService
            ?? throw new InvalidOperationException("IDataPersistenceService not registered.");

        Console.WriteLine("\nLoading data...");
        try
        {
            await persistence.LoadAllAsync();
            Console.WriteLine("Data loaded successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning during load: {ex.Message}");
        }

        try
        {
            var mainMenu = provider.GetService(typeof(MainMenu)) as MainMenu
                ?? throw new InvalidOperationException("MainMenu not registered.");
            await mainMenu.RunAsync();
        }
        finally
        {
            Console.WriteLine("\nSaving data before exit...");
            try
            {
                await persistence.SaveAllAsync();
                Console.WriteLine("Data saved.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning during save: {ex.Message}");
            }
        }
    }
}