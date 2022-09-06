using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelinePattern.TheEnd
{
    public interface IMessage { }

    public interface IFilter<T> where T : IMessage
    {
        T Handle(T message);
        PipelineBase<T> Pipeline { get; set; }
    }

    public abstract class PipelineBase<T> where T : IMessage
    {
        protected IList<IFilter<T>> filters = new List<IFilter<T>>(); //子类可以做内部添加，不对外公开
        protected IDataSink<T> dataSink;
        protected IDataSource<T> dataSource;
        public PipelineBase(IDataSource<T> dataSource,IDataSink<T> dataSink)
        {
            this.dataSink = dataSink;
            this.dataSource = dataSource;
        }
        private T data => dataSource.Get();
        public T Data => data;
        public void Add(IFilter<T> filter)
        {
            filter.Pipeline = this;
            filters.Add(filter);
        }
        public void Remove(IFilter<T> filter)
        {
            filter.Pipeline = null;
            filters.Remove(filter);
        }
        public virtual void Process()
        {
            foreach (var filter in filters)
            {
                filter.Handle(data);
            }
        }
    }

    public interface IDataSource<T> where T : IMessage
    {
        T Get();
    }
    public interface IDataSink<T> where T : IMessage { void Invoke(T message); }

    public abstract class FilterBase<T> : IFilter<T> where T : IMessage
    {
        public PipelineBase<T> Pipeline { get; set; }

        public abstract T Handle(T message);
    }

}
