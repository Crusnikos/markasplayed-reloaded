using MarkAsPlayed.Foundation;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.FileProviders;
using MarkAsPlayed.Foundation.Logger;
using Microsoft.Extensions.Options;

const string googleSecureUrl = "https://securetoken.google.com/";

var builder = WebApplication.CreateBuilder(args);

#region Config check
// Config check

var configuration = builder.Configuration.Get<Configuration>();
if (configuration == null)
    throw new ArgumentNullException(nameof(configuration));
#endregion

#region Add services to the container
// Add services to the container

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    var frontendURL = configuration.FrontendUrl;
    var exposedHeaders = configuration.Cors.ExposedHeaders.ToArray();

    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins(frontendURL).
            AllowAnyMethod().
            AllowAnyHeader().
            WithExposedHeaders(exposedHeaders);
    });
});

builder.Services.AddHealthChecks();
builder.Services.AddSingleton<Database.Factory>(_ => () => new Database(configuration.ConnectionStrings.MainDatabase));

builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(configuration.RootPath, "xml")))
        .UseCryptographicAlgorithms(
        new AuthenticatedEncryptorConfiguration
        {
            EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
            ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
        });

builder.Services.
    AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
    AddJwtBearer(options =>
    {
        var projectId = configuration.Firebase.ProjectId;
        options.Authority = googleSecureUrl + projectId;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = googleSecureUrl + projectId,
            ValidateAudience = true,
            ValidAudience = projectId,
            ValidateLifetime = true,
        };
    });

builder.Logging.ClearProviders();
builder.Services.AddSingleton<ILoggerProvider>(config =>
{
    var appSettings = new LoggerConsoleOptions() { 
        ColorBehavior = configuration.Logging.LoggerConsole.OptionsConsole.ColorBehavior,
        IncludeScopes = configuration.Logging.LoggerConsole.OptionsConsole.IncludeScopes,
        SingleLine = configuration.Logging.LoggerConsole.OptionsConsole.SingleLine,
        TimestampFormat = configuration.Logging.LoggerConsole.OptionsConsole.TimestampFormat,
    };
    IOptions<LoggerConsoleOptions> options = Options.Create(appSettings);

    return new LoggerConsoleProvider(options!);
});
builder.Services.AddSingleton<ILoggerProvider>(config =>
{
    var appSettings = new LoggerDatabaseOptions() {
        LogTable = configuration.Logging.LoggerDatabase.OptionsDatabase.LogTable,
        LogFields = configuration.Logging.LoggerDatabase.OptionsDatabase.LogFields.ToArray(),
        ConnectionString = configuration.ConnectionStrings.MainDatabase
    };
    IOptions<LoggerDatabaseOptions> options = Options.Create(appSettings);

    return new LoggerDatabaseProvider(options!);
});
#endregion

// Build

var app = builder.Build();

app.MapHealthChecks("/health");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(configuration.RootPath, "Image")),
    RequestPath = "/Image"
});

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
