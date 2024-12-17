using System.ComponentModel;

namespace Project.Discuss.Models.Enums
{
    public enum CmtStatus
    {
        /// <summary>
        /// 等待审核
        /// </summary>
        [Description("等待审核")]
        Wait = 1,
        /// <summary>
        /// 
        /// </summary>
        [Description("审核通过")]
        Approved,
        /// <summary>
        /// 审核不通过
        /// </summary>
        [Description("审核不通过")]
        Rejected,
        /// <summary>
        /// 已删除
        /// </summary>
        [Description("已删除")]
        Deleted
    }
}
