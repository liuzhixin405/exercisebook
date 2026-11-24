using ColdHub.Recorder;
using ColdHub.Shared;
using Microsoft.Extensions.Options;

// Recorder：订阅所有读数，按源按天追加到 .csv.gz 历史文件。读的一侧（下载）在 Hub。读写分开：
// 写的进程压力是持续的写，读的进程是偶发的大下载，互不拖累。
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddOptions<RecorderOptions>().BindConfiguration("Recorder").ValidateOnStart();
builder.Services.AddSingleton<IValidateOptions<RecorderOptions>, RecorderOptionsValidator>();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddValkey(builder.Configuration);
builder.Services.AddHostedService<RecorderWorker>();

WebApplication app = builder.Build();
app.MapDefaultEndpoints();
app.Run();
