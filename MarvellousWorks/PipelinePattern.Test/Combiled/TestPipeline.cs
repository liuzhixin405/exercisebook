using Microsoft.VisualStudio.TestTools.UnitTesting;
using PipelinePattern.Combiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelinePattern.Test.Combiled
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
            pipeline.Add(new AppendAFilter());
            ActiveFilter activeFilter = new ActiveFilter();
            pipeline.Add(activeFilter);
            pipeline.Add(new AppendBFilter());

            ///由主动性Filter发起的调用
            activeFilter.Action();
            Assert.AreEqual<string>(Environment.MachineName + "AB", pipeline.Message.Data);

        }
    }
    /// <summary>
    /// 具体的Message对象
    /// </summary>
    public class Message : IMessage
    {
        public string Data;
    }
    /// <summary>
    /// 具体Filter对象
    /// </summary>
    public class AppendAFilter : FilterBase<Message>
    {
        public override Message Handle(Message message)
        {
            message.Data += "A";
            return message;
        }
    }
    /// <summary>
    /// 具体的Filter对象
    /// </summary>
    public class AppendBFilter : FilterBase<Message>
    {
        public override Message Handle(Message message)
        {
            message.Data += "B";
            return message;
        }
    }
    /// <summary>
    /// 具体的DataSource对象
    /// </summary>
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
            this.dataSink = dataSink;
            this.dataSource = dataSource;
        }
        public override void Process()
        {
            base.Process();
        }
    }

    /// <summary>
    /// 主动性Filter
    /// </summary>
    public class ActiveFilter : FilterBase<Message>
    {
        public override Message Handle(Message message)
        {
            return message;
        }
        public void Action()
        {
            if (pipeline == null) throw new ArgumentNullException("pipeline");
            if(pipeline.DataSource==null) throw new ArgumentNullException("dataSource");
            Message message = pipeline.DataSource.Read();
            pipeline.Message = message;
            pipeline.Process();
        }
    }
}
