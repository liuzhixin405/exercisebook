using Cat.Seckill.Base.RabbitMq.Message;
using Cat.Seckill.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Cat.Seckill.Base.EFCore.Service
{
    public interface ISeckillService
    {
        Task<IEnumerable<SeckillGoods>> FindSeckillGoods();
        Task Seckill(int userId, int goodId);
        Task<bool> Create(SeckillGoods seckillGoods);
        Task SeckillOrder(SeckillMessage? seckillMessage);
    }
}
