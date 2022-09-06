using PipelineFilter.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineFilter.Abstract
{
    internal abstract class FilterBase<T> : IFilter<T> where T : IMessage
    {
        protected PipelineBase<T> pipeline;
        public virtual PipelineBase<T> Pipeline { get => pipeline; set => pipeline=value; }

        public abstract T Handle(T message);
       
    }
}
