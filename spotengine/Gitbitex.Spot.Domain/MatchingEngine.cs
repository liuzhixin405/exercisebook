using System.Threading.Channels;

namespace Gitbitex.Spot.Domain.Matching;

/// <summary>
/// 纯内存撮合引擎：内部用 Channel 串联订单、日志和快照，不依赖外部队列或存储。
/// 调用方通过构造函数提供订单源和日志/快照回调委托。
/// </summary>
public sealed class MatchingEngine
{
    private readonly string _productId;
    private readonly OrderBook _orderBook;

    private readonly Func<CancellationToken, Task<(long Offset, Order Order)>> _fetchOrderAsync;
    private readonly Func<IReadOnlyList<LogBase>, CancellationToken, Task> _onLogsAsync;
    private readonly Func<EngineSnapshot?, CancellationToken, Task<EngineSnapshot?>> _loadSnapshotAsync;
    private readonly Func<EngineSnapshot, CancellationToken, Task> _storeSnapshotAsync;

    private readonly Channel<(long Offset, Order Order)> _orderCh = Channel.CreateBounded<(long, Order)>(10000);
    private readonly Channel<LogBase> _logCh = Channel.CreateBounded<LogBase>(10000);
    private readonly Channel<EngineSnapshot> _snapshotReqCh = Channel.CreateBounded<EngineSnapshot>(32);
    private readonly Channel<EngineSnapshot> _snapshotApproveReqCh = Channel.CreateBounded<EngineSnapshot>(32);
    private readonly Channel<EngineSnapshot> _snapshotCh = Channel.CreateBounded<EngineSnapshot>(32);

    private long _orderOffset;

    public MatchingEngine(
        Product product,
        Func<CancellationToken, Task<(long Offset, Order Order)>> fetchOrderAsync,
        Func<IReadOnlyList<LogBase>, CancellationToken, Task> onLogsAsync,
        Func<EngineSnapshot?, CancellationToken, Task<EngineSnapshot?>> loadSnapshotAsync,
        Func<EngineSnapshot, CancellationToken, Task> storeSnapshotAsync)
    {
        _productId = product.Id;
        _orderBook = new OrderBook(product);
        _fetchOrderAsync = fetchOrderAsync;
        _onLogsAsync = onLogsAsync;
        _loadSnapshotAsync = loadSnapshotAsync;
        _storeSnapshotAsync = storeSnapshotAsync;
    }

    public async Task StartAsync(CancellationToken ct = default)
    {
        var latest = await _loadSnapshotAsync(null, ct).ConfigureAwait(false);
        if (latest is not null)
        {
            Restore(latest);
        }

        var fetcher = RunFetcherAsync(ct);
        var applier = RunApplierAsync(ct);
        var committer = RunCommitterAsync(ct);
        var snapshots = RunSnapshotsAsync(ct);

        await Task.WhenAll(fetcher, applier, committer, snapshots).ConfigureAwait(false);
    }

