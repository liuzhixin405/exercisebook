using Project.Discuss.DTO;

namespace Project.Discuss.Domain.IServices
{
    public interface ICommentService
    {
        Task<List<CommentTreeModel>> Get(long articleId, string orderBy = "CommentTime", string sortOrder = "DESC");
    }
}
