namespace FacadeDesgin.Services;

public class EventPublisher : IEventPublisher
{
    public Task PublishAsync(string topic, object payload)
    {
        // in real system this would push to a message bus; here we just log
        Console.WriteLine($"Event published: {topic} -> {System.Text.Json.JsonSerializer.Serialize(payload)}");
        return Task.CompletedTask;
    }
}
