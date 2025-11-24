namespace Gitbitex.Spot.Domain.Matching;

public sealed class Depth
{
    private readonly Dictionary<long, BookOrder> _orders = new();
    private readonly SortedDictionary<(decimal Price, long OrderId), long> _queue;

    private Depth(IComparer<(decimal Price, long OrderId)> comparer)
    {
        _queue = new SortedDictionary<(decimal Price, long OrderId), long>(comparer);
    }

    public static Depth CreateAskDepth() =>
        new Depth(Comparer<(decimal Price, long OrderId)>.Create((a, b) =>
        {
            var priceCmp = a.Price.CompareTo(b.Price);
            if (priceCmp != 0) return priceCmp;
            return a.OrderId.CompareTo(b.OrderId);
        }));

    public static Depth CreateBidDepth() =>
        new Depth(Comparer<(decimal Price, long OrderId)>.Create((a, b) =>
        {
            var priceCmp = b.Price.CompareTo(a.Price);
            if (priceCmp != 0) return priceCmp;
            return a.OrderId.CompareTo(b.OrderId);
        }));

    public IEnumerable<BookOrder> IterateInMatchOrder()
    {
        foreach (var kv in _queue)
        {
            if (_orders.TryGetValue(kv.Value, out var order))
            {
                yield return order;
            }
        }
    }

    public void Add(BookOrder order)
    {
        _orders[order.OrderId] = order;
        _queue[(order.Price, order.OrderId)] = order.OrderId;
    }

    public void DecreaseSize(long orderId, decimal size)
    {
        if (!_orders.TryGetValue(orderId, out var order))
            throw new InvalidOperationException($"order {orderId} not found on book");

        if (order.Size < size)
            throw new InvalidOperationException($"order {orderId} size {order.Size} less than {size}");

        order.Size -= size;
        if (order.Size == 0)
        {
            _orders.Remove(orderId);
            _queue.Remove((order.Price, order.OrderId));
        }
    }

    public bool TryGet(long orderId, out BookOrder order)
    {
        if (_orders.TryGetValue(orderId, out var o))
        {
            order = o;
            return true;
        }

        order = null!;
        return false;
    }

    public IEnumerable<BookOrder> AllOrders() => _orders.Values;
}
