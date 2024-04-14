using DesignPatternsForDatabase.Factory;
using DesignPatternsForDatabase.Provider;
using Microsoft.AspNetCore.OpenApi;
using System.Data;
using System.Data.Common;

namespace DesignPatternsForDatabase
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

            var app = builder.Build();
            if (GlobalConfigure.GlobalServiceProvider == null)
                GlobalConfigure.GlobalServiceProvider = app.Services;
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.MapGet("/",(context) =>
            {
                Database db = DatabaseFactory.Create("LocalSQL");
                db.BeforeExecution += ModityCommandTimeOut;
                db.AfterExecution += TraceExecuteCommand;
                DataSet dataSet = db.ExecuteDataSet("select * from shippers");
                return context.Response.WriteAsJsonAsync("hello world");
            });
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

        private static void ModityCommandTimeOut(object sender,DbEventArgs args)
        {
            IDbCommand dbCommand = args.Command;
            dbCommand.CommandTimeout = 1000;
        }
        private static void TraceExecuteCommand(object sender,DbEventArgs args)
        {
            DbCommand dbCommand = args.Command;
            System.Diagnostics.Trace.WriteLine(dbCommand.CommandText);
            System.Diagnostics.Trace.WriteLine(dbCommand.CommandTimeout);
        }
    }
}