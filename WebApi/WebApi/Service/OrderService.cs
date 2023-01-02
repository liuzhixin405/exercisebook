using WebApi.Data;
using WebApi.Models;
using WebApi.Repository;

namespace WebApi.Service
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Order, int> _orderRepository;
        private readonly IRepository<Product, int> _productRepository;
        public OrderService(IUnitOfWork unitOfWork, IRepository<Order, int> orderRepository, IRepository<Product, int> productRepository)
        {
            _unitOfWork= unitOfWork;
            _orderRepository= orderRepository;
            _productRepository= productRepository;
        }
        public async Task CreateOrder(string sku, int count)
        {
            Product product =await _productRepository.FindByCondition(x=>x.Sku == sku);
            if(product==null || product.Count < count)
            {
                throw new ArgumentException("no enugth");
            }

            product.Count -= count;
           await _productRepository.Update(product);
           await _orderRepository.Add(new Order { CreateTime= DateTime.Now, ProductId = product.Id, ShipmentState = ShipmentState.Created  });
           await _unitOfWork.CommitAsync();
        }
    }
}
