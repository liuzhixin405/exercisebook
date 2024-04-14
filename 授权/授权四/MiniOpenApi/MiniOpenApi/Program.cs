using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using MiniOpenApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthorization();
// Add services to the container.
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

app.Use(async (context, next) => {

    var ep = context.GetEndpoint();
    var allow = ep?.Metadata?.Any(t => t.GetType() == typeof(AllowAnonymousAttribute)) == true;
    if (!allow)
    {
        var cookie = context.Request.Cookies.FirstOrDefault(t => t.Key == "cookie").Value;
        if (cookie != null)
        {
            var ticket = Helper.DeSerialize(cookie);
            context.User = ticket.Principal;
        }
        else
        {
            // ���δ��¼������ 401 Unauthorized ״̬��
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("no login");
            return; // �����������󣬲���������һ���м��
        }
    }
    await next();

});

app.MapGet("/Logout", (HttpContext context) =>
{
    // ����ͻ��˵���֤ƾ�ݣ�����ɾ�� cookie
    context.Response.Cookies.Delete("cookie");

    // ����������˵��û���֤״̬����������Ự��Ϣ
    // ������Ը���ʵ��������в�������������û�����֤��Ϣ�����Ƶ�

    return "logout success";
})
.WithName("logout")
.WithOpenApi();

app.MapPost("/Login", (HttpContext context,string username,string password) =>
{
    if (username == "admin" && password == "123456")
    {
        var claims = new Claim[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role,"admin")
        };
        var identity = new ClaimsIdentity(claims);
        var princple = new ClaimsPrincipal(identity);

        context.Response.Cookies.Append("cookie", Helper.Serialize(princple));
        return "success";
    }
    context.Response.StatusCode = 500;

    return "login error";
}).AllowAnonymous()
.WithName("login")
.WithOpenApi();


app.MapGet("/Access", (HttpContext context) =>
{
    return context.User.Identity.Name;
})
.WithName("access")
.WithOpenApi();


app.Run();