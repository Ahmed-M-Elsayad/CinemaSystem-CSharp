using System.Globalization;
using Cinema.Domain.Interfaces.Infrastructure;

namespace Cinema.ConsoleUI.Adapters;

public sealed class ConsoleInputReader : IInputReader
{
    public string ReadString(string prompt, bool allowEmpty = false)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine()?.Trim() ?? string.Empty;

            if (!allowEmpty && string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Input cannot be empty. Please try again.");
                continue;
            }
            return input;
        }
    }

    public int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();
            if (int.TryParse(input, out var value) && value >= min && value <= max)
                return value;

            Console.WriteLine($"Invalid input. Please enter a number between {min} and {max}.");
        }
    }

    public decimal ReadDecimal(string prompt, decimal min, decimal max)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();
            if (decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out var value)
                && value >= min && value <= max)
                return value;

            Console.WriteLine($"Invalid input. Please enter a number between {min} and {max}.");
        }
    }

    public bool ReadYesNo(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt} (y/n): ");
            var input = Console.ReadLine()?.Trim().ToLower();
            if (input == "y" || input == "yes") return true;
            if (input == "n" || input == "no") return false;
            Console.WriteLine("Please enter 'y' or 'n'.");
        }
    }

    public void WaitForEnter(string message = "Press Enter to continue...")
    {
        Console.WriteLine(message);
        Console.ReadLine();
    }

    public void Clear() => Console.Clear();
}