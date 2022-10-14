using LiveScore_ES.Backend.DAL;
using LiveScore_ES.Config;
using LiveScore_ES.Framework;
using LiveScore_ES.Framework.Commands;
using LiveScore_ES.Framework.Sagas;
using LiveScore_ES.Services.Home;
using LiveScore_ES.Services.Live;
using Microsoft.EntityFrameworkCore;

namespace LiveScore_ES
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<WaterPoloContext>(options => options.UseSqlServer("Data Source=PC-202205262203;Initial Catalog=live-es;Persist Security Info=False;User ID=sa;Password=1230;MultipleActiveResultSets=true"));
            builder.Services.AddTransient<EventRepository>();
            builder.Services.AddTransient<HomeService>();
            builder.Services.AddTransient<LiveService>();
            
      
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            SagaConfig.Initialize();
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