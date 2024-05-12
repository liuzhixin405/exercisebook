using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStruct
{
    public class LinkedListHelper<T >
    {
            private Node<T> head;

        public LinkedListHelper()
        {   
            head = null;
        }
        public void Insert(T data)
        {
            Node<T> newNode = new Node<T>(data);
            if (head == null)
            {
                head = newNode;
            }
            else
            {
                Node<T> current = head;
                while (current.Next!= null)
                {
                    current = current.Next;
                }
                current.Next = newNode;
            }
        } 
        public void Remove(T data)
        {
            if (head == null)
            {
                return;
            }
            if (head.Data.Equals(data))
            {
                head = head.Next;
                return;
            }
            Node<T> current = head;
            while (current.Next!= null)
            {
                if (current.Next.Data.Equals(data))
                {
                    current.Next = current.Next.Next;
                    return;
                }
                current = current.Next;
            }
        }

        public Node<T> Find(T data)
        {
            Node<T> current = head;
            while (current != null)
            {
                if (current.Data.Equals(data))
                {
                    return current;
                }
                current = current.Next;
            }
            return null;
        }
        public void Print()
        {
            Node<T> current = head;
            while (current!= null)
            {
                Console.WriteLine(current.Data);
                current = current.Next;
            }
        }
    }

    public class Node<T>
    {
       public T Data;
       public Node<T> Next;
        public Node(T data)
        {
            Data = data;
            Next = null;
        }
    }
}
