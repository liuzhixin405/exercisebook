using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchTest.Model
{
    [ElasticsearchType(IdProperty = "Id")]
    public class Address
    {
        [Keyword]
        public string Id { get; set; }
        [Keyword]
        public string Country { get; set; }
        [Keyword]
        public string City { get; set; }
        [Keyword]
        public string Pronvince { get; set; }
        [Keyword]
        public string Area { get; set; }
        [Text]
        public string Address1 { get; set; }

    }
}
