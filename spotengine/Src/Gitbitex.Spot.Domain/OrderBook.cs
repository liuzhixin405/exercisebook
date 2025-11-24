namespace Gitbitex.Spot.Domain.Matching;

public sealed class OrderBook
{
    private readonly Product _product;
    private readonly Depth _bids;
    private readonly Depth _asks;
    private readonly OrderIdWindow _orderIdWindow;
    private long _tradeSeq;
    private long _logSeq;

    public OrderBook(Product product)
    {
        _product = product;
        _bids = Depth.CreateBidDepth();
        _asks = Depth.CreateAskDepth();
        _orderIdWindow = new OrderIdWindow(0, 10000);
    }

    private Depth GetDepth(Side side) => side == Side.Buy ? _bids : _asks;

    private long NextLogSeq() => ++_logSeq;
    private long NextTradeSeq() => ++_tradeSeq;

    public long CurrentLogSeq => _logSeq;

    public IReadOnlyList<LogBase> ApplyOrder(Order order)
    {
        if (!_orderIdWindow.TryPut(order.Id))
            return Array.Empty<LogBase>();

        var logs = new List<LogBase>();
        var taker = BookOrder.From(order);

        if (taker.Type == OrderType.Market)
        {
            taker.Price = taker.Side == Side.Buy ? decimal.MaxValue : decimal.Zero;
        }

        var makerDepth = taker.Side == Side.Buy ? _asks : _bids;

        foreach (var maker in makerDepth.IterateInMatchOrder())
        {
            if (taker.Side == Side.Buy && taker.Price < maker.Price) break;
            if (taker.Side == Side.Sell && taker.Price > maker.Price) break;

            var price = maker.Price;
            decimal size;

            if (taker.Type == OrderType.Limit ||
                (taker.Type == OrderType.Market && taker.Side == Side.Sell))
            {
                if (taker.Size <= 0) break;
                size = decimal.Min(taker.Size, maker.Size);
                taker.Size -= size;
            }
            else
            {
                if (taker.Funds <= 0) break;

                var scale = (decimal)Math.Pow(10, _product.BaseScale);
                var takerSize = Math.Truncate((double)(taker.Funds / price * scale)) / (double)scale;
                var takerSizeDec = (decimal)takerSize;

                if (takerSizeDec <= 0) break;

                size = decimal.Min(takerSizeDec, maker.Size);
                var funds = size * price;
                taker.Funds -= funds;
            }

            makerDepth.DecreaseSize(maker.OrderId, size);

            logs.Add(new MatchLog(NextLogSeq(), _product.Id, NextTradeSeq(),
                taker, maker, price, size));

            if (maker.Size == 0)
            {
                logs.Add(new DoneLog(NextLogSeq(), _product.Id, maker, 0m, DoneReason.Filled));
            }
        }

        if (taker.Type == OrderType.Limit && taker.Size > 0)
        {
            GetDepth(taker.Side).Add(taker);
            logs.Add(new OpenLog(NextLogSeq(), _product.Id, taker));
        }
        else
        {
            var remainingSize = taker.Size;
            var reason = DoneReason.Filled;

            if (taker.Type == OrderType.Market)
            {
                taker.Price = 0m;
                remainingSize = 0m;
                if ((taker.Side == Side.Sell && taker.Size > 0) ||
                    (taker.Side == Side.Buy && taker.Funds > 0))
                {
                    reason = DoneReason.Cancelled;
                }
            }

            logs.Add(new DoneLog(NextLogSeq(), _product.Id, taker, remainingSize, reason));
        }

        return logs;
    }

    public IReadOnlyList<LogBase> CancelOrder(Order order)
    {
        _orderIdWindow.TryPut(order.Id);

        var depth = GetDepth(order.Side);
        if (!depth.TryGet(order.Id, out var bookOrder))
        {
            return Array.Empty<LogBase>();
        }

        var remainingSize = bookOrder.Size;
        depth.DecreaseSize(order.Id, bookOrder.Size);

        var doneLog = new DoneLog(NextLogSeq(), _product.Id, bookOrder, remainingSize, DoneReason.Cancelled);
        return new LogBase[] { doneLog };
    }

    public OrderBookSnapshot Snapshot()
    {
        var orders = new List<BookOrder>();

        orders.AddRange(_asks.AllOrders());
        orders.AddRange(_bids.AllOrders());

        return new OrderBookSnapshot
        {
            ProductId = _product.Id,
            Orders = orders,
            LogSeq = _logSeq,
            TradeSeq = _tradeSeq
        };
    }

    public void Restore(OrderBookSnapshot snapshot)
    {
        _logSeq = snapshot.LogSeq;
        _tradeSeq = snapshot.TradeSeq;

        foreach (var order in snapshot.Orders)
        {
            GetDepth(order.Side).Add(order);
        }
    }
}
