using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mfc.Core
{
    public enum EntityState
    {
        Added,
        Modified,
        Deleted,
        Unchanged,
        Detached  //不跟踪
    }
}
