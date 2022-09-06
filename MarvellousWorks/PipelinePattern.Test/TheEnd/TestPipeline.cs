using Microsoft.VisualStudio.TestTools.UnitTesting;
using PipelinePattern.TheEnd;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelinePattern.Test.TheEnd
{
    public class Message : IMessage
    {
       public string Data { get; set; }
    }

    public class DataSource : IDataSource<Message>
    {
        public Message Get()
        {
            Message message = new Message();
            message.Data = "数据源";
            return message;
        }
    }

    public class DataSink : IDataSink<Message>
    {
        public void Invoke(Message message)
        {
            //DOTO:
            Trace.WriteLine(message);
        }
    }

    public class AppandFooFilter : FilterBase<Message>
    {
        public override Message Handle(Message message)
        {
            return new Message { Data = $"{message} 第一次" };
        }
    }
    public class AppandFirstFilter : FilterBase<Message>
    {
        public override Message Handle(Message message)
        {
            return new Message { Data = $"{message} 第一次" };
        }
    }
    public class AppandSecondFilter : FilterBase<Message>
    {
        public override Message Handle(Message message)
        {
            return new Message { Data = $"{message} 第二次" };
        }
        public void Action()
        {
            base.Pipeline.Process();
        }
    }

    public class AppandThirdFilter : FilterBase<Message>
    {
        public override Message Handle(Message message)
        {
            return new Message { Data = $"{message} 第三次" };
        }
    }

    public class PipeLine : PipelineBase<Message>
    {
        public PipeLine(IDataSource<Message> dataSource, IDataSink<Message> dataSink) : base(dataSource, dataSink)
        {
        }

        public override void Process()
        {
            base.Process();
        }
    }
    [TestClass]
    public class Program
    {
        [TestMethod]
        void Test()
        {
            IDataSource<Message> dataSource = new DataSource();
            IDataSink<Message> dataSink = new DataSink();
            PipeLine pipeLine = new PipeLine(dataSource, dataSink);
            var sencondFilter = new AppandSecondFilter();
            pipeLine.Add(new AppandFirstFilter());
            pipeLine.Add(sencondFilter);
            pipeLine.Add(new AppandThirdFilter());
            sencondFilter.Action();
        }
    }
}
