namespace MarkAsPlayed.Foundation.Logger;

public class LoggerDatabaseOptions
{
    public string ConnectionString { get; init; }

    public string[] LogFields { get; init; }

    public string LogTable { get; init; }

    public LoggerDatabaseOptions()
    {
    }
}
