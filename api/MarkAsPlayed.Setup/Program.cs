using MarkAsPlayed.Foundation;
using MarkAsPlayed.Setup;
using System.Text.Json;

ConsoleExtension.WriteLine("Starting the setup", ConsoleColor.Green, true);

#region Read configuration
ConsoleExtension.WriteLine("- Read configuration");
var currentDirectory = Directory.GetCurrentDirectory();
var configString = File.ReadAllText($"{currentDirectory}/MarkAsPlayed.Api/appsettings.json");
if (configString == null)
    throw new ArgumentNullException("\u2716 Missing configuration file");

var config = JsonSerializer.Deserialize<Configuration>(configString);
ConsoleExtension.WriteLine("\u2714 Configuration resolved\n", ConsoleColor.Green);
#endregion

#region Checking configuration data
ConsoleExtension.WriteLine("- Checking database connection string");
var dbConnection = config?.ConnectionStrings?.MainDatabase;
if (dbConnection == null)
    throw new ArgumentNullException("\u2716 Missing database connection string");

ConsoleExtension.WriteLine("- Checking administration users data");
var administrationUsers = config?.AdministrationUsers;
if (administrationUsers == null || administrationUsers.Count == 0)
    throw new ArgumentNullException("\u2716 Missing administration users data");

ConsoleExtension.WriteLine("- Checking firebase project id");
var firebaseProjectId = config?.Firebase?.ProjectId;
if (firebaseProjectId == null)
    throw new ArgumentNullException("\u2716 Missing firebase project id");

ConsoleExtension.WriteLine("\u2714 Checking configuration data completed\n", ConsoleColor.Green);
#endregion

var migrator = new Migrator();

#region Database migrations
ConsoleExtension.WriteLine("- Start migrations");
var executedScripts = await migrator.RunAsync(dbConnection!);
ConsoleExtension.WriteLine("\u2714 Migrations completed\n", ConsoleColor.Green);
#endregion

var initializer = new Initializer();

#region Insert administration users
ConsoleExtension.WriteLine("- Insert administration users");
await initializer.InsertAdministrationUsers(config!.AdministrationUsers, dbConnection);
#endregion

ConsoleExtension.WriteLine("Setup completed", ConsoleColor.Green, true);
