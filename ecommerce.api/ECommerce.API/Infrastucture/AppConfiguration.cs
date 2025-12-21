using ECommerce.API.Application;

namespace ECommerce.API.Infrastucture
{
    // 配置单例
    public sealed class AppConfiguration : IAppConfiguration
    {
        private static readonly Lazy<AppConfiguration> _instance =
            new Lazy<AppConfiguration>(() => new AppConfiguration());

        public static AppConfiguration Instance => _instance.Value;

        private AppConfiguration() { }

        public string DatabaseConnection { get; private set; }
        public string ServiceUrl { get; private set; }
        public int TimeoutSeconds { get; private set; }

        // 建造者类
        public class Builder
        {
            private readonly AppConfiguration _config = new AppConfiguration();

            public Builder WithDatabaseConnection(string connection)
            {
                _config.DatabaseConnection = connection;
                return this;
            }

            public Builder WithServiceUrl(string url)
            {
                _config.ServiceUrl = url;
                return this;
            }

            public Builder WithTimeout(int seconds)
            {
                _config.TimeoutSeconds = seconds;
                return this;
            }

            public AppConfiguration Build()
            {
                return _config;
            }
        }
    }

}
