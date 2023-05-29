
using Microsoft.AspNetCore.Authentication.Cookies;

namespace auth_cookie
{
    /// <summary>
    /// һ���򵥵�Cookie�����֤����Ȩʾ��
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // ����Cookie�����֤
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "YourAuthCookie"; // ����Cookie������
                    options.LoginPath = "/api/Auth/Login"; // ���õ�¼·��
                });

            // ������Ȩ����
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
            app.UseAuthentication(); // ���������֤
            app.UseAuthorization(); // ������Ȩ


            app.MapControllers();

            app.Run();
        }
    }
}