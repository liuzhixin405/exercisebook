namespace ECommerce.API.Application
{
    // 配置接口
    public interface IAppConfiguration
    {
        string DatabaseConnection { get; }
        string ServiceUrl { get; }
        int TimeoutSeconds { get; }
    }
}
