using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleAppTwo.Extends
{
    public class InvocationSourceFeature : IInvocationSourceFeature
    {
        public string Source { get; }
        public InvocationSourceFeature(string source)
        {
            Source = source;
        }
    }
}
