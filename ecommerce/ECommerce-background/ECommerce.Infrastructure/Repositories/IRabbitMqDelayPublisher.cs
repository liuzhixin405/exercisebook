public interface IRabbitMqDelayPublisher
{
    Task PublishOrderDelayMessageAsync(Guid orderId, TimeSpan delay);
}