using MarkAsPlayed.Foundation;
using Npgsql;

namespace MarkAsPlayed.Setup;

internal class Initializer
{
    public async Task InsertAdministrationUsers(
        ICollection<AdministrationUsers> administrationUsers,
        string connectionString,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using (Database dbConection = new Database(connectionString))
            {
                foreach (var user in administrationUsers.Distinct())
                {
                    
                }
            }
        }
        catch (PostgresException e)
        {
            if (e.SqlState == PostgresErrorCodes.AdminShutdown)
                throw;
            else
                throw new Exception(e.Message);
        }

        ConsoleExtension.WriteLine("\u2714 Users inserted correctly", ConsoleColor.Green);

        return;
    }
}
