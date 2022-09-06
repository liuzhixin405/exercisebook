using PipelineFilter.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineFilter.Service
{
    internal class AppendBFilter : FilterBase<Message>
    {
        public override Message Handle(Message message)
        {
            message.Data += " World";
            return message;
        }
    }
}
