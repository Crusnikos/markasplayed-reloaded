using Microsoft.Extensions.Logging;

namespace MarkAsPlayed.Foundation.Logger;

public class DatabaseLogger : ILogger
{
    private readonly LoggerDatabaseProvider _dbLoggerProvider;

    public DatabaseLogger(LoggerDatabaseProvider dbLoggerProvider)
    {
        _dbLoggerProvider = dbLoggerProvider;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }

    public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
    {
        throw new NotImplementedException();
    }

    public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        throw new NotImplementedException();
    }
}
