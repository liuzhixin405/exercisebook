using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BqArticle
    {
        public uint ArticleId { get; set; }
        /// <summary>
        /// 分类id(1公告咨询, 2信息披露)
        /// </summary>
        public ushort CategoryId { get; set; }
        /// <summary>
        /// 新闻标题
        /// </summary>
        public string Title { get; set; } = null!;
        /// <summary>
        /// 关键词
        /// </summary>
        public string Keyword { get; set; } = null!;
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; } = null!;
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; } = null!;
        /// <summary>
        /// 发布时间
        /// </summary>
        public uint Dateline { get; set; }
        /// <summary>
        /// 0:普通  1:置顶
        /// </summary>
        public bool Istop { get; set; }
        /// <summary>
        /// 显示顺序
        /// </summary>
        public uint Displayorder { get; set; }
        /// <summary>
        /// 0:未发布 1:发布中
        /// </summary>
        public sbyte State { get; set; }
        public byte[]? Page { get; set; }
        /// <summary>
        /// 语言
        /// </summary>
        public string Language { get; set; } = null!;
    }
}
