namespace ECommerce.API.Application
{
    //事件处理器接口
    public interface IEventHandler<T> where T : IDomainEvent
    {
        Task HandleAsync(T @event);
    }
}
