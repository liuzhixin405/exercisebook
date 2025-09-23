using GrpcRealtimePush.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
// 添加gRPC反射服务
builder.Services.AddGrpcReflection();
// Configure CORS policy for gRPC-Web
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding", "Content-Type");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

// Enable CORS
app.UseCors("AllowAll");

// Enable gRPC-Web middleware
app.UseGrpcWeb();

// Configure HTTPS redirection (required for gRPC-Web)
app.UseHttpsRedirection();

// Map gRPC services with gRPC-Web support
app.MapGrpcService<ChatService>().EnableGrpcWeb();
// 启用gRPC反射服务
app.MapGrpcReflectionService();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
