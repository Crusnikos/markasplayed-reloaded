using MarkAsPlayed.Setup;
using Newtonsoft.Json.Linq;

ConsoleExtension.WriteLine("Starting the setup", ConsoleColor.Green, true);

ConsoleExtension.WriteLine("- Parsing database string");
var currentDirectory = Directory.GetCurrentDirectory();
var json = File.ReadAllText($"{currentDirectory}/MarkAsPlayed.Api/appsettings.json");
var dbConnection = (string?)JObject.Parse(json)["ConnectionStrings"]!["MainDatabase"];

if (dbConnection == null)
    throw new ArgumentNullException("\u2716 Missing connection string");

ConsoleExtension.WriteLine("- Start migrations");
var migrator = new Migrator();
var executedScripts = await migrator.RunAsync(dbConnection!);


ConsoleExtension.WriteLine("Setup completed", ConsoleColor.Green, true);
