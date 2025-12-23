namespace FacadeDesgin.Facades;

public class PaymentNotificationBridge : NotificationBase
{
    public PaymentNotificationBridge(IMessageSender sender) : base(sender) { }

    public override Task SendAsync(string to, string message)
    {
        var subject = "Payment Status";
        var body = $"Payment Notification: {message}";
        return _sender.SendAsync(to, subject, body);
    }
}
