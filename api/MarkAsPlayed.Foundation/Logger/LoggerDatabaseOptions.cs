namespace MarkAsPlayed.Foundation.Logger;

public class LoggerDatabaseOptions
{
    public string ConnectionString { get; init; } = default!;

    public string[] LogFields { get; init; } = default!;

    public string LogTable { get; init; } = default!;
}
