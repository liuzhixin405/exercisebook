using ElasticSearchTest.Elastic;
using ElasticSearchTest.IService;
using ElasticSearchTest.Model;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchTest.Service
{
    public class AddressContext : BaseEsContext<Address>
    {
        public override string IndexName => "address";
        public AddressContext(IEsClientProvider provider) : base(provider) { }

        public List<Address> GetAddresses(string province, int pageIndex, int pageSize)
        {
            var client = _esClientProvider.GetClient(IndexName);
            var musts = new List<Func<QueryContainerDescriptor<Address>, QueryContainer>>();
            musts.Add(p => p.Term(m => m.Field(a => a.Pronvince).Value(province)));
            var search = new SearchDescriptor<Address>();
            search = search.Query(p => p.Bool(m => m.Must(musts))).From
                ((pageIndex - 1) * pageSize).Take(pageSize);
            var response = client.Search<Address>(search);
            return response.Documents.ToList();
        }

        /// <summary>
        /// 获取所有地址
        /// </summary>
        /// <returns></returns>
        public List<Address> GetAllAddresses()
        {
            var client = _esClientProvider.GetClient(IndexName);
            var searchDescriptor = new SearchDescriptor<Address>();
            // searchDescriptor = searchDescriptor.Index(IndexName).Query(p => p.MatchAll());
            searchDescriptor = searchDescriptor.Query(p => p.MatchAll());
            var response = client.Search<Address>(searchDescriptor);
            return response.Documents.ToList();
        }
        /// <summary>
        /// 删除指定城市的数据
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public bool DeleteByQuery(string city)
        {
            var client = _esClientProvider.GetClient(IndexName);
            var musts = new List<Func<QueryContainerDescriptor<Address>, QueryContainer>>();
            musts.Add(p => p.Term(m => m.Field(f => f.City).Value(city)));
            var search = new DeleteByQueryDescriptor<Address>().Index(IndexName);
            search = search.Query(p => p.Bool(m => m.Must(musts)));
            var response = client.DeleteByQuery<Address>(p => search);
            return response.IsValid;
        }
    }
}
