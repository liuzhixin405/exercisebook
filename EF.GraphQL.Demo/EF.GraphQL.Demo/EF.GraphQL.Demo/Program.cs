using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using NSwag;
using NSwag.AspNetCore;
using NSwag.AspNetCore.Middlewares;

namespace EF.GraphQL.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<CustomDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddGraphQLServer().AddQueryType<Query>().AddMutationType<Mutation>().RegisterDbContextFactory<CustomDbContext>().AddProjections().AddFiltering().AddSorting();
            builder.Services.AddOpenApiDocument(settings =>
            {
                settings.Title = "graphql-demo.Api";
              
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseOpenApi();
            app.UseSwaggerUi();
            app.UseHttpsRedirection();

            //app.UseAuthorization();
            app.MapControllers();
            app.MapGraphQL("/graphql");
            app.Run();
        }
    }
}
