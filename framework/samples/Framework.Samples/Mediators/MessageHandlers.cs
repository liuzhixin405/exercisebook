using Framework.Infrastructure.Mediators;

namespace Framework.Samples.Mediators;

/// <summary>
/// 订单消息 - 中介者模式示例
/// </summary>
public class OrderMessage
{
    public Guid OrderId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

/// <summary>
/// 订单处理器 - 中介者模式示例
/// </summary>
public class OrderMessageHandler : IMessageHandler<OrderMessage>
{
    public string Name => "订单处理器";
    public int Priority => 1;

    public Task HandleAsync(OrderMessage message)
    {
        Console.WriteLine($"[中介者示例] 处理订单消息:");
        Console.WriteLine($"  订单ID: {message.OrderId}");
        Console.WriteLine($"  产品: {message.ProductName}");
        Console.WriteLine($"  数量: {message.Quantity}");
        Console.WriteLine($"  价格: {message.Price:C}");
        return Task.CompletedTask;
    }

    public bool ShouldHandle(OrderMessage message)
    {
        return message.Quantity > 0 && message.Price > 0;
    }
}

/// <summary>
/// 支付消息 - 中介者模式示例
/// </summary>
public class PaymentMessage
{
    public Guid PaymentId { get; set; }
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
}

/// <summary>
/// 支付处理器（带返回值） - 中介者模式示例
/// </summary>
public class PaymentMessageHandler : IMessageHandler<PaymentMessage, PaymentResult>
{
    public string Name => "支付处理器";
    public int Priority => 2;

    public Task<PaymentResult> HandleAsync(PaymentMessage message)
    {
        Console.WriteLine($"[中介者示例] 处理支付消息:");
        Console.WriteLine($"  支付ID: {message.PaymentId}");
        Console.WriteLine($"  订单ID: {message.OrderId}");
        Console.WriteLine($"  金额: {message.Amount:C}");
        Console.WriteLine($"  支付方式: {message.PaymentMethod}");

        // 模拟支付处理
        var result = new PaymentResult
        {
            Success = message.Amount > 0,
            TransactionId = Guid.NewGuid().ToString(),
            Message = message.Amount > 0 ? "支付成功" : "支付失败：金额无效"
        };

        return Task.FromResult(result);
    }

    public bool ShouldHandle(PaymentMessage message)
    {
        return !string.IsNullOrEmpty(message.PaymentMethod);
    }
}

/// <summary>
/// 支付结果
/// </summary>
public class PaymentResult
{
    public bool Success { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// 通知消息 - 中介者模式示例
/// </summary>
public class NotificationMessage
{
    public string Recipient { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
}

/// <summary>
/// 通知类型
/// </summary>
public enum NotificationType
{
    Email,
    SMS,
    Push
}

/// <summary>
/// 通知处理器 - 中介者模式示例
/// </summary>
public class NotificationMessageHandler : IMessageHandler<NotificationMessage>
{
    public string Name => "通知处理器";
    public int Priority => 3;

    public Task HandleAsync(NotificationMessage message)
    {
        Console.WriteLine($"[中介者示例] 发送{message.Type}通知:");
        Console.WriteLine($"  收件人: {message.Recipient}");
        Console.WriteLine($"  主题: {message.Subject}");
        Console.WriteLine($"  内容: {message.Content}");
        return Task.CompletedTask;
    }

    public bool ShouldHandle(NotificationMessage message)
    {
        return !string.IsNullOrEmpty(message.Recipient);
    }
}
