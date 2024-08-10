using LinqToDB.Data;
using MarkAsPlayed.Foundation;
using MarkAsPlayed.Foundation.Configuration;
using Npgsql;
using System.Text.Json;

namespace MarkAsPlayed.Setup;

internal sealed class Initializer
{
    const string initialsPath = "/MarkAsPlayed.Setup/Initials";

    public async Task<int> InsertAdministrationUsersAsync(
        ICollection<AdministrationUsers> administrationUsers,
        string connectionString,
        CancellationToken cancellationToken = default)
    {
        var result = 0;

        try
        {
            using (Database dbConection = new Database(connectionString))
            {
                foreach (var user in administrationUsers.Distinct())
                {
                    try
                    {
                        await dbConection.ExecuteAsync(
                            "INSERT INTO author " +
                            "(firebase_id, name, description_pl, description_en) " +
                            "VALUES " +
                            $"('{user.FirebaseId}', '{user.Name}', '{user.DescriptionPl}', '{user.DescriptionEn}')",
                            cancellationToken);
                    }
                    catch (PostgresException e)
                    {
                        if (e.SqlState == PostgresErrorCodes.UniqueViolation)
                            continue;
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }

                    result++;
                }
            }
        }
        catch (Exception e) 
        { 
            throw new Exception(e.Message);
        }

        return result;
    }

    public async Task<Dictionary<string,int>> InsertInitialsAsync(
        string connectionString,
        CancellationToken cancellationToken = default)
    {
        var result = new Dictionary<string,int>();

        try
        {
            using (Database dbConection = new Database(connectionString))
            {
                var currentDirectory = Directory.GetCurrentDirectory();
                var initials = Directory.GetFiles($"{currentDirectory}{initialsPath}");

                if (initials.Length == 0)
                    return result;

                foreach (var initial in initials)
                    await InsertInitialAsync(initial, dbConection, result);
            }
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }

        return result;
    }

    private async Task InsertInitialAsync(
        string initial, 
        Database dbConection,
        Dictionary<string, int> result,
        CancellationToken cancellationToken = default)
    {
        var counter = 0;
        var tableName = Path.GetFileNameWithoutExtension(initial);

        var fileText = File.ReadAllText(initial);
        if (fileText == null)
            throw new ArgumentNullException($"Missing text in {initial}");

        var initialRecords = JsonSerializer.Deserialize<List<InitialRecord>>(fileText);

        if (initialRecords == null || initialRecords.Count == 0)
            result.Add(tableName, counter);

        foreach (var record in initialRecords!.Distinct())
        {
            try
            {
                await dbConection.ExecuteAsync(
                        $"INSERT INTO {tableName} " +
                        "(name, group_sign) " +
                        "VALUES " +
                        $"('{record.Name}', '{record.GroupSign}')",
                        cancellationToken);

                counter++;
            }
            catch (PostgresException e)
            {
                if (e.SqlState == PostgresErrorCodes.UniqueViolation)
                    continue;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        result.Add(tableName, counter);
    }
}
