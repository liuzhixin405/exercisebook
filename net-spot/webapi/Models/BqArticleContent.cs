using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BqArticleContent
    {
        public uint ArticleId { get; set; }
        /// <summary>
        /// 新闻内容
        /// </summary>
        public string? Content { get; set; }
        public long CreditLogId { get; set; }
        public int CreditTypeId { get; set; }
        public long Dateline { get; set; }
        public long FUserId { get; set; }
        public string? Title { get; set; }
    }
}
