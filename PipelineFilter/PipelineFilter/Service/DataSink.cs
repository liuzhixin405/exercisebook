using PipelineFilter.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineFilter.Service
{
    internal class DataSink : IDataSink<Message>
    {
        public string Content;
        public void Write(Message message)
        {
            Content = message.Data;
        }
    }
}
