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

    public Task RunAsync(string connectionString, CancellationToken cancellationToken = default)
    {
        if (connectionString == null)
            throw new ArgumentNullException("Missing connection string");

        var engine = Deploy(connectionString);

        if (!engine.IsUpgradeRequired())
            return Task.CompletedTask;

        var operation = engine.PerformUpgrade();
        if (operation.Successful)
            return Task.CompletedTask;
        else
            return Task.CompletedTask;
    }
}
