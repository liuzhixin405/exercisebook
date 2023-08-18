using grpc_service_web.Services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddCors(o=>o.AddPolicy("AllowAll",builder=>{
    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
}));
builder.WebHost.ConfigureKestrel(options=>{
    options.Listen(System.Net.IPAddress.Any,5157,listenOptions=>{
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
    });
    options.Listen(System.Net.IPAddress.Any,5158,listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
    });

});
var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapGrpcService<HelloWorldService>();
app.UseGrpcWeb(new GrpcWebOptions() { DefaultEnabled = true });
app.UseCors();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
