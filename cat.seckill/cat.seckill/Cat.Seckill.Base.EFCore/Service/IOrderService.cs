using Cat.Seckill.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cat.Seckill.Base.EFCore.Service
{
    public interface IOrderService
    {
        Task Update(int userId, int goodsId);
        Task<OrderInfo> Create(OrderInfo orderInfo);
        Task UpdatePayState(OrderInfo orderInfo);
    }
}
