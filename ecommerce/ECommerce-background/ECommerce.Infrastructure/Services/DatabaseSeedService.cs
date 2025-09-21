using ECommerce.Domain.Entities;
using ECommerce.Domain.Models;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Services
{
    /// <summary>
    /// 数据库种子数据服务
    /// </summary>
    public class DatabaseSeedService
    {
        private readonly ECommerceDbContext _context;
        private readonly ILogger<DatabaseSeedService> _logger;

        public DatabaseSeedService(ECommerceDbContext context, ILogger<DatabaseSeedService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 执行种子数据填充
        /// </summary>
        public async Task SeedAsync()
        {
            try
            {
                _logger.LogInformation("开始填充种子数据...");

                // 检查是否已有数据
                var hasUsers = await _context.Users.AnyAsync();
                var hasProducts = await _context.Products.AnyAsync();

                if (hasUsers || hasProducts)
                {
                    _logger.LogInformation("数据库已包含数据，跳过种子数据填充");
                    return;
                }

                // 填充用户数据
                await SeedUsersAsync();

                // 填充产品数据
                await SeedProductsAsync();

                await _context.SaveChangesAsync();
                _logger.LogInformation("种子数据填充完成");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "填充种子数据时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 填充用户种子数据
        /// </summary>
        private async Task SeedUsersAsync()
        {
            var users = new List<User>
            {
                new User
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    UserName = "admin",
                    Email = "admin@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    FirstName = "Admin",
                    LastName = "User",
                    PhoneNumber = "+86-138-0000-0000",
                    Address = "北京市朝阳区某某街道123号",
                    Role = "Admin",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    UserName = "customer1",
                    Email = "customer1@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("customer123"),
                    FirstName = "John",
                    LastName = "Doe",
                    PhoneNumber = "+86-139-0000-0000",
                    Address = "上海市浦东新区某某路456号",
                    Role = "User",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            foreach (var user in users)
            {
                _context.Users.Add(user);
            }
            _logger.LogInformation("添加了 {Count} 个用户种子数据", users.Count);
        }

        /// <summary>
        /// 填充产品种子数据
        /// </summary>
        private async Task SeedProductsAsync()
        {
            var products = new List<Product>
            {
                new Product
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = "iPhone 15 Pro",
                    Description = "Latest iPhone with advanced features",
                    Price = 999.99m,
                    Stock = 50,
                    Category = "Electronics",
                    ImageUrl = "https://example.com/iphone15.jpg",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Name = "MacBook Pro",
                    Description = "Powerful laptop for professionals",
                    Price = 1999.99m,
                    Stock = 25,
                    Category = "Electronics",
                    ImageUrl = "https://example.com/macbook.jpg",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    Name = "Wireless Headphones",
                    Description = "High-quality wireless headphones",
                    Price = 199.99m,
                    Stock = 100,
                    Category = "Electronics",
                    ImageUrl = "https://example.com/headphones.jpg",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            foreach (var product in products)
            {
                _context.Products.Add(product);
            }
            _logger.LogInformation("添加了 {Count} 个产品种子数据", products.Count);
        }

        /// <summary>
        /// 强制重新填充种子数据（删除现有数据）
        /// </summary>
        public async Task ForceSeedAsync()
        {
            try
            {
                _logger.LogInformation("开始强制填充种子数据...");

                // 删除现有种子数据
                await ClearSeedDataAsync();

                // 重新填充
                await SeedAsync();

                _logger.LogInformation("强制种子数据填充完成");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "强制填充种子数据时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 清除种子数据
        /// </summary>
        private async Task ClearSeedDataAsync()
        {
            // 删除产品
            var products = await _context.Products.ToListAsync();
            if (products.Any())
            {
                _context.Products.RemoveRange(products);
                _logger.LogInformation("删除了 {Count} 个产品", products.Count);
            }

            // 删除用户（注意：这里只删除种子数据用户，实际项目中可能需要更谨慎）
            var seedUserIds = new[]
            {
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Guid.Parse("22222222-2222-2222-2222-222222222222")
            };

            var users = await _context.Users.Where(u => seedUserIds.Contains(u.Id)).ToListAsync();
            if (users.Any())
            {
                _context.Users.RemoveRange(users);
                _logger.LogInformation("删除了 {Count} 个种子用户", users.Count);
            }
        }
    }
}
