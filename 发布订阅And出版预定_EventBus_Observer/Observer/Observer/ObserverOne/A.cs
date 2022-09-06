using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverOne
{
    internal class A
    {
        public int Data;
        public void Update(int data)
        {
            this.Data = data;
        }
    }
}
