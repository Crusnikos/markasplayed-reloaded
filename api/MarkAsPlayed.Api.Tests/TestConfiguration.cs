using LinqToDB.Data;
using MarkAsPlayed.Foundation;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace MarkAsPlayed.Api.Tests;

internal class TestConfiguration : IAsyncDisposable
{
    private readonly string _mainDbConnectionString; 
    public IConfiguration Value { get; }
    public string DatabaseConnectionString { get; }
    public string DatabaseName { get; }
    public string RootPath { get; }
    public List<AdministrationUsers> AdministrationUsers { get; }

    private TestConfiguration(
        IConfiguration configuration,
        string mainDbConnectionString,
        string databaseName,
        string rootPath,
        List<AdministrationUsers> administrationUsers)
    {
        _mainDbConnectionString = mainDbConnectionString;
        Value = configuration;
        DatabaseConnectionString = configuration.GetConnectionString("MainDatabase")!;
        DatabaseName = databaseName;
        RootPath = rootPath;
        AdministrationUsers = administrationUsers;
    }

    public static TestConfiguration Create()
    {
        var configuration = new ConfigurationBuilder().Build();
        var mainDbConnectionString = configuration.GetConnectionString("MainDatabase");
        var rootPath = configuration["RootPath"];

        var testDbName = GenerateTestDatabaseName(ExtractDatabaseNameFromConnectionString(mainDbConnectionString!));
        var testDbConnectionString =
            new NpgsqlConnectionStringBuilder(mainDbConnectionString) { Database = testDbName, Pooling = false }.ConnectionString;

        configuration["ConnectionStrings:MainDatabase"] = testDbConnectionString;

        var administrationUsers = configuration.GetSection("AdministrationUsers").Get<List<AdministrationUsers>>();

        return new TestConfiguration(
            configuration,
            mainDbConnectionString!,
            testDbName,
            rootPath!,
            administrationUsers!
        );
    }

    public async ValueTask DisposeAsync()
    {
        var mainDbName = ExtractDatabaseNameFromConnectionString(_mainDbConnectionString);

        using (Database dbConection = new Database(_mainDbConnectionString))
        {
            await TrySQLAction(async () => {
                if (!(await dbConection.ExecuteAsync<bool>(GenerateSqlCommand("CHECK IF DB EXIST", DatabaseName, mainDbName))))
                    return;

                await dbConection.ExecuteAsync(GenerateSqlCommand("DROP", DatabaseName));
            });
        }
    }

    public async Task SetUpTestDatabaseAsync()
    {
        var mainDbName = ExtractDatabaseNameFromConnectionString(_mainDbConnectionString);
        await new TestMigrator().RunAsync(_mainDbConnectionString);

        using (Database dbConection = new Database(_mainDbConnectionString))
        {
            await TrySQLAction(async () => {
                await SeverOtherConnections(dbConection, mainDbName);
                if (await dbConection.ExecuteAsync<bool>(GenerateSqlCommand("CHECK IF DB EXIST", DatabaseName, mainDbName)))
                    return;

                await dbConection.ExecuteAsync(GenerateSqlCommand("CREATE", DatabaseName, mainDbName));
            });
        }
    }

    private async Task TrySQLAction<T>(Func<T> block)
    {
        var attempt = 0;

        do
        {
            try
            {
                block();
            }
            catch (PostgresException e)
            {
                if (e.SqlState == PostgresErrorCodes.AdminShutdown)
                {
                    attempt++;
                    await Task.Delay(TimeSpan.FromMilliseconds(250));
                }
                else
                {
                    throw;
                }
            }
        } while (attempt < 20);
    }

    private static async Task SeverOtherConnections(Database connection, string databaseName,
        CancellationToken cancellationToken = default)
    {
        await connection.ExecuteAsync(
            @"
                SELECT
	                pg_terminate_backend(pg_stat_activity.pid)
                FROM
	                pg_stat_activity
                WHERE
	                pg_stat_activity.datname = @db_name
	                AND pid <> pg_backend_pid()
            ",
            cancellationToken,
            (object)new DataParameter("db_name", databaseName)
        );
    }
    private static string GenerateTestDatabaseName(string prefix)
    {
        return $"{prefix}_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{Guid.NewGuid().ToString().Replace("-", "0")}";
    }

    private static string GenerateSqlCommand(string command, string databaseName, string? template = null)
    {
        switch (command)
        {
            case "DROP":
                return $"DROP DATABASE {databaseName} WITH (FORCE)";
            case "CREATE":
                return $"CREATE DATABASE {databaseName} TEMPLATE {template}";
            case "CHECK IF DB EXIST":
                return $"SELECT EXISTS (SELECT * FROM pg_database WHERE datname = '{databaseName}')";
            default:
                throw new InvalidOperationException("command not recognized");
        }
    }

    private static string ExtractDatabaseNameFromConnectionString(string connectionString)
    {
        var databaseName = new NpgsqlConnectionStringBuilder(connectionString).Database;

        if (databaseName == null)
            throw new InvalidOperationException("Database value was null in the connection string builder");
        else
            return databaseName;
    }
}
