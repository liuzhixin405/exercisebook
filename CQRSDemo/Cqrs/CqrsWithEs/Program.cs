using CqrsWithEs.DataAccess;
using CqrsWithEs.Init;
using CqrsWithEs.ReadModel;
using MediatR;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddMediatR(typeof(Program));
builder.Services.AddDataAccess();
builder.Services.AddDataInitializer();
builder.Services.AddReadModels(builder.Configuration.GetConnectionString("DefaultConnection"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseDataInitializer();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
