using Mapster;
using Microsoft.EntityFrameworkCore;
using Project.Discuss.Domain;
using Project.Discuss.Domain.IServices;
using Project.Discuss.DTO;
using Project.Discuss.Models;

namespace Project.Discuss.Services
{
    public class CommentService:ICommentService
    {
        private readonly DiscussDbContext _context;
        public CommentService(DiscussDbContext context)
        {
            _context = context;
        }

        public async Task<List<CommentTreeModel>> Get(long articleId,string orderBy="CommentTime",string sortOrder = "DESC")
        {
            // 验证排序字段，防止 SQL 注入
            string validOrderBy = orderBy?.ToLower() switch
            {
                "Hot" => "Hot",
                "CommentTime" => "CommentTime",
                _ => "CommentTime" // 默认按时间排序
            };

            // 验证排序方式，防止 SQL 注入
            string validSortOrder = sortOrder?.ToUpper() switch
            {
                "ASC" => "ASC",
                _ => "DESC" // 默认按降序排序
            };

            // 获取查询的基础排序表达式
            IQueryable<BbsComment> query = _context.BbsComments
                .Where(c => c.ArticleId == articleId && c.CommentStatus == 2);

            // 动态选择排序
            if (validSortOrder == "ASC")
            {
                query = validOrderBy switch
                {
                    "Hot" => query.OrderBy(c => c.Hot),
                    "CommentTime" => query.OrderBy(c => c.CommentTime),
                    _ => query.OrderBy(c => c.CommentTime) // 默认按时间排序
                };
            }
            else
            {
                query = validOrderBy switch
                {
                    "Hot" => query.OrderByDescending(c => c.Hot),
                    "CommentTime" => query.OrderByDescending(c => c.CommentTime),
                    _ => query.OrderByDescending(c => c.CommentTime) // 默认按时间排序
                };
            }

            // 执行查询，获取评论数据
            var comments = await query.ToListAsync();


            // 使用递归函数构建评论树
            List<CommentTreeModel> BuildTree(IEnumerable<Models.BbsComment> queryComments)
            {
                var commentDict = queryComments.ToDictionary(c => c.CommentId);

                // 获取一级评论（ParentCommentID == 0）
                var rootComments = queryComments.Where(c => c.IsCommentArticle == true && c.IsShow == true).ToList();
                var result = new List<CommentTreeModel>();

                foreach (var parent in rootComments)
                {
                    var rootCommentTree = parent.Adapt<CommentTreeModel>();
                    var childrenList = new List<CommentTreeModel>();

                    BuildChildrenForAllLevels(rootCommentTree, queryComments, commentDict, childrenList);
                    rootCommentTree.Children.AddRange(childrenList); // 递归获取子评论
                    result.Add(rootCommentTree);
                }
                return result;
            }

            // 构建树形结构
            var commentTree = BuildTree(comments);

            return commentTree;
        }
        private void BuildChildrenForAllLevels(CommentTreeModel parentComment, IEnumerable<BbsComment> allComments, Dictionary<long, BbsComment> commentDict, List<CommentTreeModel> childrenList)
        {
            // 查找当前评论的所有子评论
            var children = allComments.Where(c => c.ParentCommentId == parentComment.CommentId && c.IsShow == true).ToList();


            foreach (var child in children)
            {
                var childCommentTree = child.Adapt<CommentTreeModel>();

                // 设置 ToUserId（父评论的 CommentUserID）
                if (child.ParentCommentId!=0)
                {
                    var parentc = commentDict[child.ParentCommentId];
                   
                    childCommentTree.ReplyCommentUser.UserId = parentc.CommentUserId;
                }
                if (child.IsShow)
                    // 将子评论加入到 childrenList 中（而不是递归放入子评论的 Children 中）
                    childrenList.Add(childCommentTree);

                // 递归处理子评论的子评论（但不再放入 parentComment.Children 中，而是继续递归处理）
                BuildChildrenForAllLevels(childCommentTree, allComments, commentDict, childrenList);
            }
        }
    }
}
