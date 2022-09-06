using ElasticSearchTest.Elastic;
using ElasticSearchTest.Extend;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchTest.IService
{
    public abstract class BaseEsContext<T>:IBaseEsContext where T:class
    {
        protected IEsClientProvider _esClientProvider;
        public abstract string IndexName { get; }
        public BaseEsContext(IEsClientProvider esClientProvider)
        {
            this._esClientProvider = esClientProvider;
        }
        public bool InsertMany(List<T> tList)
        {
            var client = _esClientProvider.GetClient(IndexName);
            if (!client.Indices.Exists(IndexName).Exists)
            {
                client.CreateIndex<T>(IndexName);
            }
            var response = client.IndexMany(tList);
            return response.IsValid;
        }
        public long GetTotalCount()
        {
            var client = _esClientProvider.GetClient(IndexName);
            var search = new SearchDescriptor<T>().MatchAll();
            var response = client.Search<T>(search);
            return response.Total;
        }

        public bool DeleteById(string id)
        {
            var client = _esClientProvider.GetClient(IndexName);
            var response = client.Delete<T>(id);
            return response.IsValid;
        }
    }
}
