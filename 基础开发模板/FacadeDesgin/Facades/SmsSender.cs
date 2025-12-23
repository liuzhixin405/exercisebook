namespace FacadeDesgin.Facades;

public class SmsSender : IMessageSender
{
    public Task SendAsync(string to, string subject, string body)
    {
        Console.WriteLine($"[SMS] To: {to}, Message: {body}");
        return Task.CompletedTask;
    }
}
