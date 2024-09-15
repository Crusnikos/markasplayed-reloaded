using DbUp;
using DbUp.Engine;

namespace MarkAsPlayed.Api.Tests;

internal sealed class TestMigrator
{
    private UpgradeEngine Deploy(string connectionString)
    {
        return DeployChanges.To.
            PostgresqlDatabase(connectionString).
            WithScriptsEmbeddedInAssembly(typeof(TestMigrator).Assembly).
            WithTransaction().
            WithExecutionTimeout(TimeSpan.FromMinutes(1)).
            LogToNowhere().
            Build();
    }

    public Task<List<string>> RunAsync(string connectionString, CancellationToken cancellationToken = default)
    {
        var engine = Deploy(connectionString);

        if (!engine.IsUpgradeRequired())
        {
            return Task.FromResult(new List<string>());
        }

        var executedScripts = engine.GetScriptsToExecute().Select(s => s.Name).ToList();

        var operation = engine.PerformUpgrade();

        if (operation.Successful)
        {
            return Task.FromResult(executedScripts);
        }
        else
        {
            var message = "Database could not be upgraded";
            throw new ApplicationException(message);
        }
    }
}
