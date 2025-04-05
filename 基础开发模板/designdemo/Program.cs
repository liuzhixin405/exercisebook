using designdemo;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console; // 添加此行
using System;

public class Program
{
    public static void Main()
    {
        // 创建支付信息
        var alipayPayment = new AliPayPaymentEntity
        {
            Amount = 100.00m,
            TransactionId = "ALIPAY-123456"
        };

        var weChatPayment = new WeChatPaymentEntity
        {
            Amount = 100.00m,
            TransactionId = "WECHAT-123456"
        };

        // 创建支付实现
        var alipay = new AliPayImpl();
        var weChatPay = new WeChatPayImpl();

        // 创建适配器
        var alipayAdapter = new AliPayAdapter<AliPayPaymentEntity>(alipay);
        var weChatAdapter = new WeChatPayAdapter<WeChatPaymentEntity>(weChatPay);

        // 创建LoggerFactory
        using var loggerFactory = LoggerFactory.Create(static builder => builder.AddConsole());
        var aliPayLogger = loggerFactory.CreateLogger<PaymentProcessor<AliPayPaymentEntity>>();
        var WeChatLogger = loggerFactory.CreateLogger<PaymentProcessor<WeChatPaymentEntity>>();
        // 创建支付处理器
        var alipayProcessor = new PaymentProcessor<AliPayPaymentEntity>(alipayAdapter, aliPayLogger);
        alipayProcessor.ProcessPayment(alipayPayment);

        var weChatProcessor = new PaymentProcessor<WeChatPaymentEntity>(weChatAdapter, WeChatLogger);
        weChatProcessor.ProcessPayment(weChatPayment);

        Console.ReadKey();
    }
}
