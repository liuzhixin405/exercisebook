using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RazorPagesPizza.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using RazorPagesPizza.Services;
using QRCoder;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("RazorPagesPizzaAuthConnection");
 builder.Services.AddDbContext<RazorPagesPizzaAuth>(options =>
    options.UseSqlServer(connectionString)); 
    builder.Services.AddDefaultIdentity<RazorPagesPizzaUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<RazorPagesPizzaAuth>();
// Add services to the container.
builder.Services.AddRazorPages(options =>
    options.Conventions.AuthorizePage("/AdminsOnly", "Admin"));
builder.Services.AddTransient<IEmailSender,EmailSender>();
builder.Services.AddSingleton(new QRCodeService(new QRCodeGenerator()));

builder.Services.AddAuthorization(options =>
    options.AddPolicy("Admin", policy =>
        policy.RequireAuthenticatedUser()
            .RequireClaim("IsAdmin", bool.TrueString)));
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
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

/*

CONTENTS: Please confirm your account by <a href='https://localhost:7192/Identity/Account/ConfirmEmail?userId=14cdf053-56cf-4866-9437-4ca74174beec&amp;code=Q2ZESjhFTWc1Nks2WGxoQnA1VzRoY2VUSzJzV0lhMGhXTEt3ZWEvV1BGcC94WEgzSXNMbEJxai9MWWZ1VTh2YzlKOElWY1lUKy9lbzJqYTFjWmVNa2I1Q0lQK1NMWDFIVGVMK0JFNTJrZUNUTGhDU3lSU0R3S0J1OHovTjBiQlllb0owTjVTZExxT0M2a0NTcHpNZFJldEhrZ3JvaDd3UmY5SFJjSW9JdW16YkR5YjFjd0VZSkorOXFsamV4UzBnMDVYRzEvZHVkOVVlWXNsL1VDREJBN3ViMTMxQUxjaXUvZlY2QkpjMGt3ZnZuUmdGNmZ0WmdJYUtodUhCTm9hTmx6RVp6QT09'>clicking here</a>.

这里确认email不会跳转，
需要把https://localhost:7192/Identity/Account/ConfirmEmail?userId=14cdf053-56cf-4866-9437-4ca74174beec&amp;code=
改成  https://localhost:7192/Identity/Account/ConfirmEmail?userId=14cdf053-56cf-4866-9437-4ca74174beec&code= 
*/