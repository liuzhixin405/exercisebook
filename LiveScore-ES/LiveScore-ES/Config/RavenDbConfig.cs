using Raven.Client;
using Raven.Client.Documents;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Security.Cryptography.X509Certificates;

namespace LiveScore_ES.Config
{
    public class RavenDbConfig
    {
        private static Lazy<IDocumentStore> store = new Lazy<IDocumentStore>(CreateStore);

        public static IDocumentStore Store => store.Value;

        private static IDocumentStore CreateStore()
        {
            IDocumentStore store = new DocumentStore()
            {
                // Define the cluster node URLs (required)
                Urls = new[] { "http://127.0.0.1:8080/", 
                           /*some additional nodes of this cluster*/ },

                // Set conventions as necessary (optional)
                Conventions =
            {
                MaxNumberOfRequestsPerSession = 10,
                UseOptimisticConcurrency = true
            },

                // Define a default database (optional)
                Database = "liverscoreDB",

                // Define a client certificate (optional)
                //Certificate = new X509Certificate2("C:\\path_to_your_pfx_file\\cert.pfx"),

                // Initialize the Document Store
            }.Initialize();

            return store;
        }
    }
}
