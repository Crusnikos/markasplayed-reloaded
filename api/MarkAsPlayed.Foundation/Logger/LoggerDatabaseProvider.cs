using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MarkAsPlayed.Foundation.Logger;

public class LoggerDatabaseProvider : ILoggerProvider
{
    public readonly LoggerDatabaseOptions Options;

    public LoggerDatabaseProvider(IOptions<LoggerDatabaseOptions> _options)
    {
        Options = _options.Value; // Stores all the options.
    }

    /// <summary>
    /// Creates a new instance of the db logger.
    /// </summary>
    /// <param name="categoryName"></param>
    /// <returns></returns>
    public ILogger CreateLogger(string categoryName)
    {
        return new DatabaseLogger(this);
    }

    public void Dispose()
    {
    }
}
