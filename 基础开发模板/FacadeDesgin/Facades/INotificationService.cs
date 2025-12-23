namespace FacadeDesgin.Facades;

using FacadeDesgin.Models;

public interface INotificationService
{
    Task SendAsync(CreateOrderRequest request, string subject, string message);
}
