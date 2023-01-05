using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventureGrainInterfaces;

namespace AdventureGrains
{
  
    public class FakeMessageNext : Grain, IFakeMessageNext
    {
        public Task<string> GetMessage() => Task.FromResult("success002");
    }
}
