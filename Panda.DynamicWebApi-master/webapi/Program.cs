using Panda.DynamicWebApi;
using System.Diagnostics.Metrics;
using System.Diagnostics;
using System.Reflection;
using Panda.DynamicWebApiSample.Dynamic;
using NetCore.AutoRegisterDi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.DocInclusionPredicate((docName, description) => true);
});
builder.Services.AddDynamicWebApiCore<ServiceLocalSelectController, ServiceActionRouteFactory>();
builder.Services.RegisterAssemblyPublicNonGenericClasses()
               .AsPublicImplementedInterfaces();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDynamicWebApi((serviceProvider, options) =>
{
    options.AddAssemblyOptions(Assembly.LoadFile(Directory.GetCurrentDirectory() + "\\clib.dll"));
    
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
