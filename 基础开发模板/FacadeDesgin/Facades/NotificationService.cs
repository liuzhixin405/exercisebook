namespace FacadeDesgin.Facades;

using FacadeDesgin.Models;
using System;
using System.Linq;

public class NotificationService : INotificationService
{
    private readonly Func<IMessageSender> _primaryFactory;
    private readonly IServiceProvider _provider;

    public NotificationService(Func<IMessageSender> primaryFactory, IServiceProvider provider)
    {
        _primaryFactory = primaryFactory;
        _provider = provider;
    }

    public async Task SendAsync(CreateOrderRequest request, string subject, string message)
    {
        IMessageSender? chosen = null;

        // 1) try channel from request (explicit per-request preference)
        if (request.PreferredChannel != NotificationChannel.Any)
        {
            chosen = request.PreferredChannel switch
            {
                NotificationChannel.Email => _provider.GetService(typeof(EmailSender)) as IMessageSender,
                NotificationChannel.Sms => _provider.GetService(typeof(SmsSender)) as IMessageSender,
                NotificationChannel.Push => _provider.GetService(typeof(PushSender)) as IMessageSender,
                _ => null
            };
        }

        // 2) use primary sender provided by DI (configured in Program.cs)
        chosen ??= _primaryFactory();

        // build fallback list from concrete senders registered in container
        var fallback = new IMessageSender[] {
            _provider.GetService(typeof(EmailSender)) as IMessageSender,
            _provider.GetService(typeof(SmsSender)) as IMessageSender,
            _provider.GetService(typeof(PushSender)) as IMessageSender
        }.Where(x => x != null).Cast<IMessageSender>().ToList();

        // if chosen is null, pick first available
        chosen ??= fallback.FirstOrDefault();

        if (chosen is null)
            throw new InvalidOperationException("No message senders configured");

        try
        {
            await chosen.SendAsync(request.Contact, subject, message);
        }
        catch
        {
            foreach (var s in fallback.Where(s => s != chosen))
            {
                try { await s.SendAsync(request.Contact, subject, message); return; } catch { }
            }

            throw;
        }
    }
}
