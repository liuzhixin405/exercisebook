using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverPattern.Eventing
{
    /// <summary>
    /// 具体的subjecct
    /// </summary>
    public class UserEventArgs : EventArgs
    {
        private string name;
        public UserEventArgs (string name) { this.name = name; }
        public string Name => name;
    }
    public class User
    {
        public event EventHandler<UserEventArgs> NamedChanged;
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                Name = value;
                NamedChanged(this, new UserEventArgs(value));
            }
        }
    }
}
