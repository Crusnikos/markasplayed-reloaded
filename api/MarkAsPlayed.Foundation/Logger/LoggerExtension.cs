using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MarkAsPlayed.Foundation.Logger;

public static class LoggerExtension
{
    public static ILoggingBuilder AddLoggerDatabase(this ILoggingBuilder builder, Action<LoggerDatabaseOptions> databaseConfigure, Action<LoggerDatabaseOptions> consoleConfigure)
    {
        builder.Services.AddSingleton<ILoggerProvider, LoggerConsoleProvider>();
        builder.Services.Configure(consoleConfigure);
        builder.Services.AddSingleton<ILoggerProvider, LoggerDatabaseProvider>();
        builder.Services.Configure(databaseConfigure);
        return builder;
    }
}
