namespace Framework.Samples.Singletons;

/// <summary>
/// 配置管理器 - 单例模式示例（饿汉式）
/// </summary>
public sealed class ConfigurationManager
{
    // 静态实例，在类加载时创建
    private static readonly ConfigurationManager _instance = new ConfigurationManager();

    // 私有构造函数，防止外部实例化
    private ConfigurationManager()
    {
        Console.WriteLine("[单例示例] ConfigurationManager 实例已创建（饿汉式）");
        LoadConfiguration();
    }

    // 公共静态属性，提供全局访问点
    public static ConfigurationManager Instance => _instance;

    public Dictionary<string, string> Settings { get; private set; } = new();

    private void LoadConfiguration()
    {
        // 模拟加载配置
        Settings["AppName"] = "Framework Sample";
        Settings["Version"] = "1.0.0";
        Settings["MaxConnections"] = "100";
    }

    public string GetSetting(string key)
    {
        return Settings.TryGetValue(key, out var value) ? value : string.Empty;
    }

    public void SetSetting(string key, string value)
    {
        Settings[key] = value;
        Console.WriteLine($"[单例示例] 配置已更新: {key} = {value}");
    }
}

/// <summary>
/// 日志管理器 - 单例模式示例（懒汉式 + 线程安全）
/// </summary>
public sealed class LogManager
{
    private static LogManager? _instance;
    private static readonly object _lock = new object();

    private readonly List<string> _logs = new();

    // 私有构造函数
    private LogManager()
    {
        Console.WriteLine("[单例示例] LogManager 实例已创建（懒汉式 + 双重检查锁）");
    }

    // 双重检查锁定（Double-Check Locking）
    public static LogManager Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new LogManager();
                    }
                }
            }
            return _instance;
        }
    }

    public void Log(string message)
    {
        var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
        lock (_lock)
        {
            _logs.Add(logEntry);
        }
        Console.WriteLine($"[单例示例] {logEntry}");
    }

    public IReadOnlyList<string> GetLogs()
    {
        lock (_lock)
        {
            return _logs.ToList();
        }
    }

    public void ClearLogs()
    {
        lock (_lock)
        {
            _logs.Clear();
        }
        Console.WriteLine("[单例示例] 日志已清空");
    }
}

/// <summary>
/// 数据库连接池 - 单例模式示例（使用 Lazy<T>）
/// </summary>
public sealed class DatabaseConnectionPool
{
    // 使用 Lazy<T> 实现线程安全的懒加载
    private static readonly Lazy<DatabaseConnectionPool> _instance =
        new Lazy<DatabaseConnectionPool>(() => new DatabaseConnectionPool());

    private readonly List<DatabaseConnection> _connections = new();
    private readonly int _maxConnections = 10;

    private DatabaseConnectionPool()
    {
        Console.WriteLine("[单例示例] DatabaseConnectionPool 实例已创建（Lazy<T>）");
        InitializePool();
    }

    public static DatabaseConnectionPool Instance => _instance.Value;

    private void InitializePool()
    {
        for (int i = 0; i < _maxConnections; i++)
        {
            _connections.Add(new DatabaseConnection($"Connection_{i + 1}"));
        }
        Console.WriteLine($"[单例示例] 连接池已初始化，包含 {_maxConnections} 个连接");
    }

    public DatabaseConnection GetConnection()
    {
        var availableConnection = _connections.FirstOrDefault(c => !c.IsInUse);
        if (availableConnection != null)
        {
            availableConnection.IsInUse = true;
            Console.WriteLine($"[单例示例] 获取连接: {availableConnection.Id}");
            return availableConnection;
        }
        throw new InvalidOperationException("没有可用的数据库连接");
    }

    public void ReleaseConnection(DatabaseConnection connection)
    {
        if (connection != null && _connections.Contains(connection))
        {
            connection.IsInUse = false;
            Console.WriteLine($"[单例示例] 释放连接: {connection.Id}");
        }
    }

    public int AvailableConnections => _connections.Count(c => !c.IsInUse);
    public int TotalConnections => _connections.Count;
}

/// <summary>
/// 数据库连接
/// </summary>
public class DatabaseConnection
{
    public string Id { get; }
    public bool IsInUse { get; set; }

    public DatabaseConnection(string id)
    {
        Id = id;
        IsInUse = false;
    }

    public void Execute(string query)
    {
        Console.WriteLine($"[单例示例] {Id} 执行查询: {query}");
    }
}
