using Cat.Seckill.Entities.BaseRepository;
using Cat.Seckill.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cat.Seckill.Base.EFCore.Service
{
    public class OrderService:IOrderService
    {
        private readonly IRepository<OrderInfo> orderInfoRepository;
        public OrderService(IRepository<OrderInfo> orderInfoRepository)
        {
            this.orderInfoRepository = orderInfoRepository;
        }

        public async Task<OrderInfo> Create(OrderInfo orderInfo)
        {
           await orderInfoRepository.Create(orderInfo);  
            return orderInfo;
        }

        public Task Update(int userId, int goodsId)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePayState(OrderInfo orderInfo)
        {
            throw new NotImplementedException();
        }
    }
}
