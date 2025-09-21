using ECommerce.Infrastructure.Data;
using ECommerce.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ECommerce.API
{
    /// <summary>
    /// 数据库初始化命令行工具
    /// </summary>
    public class DatabaseInitTool
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DatabaseInitTool> _logger;

        public DatabaseInitTool(IConfiguration configuration, ILogger<DatabaseInitTool> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// 执行数据库初始化
        /// </summary>
        public async Task InitializeDatabaseAsync(string[] args)
        {
            var command = args.Length > 0 ? args[0].ToLower() : "migrate";

            _logger.LogInformation("数据库初始化工具启动，命令: {Command}", command);

            try
            {
                switch (command)
                {
                    case "migrate":
                        await MigrateDatabaseAsync();
                        break;
                    case "reset":
                        await ResetDatabaseAsync();
                        break;
                    case "seed":
                        await SeedDatabaseAsync();
                        break;
                    case "force-seed":
                        await ForceSeedDatabaseAsync();
                        break;
                    case "status":
                        await CheckDatabaseStatusAsync();
                        break;
                    case "help":
                        ShowHelp();
                        break;
                    default:
                        _logger.LogError("未知命令: {Command}", command);
                        ShowHelp();
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "执行命令 {Command} 时发生错误", command);
                throw;
            }
        }

        /// <summary>
        /// 迁移数据库
        /// </summary>
        private async Task MigrateDatabaseAsync()
        {
            _logger.LogInformation("开始迁移数据库...");

            using var context = CreateDbContext();
            
            // 确保数据库存在
            await context.Database.EnsureCreatedAsync();
            
            // 执行迁移
            await context.Database.MigrateAsync();
            
            _logger.LogInformation("数据库迁移完成");
        }

        /// <summary>
        /// 重置数据库
        /// </summary>
        private async Task ResetDatabaseAsync()
        {
            _logger.LogInformation("开始重置数据库...");

            using var context = CreateDbContext();
            
            // 删除数据库
            await context.Database.EnsureDeletedAsync();
            
            // 重新创建数据库
            await context.Database.EnsureCreatedAsync();
            
            _logger.LogInformation("数据库重置完成");
        }

        /// <summary>
        /// 填充种子数据
        /// </summary>
        private async Task SeedDatabaseAsync()
        {
            _logger.LogInformation("开始填充种子数据...");

            using var context = CreateDbContext();
            
            // 创建种子数据服务
            using var loggerFactory = LoggerFactory.Create(builder =>
                builder.AddConsole().SetMinimumLevel(LogLevel.Information));
            var seedLogger = loggerFactory.CreateLogger<DatabaseSeedService>();
            var seedService = new DatabaseSeedService(context, seedLogger);
            
            // 执行种子数据填充
            await seedService.SeedAsync();
            
            _logger.LogInformation("种子数据填充完成");
        }

        /// <summary>
        /// 强制重新填充种子数据
        /// </summary>
        private async Task ForceSeedDatabaseAsync()
        {
            _logger.LogInformation("开始强制填充种子数据...");

            using var context = CreateDbContext();
            
            // 创建种子数据服务
            using var loggerFactory = LoggerFactory.Create(builder =>
                builder.AddConsole().SetMinimumLevel(LogLevel.Information));
            var seedLogger = loggerFactory.CreateLogger<DatabaseSeedService>();
            var seedService = new DatabaseSeedService(context, seedLogger);
            
            // 强制重新填充种子数据
            await seedService.ForceSeedAsync();
            
            _logger.LogInformation("强制种子数据填充完成");
        }

        /// <summary>
        /// 检查数据库状态
        /// </summary>
        private async Task CheckDatabaseStatusAsync()
        {
            _logger.LogInformation("检查数据库状态...");

            using var context = CreateDbContext();
            
            try
            {
                // 检查连接
                var canConnect = await context.Database.CanConnectAsync();
                _logger.LogInformation("数据库连接状态: {Status}", canConnect ? "正常" : "失败");

                if (canConnect)
                {
                    // 检查表
                    var tables = await context.Database.GetDbConnection().GetSchemaAsync("Tables");
                    _logger.LogInformation("数据库表数量: {Count}", tables.Rows.Count);

                    // 检查数据
                    var userCount = await context.Users.CountAsync();
                    var productCount = await context.Products.CountAsync();
                    var orderCount = await context.Orders.CountAsync();
                    
                    _logger.LogInformation("数据统计:");
                    _logger.LogInformation("  用户数: {UserCount}", userCount);
                    _logger.LogInformation("  产品数: {ProductCount}", productCount);
                    _logger.LogInformation("  订单数: {OrderCount}", orderCount);

                    // 检查迁移状态
                    var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                    if (pendingMigrations.Any())
                    {
                        _logger.LogWarning("发现 {Count} 个待执行的迁移: {Migrations}", 
                            pendingMigrations.Count(), string.Join(", ", pendingMigrations));
                    }
                    else
                    {
                        _logger.LogInformation("数据库迁移状态: 最新");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "检查数据库状态时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 显示帮助信息
        /// </summary>
        private void ShowHelp()
        {
            Console.WriteLine("数据库初始化工具使用说明:");
            Console.WriteLine();
            Console.WriteLine("命令:");
            Console.WriteLine("  migrate     - 迁移数据库（默认）");
            Console.WriteLine("  reset       - 重置数据库（删除并重新创建）");
            Console.WriteLine("  seed        - 填充种子数据（如果数据库为空）");
            Console.WriteLine("  force-seed  - 强制重新填充种子数据（删除现有种子数据）");
            Console.WriteLine("  status      - 检查数据库状态");
            Console.WriteLine("  help        - 显示此帮助信息");
            Console.WriteLine();
            Console.WriteLine("使用示例:");
            Console.WriteLine("  dotnet run -- migrate");
            Console.WriteLine("  dotnet run -- reset");
            Console.WriteLine("  dotnet run -- seed");
            Console.WriteLine("  dotnet run -- force-seed");
            Console.WriteLine("  dotnet run -- status");
        }

        /// <summary>
        /// 创建数据库上下文
        /// </summary>
        private ECommerceDbContext CreateDbContext()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            
            var options = new DbContextOptionsBuilder<ECommerceDbContext>()
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .Options;

            return new ECommerceDbContext(options);
        }
    }

    /// <summary>
    /// 程序入口点（用于数据库初始化工具）
    /// </summary>
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // 检查是否是数据库初始化命令
            if (args.Length > 0 && args[0].StartsWith("db-"))
            {
                await RunDatabaseInitTool(args);
                return;
            }

            // 正常的应用程序启动
            var builder = WebApplication.CreateBuilder(args);
            
            // 添加服务...
            // (这里包含正常的 Program.cs 内容)
            
            var app = builder.Build();
            await app.RunAsync();
        }

        private static async Task RunDatabaseInitTool(string[] args)
        {
            // 构建配置
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            // 构建日志
            using var loggerFactory = LoggerFactory.Create(builder =>
                builder.AddConsole().SetMinimumLevel(LogLevel.Information));

            var logger = loggerFactory.CreateLogger<DatabaseInitTool>();

            // 创建工具实例并执行
            var tool = new DatabaseInitTool(configuration, logger);
            
            // 移除 "db-" 前缀
            var cleanArgs = args.Select(arg => arg.StartsWith("db-") ? arg.Substring(3) : arg).ToArray();
            
            await tool.InitializeDatabaseAsync(cleanArgs);
        }
    }
}
