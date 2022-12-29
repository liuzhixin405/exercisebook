using Cat.Seckill.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cat.Seckill.Base.EFCore.Service
{
    public interface IGoodsService
    {
        Task<bool> ReduceStock(int id);
        Task<Goods> FindById(int id);
    }
}
