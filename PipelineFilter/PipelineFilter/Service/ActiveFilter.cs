using PipelineFilter.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineFilter.Service
{
    internal class ActiveFilter : FilterBase<Message>
    {
        public override Message Handle(Message message)
        {
            return message;
        }
        public void Action()
        {
            if(pipeline == null)throw new ArgumentNullException(nameof(pipeline));
            if(pipeline.Source==null)throw new ArgumentNullException(nameof(pipeline.Source));

            Message message = pipeline.Source.Read();
            pipeline.Message = message;
            pipeline.Process();
        }
    }
}
