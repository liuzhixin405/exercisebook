namespace AgenticCommerceErpDemo.Domain.Knowledge;

public sealed record KnowledgeDocument(string Id, string Title, string Content, IReadOnlyList<string> Tags);
