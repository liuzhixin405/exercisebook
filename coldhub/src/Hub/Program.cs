using ColdHub.Hub;
using ColdHub.Shared;
using Microsoft.Extensions.Options;

// Hub：对外的唯一入口。
//   /ws       实时订阅：Valkey 订阅 → 扇出给 WebSocket 客户端
//   /history  历史下载：读 Recorder 写的文件
// 客户端的 API key 在 Admin 里创建，Hub 只拿哈希来校验。
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddOptions<HubOptions>().BindConfiguration("Hub").ValidateOnStart();
builder.Services.AddSingleton<IValidateOptions<HubOptions>, HubOptionsValidator>();
builder.Services.AddValkey(builder.Configuration);
builder.Services.AddSingleton<SubscriptionHub>();

// 命名客户端 + 单例的 KeyDirectory：后台轮询的和端点里注入的必须是同一个实例
// （AddHttpClient<T> 会把 T 注册成瞬态，各拿各的，端点那份永远是空的）
builder.Services.AddHttpClient(KeyDirectory.AdminClient, (provider, client) =>
{
    HubOptions options = provider.GetRequiredService<IOptions<HubOptions>>().Value;
    client.BaseAddress = new Uri(options.AdminUrl.TrimEnd('/') + "/");
    client.Timeout = TimeSpan.FromSeconds(5);
    client.DefaultRequestHeaders.Authorization = new("ApiKey", options.ServiceKey);
});
builder.Services.AddSingleton<KeyDirectory>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<KeyDirectory>());
builder.Services.AddHealthChecks().AddCheck<KeysReadyCheck>("keys", tags: [ServiceDefaults.ReadyTag]);

WebApplication app = builder.Build();
app.UseWebSockets();
app.MapDefaultEndpoints();
app.MapSubscribe();
app.MapHistory();
app.Run();
