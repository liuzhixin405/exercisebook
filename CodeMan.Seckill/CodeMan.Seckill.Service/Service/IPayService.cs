using CodeMan.Seckill.Entities.Models;
using CodeMan.Seckill.Service.Repository;

namespace CodeMan.Seckill.Service.service
{
    public interface IPayService
    {
        public void UpdateOrderPayState(OrderInfo orderInfo);
    }
}