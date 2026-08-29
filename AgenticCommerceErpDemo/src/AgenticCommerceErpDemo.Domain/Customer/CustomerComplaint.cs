namespace AgenticCommerceErpDemo.Domain.Customer;

public sealed record CustomerComplaint(string Id, string Sku, string Channel, string Text, DateTimeOffset CreatedAt);
