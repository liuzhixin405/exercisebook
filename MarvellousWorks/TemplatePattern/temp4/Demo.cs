using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplatePattern.temp4
{
    public class TemplateList<T>
    {
        /// <summary>
        /// 表示链表的数据结构,其算法结构相对固定，但类型参数<T>可变
        /// </summary>
        private class Node
        {
            public T Data;
            public Node Next;
            public Node(T data)
            {
                this.Data = data;
                Next = null;
            }
        }
            private Node head = null;
            /// <summary>
            /// 在链表头增加新的节点
            /// </summary>
            /// <param name="data"></param>
            public void Add(T data)
            {
                Node node = new Node(data);
                node.Next = head;
                head = node;
            }

            public IEnumerator<T> GetEnumerator()
            {
                Node current = head;
                while (current != null)
                {
                    yield return current.Data;
                    current = current.Next;
                }
            }
    }
}
