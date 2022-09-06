using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Core
{
    public abstract class BaseAOP:Attribute
    {
        public virtual async Task Befor(IAOPContext context)
        {
            await Task.CompletedTask;
        }

        public virtual async Task After(IAOPContext context)
        {
            await Task.CompletedTask;
        }
    }
}
