using AgenticCommerceErpDemo.Domain.Knowledge;

namespace AgenticCommerceErpDemo.Application.Knowledge;

public interface IKnowledgeBase
{
    IReadOnlyList<KnowledgeDocument> Search(string query);
}
