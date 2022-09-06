using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandPattern01
{
    public class SetNameCommand : CommandBase
    {
        public override void Execute()
        {
            recceiver.SetName();
        }
    }
    public class SetAddressCommand : CommandBase
    {
        public override void Execute()
        {
            recceiver.SetAddress();
        }
    }
}
