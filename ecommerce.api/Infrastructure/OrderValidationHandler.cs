using ECommerce.API.Application;
using ECommerce.API.Models;

namespace ECommerce.API.Infrastructure
{
    // 订单验证责任链
    public class OrderValidationHandler : IValidationHandler<OrderRequest>
    {
        private readonly IValidationHandler<OrderRequest> _next;

        public OrderValidationHandler(IValidationHandler<OrderRequest> next = null)
        {
            _next = next;
        }

        public string ErrorMessage => "Order validation failed";

        public async Task<bool> ValidateAsync(OrderRequest request)
        {
            if (request.Amount <= 0)
            {
                return false;
            }

            return _next == null || await _next.ValidateAsync(request);
        }
    }
}
