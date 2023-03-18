namespace lx.eshop.models.Basket;

public interface IBasketRepository
{
    Task<CustomerBasket> GetBasketAsync(string customerId);
    IEnumerable<string> GetUsers();
    Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket);
    Task<bool> DeleteBasketAsync(string id);
}

