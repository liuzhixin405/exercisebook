using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merp.Infrastructure
{
    public interface IHandleMessage<in T>
    {
        void Handle(T message);
    }
}
