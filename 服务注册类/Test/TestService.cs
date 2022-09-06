using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 服务注册类;

namespace Test
{
    public class TestService:BaseService
    {
        public string GetValue()
        {
            return this.ExcuteFunc<ITestDomain, string>((domain) =>
            {
                return domain.GetValue();
            });
        }
    }
}
