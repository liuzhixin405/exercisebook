using IDCM.Contract.Foundation;
using Orleans;
using System;
using System.Threading.Tasks;

namespace IDCM.Contract.BarWebApi.Orleans
{
    public class FoundationGrains : Grain, IFoundationGrains
    {
        public Task<bool> GetError()
        {
            throw new Exception("未实现");
        }
    }
}
