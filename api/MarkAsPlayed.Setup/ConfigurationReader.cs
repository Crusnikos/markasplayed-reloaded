using MarkAsPlayed.Foundation;
using MarkAsPlayed.Foundation.Logger;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MarkAsPlayed.Setup;

internal static class ConfigurationReader
{
    public static async Task<Configuration> ReadConfigurationAsync(ILogger logger)
    {
        var message = string.Empty;

        var configurationFilePath = $"{Directory.GetCurrentDirectory()}/MarkAsPlayed.Api/appsettings.json";
        if (!File.Exists(configurationFilePath))
        {
            message = $"{configurationFilePath} file not found";
            logger.LogError(message);
            throw new ArgumentNullException(message);
        }

        var configString = await File.ReadAllTextAsync(configurationFilePath);
        if (configString == null)
        {
            message = "Missing configuration file";
            logger.LogError(message);
            throw new ArgumentNullException(message);
        }

        var config = JsonConvert.DeserializeObject<Configuration>(configString);
        return await Task.FromResult(config!);
    }

    public static void ValidateConfiguration(ILogger logger, Configuration? config) {
        var message = string.Empty;

        logger.LogInformation("- Checking database connection string");
        var dbConnection = config?.ConnectionStrings?.MainDatabase;
        if (dbConnection == null)
        {
            message = "Missing database connection string";
            logger.LogError($"{LoggerHelper.RedColor}{message}{LoggerHelper.WhiteColor}");
            throw new ArgumentNullException(message);
        } 

        logger.LogInformation("- Checking administration users data");
        var administrationUsers = config?.AdministrationUsers;
        if (administrationUsers == null || administrationUsers.Count == 0)
        {
            message = "Missing administration users data";
            logger.LogError($"{LoggerHelper.RedColor}{message}{LoggerHelper.WhiteColor}");
            throw new ArgumentNullException(message);
        }

        logger.LogInformation("- Checking firebase project id");
        if (config?.Firebase?.ProjectId == null)
        {
            message = "Missing firebase project id";
            logger.LogError($"{LoggerHelper.RedColor}{message}{LoggerHelper.WhiteColor}");
            throw new ArgumentNullException(message);
        }
    }
}
