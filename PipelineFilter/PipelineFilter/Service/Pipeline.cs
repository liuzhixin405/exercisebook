using PipelineFilter.Abstract;
using PipelineFilter.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineFilter.Service
{
    internal class Pipeline:PipelineBase<Message>
    {
        public Pipeline(IDataSource<Message> dataSource,IDataSink<Message> dataSink)
        {
            this.Source = dataSource;
            this.Sink = dataSink;
        }
        public override void Process()
        {
            base.Process();
        }
    }
}
