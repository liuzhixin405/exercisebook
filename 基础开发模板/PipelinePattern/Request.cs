using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelinePattern
{
    internal class Request
    {
        public StringBuilder Head { get; set; }=new StringBuilder();
        public RequestClientType ClientType { get; set; } = new RequestClientType();
        public RequestContent Content { get; set; }=new RequestContent();
    }
}
