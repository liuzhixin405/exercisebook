using System;
using System.Collections.Generic;

namespace Project.Discuss.Models;

public partial class BbsArticleContent
{
    /// <summary>
    /// 文章id
    /// </summary>
    public long ArticleId { get; set; }

    /// <summary>
    /// 文章内容带html
    /// </summary>
    public string ContentHtml { get; set; } = null!;
}
