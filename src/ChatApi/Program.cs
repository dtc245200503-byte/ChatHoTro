using System.Text.Json.Serialization;
using ChatApi.Data;
using ChatApi.Endpoints;
using ChatApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Connection string 'DefaultConnection' is required. Configure ConnectionStrings__DefaultConnection.");
}

builder.Services.AddDbContext<ChatDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        sqlServer => sqlServer.EnableRetryOnFailure()));
builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddSignalR();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:5500", "http://localhost:8080"];
builder.Services.AddCors(options =>
    options.AddPolicy("Client", policy =>
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()));

var app = builder.Build();

await MigrateDatabaseAsync(app);

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseCors("Client");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => TypedResults.Ok(new { service = "ChatApi", status = "ready" }))
    .WithSummary("Check ChatApi readiness");
app.MapConversationEndpoints();

app.Run();

static async Task MigrateDatabaseAsync(WebApplication app)
{
    const int maxAttempts = 10;
    var logger = app.Services.GetRequiredService<ILogger<Program>>();

    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            await using var scope = app.Services.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("SQL Server database migration completed");
            return;
        }
        catch (Exception exception)
        {
            logger.LogWarning(
                exception,
                "Database migration attempt {Attempt}/{MaxAttempts} failed",
                attempt,
                maxAttempts);

            if (attempt == maxAttempts)
            {
                throw new InvalidOperationException(
                    $"Could not migrate the SQL Server database after {maxAttempts} attempts.",
                    exception);
            }

            logger.LogInformation("Retrying database migration in 3 seconds");
            await Task.Delay(TimeSpan.FromSeconds(3));
        }
    }
}
