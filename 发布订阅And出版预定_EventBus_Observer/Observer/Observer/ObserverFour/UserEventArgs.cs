using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverFour
{
    internal class UserEventArgs:EventArgs
    {
        private string name;
        public string Name => name;

        public UserEventArgs(string name)
        {
            this.name = name;
        }

    }
}
