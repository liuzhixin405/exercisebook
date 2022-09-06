using System;
using System.Threading.Tasks;

namespace IDCM.Contract.BaseInfo
{
    public interface IBaseDataGrains : Orleans.IGrainWithIntegerKey
    {
        Task<bool> GetError();
        Task<bool> SaveOrder();
    }
}
