using Cat.Seckill.Entities.BaseRepository;
using Cat.Seckill.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cat.Seckill.Base.EFCore.Service
{
    public class PayService : IPayService
    {
        private readonly IRepository<OrderInfo> _repositoryWrapper;

        public PayService(IRepository<OrderInfo> repositoryWrapper)
        {
            _repositoryWrapper = repositoryWrapper;
        }

        public void UpdateOrderPayState(OrderInfo orderInfo)
        {
            _repositoryWrapper.Update(orderInfo);
        }
    }
}
