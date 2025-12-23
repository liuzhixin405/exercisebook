namespace FacadeDesgin.Facades;

public class EmailSender : IMessageSender
{
    public Task SendAsync(string to, string subject, string body)
    {
        Console.WriteLine($"[Email] To: {to}, Subject: {subject}, Body: {body}");
        return Task.CompletedTask;
    }
}
