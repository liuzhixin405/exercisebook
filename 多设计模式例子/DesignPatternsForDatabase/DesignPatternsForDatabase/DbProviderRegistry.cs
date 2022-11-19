using System.Collections.Specialized;

namespace DesignPatternsForDatabase
{
    /// <summary>
    /// dbprovider与具体的database类型的注册表类型
    /// </summary>
    public class DbProviderRegistry
    {
        private const string GroupName = "marvellousWorks.PracticalPattern.ShowCase";
        private const string SectionName = "dbProviderMapping";
        private static NameValueCollection collection;

        static DbProviderRegistry()
        {
            IConfiguration configuration = GlobalConfigure.GlobalServiceProvider.CreateAsyncScope().ServiceProvider.GetRequiredService<IConfiguration>();
            var providerName = configuration.GetSection(GroupName).GetSection(SectionName);
            collection = (NameValueCollection)providerName;
        }

        public string GetDbType(string providerName)
        {
            return collection[providerName];
        }
    }
}
