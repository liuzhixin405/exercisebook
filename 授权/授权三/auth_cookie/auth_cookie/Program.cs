
using Microsoft.AspNetCore.Authentication.Cookies;

namespace auth_cookie
{
    /// <summary>
    /// 一个简单的Cookie身份验证和授权示例
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // 配置Cookie身份验证
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "YourAuthCookie"; // 设置Cookie的名称
                    options.LoginPath = "/api/Auth/Login"; // 设置登录路径
                });

            // 配置授权服务
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
            });
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

            app.UseHttpsRedirection();
            app.UseAuthentication(); // 启用身份验证
            app.UseAuthorization(); // 启用授权


            app.MapControllers();

            app.Run();
        }
    }
}