using Microsoft.EntityFrameworkCore;

namespace DataConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<ContextFacade>(options => options.UseInMemoryDatabase("inmdb_01"));
            builder.Services.AddDbContext<QueryModelDatabase>(options => options.UseInMemoryDatabase("inmdb_02"));
            builder.Services.AddControllers();
            var host = builder.Build();
            host.MapGet("/", context=>
            {
                using var scope =host.Services.CreateAsyncScope();
                var dbContext= scope.ServiceProvider.GetRequiredService<ContextFacade>();
                dbContext.Add<Customer>(new Customer { Id=1,Name
                ="Jack"});
                dbContext.Order.Add(new Order { Id = 1, CustomerId = 1 });
                dbContext.SaveChanges();
                 
                var customer = dbContext.Find<Customer>(new object[] {1});
                return context.Response.WriteAsync($"hello world,{customer?.Name},orderid={dbContext.Order?.FirstOrDefault()?.Id??0}");
            });

            host.Run();
        }
    }
}