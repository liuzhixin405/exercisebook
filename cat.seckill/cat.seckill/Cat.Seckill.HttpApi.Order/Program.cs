
using Cat.Seckill.Base.EFCore;
using Cat.Seckill.Base.EFCore.Repository;
using Cat.Seckill.Base.EFCore.Service;
using Cat.Seckill.Base.RabbitMq;
using Cat.Seckill.Base.RabbitMq.Config;
using Cat.Seckill.Base.Redis;
using Cat.Seckill.Entities.BaseRepository;
using Cat.Seckill.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using static System.Collections.Specialized.BitVector32;

namespace Cat.Seckill.HttpApi.Order
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDbContext<SeckillDbContext>(op=>op.UseSqlServer("Data Source=PC-202205262203;Initial Catalog=seckilldb;Persist Security Info=False;User ID=sa;Password=1230;MultipleActiveResultSets=true;TrustServerCertificate=true"));
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddTransient<IRepository<Goods>,GoodsRepository>();
            builder.Services.AddTransient<IRepository<SeckillGoods>, SeckillGoodsRepository>();
            builder.Services.AddTransient<IRepository<Account>, UserRepository>();
            builder.Services.AddTransient<IRepository<OrderInfo>, OrderRepository>();

            ServiceProvider sp = builder.Services.BuildServiceProvider();
            var _config = sp.GetRequiredService<IConfiguration>();
            builder.Services.AddSingleton(new RedisHelper(_config.GetSection("Redis:Default").Get<RedisOption>()));
            builder.Services.AddSingleton(new RabbitConnection(_config.GetSection("RabbitMQ").Get<RabbitOption>()));
            builder.Services.AddScoped<IGoodsService, GoodsService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ISeckillService, SeckillService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddSingleton<IRabbitProducer, RabbitProducer>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.MapGet("/", async context =>
            {
                using var scope = context.RequestServices.CreateAsyncScope();

                var seckillContext = scope.ServiceProvider.GetRequiredService<SeckillDbContext>();
                if (!await (seckillContext.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).ExistsAsync()) //数据库不存在自动创建，并建表
                {
                    await seckillContext.Database.EnsureDeletedAsync();
                    await seckillContext.Database.EnsureCreatedAsync();

                }
                await context.Response.WriteAsync("db created");
            });
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}