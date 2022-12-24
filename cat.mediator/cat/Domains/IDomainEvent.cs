using MediatR;

namespace cat.Domains
{
    public interface IDomainEvent:INotification
    {
        DateTime OccurredOn { get; }
    }
}
