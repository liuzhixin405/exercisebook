using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelinePattern
{
    internal class RequestClientTypeFactory
    {
        public static RequestClientType CreateRequestClientTypeForApp()
        {
            return new RequestClientType
            {
                type = RequestClientType.App
            };
        }
        public static RequestClientType CreateRequestClientTypeForNetClient()
        {
            return new RequestClientType
            {
                type = RequestClientType.NetClient
            };
        }
    }
}
