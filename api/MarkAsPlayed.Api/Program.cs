using MarkAsPlayed.Foundation;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.DependencyInjection;

const string googleSecureUrl = "https://securetoken.google.com/";

var builder = WebApplication.CreateBuilder(args);

#region Config check
// Config check

var config = builder.Configuration.Get<Configuration>();
if (config == null)
    throw new ArgumentNullException(nameof(config));
#endregion

#region Add services to the container
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

builder.Services.AddScoped<LoggerHelper>();
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
    FileProvider = new PhysicalFileProvider(Path.Combine(config.RootPath, "Image")),
    RequestPath = "/Image"
});

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
