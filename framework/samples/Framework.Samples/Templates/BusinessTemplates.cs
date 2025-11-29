using Framework.Infrastructure.Templates;

namespace Framework.Samples.Templates;

/// <summary>
/// 用户注册流程 - 模板方法模式示例
/// </summary>
public class UserRegistrationTemplate : TemplateMethodBase<UserRegistrationContext, RegistrationResult>
{
    public override string Name => "用户注册流程";

    public override async Task InitializeAsync(UserRegistrationContext context)
    {
        Console.WriteLine($"[模板方法示例] 1. 初始化注册流程");
        context.StartTime = DateTime.UtcNow;
        await Task.CompletedTask;
    }

    public override async Task<bool> ValidateAsync(UserRegistrationContext context)
    {
        Console.WriteLine($"[模板方法示例] 2. 验证用户信息");
        
        if (string.IsNullOrEmpty(context.UserName))
        {
            Console.WriteLine("  ❌ 用户名不能为空");
            return false;
        }

        if (string.IsNullOrEmpty(context.Email) || !context.Email.Contains("@"))
        {
            Console.WriteLine("  ❌ 邮箱格式无效");
            return false;
        }

        if (context.Password.Length < 6)
        {
            Console.WriteLine("  ❌ 密码长度不足6位");
            return false;
        }

        Console.WriteLine("  ✓ 验证通过");
        return await Task.FromResult(true);
    }

    public override async Task<RegistrationResult> ProcessAsync(UserRegistrationContext context)
    {
        Console.WriteLine($"[模板方法示例] 3. 处理注册逻辑");
        Console.WriteLine($"  - 创建用户账号: {context.UserName}");
        Console.WriteLine($"  - 设置邮箱: {context.Email}");
        Console.WriteLine($"  - 生成用户ID");
        
        // 模拟数据库操作
        await Task.Delay(100);
        
        return new RegistrationResult
        {
            Success = true,
            UserId = Guid.NewGuid(),
            Message = "注册成功"
        };
    }

    public override async Task CleanupAsync(UserRegistrationContext context, RegistrationResult result)
    {
        Console.WriteLine($"[模板方法示例] 4. 清理和后续操作");
        Console.WriteLine($"  - 发送欢迎邮件到: {context.Email}");
        Console.WriteLine($"  - 记录注册日志");
        Console.WriteLine($"  - 总耗时: {(DateTime.UtcNow - context.StartTime).TotalMilliseconds:F0}ms");
        await Task.CompletedTask;
    }

    protected override async Task OnErrorAsync(UserRegistrationContext context, Exception exception)
    {
        Console.WriteLine($"[模板方法示例] ❌ 注册失败: {exception.Message}");
        await Task.CompletedTask;
    }
}

/// <summary>
/// 订单处理流程 - 模板方法模式示例
/// </summary>
public class OrderProcessingTemplate : TemplateMethodBase<OrderContext, OrderResult>
{
    public override string Name => "订单处理流程";

    public override async Task InitializeAsync(OrderContext context)
    {
        Console.WriteLine($"[模板方法示例] 1. 初始化订单处理");
        context.OrderId = Guid.NewGuid();
        context.ProcessStartTime = DateTime.UtcNow;
        await Task.CompletedTask;
    }

    public override async Task<bool> ValidateAsync(OrderContext context)
    {
        Console.WriteLine($"[模板方法示例] 2. 验证订单信息");
        
        if (context.Items == null || context.Items.Count == 0)
        {
            Console.WriteLine("  ❌ 订单项不能为空");
            return false;
        }

        if (context.TotalAmount <= 0)
        {
            Console.WriteLine("  ❌ 订单金额无效");
            return false;
        }

        if (string.IsNullOrEmpty(context.CustomerName))
        {
            Console.WriteLine("  ❌ 客户名称不能为空");
            return false;
        }

        Console.WriteLine($"  ✓ 订单验证通过 (商品数量: {context.Items.Count}, 总额: {context.TotalAmount:C})");
        return await Task.FromResult(true);
    }

    public override async Task<OrderResult> ProcessAsync(OrderContext context)
    {
        Console.WriteLine($"[模板方法示例] 3. 处理订单");
        Console.WriteLine($"  - 订单ID: {context.OrderId}");
        Console.WriteLine($"  - 客户: {context.CustomerName}");
        Console.WriteLine($"  - 检查库存");
        
        await Task.Delay(50);
        
        Console.WriteLine($"  - 锁定库存");
        Console.WriteLine($"  - 创建发货单");
        Console.WriteLine($"  - 更新订单状态");

        return new OrderResult
        {
            Success = true,
            OrderId = context.OrderId,
            EstimatedDeliveryDate = DateTime.UtcNow.AddDays(3),
            Message = "订单处理成功"
        };
    }

    public override async Task CleanupAsync(OrderContext context, OrderResult result)
    {
        Console.WriteLine($"[模板方法示例] 4. 订单后续处理");
        Console.WriteLine($"  - 发送订单确认邮件");
        Console.WriteLine($"  - 通知仓库发货");
        Console.WriteLine($"  - 预计送达时间: {result.EstimatedDeliveryDate:yyyy-MM-dd}");
        await Task.CompletedTask;
    }
}

/// <summary>
/// 用户注册上下文
/// </summary>
public class UserRegistrationContext
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
}

/// <summary>
/// 注册结果
/// </summary>
public class RegistrationResult
{
    public bool Success { get; set; }
    public Guid UserId { get; set; }
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// 订单上下文
/// </summary>
public class OrderContext
{
    public Guid OrderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public List<OrderItem> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public DateTime ProcessStartTime { get; set; }
}

/// <summary>
/// 订单项
/// </summary>
public class OrderItem
{
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

/// <summary>
/// 订单结果
/// </summary>
public class OrderResult
{
    public bool Success { get; set; }
    public Guid OrderId { get; set; }
    public DateTime EstimatedDeliveryDate { get; set; }
    public string Message { get; set; } = string.Empty;
}
