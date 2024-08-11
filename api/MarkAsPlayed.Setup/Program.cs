using MarkAsPlayed.Foundation;
using MarkAsPlayed.Setup;
using Newtonsoft.Json;

ConsoleExtension.WriteLine("Starting the setup", ConsoleColor.Green, true);

#region Read configuration
ConsoleExtension.WriteLine("- Read configuration");

var configurationFilePath = $"{Directory.GetCurrentDirectory()}/MarkAsPlayed.Api/appsettings.json";

if (!File.Exists(configurationFilePath))
    throw new FileNotFoundException($"{configurationFilePath} file not found");

var configString = await File.ReadAllTextAsync(configurationFilePath);
if (configString == null)
    throw new ArgumentNullException("Missing configuration file");

var config = JsonConvert.DeserializeObject<Configuration>(configString);

ConsoleExtension.WriteLine("\u2714 Configuration resolved\n", ConsoleColor.Green);
#endregion

#region Checking configuration data
ConsoleExtension.WriteLine("- Checking database connection string");
var dbConnection = config?.ConnectionStrings?.MainDatabase;
if (dbConnection == null)
    throw new ArgumentNullException("Missing database connection string");

ConsoleExtension.WriteLine("- Checking administration users data");
var administrationUsers = config?.AdministrationUsers;
if (administrationUsers == null || administrationUsers.Count == 0)
    throw new ArgumentNullException("Missing administration users data");

ConsoleExtension.WriteLine("- Checking firebase project id");
if (config?.Firebase?.ProjectId == null)
    throw new ArgumentNullException("Missing firebase project id");

ConsoleExtension.WriteLine("\u2714 Checking configuration data completed\n", ConsoleColor.Green);
#endregion

var migrator = new Migrator();

#region Database migrations
ConsoleExtension.WriteLine("- Start migrations");

var executedScripts = await migrator.RunAsync(dbConnection!);

ConsoleExtension.WriteLine("\u2714 Migrations completed\n", ConsoleColor.Green);
#endregion

var initializer = new Initializer();

#region Insert basic data
var usersInsertResult = await initializer.InsertAdministrationUsersAsync(administrationUsers, dbConnection);
ConsoleExtension.WriteLine($"- {usersInsertResult} users inserted correctly");

var initialsResult = await initializer.InsertInitialsAsync(dbConnection);
foreach (var result in initialsResult)
    ConsoleExtension.WriteLine($"- {result.Value} for {result.Key} inserted correctly");

ConsoleExtension.WriteLine($"\u2714 Insert basic data completed", ConsoleColor.Green);
#endregion

ConsoleExtension.WriteLine("Setup completed", ConsoleColor.Green, true);
