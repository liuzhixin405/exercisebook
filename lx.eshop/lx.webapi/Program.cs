using lx.eshop.business.Basket;
using lx.eshop.models.Basket;
using lx.eshop.models.Catalog.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Polly;
using StackExchange.Redis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace lx.webapi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddTransient<IBasketRepository, BasketRepository>();
            builder.Services.AddEntityFrameworkSqlServer()
            .AddDbContext<CatalogContext>(options =>
            {
                options.UseSqlServer(builder.Configuration["ConnectionString"],
                                        sqlServerOptionsAction: sqlOptions =>
                                        {
                                            sqlOptions.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                                            //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                                            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                        });
            });
            builder.Services.MigrateDbContext<CatalogContext>((context, services) =>
            {

            });
            builder.Services.AddSingleton<ConnectionMultiplexer>(sp =>
            {
               
                var configuration = ConfigurationOptions.Parse(builder.Configuration["redisConn"], true);

                return ConnectionMultiplexer.Connect(configuration);
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

          }

    public static class Extensions
    {
        public static IServiceCollection MigrateDbContext<TContext>(this IServiceCollection sc, Action<TContext, IServiceProvider> seeder) where TContext : DbContext
        {

            using var scope = sc.BuildServiceProvider().CreateScope();
            var services = scope.ServiceProvider;

            var logger = services.GetRequiredService<ILogger<TContext>>();

            var context = services.GetService<TContext>();

            try
            {
                logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);



                var retry = Policy.Handle<SqlException>()
                        .WaitAndRetry(new TimeSpan[]
                        {
                        TimeSpan.FromSeconds(3),
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(8),
                        });

                //if the sql server container is not created on run docker compose this
                //migration can't fail for network related exception. The retry options for DbContext only 
                //apply to transient exceptions
                // Note that this is NOT applied when running some orchestrators (let the orchestrator to recreate the failing service)
                retry.Execute(() => InvokeSeeder(seeder, context, services));


                logger.LogInformation("Migrated database associated with context {DbContextName}", typeof(TContext).Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(TContext).Name);

            }

            return sc;
        }

        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context, IServiceProvider services)
            where TContext : DbContext
        {
            //context.Database.Migrate();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            seeder(context, services);
        }

    }
}