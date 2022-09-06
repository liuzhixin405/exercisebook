using PipelineFilter.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineFilter.IService
{
    /// <summary>
    /// 抽象的filter对象接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal interface IFilter<T> where T:IMessage
    {
        /// <summary>
        /// 每个filter需要执行的基本功能
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        T Handle(T message);
        /// <summary>
        ///主动filter 回溯 找到data source和data link
        /// </summary>
        PipelineBase<T> Pipeline { get; set; }
    }
}
