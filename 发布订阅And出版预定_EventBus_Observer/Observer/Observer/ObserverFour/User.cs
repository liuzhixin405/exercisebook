using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverFour
{
    internal class User
    {
        public event EventHandler<UserEventArgs> NameChanged;
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NameChanged?.Invoke(this, new UserEventArgs(value));
            }
        }
    }
}
