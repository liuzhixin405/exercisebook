using IDCM.Contract.IBusiness;
using System;
using System.Threading.Tasks;

namespace IDCM.Contract.Business
{
    public class OrderBusiness : IOrderBusiness
    {
        public Task<bool> Save(object data)
        {
            return Task.FromResult(true);
        }
    }
}
