namespace MiniGin;

/// <summary>
/// MiniGin 工厂方法
/// </summary>
public static class Gin
{
    /// <summary>
    /// 创建默认引擎（包含 Logger 和 Recovery 中间件）
    /// </summary>
    public static Engine Default()
    {
        var engine = new Engine();
        engine.Use(Middleware.Logger(), Middleware.Recovery());
        return engine;
    }

    /// <summary>
    /// 创建空白引擎（不包含任何中间件）
    /// </summary>
    public static Engine New()
    {
        return new Engine();
    }
}
