using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericesLib
{
    public class CompositeIterator
    {
        private IDictionary<object, IEnumerator> items = new Dictionary<object, IEnumerator>();

        public void Add(object data)
        {
            items.Add(data, GetEnumerator(data));
        }

        public IEnumerator GetEnumerator()
        {
            if(items!=null && items.Count > 0)
            {
                foreach (IEnumerator item in items.Values)
                {
                    while (item.MoveNext())
                        yield return item.Current;
                }
            }
        }
        public static IEnumerator GetEnumerator(object data)
        {
            if (data == null) throw new NullReferenceException();
            Type type = data.GetType();
            if(type.IsAssignableFrom(typeof(Stack))||
                type.IsAssignableFrom(typeof(Stack<ObjectWithName>)))
                {
                return DynamicInvokeEnumerator(data);
            }
            if(type.IsAssignableFrom(typeof(Queue))|| type.IsAssignableFrom(typeof(Queue<ObjectWithName>)))
            {
                return DynamicInvokeEnumerator(data);
            }
            if(type.IsArray && type.GetElementType().IsAssignableFrom(typeof(ObjectWithName)))
                    return ((ObjectWithName[])data).GetEnumerator();
            if (type.IsAssignableFrom(typeof(BinaryTreeNode)))
            {
                return ((BinaryTreeNode)data).GetEnumerator();
            }
            throw new NotSupportedException();
        }

        private static IEnumerator DynamicInvokeEnumerator(object data)
        {
            if (data == null) throw new NullReferenceException();
            Type type = data.GetType();
            return (IEnumerator)type.InvokeMember("GetEnumerator", System.Reflection.BindingFlags.InvokeMethod, null, data, null);
        }
    }
}
