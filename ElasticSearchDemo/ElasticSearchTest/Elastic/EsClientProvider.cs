using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchTest.Elastic
{
    public class EsClientProvider : IEsClientProvider
    {
        private readonly IOptions<EsConfig> _esConfig;

        public EsClientProvider(IOptions<EsConfig> esConfig)
        {
            this._esConfig = esConfig;
        }
        public ElasticClient GetClient()
        {
            if (_esConfig == null || _esConfig.Value ==null||_esConfig.Value.Urls==null|| _esConfig.Value.Urls.Count == 0)
            {
                throw new ArgumentNullException("urls");
            }
            return GetClient(_esConfig.Value.Urls.ToArray(), "");
        }

        private ElasticClient GetClient(string[] urls, string defaultIndex = "")
        {
            if(urls==null || urls.Length == 0)
            {
                throw new ArgumentNullException("urls");
            }
            var uris = urls.Select(p => new Uri(p)).ToArray();
            var connectionPool = new SniffingConnectionPool(uris);
            var connectionSetting = new ConnectionSettings(connectionPool);
            if (!string.IsNullOrWhiteSpace(defaultIndex))
            {
                connectionSetting.DefaultIndex(defaultIndex);
            }
            //设置账号密码
            connectionSetting.BasicAuthentication("elastic", "12301230").RequestTimeout(TimeSpan.FromSeconds(30));
            return new ElasticClient(connectionSetting);
        }

        public ElasticClient GetClient(string indexName)
        {
            if (_esConfig == null || _esConfig.Value == null || _esConfig.Value.Urls == null || _esConfig.Value.Urls.Count == 0)
            {
                throw new ArgumentNullException("urls");
            }
            return GetClient(_esConfig.Value.Urls.ToArray(), indexName);
        }
    }
}
