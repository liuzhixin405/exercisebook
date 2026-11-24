using ColdHub.Shared;
using ColdHub.Source;
using ColdHub.Source.Connectors;
using ColdHub.Source.Publishing;
using ColdHub.Source.Valkey;
using Microsoft.Extensions.Options;

// Source：一个进程 = 一个已注册的源 + 一个连接器。把数据校验、编码后发到 Valkey 的 Reading:{Source}:{Device}。
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddOptions<SourceOptions>().BindConfiguration("Source").ValidateOnStart();
builder.Services.AddSingleton<IValidateOptions<SourceOptions>, SourceOptionsValidator>();
builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddValkey(builder.Configuration);
builder.Services.AddSingleton<IReadingPublisher, ValkeyPublisher>();
builder.Services.AddSingleton<ReadingPublisher>();
builder.Services.AddSingleton<ConfigHolder>();
builder.Services.AddSingleton(provider => ConnectorCatalog.Create(provider.GetRequiredService<IOptions<SourceOptions>>().Value.Connector));

// 调 Admin 的 HttpClient：地址和 service key 来自 Options，每个请求都带 Authorization: ApiKey
builder.Services.AddHttpClient<ConfigWatcher>((provider, client) =>
{
    SourceOptions options = provider.GetRequiredService<IOptions<SourceOptions>>().Value;
    client.BaseAddress = new Uri(options.AdminUrl.TrimEnd('/') + "/");
    client.Timeout = TimeSpan.FromSeconds(5);
    client.DefaultRequestHeaders.Authorization = new("ApiKey", options.ServiceKey);
});
builder.Services.AddHostedService(provider => provider.GetRequiredService<ConfigWatcher>());
builder.Services.AddHostedService<SourceWorker>();

builder.Services.AddHealthChecks().AddCheck<ConfigReadyCheck>("config", tags: [ServiceDefaults.ReadyTag]);

WebApplication app = builder.Build();

// 启动时就解析连接器：名字写错立刻失败，不是等到第一条数据
_ = app.Services.GetRequiredService<IConnector>();

app.MapDefaultEndpoints();
app.Run();
