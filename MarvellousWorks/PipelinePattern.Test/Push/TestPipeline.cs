using Microsoft.VisualStudio.TestTools.UnitTesting;
using PipelinePattern.Push;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelinePattern.Test.Push
{
    [TestClass]
    public class TestPipeline
    {
        [TestMethod]
        public void Test()
        {
            IDataSource<Message> dataSource = new DataSource();
            IDataSink<Message> dataSink = new DataSink();
            PipelineBase<Message> pipeline = new Pipeline(dataSource, dataSink);
            pipeline.Process();
            Assert.AreEqual<string>(Environment.MachineName + "AB", pipeline.Message.Data);
        }
    }
    /// <summary>
    /// 具体Message对象
    /// </summary>
    public class Message : IMessage
    {
        public string Data;
    }

    public class AppendAFilter : FilterBase<Message>
    {
        public override Message Handle(Message message)
        {
            message.Data += "A";
            return message;
        }
    }
    public class AppendBFilter : FilterBase<Message>
    {
        public override Message Handle(Message message)
        {
            message.Data += "B";
            return message;
        }
    }
    public class DataSource : IDataSource<Message>
    {
        public virtual Message Read()
        {
            Message message = new Message();
            message.Data = Environment.MachineName;
            return message;
        }
    }
    public class DataSink : IDataSink<Message>
    {
        public string Content;
        public void Write(Message message)
        {
            Content = message.Data;
        }
    }

    public class Pipeline : PipelineBase<Message>
    {
        public Pipeline(IDataSource<Message> dataSource,IDataSink<Message> dataSink)
        {
            Add(new AppendAFilter());
            Add(new AppendBFilter());
            this.dataSink = dataSink;
            this.dataSource = dataSource;
        }

        public override void Process()
        {
            this.message = dataSource.Read();
            base.Process();
        }
    }
}
