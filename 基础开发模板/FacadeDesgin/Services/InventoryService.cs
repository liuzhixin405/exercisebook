namespace FacadeDesgin.Services;

public class InventoryService : IInventoryService
{
    private readonly Dictionary<string,int> _stock = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Widget"] = 10,
        ["Gadget"] = 5,
        ["Thing"] = 20
    };

    public Task<bool> ReserveAsync(string product, int quantity)
    {
        lock (_stock)
        {
            if (!_stock.TryGetValue(product, out var available) || available < quantity)
                return Task.FromResult(false);

            _stock[product] = available - quantity;
            return Task.FromResult(true);
        }
    }

    public Task ReleaseAsync(string product, int quantity)
    {
        lock (_stock)
        {
            if (!_stock.ContainsKey(product)) _stock[product] = 0;
            _stock[product] += quantity;
            return Task.CompletedTask;
        }
    }
}
