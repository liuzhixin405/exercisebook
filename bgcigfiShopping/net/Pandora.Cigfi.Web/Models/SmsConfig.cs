namespace FXH.CMS.Web
{
    public class SmsConfig
    {
        /// <summary>
        ///短信发送接口地址
        /// </summary>
        public string SendUrl { get; set; }

        /// <summary>
        ///// 短信内容模版
        /// </summary>
        public string ContextFormat { get; set; }
    }
}
