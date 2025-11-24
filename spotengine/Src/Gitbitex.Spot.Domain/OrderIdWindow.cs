namespace Gitbitex.Spot.Domain.Matching;

public sealed class OrderIdWindow
{
    private readonly long _cap;
    private readonly Queue<long> _queue = new();
    private readonly HashSet<long> _set = new();

    public OrderIdWindow(long start, long cap)
    {
        _cap = cap;
    }

    public bool TryPut(long orderId)
    {
        if (_set.Contains(orderId))
            return false;

        _queue.Enqueue(orderId);
        _set.Add(orderId);

        while (_queue.Count > _cap)
        {
            var old = _queue.Dequeue();
            _set.Remove(old);
        }

        return true;
    }
}
