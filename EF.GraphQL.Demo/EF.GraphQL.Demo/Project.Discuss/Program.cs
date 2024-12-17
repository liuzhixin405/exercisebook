
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;
using Project.Discuss.Domain;
using Project.Discuss.Models;
using NSwag.AspNetCore;
using NSwag;
using Project.Discuss.Domain.IServices;
using Project.Discuss.Services;
namespace Project.Discuss
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var modelBuilder = new ODataConventionModelBuilder();
            modelBuilder.EntityType<BbsArticle>();
            modelBuilder.EntityType<BbsComment>();
            modelBuilder.EntitySet<BbsArticleCoin>("BbsArticleCoins");
            builder.Services.AddControllers().AddOData(options =>
            options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(null).AddRouteComponents("odata", modelBuilder.GetEdmModel()));
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            
            builder.Services.AddScoped<DiscussDbContext>();
            builder.Services.AddScoped<ICommentService, CommentService>();
            builder.Services.AddOpenApiDocument(settings =>
            {
                settings.Title = "discuss";
               
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseOpenApi();
                app.UseSwaggerUi();
                app.UseReDoc(options =>
                {
                    options.Path = "/redoc";
                    options.UseModuleTypeForCustomJavaScript = true;
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
