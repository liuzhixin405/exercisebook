namespace FacadeDesgin.Facades;

public class OrderNotificationBridge : NotificationBase
{
    public OrderNotificationBridge(IMessageSender sender) : base(sender) { }

    public override Task SendAsync(string to, string message)
    {
        var subject = "Order Update";
        var body = $"Order Notification: {message}";
        return _sender.SendAsync(to, subject, body);
    }
}
