namespace MarkAsPlayed.Foundation.Configuration;

public class Validation
{
    public async Task<string> GetConfigurationString(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"{path} file not found");

        var configString = await File.ReadAllTextAsync(path);
        if (configString == null)
            throw new ArgumentNullException("Missing configuration file");

        return configString;
    }
}
