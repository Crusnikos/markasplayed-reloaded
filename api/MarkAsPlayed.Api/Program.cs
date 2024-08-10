using MarkAsPlayed.Foundation;
using MarkAsPlayed.Foundation.Configuration;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration.Get<Configuration>();

if (config == null)
    throw new ArgumentNullException(nameof(config));

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

var app = builder.Build();

app.MapHealthChecks("/health");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
