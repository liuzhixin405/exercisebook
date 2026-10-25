using Microsoft.EntityFrameworkCore;
using AiDevPlatform.Infrastructure.Data;

namespace AiDevPlatform.API.Extensions;

public static class WebApplicationExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AiDevPlatformDbContext>();

        try
        {
            // For in-memory database, just ensure it's created
            await context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }
    }
}
