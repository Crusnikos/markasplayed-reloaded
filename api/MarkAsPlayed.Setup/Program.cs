using MarkAsPlayed.Foundation.Logger;
using MarkAsPlayed.Setup;
using Microsoft.Extensions.Logging;

//Starting setup
ILogger logger = new LoggerConsoleProvider(null!).CreateLogger("Setup");
logger.LogInformation("Starting the setup");

//Read configuration
logger.LogInformation("Read configuration");
var config = await ConfigurationReader.ReadConfigurationAsync(logger);
logger.LogInformation($"{LoggerConsoleProvider.GreenColor}Configuration resolved{LoggerConsoleProvider.WhiteColor}");

//Validate configuration
ConfigurationReader.ValidateConfiguration(logger, config);
logger.LogInformation($"{LoggerConsoleProvider.GreenColor}Validating configuration data completed{LoggerConsoleProvider.WhiteColor}");

//Start migrations
logger.LogInformation($"Start migrations");
var migrator = new Migrator();
var executedScripts = await migrator.RunAsync(config.ConnectionStrings.MainDatabase, logger);
logger.LogInformation($"{LoggerConsoleProvider.GreenColor}Migrations completed{LoggerConsoleProvider.WhiteColor}");

//Initialize basic data
var initializer = new Initializer();

var usersInsertResult = await initializer.InsertAdministrationUsersAsync(
    config.AdministrationUsers, 
    config.ConnectionStrings.MainDatabase);
logger.LogInformation($"- {usersInsertResult} user(s) inserted correctly");

var initialsResult = await initializer.InsertInitialsAsync(config.ConnectionStrings.MainDatabase);
foreach (var result in initialsResult)
    logger.LogInformation($"- {result.Value} {result.Key}(s) inserted correctly");

logger.LogInformation($"{LoggerConsoleProvider.GreenColor}Insert basic data completed{LoggerConsoleProvider.WhiteColor}");

//Setup completed
logger.LogInformation($"{LoggerConsoleProvider.GreenColor}Setup completed{LoggerConsoleProvider.WhiteColor}");
