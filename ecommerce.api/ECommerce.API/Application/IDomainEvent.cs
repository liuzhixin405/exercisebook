namespace ECommerce.API.Application
{
    // 领域事件接口
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}
