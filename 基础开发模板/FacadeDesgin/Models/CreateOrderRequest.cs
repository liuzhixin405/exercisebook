namespace FacadeDesgin.Models;

public class CreateOrderRequest
{
    public string CustomerName { get; set; } = string.Empty;
    public List<OrderItem> Items { get; set; } = new();
    public PaymentInfo Payment { get; set; } = new();
    public string Contact { get; set; } = string.Empty; // email, phone or device token depending on channel
    public NotificationChannel PreferredChannel { get; set; } = NotificationChannel.Email;
}

public class OrderItem
{
    public string Product { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class PaymentInfo
{
    public string CardNumber { get; set; } = string.Empty;
    public string CardHolder { get; set; } = string.Empty;
    public string Expiry { get; set; } = string.Empty;
    public string Cvv { get; set; } = string.Empty;
}

public enum NotificationChannel
{
    Email,
    Sms,
    Push,
    Any
}
