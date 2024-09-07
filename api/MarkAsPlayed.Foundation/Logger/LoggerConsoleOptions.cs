namespace MarkAsPlayed.Foundation.Logger;

public class LoggerConsoleOptions
{
    public bool IncludeScopes { get; init; } = default!;

    public bool SingleLine { get; init; } = default!;

    public string TimestampFormat { get; init; } = default!;

    public int ColorBehavior { get; init; } = default!;
}
