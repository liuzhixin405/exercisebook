using CodeMan.Seckill.Entities.Models;
using CodeMan.Seckill.Service.Repository;

namespace CodeMan.Seckill.Service.service
{
    public class PayService : IPayService
    {
        private readonly IRepositoryWrapper _repositoryWrapper;

        public PayService(IRepositoryWrapper repositoryWrapper)
        {
            _repositoryWrapper = repositoryWrapper;
        }
        
        public void UpdateOrderPayState(OrderInfo orderInfo)
        {
            _repositoryWrapper.OrderInfo.Update(orderInfo);
        }
    }
}