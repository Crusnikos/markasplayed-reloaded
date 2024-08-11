using MarkAsPlayed.Foundation;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MSLogging = Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Config check

var config = builder.Configuration.Get<Configuration>();
if (config == null)
    throw new ArgumentNullException(nameof(config));

// Add services to the container

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    var frontendURL = config.FrontendUrl;
    var exposedHeaders = config.Cors.ExposedHeaders.ToArray();

    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins(frontendURL).
            AllowAnyMethod().
            AllowAnyHeader().
            WithExposedHeaders(exposedHeaders);
    });
});

builder.Services.AddHealthChecks();
builder.Services.AddSingleton<Database.Factory>(_ => () => new Database(config.ConnectionStrings.MainDatabase));

builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(config.RootPath, "xml")))
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
        var projectId = config.Firebase.ProjectId;
        options.Authority = "https://securetoken.google.com/" + projectId;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://securetoken.google.com/" + projectId,
            ValidateAudience = true,
            ValidAudience = projectId,
            ValidateLifetime = true,
        };
    });

var loggingConfiguration = builder.Configuration.GetSection("Logging");
builder.Services.
    AddLogging(builder =>
    {
        builder
            .AddDebug()
            .AddConsole()
            .AddConfiguration(loggingConfiguration)
            .SetMinimumLevel(MSLogging.LogLevel.Information);
    });

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
    FileProvider = new PhysicalFileProvider(Path.Combine(config.RootPath, "Image")),
    RequestPath = "/Image"
});

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
