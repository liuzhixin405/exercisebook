namespace Pandora.Cigfi.Web
{
    /// <summary>
    /// 图片配置
    /// </summary>
    public class ImageConfig
    {
        /// <summary>
        /// 用户头像
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// 图片前缀地址
        /// </summary>
        public string PrefixUrl { get; set; }

        /// <summary>
        /// M端图片前缀地址
        /// </summary>
        public string MobilePrefixUrl { get; set; }

        /// <summary>
        /// 图片上传的接口地址
        /// </summary>
        public string UploadImageAPI { get; set; }

        /// <summary>
        /// 资讯正文图片上传目录
        /// </summary>
        public string NewsContent { get; set; }

        /// <summary>
        /// 钱包二维码上传目录
        /// </summary>
        public string WltQrcode { get; set; }

        /// <summary>
        /// 钱包应用app二维码上传目录
        /// </summary>
        public string WltAppQrcode { get; set; }

        /// <summary>
        /// 交易所二维码上传目录
        /// </summary>
        public string ExQrcode { get; set; }

        /// <summary>
        /// 交易所应用app二维码上传目录
        /// </summary>
        public string ExAppQrcode { get; set; }

        /// <summary>
        /// 发现栏目图片上传目录
        /// </summary>
        public string ConfigDiscover { get; set; }
        /// <summary>
        /// NFT图片上传目录
        /// </summary>
        public string NFT { get; set; } = "nft/";
        /// <summary>
        /// 入驻用户
        /// </summary>
        public string Snsuser { get; set; }

        /// <summary>
        /// 币种logo图片上传目录
        /// </summary>
        public string CoinLogo { get; set; }

        /// <summary>
        /// 交易所logo图片上传目录
        /// </summary>
        public string ExchangeLogo { get; set; }

        /// <summary>
        /// 币圈百科
        /// </summary>
        public string CoinBaike { get; set; }
        /// <summary>
        ///友链（网站）导航
        /// </summary>
        public string FriendLink { get; set; }

        /// <summary>
        ///app 更新包文件
        /// </summary>
        public string AppDownLoad { get; set; }

        /// <summary>
        ///广告图
        /// </summary>
        public string AdConfig { get; set; }


        /// <summary>
        ///钱包
        /// </summary>
        public string Wallet { get; set; }
        /// <summary>
        ///圈子话题
        /// </summary>
        public string SnsTopics { get; set; }
        /// <summary>
        /// 临时目录
        /// </summary>

        public string Temp { get; set; } = "temp/";
        public string ImageCDN { get; set; } 

    }
}
