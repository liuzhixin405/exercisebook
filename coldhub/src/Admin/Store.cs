using System.Text.Json;
using ColdHub.Shared;
using Microsoft.Extensions.Options;

namespace ColdHub.Admin;

/// <summary>
/// 用一个 JSON 文件当数据库，足够演示"注册表 + 配置 + key 哈希"。换成真正的数据库也不影响上层。
/// 所有读写走同一把锁；每次修改立刻落盘。
/// </summary>
public sealed class Store
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    private readonly Lock _gate = new();
    private readonly string _file;
    private readonly State _state;

    public Store(IOptions<AdminOptions> options)
    {
        _file = options.Value.DataFile;
        _state = File.Exists(_file)
            ? JsonSerializer.Deserialize<State>(File.ReadAllText(_file), Json) ?? new State()
            : Seed(options.Value);
        Save();
    }

    /// <summary>首次启动：放两个源和一个客户端 key，让 demo 开箱即用。</summary>
    private static State Seed(AdminOptions options) => new()
    {
        Sources =
        [
            new SourceEntry
            {
                Name = "warehouse-a", Connector = "sine",
                Config = new SourceConfig { Devices = ["freezer-1", "freezer-2", "chiller-1"], IntervalMs = 500 }
            },
            new SourceEntry
            {
                Name = "warehouse-b", Connector = "random",
                Config = new SourceConfig { Devices = ["dock-1", "dock-2"], IntervalMs = 500, MaxPerSecond = 1 }
            }
        ],
        Keys = [new KeyEntry { Id = "bootstrap", Owner = "demo", Hash = ApiKeys.Hash(options.BootstrapClientKey) }]
    };

    public T Read<T>(Func<State, T> read)
    {
        lock (_gate)
        {
            return read(_state);
        }
    }

    /// <summary>在锁内改，改完落盘；改的函数返回 false 表示没改动，不写盘。</summary>
    public T Write<T>(Func<State, (T Result, bool Changed)> change)
    {
        lock (_gate)
        {
            (T result, bool changed) = change(_state);

            if (changed)
            {
                Save();
            }

            return result;
        }
    }

    private void Save() => File.WriteAllText(_file, JsonSerializer.Serialize(_state, Json));
}
