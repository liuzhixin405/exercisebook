namespace ColdHub.Admin;

/// <summary>某个 Source 的运行配置。Source 进程轮询它并热更新。</summary>
public sealed class SourceConfig
{
    public List<string> Devices { get; set; } = [];

    /// <summary>连接器每隔多久读一次传感器。</summary>
    public int IntervalMs { get; set; } = 1000;

    /// <summary>每个设备每秒最多发布几条，0 表示不限。</summary>
    public int MaxPerSecond { get; set; }
}

/// <summary>
/// 注册表里的一个源。名字注册后永不复用：退役（Retired）只是打标记，名字依然占着。
/// 注意连接器（Connector）只是登记信息；真正用哪个连接器由 Source 进程的 Source__Connector 决定。
/// </summary>
public sealed class SourceEntry
{
    public string Name { get; set; } = "";
    public string Connector { get; set; } = "";
    public bool Retired { get; set; }
    public int Version { get; set; } = 1;
    public SourceConfig Config { get; set; } = new();
}

public sealed class KeyEntry
{
    public string Id { get; set; } = "";
    public string Owner { get; set; } = "";
    public string Hash { get; set; } = "";
    public bool Revoked { get; set; }
}

public sealed class State
{
    public List<SourceEntry> Sources { get; set; } = [];
    public List<KeyEntry> Keys { get; set; } = [];
}
