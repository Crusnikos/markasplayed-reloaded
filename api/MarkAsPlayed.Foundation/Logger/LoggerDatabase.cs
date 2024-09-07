using LinqToDB.Data;
using Microsoft.Extensions.Logging;
using LoggingLogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace MarkAsPlayed.Foundation.Logger;

public class LoggerDatabase : ILogger
{
    private readonly LoggerDatabaseProvider _loggerDatabaseProvider;

    public LoggerDatabase(LoggerDatabaseProvider loggerDatabaseProvider)
    {
        _loggerDatabaseProvider = loggerDatabaseProvider;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LoggingLogLevel logLevel)
    {
        return (int)logLevel >= (int)LoggingLogLevel.Warning;
    }

    public async void Log<TState>(LoggingLogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        try
        {
            if (!IsEnabled(logLevel) || !_loggerDatabaseProvider.Options.LogFields.Any())
                return;

            var log = CreateLog(logLevel, eventId, state, exception, formatter);

            using (Database dbConection = new Database(_loggerDatabaseProvider.Options.ConnectionString))
            {
                await InsertLogAsync(dbConection, log);
            }
        }
        catch (Exception)
        {
            return;
        }
    }

    private Dictionary<string, string> CreateLog<TState>(LoggingLogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var values = new Dictionary<string, string>();

        foreach (var logField in _loggerDatabaseProvider.Options.LogFields)
        {
            switch (logField)
            {
                case "LogLevel":
                    if (!string.IsNullOrWhiteSpace(logLevel.ToString()))
                        values.Add("log_level", logLevel.ToString());
                    break;
                case "ThreadId":
                    values.Add("thread_id", Thread.CurrentThread.ManagedThreadId.ToString());
                    break;
                case "EventId":
                    values.Add("event_id", eventId.Id.ToString());
                    break;
                case "EventName":
                    if (!string.IsNullOrWhiteSpace(eventId.Name))
                        values.Add("event_name", eventId.Name);
                    break;
                case "Message":
                    if (!string.IsNullOrWhiteSpace(formatter(state, exception)))
                        values.Add("message", formatter(state, exception));
                    break;
                case "ExceptionMessage":
                    if (!string.IsNullOrWhiteSpace(exception?.Message))
                        values.Add("exception_message", exception?.Message!);
                    break;
                case "ExceptionStackTrace":
                    if (!string.IsNullOrWhiteSpace(exception?.StackTrace))
                        values.Add("exception_stack_trace", exception?.StackTrace!);
                    break;
                case "ExceptionSource":
                    if (!string.IsNullOrWhiteSpace(exception?.Source))
                        values.Add("exception_source", exception?.Source!);
                    break;
            }
        }

        return values;
    }

    private async Task InsertLogAsync(Database dbConection, Dictionary<string, string> log, CancellationToken cancellationToken = default)
    {
        try
        {
            var keys = string.Join(", ", log.Select(l => l.Key));
            var values = string.Join(", ", log.Select(l => $"'{l.Value}'"));

            await dbConection.ExecuteAsync(
                $"INSERT INTO {_loggerDatabaseProvider.Options.LogTable} ({keys}) VALUES ({values})", 
                cancellationToken);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}
