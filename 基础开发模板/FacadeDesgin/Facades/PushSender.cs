namespace FacadeDesgin.Facades;

public class PushSender : IMessageSender
{
    public Task SendAsync(string to, string subject, string body)
    {
        Console.WriteLine($"[Push] To: {to}, Title: {subject}, Body: {body}");
        return Task.CompletedTask;
    }
}
