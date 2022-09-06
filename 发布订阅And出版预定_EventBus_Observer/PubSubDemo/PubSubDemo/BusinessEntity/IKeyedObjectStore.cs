using PubSubDemo.DataEntity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubDemo.BusinessEntity
{
    /// <summary>
    /// 抽象持久层对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal interface IKeyedObjectStore<T> where T:class,IObjectWithKey
    {
        void Save(T target);
        //按照key提取
        T Select(string key);
        void Remove(string key);
        //遍历
        IEnumerator GetEnumerator();
    }
}
