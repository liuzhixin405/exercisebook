using System.Collections.Specialized;

namespace DesignPatternsForDatabase
{
    public class DbProviderFactory
    {
        private const string GroupName = "";
        private const string SectionName = "";
        private static NameValueCollection collection;

        static DbProviderFactory()
        {
            IConfiguration configuration = GlobalConfigure.GlobalServiceProvider.CreateAsyncScope().ServiceProvider.GetRequiredService<IConfiguration>();
            var providerName = configuration.GetSection("marvellousWorks.PracticalPattern.ShowCase").GetSection("dbProviderMapping");
            collection = (NameValueCollection)providerName;
        }

        public string GetDbType(string providerName)
        {
            return collection[providerName];
        }
    }
}
