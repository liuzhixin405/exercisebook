namespace Gitbitex.Spot.Domain.Matching;

public abstract record LogBase(long Seq, string ProductId);

public sealed record OpenLog(long Seq, string ProductId, BookOrder Order)
    : LogBase(Seq, ProductId);

public sealed record MatchLog(long Seq, string ProductId, long TradeSeq,
    BookOrder Taker, BookOrder Maker, decimal Price, decimal Size)
    : LogBase(Seq, ProductId);

public sealed record DoneLog(long Seq, string ProductId, BookOrder Order,
    decimal RemainingSize, DoneReason Reason)
    : LogBase(Seq, ProductId);
