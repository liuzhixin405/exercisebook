using IBuyStuff.Application.Services;
using IBuyStuff.Application.Services.Order;
using IBuyStuff.Domain.Repositories;
using IBuyStuff.Domain.Services;
using IBuyStuff.Domain.Services.Impl;
using IBuyStuff.Persistence.Facade;
using IBuyStuff.Persistence.Repositories;
using IBuyStuff.QueryModel;
using IBuyStuff.QueryModel.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IBuyStuffer.Server
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
            builder.Services.AddDbContext<QueryModelDatabase>(options => options.UseInMemoryDatabase("memorydb"), ServiceLifetime.Scoped);
            builder.Services.AddDbContext<CommandModelDatabase>(options => options.UseInMemoryDatabase("memorydb"), ServiceLifetime.Scoped);
            builder.Services.AddTransient<IProductRepository,ProductRepository>();
            builder.Services.AddTransient<ICustomerRepository,CustomerRepository>();
            builder.Services.AddTransient<ISubscriberRepository, SubscriberRepository>();
            builder.Services.AddTransient<IOrderRepository,OrderRepository>();
            builder.Services.AddTransient<ICatalogService ,CatalogService>();
            builder.Services.AddTransient<IOrderRequestService,OrderRequestService>();
            builder.Services.AddTransient<IShipmentService,ShipmentService>();

            builder.Services.AddTransient<IOrderControllerService, OrderControllerService>();
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
}