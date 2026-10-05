namespace Cinema.Domain.Interfaces.Infrastructure;

public interface IOutputWriter
{
    void WriteLine(string message = "");
    void Write(string message);
    void WriteError(string message);
    void WriteSuccess(string message);
    void WriteWarning(string message);
    void WriteHeader(string title);
    void WriteTable<T>(IEnumerable<T> rows, params string[] columnTitles);
}