namespace Cinema.Domain.Interfaces.Infrastructure;

public interface IInputReader
{
    string ReadString(string prompt, bool allowEmpty = false);
    int ReadInt(string prompt, int min, int max);
    decimal ReadDecimal(string prompt, decimal min, decimal max);
    bool ReadYesNo(string prompt);
    void WaitForEnter(string message = "Press Enter to continue...");
    void Clear();
}