using Cinema.Domain.Interfaces.Infrastructure;

namespace Cinema.ConsoleUI.Adapters;

public sealed class ConsoleOutputWriter : IOutputWriter
{
    public void WriteLine(string message = "") => Console.WriteLine(message);
    public void Write(string message) => Console.Write(message);

    public void WriteError(string message)
    {
        var old = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ForegroundColor = old;
    }

    public void WriteSuccess(string message)
    {
        var old = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ForegroundColor = old;
    }

    public void WriteWarning(string message)
    {
        var old = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(message);
        Console.ForegroundColor = old;
    }

    public void WriteHeader(string title)
    {
        var bar = new string('=', title.Length + 8);
        Console.WriteLine(bar);
        Console.WriteLine($"=== {title} ===");
        Console.WriteLine(bar);
    }

    public void WriteTable<T>(IEnumerable<T> rows, params string[] columnTitles)
    {
        var rowList = rows.ToList();
        if (rowList.Count == 0)
        {
            Console.WriteLine("(no data)");
            return;
        }

        // Simple table: join fields with ' | ' based on ToString() or reflection
        foreach (var row in rowList)
            Console.WriteLine(row);
    }
}