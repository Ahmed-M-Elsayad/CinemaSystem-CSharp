using Cinema.Domain.Interfaces.Infrastructure;
using Cinema.Domain.Interfaces.Services;

namespace Cinema.ConsoleUI.Menus;

public sealed class MainMenu
{
    private readonly IInputReader _input;
    private readonly IOutputWriter _output;
    private readonly CustomerMenu _customerMenu;
    private readonly AdminMenu _adminMenu;

    public MainMenu(
        IInputReader input,
        IOutputWriter output,
        CustomerMenu customerMenu,
        AdminMenu adminMenu)
    {
        _input = input;
        _output = output;
        _customerMenu = customerMenu;
        _adminMenu = adminMenu;
    }

    public async Task RunAsync()
    {
        while (true)
        {
            _input.Clear();
            _output.WriteHeader("CINEMA SYSTEM");
            _output.WriteLine("1. Customer Mode (Booking)");
            _output.WriteLine("2. Admin Mode");
            _output.WriteLine("3. Exit");
            _output.WriteLine();

            var choice = _input.ReadInt("Enter your choice: ", 1, 3);

            switch (choice)
            {
                case 1:
                    await _customerMenu.RunAsync();
                    break;
                case 2:
                    await _adminMenu.RunAsync();
                    break;
                case 3:
                    _output.WriteSuccess("Goodbye!");
                    return;
            }
        }
    }
}