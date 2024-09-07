using MarkAsPlayed.Foundation;
using MarkAsPlayed.Setup;
using Microsoft.Extensions.Logging;

//Starting setup
ILogger logger = new LoggerHelper().CreateLogger("Setup");
logger.LogInformation("Starting the setup");

//Read configuration
logger.LogInformation("Read configuration");
var config = await ConfigurationReader.ReadConfigurationAsync(logger);
logger.LogInformation($"{LoggerHelper.GreenColor}Configuration resolved{LoggerHelper.WhiteColor}");

//Validate configuration
ConfigurationReader.ValidateConfiguration(logger, config);
logger.LogInformation($"{LoggerHelper.GreenColor}Validating configuration data completed{LoggerHelper.WhiteColor}");

//Start migrations
logger.LogInformation($"Start migrations");
var migrator = new Migrator();
var executedScripts = await migrator.RunAsync(config.ConnectionStrings.MainDatabase, logger);
logger.LogInformation($"{LoggerHelper.GreenColor}Migrations completed{LoggerHelper.WhiteColor}");

//Initialize basic data
var initializer = new Initializer();

var usersInsertResult = await initializer.InsertAdministrationUsersAsync(
    config.AdministrationUsers, 
    config.ConnectionStrings.MainDatabase);
logger.LogInformation($"- {usersInsertResult} users inserted correctly");

var initialsResult = await initializer.InsertInitialsAsync(config.ConnectionStrings.MainDatabase);
foreach (var result in initialsResult)
    logger.LogInformation($"- {result.Value} for {result.Key} inserted correctly");

logger.LogInformation($"{LoggerHelper.GreenColor}Insert basic data completed{LoggerHelper.WhiteColor}");

//Setup completed
logger.LogInformation($"{LoggerHelper.GreenColor}Setup completed{LoggerHelper.WhiteColor}");
