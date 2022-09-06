using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandPattern01
{
    public class Receiver
    {
        private string name = string.Empty;
        public string Name => name;
        private string address = string.Empty;
        public string Address => address;
        public void SetName() => name = "name";
        public void SetAddress()=>address = "address";
    }
}
