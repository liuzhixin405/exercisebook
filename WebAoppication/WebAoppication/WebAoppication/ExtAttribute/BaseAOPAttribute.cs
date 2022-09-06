using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAoppication
{
    public class BaseAOPAttribute:Attribute
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
