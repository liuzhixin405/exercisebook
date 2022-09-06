using System;
using System.Threading.Tasks;

namespace IDCM.Contract.IBusiness
{
    public interface IOrderBusiness
    {
        Task<bool> Save(object data);
    }
}
