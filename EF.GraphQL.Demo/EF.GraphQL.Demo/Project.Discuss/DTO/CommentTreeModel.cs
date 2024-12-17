using Project.Discuss.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Discuss.DTO
{
    public class CommentTreeModel
    {
        #region 评论
        /// <summary>
        /// 评论ID
        /// </summary>
      
        public long CommentId { get; set; }

        /// <summary>
        /// 文章ID
        /// </summary>
        public long ArticleId { get; set; }
        /// <summary>
        /// 父评论ID
        /// </summary>

    
        public long? ParentCommentId { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
      
        public string CommentContent { get; set; }
        /// <summary>
        /// 评论图片地址
        /// </summary>

      
        public string ImageAddress { get; set; }
        /// <summary>
        /// 评论点赞数
        /// </summary>

   
        public int Likes { get; set; }
        /// <summary>
        /// 评论分享数 用不上可忽略
        /// </summary>
    
        public int Shares { get; set; }
        /// <summary>
        /// 评论数
        /// </summary>
      
        public int Comments { get; set; }
        /// <summary>
        /// 评论来源类型 暂时没用上 可忽略
        /// </summary>

       
        public int SourceType { get; set; }

        /// <summary>
        /// 评论状态
        /// </summary>
      
        public CmtStatus CommentStatus { get; set; }
        /// <summary>
        /// 评论热度
        /// </summary>
      

        public decimal Hot { get; set; }
        /// <summary>
        /// 是否为文章评论
        /// </summary>

      
        public bool IsCommentArticle { get; set; }

        /// <summary>
        /// 评论时间戳
        /// </summary>
      
        public long CommentTimespan { get; set; }

        #endregion

        /// <summary>
        /// 子评论
        /// </summary>

        public List<CommentTreeModel> Children { get; set; } = new();

        /// <summary>
        /// 格式化后图片地址
        /// </summary>
     
        public List<ImgDto> ImageAddresses { get; set; } // 新增字段：图片地址

        public CommentUserDto CommentUser { get; set; }

        /// <summary>
        /// 评论是否点赞过
        /// </summary>
       
        public bool CurrentUserLiked { get; set; }

        public CommentUserDto ReplyCommentUser { get; set; } = new();
        /// <summary>
        /// 评论时间
        /// </summary>
      
        public string CommentTime { get; set; }
    }
}
