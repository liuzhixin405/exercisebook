namespace FacadeDesgin.Services;

public interface IEventPublisher
{
    Task PublishAsync(string topic, object payload);
}
