namespace Project.Discuss.DTO
{
    public class CommentUserDto
    {
        public long UserId { get; set; }
        /// <summary>
        /// 评论用户名
        /// </summary>

        public string UserName { get; set; } // 新增字段：用户名


        public string Avatar { get; set; } // 新增字段：用户头像
        /// <summary>
        /// 用户是否认证
        /// </summary>

        public int? UserCertType { get; set; }
    }
}
