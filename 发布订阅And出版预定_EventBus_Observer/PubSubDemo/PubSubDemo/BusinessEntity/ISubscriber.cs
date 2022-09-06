using PubSubDemo.DataEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubDemo.BusinessEntity
{
    /// <summary>
    /// 抽象预定接口
    /// </summary>
    internal interface ISubscriber
    {
        void Enqueue(Article article);
        string Peek(string category);
        string Dequeue(string category);
    }
}
