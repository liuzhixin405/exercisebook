namespace Framework.Samples.Factories;

/// <summary>
/// 通知类型
/// </summary>
public enum NotificationChannel
{
    Email,
    SMS,
    Push,
    WeChat
}

/// <summary>
/// 通知接口
/// </summary>
public interface INotification
{
    string Channel { get; }
    Task SendAsync(string recipient, string message);
}

/// <summary>
/// 邮件通知
/// </summary>
public class EmailNotification : INotification
{
    public string Channel => "Email";

    public Task SendAsync(string recipient, string message)
    {
        Console.WriteLine($"[工厂示例] 发送邮件到 {recipient}");
        Console.WriteLine($"  内容: {message}");
        return Task.CompletedTask;
    }
}

/// <summary>
/// 短信通知
/// </summary>
public class SmsNotification : INotification
{
    public string Channel => "SMS";

    public Task SendAsync(string recipient, string message)
    {
        Console.WriteLine($"[工厂示例] 发送短信到 {recipient}");
        Console.WriteLine($"  内容: {message}");
        return Task.CompletedTask;
    }
}

/// <summary>
/// 推送通知
/// </summary>
public class PushNotification : INotification
{
    public string Channel => "Push";

    public Task SendAsync(string recipient, string message)
    {
        Console.WriteLine($"[工厂示例] 发送推送到 {recipient}");
        Console.WriteLine($"  内容: {message}");
        return Task.CompletedTask;
    }
}

/// <summary>
/// 微信通知
/// </summary>
public class WeChatNotification : INotification
{
    public string Channel => "WeChat";

    public Task SendAsync(string recipient, string message)
    {
        Console.WriteLine($"[工厂示例] 发送微信消息到 {recipient}");
        Console.WriteLine($"  内容: {message}");
        return Task.CompletedTask;
    }
}

/// <summary>
/// 通知工厂 - 简单工厂模式
/// </summary>
public class NotificationFactory
{
    public static INotification CreateNotification(NotificationChannel channel)
    {
        return channel switch
        {
            NotificationChannel.Email => new EmailNotification(),
            NotificationChannel.SMS => new SmsNotification(),
            NotificationChannel.Push => new PushNotification(),
            NotificationChannel.WeChat => new WeChatNotification(),
            _ => throw new ArgumentException($"不支持的通知渠道: {channel}")
        };
    }
}

/// <summary>
/// 日志记录器接口
/// </summary>
public interface ILogger
{
    string Type { get; }
    void Log(string message);
}

/// <summary>
/// 文件日志记录器
/// </summary>
public class FileLogger : ILogger
{
    public string Type => "File";

    public void Log(string message)
    {
        Console.WriteLine($"[工厂示例] [文件日志] {message}");
    }
}

/// <summary>
/// 数据库日志记录器
/// </summary>
public class DatabaseLogger : ILogger
{
    public string Type => "Database";

    public void Log(string message)
    {
        Console.WriteLine($"[工厂示例] [数据库日志] {message}");
    }
}

/// <summary>
/// 云日志记录器
/// </summary>
public class CloudLogger : ILogger
{
    public string Type => "Cloud";

    public void Log(string message)
    {
        Console.WriteLine($"[工厂示例] [云日志] {message}");
    }
}

/// <summary>
/// 日志工厂接口 - 工厂方法模式
/// </summary>
public abstract class LoggerFactory
{
    public abstract ILogger CreateLogger();

    public void LogMessage(string message)
    {
        var logger = CreateLogger();
        logger.Log(message);
    }
}

/// <summary>
/// 文件日志工厂
/// </summary>
public class FileLoggerFactory : LoggerFactory
{
    public override ILogger CreateLogger()
    {
        Console.WriteLine("[工厂示例] 创建文件日志记录器");
        return new FileLogger();
    }
}

/// <summary>
/// 数据库日志工厂
/// </summary>
public class DatabaseLoggerFactory : LoggerFactory
{
    public override ILogger CreateLogger()
    {
        Console.WriteLine("[工厂示例] 创建数据库日志记录器");
        return new DatabaseLogger();
    }
}

/// <summary>
/// 云日志工厂
/// </summary>
public class CloudLoggerFactory : LoggerFactory
{
    public override ILogger CreateLogger()
    {
        Console.WriteLine("[工厂示例] 创建云日志记录器");
        return new CloudLogger();
    }
}

/// <summary>
/// UI组件接口
/// </summary>
public interface IButton
{
    void Render();
    void Click();
}

public interface ITextBox
{
    void Render();
    void Input(string text);
}

/// <summary>
/// Windows风格组件
/// </summary>
public class WindowsButton : IButton
{
    public void Render() => Console.WriteLine("[抽象工厂示例] 渲染 Windows 风格按钮");
    public void Click() => Console.WriteLine("[抽象工厂示例] Windows 按钮被点击");
}

public class WindowsTextBox : ITextBox
{
    public void Render() => Console.WriteLine("[抽象工厂示例] 渲染 Windows 风格文本框");
    public void Input(string text) => Console.WriteLine($"[抽象工厂示例] Windows 文本框输入: {text}");
}

/// <summary>
/// Mac风格组件
/// </summary>
public class MacButton : IButton
{
    public void Render() => Console.WriteLine("[抽象工厂示例] 渲染 Mac 风格按钮");
    public void Click() => Console.WriteLine("[抽象工厂示例] Mac 按钮被点击");
}

public class MacTextBox : ITextBox
{
    public void Render() => Console.WriteLine("[抽象工厂示例] 渲染 Mac 风格文本框");
    public void Input(string text) => Console.WriteLine($"[抽象工厂示例] Mac 文本框输入: {text}");
}

/// <summary>
/// UI工厂接口 - 抽象工厂模式
/// </summary>
public interface IUIFactory
{
    IButton CreateButton();
    ITextBox CreateTextBox();
}

/// <summary>
/// Windows UI工厂
/// </summary>
public class WindowsUIFactory : IUIFactory
{
    public IButton CreateButton()
    {
        Console.WriteLine("[抽象工厂示例] Windows工厂创建按钮");
        return new WindowsButton();
    }

    public ITextBox CreateTextBox()
    {
        Console.WriteLine("[抽象工厂示例] Windows工厂创建文本框");
        return new WindowsTextBox();
    }
}

/// <summary>
/// Mac UI工厂
/// </summary>
public class MacUIFactory : IUIFactory
{
    public IButton CreateButton()
    {
        Console.WriteLine("[抽象工厂示例] Mac工厂创建按钮");
        return new MacButton();
    }

    public ITextBox CreateTextBox()
    {
        Console.WriteLine("[抽象工厂示例] Mac工厂创建文本框");
        return new MacTextBox();
    }
}
