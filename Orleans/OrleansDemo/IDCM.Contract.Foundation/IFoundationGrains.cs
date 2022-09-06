using System;
using System.Threading.Tasks;

namespace IDCM.Contract.Foundation
{
    public interface IFoundationGrains : Orleans.IGrainWithIntegerKey
    {
        Task<bool> GetError();
    }
}
