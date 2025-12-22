using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ECommerce.API.Infrastructure
{
    public class MigrationHelper
    {
        public static async Task EnsureDatabaseCreatedAsync(ECommerceDbContext context, ILogger logger)
        {
            try
            {
                logger.LogInformation("Checking database existence...");

                // 尝试连接到数据库
                if (!await context.Database.CanConnectAsync())
                {
                    logger.LogInformation("Database does not exist. Creating database and tables...");
                    await context.Database.EnsureCreatedAsync();
                }

                // 获取待执行的迁移
                var pendingMigrations = (await context.Database.GetPendingMigrationsAsync()).ToList();
                if (pendingMigrations.Count > 0)
                {
                    logger.LogInformation("Found {Count} pending migrations. Applying migrations...", pendingMigrations.Count);
                    foreach (var migration in pendingMigrations)
                    {
                        logger.LogInformation("Applying migration: {MigrationName}", migration);
                    }
                    await context.Database.MigrateAsync();
                    logger.LogInformation("All migrations applied successfully.");
                }
                else
                {
                    logger.LogInformation("No pending migrations found.");
                }

                logger.LogInformation("Database is ready.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error ensuring database creation");
                throw;
            }
        }
    }
}
