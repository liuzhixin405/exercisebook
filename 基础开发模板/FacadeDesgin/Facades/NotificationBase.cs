namespace FacadeDesgin.Facades;

public abstract class NotificationBase
{
    protected readonly IMessageSender _sender;

    protected NotificationBase(IMessageSender sender)
    {
        _sender = sender;
    }

    public abstract Task SendAsync(string to, string message);
}
