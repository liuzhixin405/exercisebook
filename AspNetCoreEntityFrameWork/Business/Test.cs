using EF.Core.Extensions;
using Project.IOC.IBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.IOC.Business
{
    public class Test : ITest, ITransientDependency
    {
        public string GetStr()
        {
            return "success";
        }
    }
}
