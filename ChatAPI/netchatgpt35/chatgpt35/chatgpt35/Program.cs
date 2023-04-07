
using Microsoft.Extensions.DependencyInjection;
using OpenAI.GPT3.Extensions;

namespace chatgpt35
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
            builder.Services.AddOpenAIService(settings => { settings.ApiKey = "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"; });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseMiddleware<HttpContextMiddleware>();
            app.MapControllers();

            app.Run();
        }
    }
}