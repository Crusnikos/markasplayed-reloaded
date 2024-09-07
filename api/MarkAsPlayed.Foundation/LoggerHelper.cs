using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace MarkAsPlayed.Foundation;

public sealed class LoggerHelper
{
    public static readonly string GreenColor = "\x1B[1m\x1B[32m";
    public static readonly string YellowColor = "\x1B[1m\x1B[33m";
    public static readonly string RedColor = "\x1B[1m\x1B[31m";
    public static readonly string WhiteColor = "\x1B[39m\x1B[22m";

    public ILogger CreateLogger(string categoryName)
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder => {
            builder.AddConsole();
            builder.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = true;
                options.TimestampFormat = "hh:mm:ss ";
                options.ColorBehavior = LoggerColorBehavior.Enabled;
            });
        });

        return factory.CreateLogger(categoryName);
    }
}
