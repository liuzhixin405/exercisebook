using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Project.Discuss.Models;

public partial class BbsArticleCoin
{
    [Key]
    public long ArticleId { get; set; }

    public string CoinCode { get; set; } = null!;

    public string? CoinName { get; set; }

    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// 币种顺序，升序
    /// </summary>
    public sbyte? Sort { get; set; }

    public virtual BbsArticle Article { get; set; } = null!;
}
