namespace Pandora.Cigfi.Web
{
    public class ApiConfig
    {
        /// <summary>
        /// 资讯直接推送接口地址
        /// </summary>
        public string MediaPushUrl { get; set; }
        /// <summary>
        /// 邮件发送接口地址
        /// </summary>
        public string PostEmailUrl { get; set; }

        /// <summary>
        /// 入驻方资讯文章链接前缀
        /// </summary>
        public string InpartyNewsUrl { get; set; }


        /// <summary>
        /// 文件上传阿里云前缀
        /// </summary>
        public string AliyunOssUrl { get; set; }


        /// <summary>
        /// 用户服务api地址
        /// </summary>
        public string UserServiceUri { get; set; }
    }
}
