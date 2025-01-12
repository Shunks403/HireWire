namespace HireWireBackend.Core.Interfaces.ILoggers;

public interface IBlobLogger
{
    Task LogAsync(string message, string level);
}