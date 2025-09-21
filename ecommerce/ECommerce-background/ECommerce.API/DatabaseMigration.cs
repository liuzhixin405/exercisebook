using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ECommerce.API
{
    /// <summary>
    /// 数据库迁移和初始化服务
    /// </summary>
    public class DatabaseMigrationService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseMigrationService> _logger;

        public DatabaseMigrationService(IServiceProvider serviceProvider, ILogger<DatabaseMigrationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("开始数据库迁移和初始化...");

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();

            try
            {
                // 1. 确保数据库存在
                await EnsureDatabaseCreatedAsync(context);

                // 2. 执行迁移
                await MigrateDatabaseAsync(context);

                // 3. 验证数据库连接
                await ValidateDatabaseConnectionAsync(context);

                _logger.LogInformation("数据库迁移和初始化完成");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "数据库迁移和初始化失败");
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 确保数据库存在
        /// </summary>
        private async Task EnsureDatabaseCreatedAsync(ECommerceDbContext context)
        {
            try
            {
                _logger.LogInformation("检查数据库是否存在...");
                
                // 检查数据库连接
                var canConnect = await context.Database.CanConnectAsync();
                if (!canConnect)
                {
                    _logger.LogInformation("数据库不存在，正在创建...");
                    await context.Database.EnsureCreatedAsync();
                    _logger.LogInformation("数据库创建成功");
                }
                else
                {
                    _logger.LogInformation("数据库已存在");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建数据库时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 执行数据库迁移
        /// </summary>
        private async Task MigrateDatabaseAsync(ECommerceDbContext context)
        {
            try
            {
                _logger.LogInformation("开始执行数据库迁移...");
                
                // 获取待执行的迁移
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                
                if (pendingMigrations.Any())
                {
                    _logger.LogInformation("发现 {Count} 个待执行的迁移: {Migrations}", 
                        pendingMigrations.Count(), string.Join(", ", pendingMigrations));
                    
                    await context.Database.MigrateAsync();
                    _logger.LogInformation("数据库迁移执行完成");
                }
                else
                {
                    _logger.LogInformation("没有待执行的迁移");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "执行数据库迁移时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 验证数据库连接
        /// </summary>
        private async Task ValidateDatabaseConnectionAsync(ECommerceDbContext context)
        {
            try
            {
                _logger.LogInformation("验证数据库连接...");
                
                // 执行简单查询验证连接
                var userCount = await context.Users.CountAsync();
                var productCount = await context.Products.CountAsync();
                
                _logger.LogInformation("数据库连接验证成功 - 用户数: {UserCount}, 产品数: {ProductCount}", 
                    userCount, productCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "验证数据库连接时发生错误");
                throw;
            }
        }
    }

    /// <summary>
    /// 数据库初始化扩展方法
    /// </summary>
    public static class DatabaseInitializationExtensions
    {
        /// <summary>
        /// 添加数据库迁移服务
        /// </summary>
        public static IServiceCollection AddDatabaseMigration(this IServiceCollection services)
        {
            services.AddHostedService<DatabaseMigrationService>();
            return services;
        }

        /// <summary>
        /// 手动执行数据库迁移（用于开发环境）
        /// </summary>
        public static async Task MigrateDatabaseAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();
            
            // 确保数据库存在
            await context.Database.EnsureCreatedAsync();
            
            // 执行迁移
            await context.Database.MigrateAsync();
        }

        /// <summary>
        /// 重置数据库（删除并重新创建）
        /// </summary>
        public static async Task ResetDatabaseAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();
            
            // 删除数据库
            await context.Database.EnsureDeletedAsync();
            
            // 重新创建数据库
            await context.Database.EnsureCreatedAsync();
        }
    }
}
