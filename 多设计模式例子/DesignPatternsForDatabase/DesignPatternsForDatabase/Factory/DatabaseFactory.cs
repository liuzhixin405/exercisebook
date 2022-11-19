using DesignPatternsForDatabase.Provider;

namespace DesignPatternsForDatabase.Factory
{
    public static class DatabaseFactory
    {
        private static DbProviderRegistry registry =new DbProviderRegistry();

        public static Database Create(String name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            IConfiguration configuration = GlobalConfigure.GlobalServiceProvider.CreateAsyncScope().ServiceProvider.GetRequiredService<IConfiguration>();
            var providerName = configuration.GetSection("ConnectionStrings").GetSection("LocalSQL").GetSection("ProviderName").ToString();
            //switch (providerName)
            //{
            //    case "System.Data.SqlClient":
            //        return new SqlServerDatabase(name);
            //    case "System.Data.OracleClient":
            //        return new OracleDatabase(name);
            //    default:
            //        throw new ArgumentException("类型未知");
            //}
            Type type = Type.GetType(registry.GetDbType(providerName)); //加配置后的
            return (Database)Activator.CreateInstance(type);
        }
    }
}
