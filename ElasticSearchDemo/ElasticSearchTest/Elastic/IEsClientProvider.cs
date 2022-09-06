using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchTest.Elastic
{
    public interface IEsClientProvider
    {
        ElasticClient GetClient();
        ElasticClient GetClient(string indexName);
    }
}
