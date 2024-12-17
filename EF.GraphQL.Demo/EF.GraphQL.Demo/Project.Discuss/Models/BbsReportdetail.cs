using System;
using System.Collections.Generic;

namespace Project.Discuss.Models;

/// <summary>
/// 文章举报明细
/// </summary>
public partial class BbsReportdetail
{
    /// <summary>
    /// 举报明细ID
    /// </summary>
    public long ReportId { get; set; }

    /// <summary>
    /// 文章ID
    /// </summary>
    public long ArticleId { get; set; }

    /// <summary>
    /// 举报人
    /// </summary>
    public long ReportUserId { get; set; }

    /// <summary>
    /// 举报理由，0：未设置，1：色情，2：广告，4：政治，8：其他
    /// </summary>
    public short ReportType { get; set; }

    /// <summary>
    /// 举报说明
    /// </summary>
    public string ReportDescription { get; set; } = null!;

    /// <summary>
    /// 举报时间
    /// </summary>
    public DateTime ReportTime { get; set; }

    /// <summary>
    /// 举报状态（1-待确认，2-已确认，3-已处理）
    /// </summary>
    public short ReportStatus { get; set; }

    /// <summary>
    /// 举报处理结果
    /// </summary>
    public short ReportResult { get; set; }

    /// <summary>
    /// 举报处理结果说明
    /// </summary>
    public string? ReportResultDescription { get; set; }
}
