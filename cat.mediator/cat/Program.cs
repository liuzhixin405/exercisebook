
using cat.Data;
using cat.DbProvider;
using cat.Events;
using cat.Globals.Exceptions;
using cat.Repositories;
using MediatR;

namespace cat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers(options=>options.Filters.Add<GlobalExceptionFilter>());
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
          
            builder.Services.AddTransient<IContextProvider, ContextProvider>();
            builder.Services.AddTransient<IRepository, Repository>();
            builder.Services.AddMediatR(typeof(IRepository));
          
            InitData.InitializationDb();
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