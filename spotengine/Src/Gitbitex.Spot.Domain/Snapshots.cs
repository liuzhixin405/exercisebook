namespace Gitbitex.Spot.Domain.Matching;

public sealed class EngineSnapshot
{
    public required string ProductId { get; set; }
    public required OrderBookSnapshot OrderBookSnapshot { get; set; }
    public long OrderOffset { get; set; }
}

public sealed class OrderBookSnapshot
{
    public required string ProductId { get; set; }
    public required IReadOnlyList<BookOrder> Orders { get; set; }
    public long TradeSeq { get; set; }
    public long LogSeq { get; set; }
}
