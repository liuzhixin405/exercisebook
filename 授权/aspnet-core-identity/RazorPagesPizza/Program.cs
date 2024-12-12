using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using RazorPagesPizza.Areas.Identity.Data;
using RazorPagesPizza.Services;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("RazorPagesPizzaAuthConnection") ?? throw new InvalidOperationException("Connection string 'RazorPagesPizzaAuthConnection' not found.");

builder.Services.AddDbContext<RazorPagesPizzaAuth>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<RazorPagesPizzaUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<RazorPagesPizzaAuth>();

// Add services to the container.
builder.Services.AddRazorPages(options =>
    options.Conventions.AuthorizePage("/AdminsOnly", "Admin"));
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddSingleton(new QRCodeService(new QRCodeGenerator()));
builder.Services.AddAuthorization(options => options.AddPolicy("Admin", policy => policy.RequireAuthenticatedUser().RequireClaim("IsAdmin", bool.TrueString)));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
