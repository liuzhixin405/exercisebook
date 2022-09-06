using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverOne
{
    internal class B
    {
        public int Count;
        public void Notify(int data)
        {
            this.Count = data;
        }
    }
}
