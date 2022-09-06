using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverTwo
{
    internal class A : IUpdatebleObject
    {
        public int Data => data;
        private int data;

        public void Update(int newData)
        {
            this.data = newData;
        }
    }
}
