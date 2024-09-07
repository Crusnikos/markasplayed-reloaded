using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MarkAsPlayed.Foundation.Logger;

[ProviderAlias("LoggerDatabase")]
public class LoggerDatabaseProvider : ILoggerProvider
{
    public readonly LoggerDatabaseOptions Options;

    public LoggerDatabaseProvider(IOptions<LoggerDatabaseOptions> _options)
    {
        Options = _options.Value;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new LoggerDatabase(this);
    }

    public void Dispose()
    {
    }
}
