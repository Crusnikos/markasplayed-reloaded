using DbUp;
using DbUp.Engine;

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

    public Task<List<string>> RunAsync(string connectionString, CancellationToken cancellationToken = default)
    {
        var engine = Deploy(connectionString);

        if (!engine.IsUpgradeRequired())
        {
            ConsoleExtension.WriteLine("\u2713 Database upgrade is not required", ConsoleColor.Green);
            return Task.FromResult(new List<string>());
        }

        var executedScripts = engine.GetScriptsToExecute().Select(s => s.Name).ToList();
        ConsoleExtension.WriteLine($"- Number of scripts to execute: {engine.GetScriptsToExecute().Count}");

        var operation = engine.PerformUpgrade();

        if (operation.Successful)
        {
            ConsoleExtension.WriteLine("\u2714 Database successfully upgraded", ConsoleColor.Green);

            return Task.FromResult(executedScripts);
        }
        else
        {
            ConsoleExtension.WriteLine("\u2716 Database could not be updated", ConsoleColor.Red);
            ConsoleExtension.WriteLine("Error:");
            ConsoleExtension.WriteLine(operation.Error.Message);
            ConsoleExtension.WriteLine("\n");

            return Task.FromResult(executedScripts);
        }
    }
}
