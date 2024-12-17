using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;
using ODataService.WebNew.Models;

namespace ODataService.WebNew
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            //builder.Services.AddEndpointsApiExplorer();
            var modelBuilder = new ODataConventionModelBuilder();
            modelBuilder.EntityType<Order>();
            modelBuilder.EntitySet<Customer>("Customers");
            builder.Services.AddControllers().AddOData(options =>
            options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(null).AddRouteComponents("odata",modelBuilder.GetEdmModel()));
            //builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                //app.UseSwagger();
                //app.UseSwaggerUI();
            }
            app.UseRouting();
            //app.UseAuthorization();


            app.UseEndpoints(endpoints => endpoints.MapControllers());

            app.Run();
        }
    }
}
