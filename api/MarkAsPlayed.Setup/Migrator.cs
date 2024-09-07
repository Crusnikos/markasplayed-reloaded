using DbUp;
using DbUp.Engine;
using MarkAsPlayed.Foundation.Logger;
using Microsoft.Extensions.Logging;

namespace MarkAsPlayed.Setup;

internal sealed class Migrator
{
    private UpgradeEngine Deploy(string connectionString)
    {
        return DeployChanges.To.
            PostgresqlDatabase(connectionString).
            WithScriptsEmbeddedInAssembly(typeof(Migrator).Assembly).
            WithTransaction().
            WithExecutionTimeout(TimeSpan.FromMinutes(1)).
            LogToNowhere().
            Build();
    }

    public Task<List<string>> RunAsync(string connectionString, ILogger logger, CancellationToken cancellationToken = default)
    {
        var engine = Deploy(connectionString);

        if (!engine.IsUpgradeRequired())
        {
            logger.LogInformation("Database upgrade is not required");
            return Task.FromResult(new List<string>());
        }

        var executedScripts = engine.GetScriptsToExecute().Select(s => s.Name).ToList();
        logger.LogInformation($"- Number of scripts to execute: {engine.GetScriptsToExecute().Count}");

        var operation = engine.PerformUpgrade();

        if (operation.Successful)
        {
            logger.LogInformation("- Database successfully upgraded");
            return Task.FromResult(executedScripts);
        }
        else
        {
            var message = "Database could not be upgraded";
            logger.LogError(operation.Error, $"{LoggerConsoleProvider.RedColor}{message}{LoggerConsoleProvider.WhiteColor}");
            throw new ApplicationException(message);
        }
    }
}
