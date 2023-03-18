namespace lx.eshop.models.Basket;

public class CustomerBasket
{
    public string BuyerId { get; set; }

    public List<BasketItem> Items { get; set; } = new();

    public CustomerBasket()
    {

    }

    public CustomerBasket(string customerId)
    {
        BuyerId = customerId;
    }
}

