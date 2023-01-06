using AdventureGrainInterfaces;
using Test.Interfaces;

namespace Test.Implement
{
    public class FakeWorld:IFakeWorld
    {
        public Task<string> ReturnDestory()
        {
            return Task.FromResult( "__________" );
        }
    }
}