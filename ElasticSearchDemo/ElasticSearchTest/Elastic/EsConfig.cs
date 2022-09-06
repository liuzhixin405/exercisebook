using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchTest.Elastic
{
    public class EsConfig : IOptions<EsConfig>
    {
        public IList<string> Urls { get; set; }
        public EsConfig Value => this;
    }
}
