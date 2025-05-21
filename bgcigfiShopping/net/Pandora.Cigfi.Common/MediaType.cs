using System.ComponentModel;

namespace Pandora.Cigfi.Common
{
    /// <summary>
    /// 媒体类型
    /// </summary>
    public enum MediaType : int
    {
        /// <summary>
        /// 图片
        /// </summary>
        [Description("图片")]
        image = 1,
        /// <summary>
        /// 视频
        /// </summary>
        [Description("视频")]
        video = 2
    }

    /// <summary>
    /// APP端 跳转类型 枚举
    /// </summary>
    public enum AppJumpTypeEnum : int
    {
        /// <summary>
        /// 不跳
        /// </summary>
        [Description("不跳")]
        NO_JUMP = 0,

        /// <summary>
        /// H5跳转
        /// </summary>
        [Description("H5跳转")]
        H5_JUMP = 1,

        /// <summary>
        /// APP端原生跳转
        /// </summary>
        [Description("APP端原生跳转")]
        APP_ORG_JUMP = 2,

        /// <summary>
        /// 弹出框
        /// </summary>
        [Description("弹出框")]
        DIALOG_JUMP = 3,
    }
}
