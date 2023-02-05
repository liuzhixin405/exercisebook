using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAoppication.Core;
using WebAoppication.IBusiness;

namespace WebAoppication.Business
{
    public class TestBusiness: ITestBusiness,ITransientDependency
    {
        public string GetString()
        {
            return "OK";
        }
    }
}
