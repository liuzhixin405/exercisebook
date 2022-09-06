using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverPattern.Concept2
{
    public interface IUpdatableObject
    {
        int Data { get; }
        void Update(int newData);
    }
    public class A : IUpdatableObject
    {
        private int data;
        public int Data => this.data;

        public void Update(int newData)
        {
            this.data = newData;
        }
    }
    public class B : IUpdatableObject
    {
        private int count;
        public int Data => count;

        public void Update(int newData)
        {
            this.count = newData;
        }
    }
    public class C : IUpdatableObject
    {
        private int n;
        public int Data => n;

        public void Update(int newData)
        {
            this.n = newData;
        }
    }

    public class X
    {
        private IUpdatableObject[] objects = new IUpdatableObject[3];
        public IUpdatableObject this[int index] { set { objects[index]= value; } }
        private int data;
        public void Update(int newData)
        {
            this.data = newData;
            foreach (var obj in objects)
            {
                obj.Update(newData);
            }
        }
    }
}
