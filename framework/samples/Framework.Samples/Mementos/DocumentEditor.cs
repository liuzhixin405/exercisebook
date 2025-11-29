using Framework.Infrastructure.Memento;

namespace Framework.Samples.Mementos;

/// <summary>
/// 文档编辑器 - 备忘录模式示例
/// </summary>
public class DocumentEditor
{
    private readonly IMementoManager _mementoManager;
    private DocumentState _currentState;

    public DocumentEditor(IMementoManager mementoManager)
    {
        _mementoManager = mementoManager ?? throw new ArgumentNullException(nameof(mementoManager));
        _currentState = new DocumentState();
    }

    public string Content
    {
        get => _currentState.Content;
        set
        {
            _currentState.Content = value;
            Console.WriteLine($"[备忘录示例] 内容已更新: {value.Substring(0, Math.Min(50, value.Length))}...");
        }
    }

    public string Title
    {
        get => _currentState.Title;
        set
        {
            _currentState.Title = value;
            Console.WriteLine($"[备忘录示例] 标题已更新: {value}");
        }
    }

    public DateTime LastModified => _currentState.LastModified;

    /// <summary>
    /// 保存当前状态
    /// </summary>
    public string Save()
    {
        _currentState.LastModified = DateTime.UtcNow;
        var memento = _mementoManager.SaveState(_currentState.Clone());
        Console.WriteLine($"[备忘录示例] 状态已保存: {memento.Id}");
        return memento.Id;
    }

    /// <summary>
    /// 恢复到指定状态
    /// </summary>
    public void Restore(string mementoId)
    {
        var memento = _mementoManager.GetMemento(mementoId);
        if (memento != null)
        {
            _currentState = _mementoManager.RestoreState<DocumentState>(memento);
            Console.WriteLine($"[备忘录示例] 状态已恢复: {mementoId}");
            Console.WriteLine($"  标题: {_currentState.Title}");
            Console.WriteLine($"  内容长度: {_currentState.Content.Length}");
        }
    }

    /// <summary>
    /// 获取所有保存的版本
    /// </summary>
    public IEnumerable<IMemento> GetVersionHistory()
    {
        return _mementoManager.GetAllMementos();
    }
}

/// <summary>
/// 文档状态
/// </summary>
public class DocumentState
{
    public string Title { get; set; } = "未命名文档";
    public string Content { get; set; } = string.Empty;
    public DateTime LastModified { get; set; } = DateTime.UtcNow;

    public DocumentState Clone()
    {
        return new DocumentState
        {
            Title = this.Title,
            Content = this.Content,
            LastModified = this.LastModified
        };
    }
}

/// <summary>
/// 游戏存档管理器 - 备忘录模式示例
/// </summary>
public class GameSaveManager
{
    private readonly IMementoManager _mementoManager;
    private GameState _currentState;

    public GameSaveManager(IMementoManager mementoManager)
    {
        _mementoManager = mementoManager ?? throw new ArgumentNullException(nameof(mementoManager));
        _currentState = new GameState
        {
            PlayerName = "玩家1",
            Level = 1,
            Score = 0,
            Position = new Position { X = 0, Y = 0 }
        };
    }

    public void Play()
    {
        _currentState.Score += 100;
        _currentState.Position.X += 10;
        _currentState.Position.Y += 5;
        Console.WriteLine($"[备忘录示例] 游戏进行中... 得分: {_currentState.Score}, 位置: ({_currentState.Position.X}, {_currentState.Position.Y})");
    }

    public void LevelUp()
    {
        _currentState.Level++;
        Console.WriteLine($"[备忘录示例] 升级！当前等级: {_currentState.Level}");
    }

    public string SaveGame()
    {
        var memento = _mementoManager.SaveState(_currentState.Clone());
        Console.WriteLine($"[备忘录示例] 游戏已保存: {memento.CreatedAt:yyyy-MM-dd HH:mm:ss}");
        return memento.Id;
    }

    public void LoadGame(string saveId)
    {
        var memento = _mementoManager.GetMemento(saveId);
        if (memento != null)
        {
            _currentState = _mementoManager.RestoreState<GameState>(memento);
            Console.WriteLine($"[备忘录示例] 游戏已加载:");
            Console.WriteLine($"  玩家: {_currentState.PlayerName}");
            Console.WriteLine($"  等级: {_currentState.Level}");
            Console.WriteLine($"  得分: {_currentState.Score}");
            Console.WriteLine($"  位置: ({_currentState.Position.X}, {_currentState.Position.Y})");
        }
    }

    public void DisplayCurrentState()
    {
        Console.WriteLine($"[备忘录示例] 当前游戏状态:");
        Console.WriteLine($"  玩家: {_currentState.PlayerName}");
        Console.WriteLine($"  等级: {_currentState.Level}");
        Console.WriteLine($"  得分: {_currentState.Score}");
        Console.WriteLine($"  位置: ({_currentState.Position.X}, {_currentState.Position.Y})");
    }
}

/// <summary>
/// 游戏状态
/// </summary>
public class GameState
{
    public string PlayerName { get; set; } = string.Empty;
    public int Level { get; set; }
    public int Score { get; set; }
    public Position Position { get; set; } = new Position();

    public GameState Clone()
    {
        return new GameState
        {
            PlayerName = this.PlayerName,
            Level = this.Level,
            Score = this.Score,
            Position = new Position { X = this.Position.X, Y = this.Position.Y }
        };
    }
}

/// <summary>
/// 位置
/// </summary>
public class Position
{
    public int X { get; set; }
    public int Y { get; set; }
}
