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
                    values.Add("log_level", SafeToString(logLevel));
                    continue;
                case "ThreadId":
                    values.Add("thread_id", SafeToString(Thread.CurrentThread.ManagedThreadId));
                    continue;
                case "EventId":
                    values.Add("event_id", SafeToString(eventId.Id));
                    continue;
                case "EventName":
                    values.Add("event_name", SafeToString(eventId.Name));
                    continue;
                case "Message":
                    values.Add("message", SafeToString(formatter(state, exception)));
                    continue;
                case "ExceptionMessage":
                    values.Add("exception_message", SafeToString(exception?.Message));
                    continue;
                case "ExceptionStackTrace":
                    values.Add("exception_stack_trace", SafeToString(exception?.StackTrace));
                    continue;
                case "ExceptionSource":
                    values.Add("exception_source", SafeToString(exception?.Source));
                    continue;
            }
        }

        return values;
    }

    private static string SafeToString(object? text)
    {
        try
        {
            if (text == null) 
                return string.Empty;

            return Convert.ToString(text)!;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    private async Task InsertLogAsync(Database dbConection, Dictionary<string, string> log, CancellationToken cancellationToken = default)
    {
        try
        {
            var keys = string.Join(", ", log.Select(l => l.Key));
            var values = string.Join(", ", log.Select(l => $"'{l.Value.Replace("\'","\"")}'"));

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
