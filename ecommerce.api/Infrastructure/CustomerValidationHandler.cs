using ECommerce.API.Application;
using ECommerce.API.Models;

namespace ECommerce.API.Infrastructure
{
    public class CustomerValidationHandler : IValidationHandler<OrderRequest>
    {
        private readonly IValidationHandler<OrderRequest> _next;

        public CustomerValidationHandler(IValidationHandler<OrderRequest> next = null)
        {
            _next = next;
        }

        public string ErrorMessage => "Customer validation failed";

        public async Task<bool> ValidateAsync(OrderRequest request)
        {
            if (string.IsNullOrEmpty(request.CustomerId))
            {
                return false;
            }

            return _next == null || await _next.ValidateAsync(request);
        }
    }
}
