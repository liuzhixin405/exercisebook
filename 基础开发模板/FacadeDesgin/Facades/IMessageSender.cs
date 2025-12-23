namespace FacadeDesgin.Facades;

public interface IMessageSender
{
    Task SendAsync(string to, string subject, string body);
}
