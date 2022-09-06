using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverTwo
{
    internal class X
    {
        private IUpdatebleObject[] updates=new IUpdatebleObject[3];

        public IUpdatebleObject this[int index]
        {
            set { updates[index] = value; }
        }
        private int data;
        public void Update(int newData)
        {
            this.data = newData;
            foreach (var update in updates)
            {
                update.Update(newData);
            }
        }
    }
}
