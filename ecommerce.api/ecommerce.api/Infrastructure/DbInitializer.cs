using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Infrastructure
{
    public interface IDbInitializer
    {
        Task InitializeAsync();
    }

    public class DbInitializer : IDbInitializer
    {
        private readonly ECommerceDbContext _context;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(ECommerceDbContext context, ILogger<DbInitializer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                _logger.LogInformation("Starting database initialization...");

                // 检查数据库连接
                var canConnect = await _context.Database.CanConnectAsync();
                if (!canConnect)
                {
                    _logger.LogInformation("Database does not exist. Creating database...");
                    await _context.Database.EnsureCreatedAsync();
                    _logger.LogInformation("Database created successfully.");
                }
                else
                {
                    _logger.LogInformation("Database already exists.");
                }

                // 执行迁移（如果使用了迁移）
                var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    _logger.LogInformation("Applying pending migrations...");
                    await _context.Database.MigrateAsync();
                    _logger.LogInformation("Migrations applied successfully.");
                }

                // 验证表是否创建
                var orderTableExists = await CheckTableExistsAsync("orders");
                if (orderTableExists)
                {
                    _logger.LogInformation("Orders table verified.");
                }

                _logger.LogInformation("Database initialization completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during database initialization");
                throw;
            }
        }

        private async Task<bool> CheckTableExistsAsync(string tableName)
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = '{tableName}'";
                    var result = await command.ExecuteScalarAsync();
                    return result != null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if table {TableName} exists", tableName);
                return false;
            }
        }
    }
}
