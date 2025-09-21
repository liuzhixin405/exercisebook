namespace ECommerce.Domain.Models
{
    /// <summary>
    /// 支付请求
    /// </summary>
    public class PaymentRequest
    {
        public Guid OrderId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "CNY";
        public string Description { get; set; } = string.Empty;
        public Dictionary<string, string> Metadata { get; set; } = new();
    }

    /// <summary>
    /// 支付结果
    /// </summary>
    public class PaymentResult
    {
        public bool Success { get; set; }
        public string PaymentId { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime ProcessedAt { get; set; }
        public Dictionary<string, string> AdditionalData { get; set; } = new();
    }

    /// <summary>
    /// 支付验证结果
    /// </summary>
    public class PaymentValidationResult
    {
        public bool IsValid { get; set; }
        public string PaymentId { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime ValidatedAt { get; set; }
    }

    /// <summary>
    /// 退款请求
    /// </summary>
    public class RefundRequest
    {
        public Guid OrderId { get; set; }
        public string PaymentId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// 退款结果
    /// </summary>
    public class RefundResult
    {
        public bool Success { get; set; }
        public string RefundId { get; set; } = string.Empty;
        public string PaymentId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public RefundStatus Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime ProcessedAt { get; set; }
    }

    /// <summary>
    /// 支付状态
    /// </summary>
    public class PaymentStatus
    {
        public string PaymentId { get; set; } = string.Empty;
        public Guid OrderId { get; set; }
        public PaymentState State { get; set; } = PaymentState.Pending;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "CNY";
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// 支付状态枚举
    /// </summary>
    public enum PaymentState
    {
        Pending,        // 待支付
        Processing,     // 处理中
        Completed,      // 已完成
        Failed,         // 失败
        Cancelled,      // 已取消
        Refunded        // 已退款
    }

    /// <summary>
    /// 退款状态枚举
    /// </summary>
    public enum RefundStatus
    {
        Pending,        // 待处理
        Processing,     // 处理中
        Completed,      // 已完成
        Failed,         // 失败
        Cancelled       // 已取消
    }

    /// <summary>
    /// 支付方式枚举
    /// </summary>
    public enum PaymentMethod
    {
        CreditCard,     // 信用卡
        DebitCard,      // 借记卡
        PayPal,         // PayPal
        Alipay,         // 支付宝
        WeChatPay,      // 微信支付
        BankTransfer,   // 银行转账
        Cash,           // 现金
        Other           // 其他
    }
}
