using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverOne
{
    internal class X
    {
        private int data;
        public A instanceA;
        public B instanceB;
        public C instanceC;
        public void SetData(int data)
        {
            this.data = data;
            instanceA.Update(data);
            instanceB.Notify(data);
            instanceC.Set(data);
        }
    }
}
