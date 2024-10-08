﻿using Newtonsoft.Json;

namespace MarkAsPlayed.Foundation;

public class Configuration
{
    [JsonConstructor]
    public Configuration()
    {
        
    }

    public ConnectionStrings ConnectionStrings { get; set; } = default!;
    public Cors Cors { get; set; } = default!;
    public Logging Logging { get; set; } = default!;
    public string FrontendUrl { get; set; } = default!;
    public string AllowedHosts { get; set; } = default!;
    public string RootPath { get; set; } = default!;
    public Firebase Firebase { get; set; } = default!;
    public bool TestingEnvironment { get; set; }
    public ICollection<AdministrationUsers> AdministrationUsers { get; set; } = new List<AdministrationUsers>();
    public int DefaultPageSize { get; set; }
}

public class AdministrationUsers
{
    public string FirebaseId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string DescriptionPl { get; set; } = default!;
    public string DescriptionEn { get; set; } = default!;
}

public class Firebase
{
    public string ProjectId { get; set; } = default!;
    public string TestUserId { get; set; } = default!;
}

public class Console
{
    public bool IncludeScopes { get; set; }
}

public class Logging
{
    public LogLevel LogLevel { get; set; } = default!;
    public Console Console { get; set; } = default!;
    public LoggerDatabase LoggerDatabase { get; set; } = default!;
    public LoggerConsole LoggerConsole { get; set; } = default!;
}

public class LoggerDatabase
{
    public OptionsDatabase OptionsDatabase { get; set; } = default!;
}

public class LoggerConsole
{
    public OptionsConsole OptionsConsole { get; set; } = default!;
}

public class OptionsConsole
{
    public bool IncludeScopes { get; set; }
    public bool SingleLine { get; set; }
    public string TimestampFormat { get; set; } = default!;
    public int ColorBehavior { get; set; }
}

public class OptionsDatabase
{
    public ICollection<string> LogFields { get; set; } = new List<string>();
    public string LogTable { get; set; } = default!;
}

public class LogLevel
{
    public string Default { get; set; } = default!;
    public string System { get; set; } = default!;
    public string AspNetCore { get; set; } = default!;
}

public class Cors
{
    public ICollection<string> ExposedHeaders { get; set; } = new List<string>();
}

public class ConnectionStrings
{
    public string MainDatabase { get; set; } = default!;
}