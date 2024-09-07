using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace MarkAsPlayed.Foundation.Logger;

public class LoggerConsoleProvider : ILoggerProvider
{
    public static readonly string GreenColor = "\x1B[1m\x1B[32m";
    public static readonly string YellowColor = "\x1B[1m\x1B[33m";
    public static readonly string RedColor = "\x1B[1m\x1B[31m";
    public static readonly string WhiteColor = "\x1B[39m\x1B[22m";

    public readonly LoggerConsoleOptions Options;

    public LoggerConsoleProvider(IOptions<LoggerConsoleOptions> _options = null!)
    {
        Options = Options != null ? _options.Value 
            : new LoggerConsoleOptions()
            {
                IncludeScopes = true,
                SingleLine = true,
                TimestampFormat = "hh:mm:ss ",
                ColorBehavior = 1
            };
    }

    public ILogger CreateLogger(string categoryName)
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.AddSimpleConsole(options =>
            {
                options.IncludeScopes = Options.IncludeScopes;
                options.SingleLine = Options.SingleLine;
                options.TimestampFormat = Options.TimestampFormat;
                options.ColorBehavior = (LoggerColorBehavior)Options.ColorBehavior;
            });
        });

        return factory.CreateLogger(categoryName);
    }

    public void Dispose()
    {
    }
}
