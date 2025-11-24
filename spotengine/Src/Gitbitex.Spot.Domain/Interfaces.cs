namespace Gitbitex.Spot.Domain.Matching;

public interface IOrderReader
{
    Task SetOffsetAsync(long offset, CancellationToken ct = default);
    Task<(long Offset, Order Order)> FetchOrderAsync(CancellationToken ct = default);
}

public interface ILogStore
{
    Task StoreAsync(IEnumerable<object> logs, CancellationToken ct = default);
}

public interface ISnapshotStore
{
    Task StoreAsync(EngineSnapshot snapshot, CancellationToken ct = default);
    Task<EngineSnapshot?> GetLatestAsync(CancellationToken ct = default);
}
