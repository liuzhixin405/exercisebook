namespace ECommerce.API.Application
{
    // 事件总线
    public interface IEventBus
    {
        Task PublishAsync<T>(T @event) where T : IDomainEvent;
        void Subscribe<T, H>() where T : IDomainEvent where H : IEventHandler<T>;
    }
}
