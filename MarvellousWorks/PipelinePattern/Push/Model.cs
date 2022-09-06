using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelinePattern.Push
{
    /// <summary>
    /// 用于在pipeline中传递处理的对象
    /// </summary>
    public interface IMessage { }

    /// <summary>
    /// 抽象的Filter对象接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFilter<T> where T : IMessage
    {
        /// <summary>
        /// 每个Filter需要执行的基本功能
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        T Handle(T message);
        /// <summary>
        /// 当前Filter实例所在的pipeline
        /// 主要是为了设计主动Filter时,可以同故宫回溯Pipeline
        /// 找到Data Source和Data Sink
        /// </summary>
        PipelineBase<T> Pipeline { get; set; }
    }

    public abstract class FilterBase<T> : IFilter<T> where T : IMessage
    {
        protected PipelineBase<T> pipeline;
        public virtual PipelineBase<T> Pipeline { get => pipeline; set => pipeline = value; }

        public abstract T Handle(T message);

    }
    /// <summary>
    /// 抽象Datasource接口 数据源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataSource<T> where T : IMessage
    {
        T Read();
    }
    /// <summary>
    /// 抽象DataSink接口 目标
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataSink<T> where T : IMessage
    {
        void Write(T message);
    }

    /// <summary>
    /// 抽象Pipeline对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PipelineBase<T> where T : IMessage
    {
        protected IList<IFilter<T>> filters = new List<IFilter<T>>();

        protected T message;
        protected IDataSource<T> dataSource;
        protected IDataSink<T> dataSink;
        /// <summary>
        /// 依次调用每个Filter完成消息的处理
        /// </summary>
        public virtual void Process()
        {
            if (DataSource == null || DataSink == null || message == null) throw new ArgumentNullException("dataSource or dataSink or message");
            foreach (var filter in filters)
            {
                message = filter.Handle(message);
            }
        }
        /// <summary>
        /// 增加新的Filter
        /// </summary>
        /// <param name="filter"></param>
        public virtual void Add(IFilter<T> filter)
        {
            if (filter == null) throw new ArgumentNullException("filter");
            filter.Pipeline = this;
            filters.Add(filter);
        }
        /// <summary>
        /// 剔除已有的Filter
        /// </summary>
        /// <param name="filter"></param>
        public virtual void Remove(IFilter<T> filter)
        {
            if (filter == null) throw new ArgumentNullException("filter");
            if (!filters.Contains(filter))
            {
                return;
            }
            else
            {
                filter.Pipeline = null;
                filters.Remove(filter);
            }
        }
        /// <summary>
        /// 当前管道管理的消息对象
        /// </summary>
        public virtual T Message
        {
            get => message;
            set => message = value;
        }
        /// <summary>
        /// 当前Pipeline关联的DataSource
        /// </summary>
        public IDataSource<T> DataSource { get => dataSource; }
        /// <summary>
        /// 当前Pipeline关联的DataSink
        /// </summary>
        public IDataSink<T> DataSink { get => dataSink; }
    }
}