    private async Task RunFetcherAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var (offset, order) = await _fetchOrderAsync(ct).ConfigureAwait(false);
            await _orderCh.Writer.WriteAsync((offset, order), ct).ConfigureAwait(false);
        }
    }

    private async Task RunApplierAsync(CancellationToken ct)
    {
        var orderOffset = _orderOffset;

        while (!ct.IsCancellationRequested)
        {
            var readOrderTask = _orderCh.Reader.ReadAsync(ct).AsTask();
            var readSnapshotReqTask = _snapshotReqCh.Reader.ReadAsync(ct).AsTask();

            var completed = await Task.WhenAny(readOrderTask, readSnapshotReqTask).ConfigureAwait(false);

            if (completed == readOrderTask)
            {
                var (offset, order) = await readOrderTask.ConfigureAwait(false);

                IReadOnlyList<LogBase> logs;
                if (string.Equals(order.Status, "Cancelling", StringComparison.OrdinalIgnoreCase))
                {
                    logs = _orderBook.CancelOrder(order);
                }
                else
                {
                    logs = _orderBook.ApplyOrder(order);
                }

                foreach (var log in logs)
                {
                    await _logCh.Writer.WriteAsync(log, ct).ConfigureAwait(false);
                }

                orderOffset = offset;
            }
            else
            {
                var snapshot = await readSnapshotReqTask.ConfigureAwait(false);
                var delta = orderOffset - snapshot.OrderOffset;
                if (delta <= 1000)
                {
                    continue;
                }

                var obSnapshot = _orderBook.Snapshot();
                snapshot.OrderBookSnapshot = obSnapshot;
                snapshot.OrderOffset = orderOffset;

                await _snapshotApproveReqCh.Writer.WriteAsync(snapshot, ct).ConfigureAwait(false);
            }
        }
    }

    private async Task RunCommitterAsync(CancellationToken ct)
    {
        long seq = _orderBook.CurrentLogSeq;
        EngineSnapshot? pending = null;
        var buffer = new List<LogBase>();

        while (!ct.IsCancellationRequested)
        {
            var readLogTask = _logCh.Reader.ReadAsync(ct).AsTask();
            var readSnapshotApproveTask = _snapshotApproveReqCh.Reader.ReadAsync(ct).AsTask();

            var completed = await Task.WhenAny(readLogTask, readSnapshotApproveTask).ConfigureAwait(false);

            if (completed == readLogTask)
            {
                var log = await readLogTask.ConfigureAwait(false);
                if (log.Seq <= seq)
                {
                    continue;
                }

                seq = log.Seq;
                buffer.Add(log);

                while (_logCh.Reader.TryRead(out var more) && buffer.Count < 100)
                {
                    if (more.Seq <= seq) continue;
                    seq = more.Seq;
                    buffer.Add(more);
                }

                await _onLogsAsync(buffer, ct).ConfigureAwait(false);
                buffer.Clear();

                if (pending is not null && seq >= pending.OrderBookSnapshot.LogSeq)
                {
                    await _snapshotCh.Writer.WriteAsync(pending, ct).ConfigureAwait(false);
                    pending = null;
                }
            }
            else
            {
                var snapshot = await readSnapshotApproveTask.ConfigureAwait(false);
                if (seq >= snapshot.OrderBookSnapshot.LogSeq)
                {
                    await _snapshotCh.Writer.WriteAsync(snapshot, ct).ConfigureAwait(false);
                    pending = null;
                    continue;
                }

                pending = snapshot;
            }
        }
    }

    private async Task RunSnapshotsAsync(CancellationToken ct)
    {
        var orderOffset = _orderOffset;

        while (!ct.IsCancellationRequested)
        {
            var delayTask = Task.Delay(TimeSpan.FromSeconds(10), ct);
            var readSnapshotTask = _snapshotCh.Reader.ReadAsync(ct).AsTask();

            var completed = await Task.WhenAny(delayTask, readSnapshotTask).ConfigureAwait(false);

            if (completed == delayTask)
            {
                await _snapshotReqCh.Writer.WriteAsync(new EngineSnapshot
                {
                    ProductId = _productId,
                    OrderBookSnapshot = new OrderBookSnapshot
                    {
                        ProductId = _productId,
                        Orders = Array.Empty<BookOrder>(),
                        LogSeq = 0,
                        TradeSeq = 0
                    },
                    OrderOffset = orderOffset
                }, ct).ConfigureAwait(false);
            }
            else
            {
                var snapshot = await readSnapshotTask.ConfigureAwait(false);
                await _storeSnapshotAsync(snapshot, ct).ConfigureAwait(false);
                orderOffset = snapshot.OrderOffset;
            }
        }
    }

    private void Restore(EngineSnapshot snapshot)
    {
        _orderOffset = snapshot.OrderOffset;
        _orderBook.Restore(snapshot.OrderBookSnapshot);
    }
}
