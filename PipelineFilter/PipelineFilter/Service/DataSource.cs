using PipelineFilter.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineFilter.Service
{
    internal class DataSource : IDataSource<Message>
    {
        public Message Read()
        {
            Message message = new Message();
            message.Data = Environment.MachineName;
            return message;
        }
    }
}
