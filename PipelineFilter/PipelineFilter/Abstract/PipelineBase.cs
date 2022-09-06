using PipelineFilter.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineFilter.Abstract
{
    internal abstract class PipelineBase<T> where T : IMessage
    {
        protected IList<IFilter<T>> filters = new List<IFilter<T>>();

        protected T message;
        protected IDataSource<T> source;
        protected IDataSink<T> sink;

        public virtual void Process()
        {
            if (source == null) throw new ArgumentNullException("source");
            if (sink == null) throw new ArgumentNullException("sink");
            foreach (IFilter<T> filter in filters)
            {
                message = filter.Handle(message);
            }
        }

        public virtual void Add(IFilter<T> filter)
        {
            if (filter == null) throw new ArgumentNullException("filter");
            filter.Pipeline = this;
            filters.Add(filter);
        }

        public virtual void Remove(IFilter<T> filter)
        {
            if (filter == null) throw new ArgumentNullException("filter");
            if (!filters.Contains(filter))
                return;
            else
            {
                filter.Pipeline = null;
                filters.Remove(filter);
            }
        }

        public virtual T Message { get => message; set => message = value; }
        public IDataSource<T> Source { get => source; set => source = value; }
        public IDataSink<T> Sink { get => sink; set => sink = value; }
    }
}
