using LinqToDB;
using LinqToDB.Data;

namespace MarkAsPlayed.Foundation;
public sealed class Database : DataConnection
{
    public delegate Database Factory();

    public Database(string connectionString) : base(
        ProviderName.PostgreSQL95,
        connectionString
    )
    {
    }
}

