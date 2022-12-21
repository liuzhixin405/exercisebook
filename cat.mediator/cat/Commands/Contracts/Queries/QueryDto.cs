namespace cat.Commands.Contracts.Queries
{
    public record struct QueryDto(int? id,string? name,DateTime?startTime,DateTime? endTime);
}
