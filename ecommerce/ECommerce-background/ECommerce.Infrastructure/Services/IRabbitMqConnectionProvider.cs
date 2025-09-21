using RabbitMQ.Client;

public interface IRabbitMqConnectionProvider
{
    IConnection GetConnection();
    IModel GetChannel();
    void EnsureQueuesAndExchanges();
}