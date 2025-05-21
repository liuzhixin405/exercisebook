using System;
using System.Collections.Generic;
using System.Text;

namespace Pandora.Cigfi.Models.Consts
{
    /// <summary>
    /// 事件按钮常量
    /// </summary>
    public class ResponseMsg
    {
        //
        // 摘要:
        //     成功
        public const string SUCCESS = "success";
        //
        // 摘要:
        //     失败
        public const string FAIL = "fail";
        //
        // 摘要:
        //     出错
        public const string ERROR = "error";
        //
        // 摘要:
        //     鉴权失败
        public const string AUTHFAIL = "authfail";
        //
        // 摘要:
        //     鉴权成功但是用户无权访问
        public const string AUTHSUC_NORIGHT = "noright";
        //
        // 摘要:
        //     找不到
        public const string NOTFOUND = "notfound";
        //
        // 摘要:
        //     给出的参数超出范围
        public const string PARAM_ERROR = "paramerror";
        //
        // 摘要:
        //     服务器出错
        public const string SERVER_ERROR = "servererror";


        ///// <summary>
        ///// 用户未登录，请先登录
        ///// </summary>
        //public const string UNLOGIN = "用户未登录，请先登录！";
        /// <summary>
        /// 无效签名
        /// </summary>
        public const string INVALIDSIGNATURE = "invalidsignature";
        /// <summary>
        /// 缺少qq防水墙的ticket参数
        /// </summary>
        public const string INVALID_QQTICKET = "缺失Ticket或Randstr参数！";


        #region 资产相关

        /// <summary>
        /// 请选择币种
        /// </summary>
        public const string ASSET_SELECTCOIN = "selectcoin";
        /// <summary>
        /// 请输入买入数量/请输入卖出数量
        /// </summary>
        public const string ASSET_INPUTBUYORSELLNUM = "inputbuyorsellnum";

        /// <summary>
        /// 请输入买入数量/请输入卖出价格
        /// </summary>
        public const string ASSET_INPUTBUYORSELLPRICE = "inputbuyorsellprice";
        /// <summary>
        /// 余额不足
        /// </summary>
        public const string ASSET_INSUFFIENTBALANCE = "insufficientbalance";
        /// <summary>
        /// 备注字数超出范围
        /// </summary>
        public const string ASSET_REMARKOUTRANGE = "remarkoutrange";
        /// <summary>
        /// 钱包地址格式不对
        /// </summary>
        public const string ASSET_WALLETADDRERROR = "walletaddrerror";
        /// <summary>
        /// 未选择
        /// </summary> 
        public const string ASSET_UNSELECTED = "unselected";

        #endregion

        /// <summary>
        /// 删除失败
        /// </summary>
        public const string DELFAIL = "delfail";
        /// <summary>
        /// 添加失败
        /// </summary>
        public const string ADDFAIL = "addfail";
        /// <summary>
        /// 编辑失败
        /// </summary>
        public const string EDITFAIL = "editfail";
        /// <summary>
        /// 关注失败
        /// </summary>
        public const string FOCUSFAIL = "focusfail";
        /// <summary>
        /// 保存失败
        /// </summary>
        public const string SAVEFAIL = "savefail";

        /// <summary>
        /// 已存在该手机号码用户
        /// </summary>
        public const string EXISTSPHONE = "existsphone";
        /// <summary>
        /// 请输入正确的手机号码
        /// </summary>
        public const string PHONEERROR = "phoneerror";
        /// <summary>
        /// 请输入正确的短信验证码
        /// </summary>
        public const string SMSCODEERROR = "smscodeerror";
        /// <summary>
        /// 请输入正确的用户密码
        /// </summary>
        public const string PWDERROR = "pwderror";
        /// <summary>
        /// 注册失败
        /// </summary>
        public const string REGFAIL = "regfail";
        /// <summary>
        /// 登录失败
        /// </summary>
        public const string LOGINFAIL = "loginfail";


        /// <summary>
        /// 密码不能小于6个字符
        /// </summary>
        public const string PWDLENGTH = "pwdlength";

        /// <summary>
        /// 两次输入的密码不一致
        /// </summary>
        public const string PWDUNEQUAL = "pwdunequal";

        /// <summary>
        /// 重置密码失败
        /// </summary>
        public const string RESETPWDFAIL = "resetpwdfail";
        /// <summary>
        /// 注销失败
        /// </summary>
        public const string LOGOUTFAIL = "logoutfail";
        /// <summary>
        /// 上传的图片格式有误
        /// </summary>
        public const string IMAGEFORMATERROR = "imageformaterror";

        /// <summary>
        /// 上传图片失败
        /// </summary>
        public const string UNLOADERROR = "uploaderror";
        /// <summary>
        /// 修改手机号码失败
        /// </summary>
        public const string EDITPHONEFAIL = "editphonefail";
        /// <summary>
        /// 用户信息修改失败
        /// </summary>
        public const string EDITUSERINFOFAIL = "edituserinfofail";
        /// <summary>
        /// 获取数据失败
        /// </summary>
        public const string GETDATAERROR = "getdataerror";
        /// <summary>
        /// 用户未找到
        /// </summary>
        public const string USERNOTFOUND = "usernotfound";
        /// <summary>
        /// 已关注
        /// </summary>
        public const string FOCUSED = "alreadyfocus";
        /// <summary>
        /// 已关注
        /// </summary>
        public const string UNFOCUS = "unfocus";
        /// <summary>
        /// 昵称包含敏感信息
        /// </summary>
        public const string NICKNAMES_CONTAIN_SENSITIVE_INFORMATION = "Nicknames can not contain sensitive words, please modify";
        /// <summary>
        /// 昵称重复
        /// </summary>
        public const string NICKNAME_REPETITION = "Nickname Repetition";
        /// <summary>
        /// 请求次数太多
        /// </summary>
        public const string TOO_MANY_REQUESTS = "TOO_MANY_REQUESTS";

        //todo：翻译
        /// <summary>
        /// 上传类型错误
        /// </summary>
        public const string UPDLOADTYPE_FORMATERROR = "UPDLOADTYPE_FORMATERROR";
        /// <summary>
        /// 已经存在相似的资讯了
        /// </summary>
        public const string NEWS_REPETITION = "NEWS_REPETITION";
        /// <summary>
        /// 事件名称不能为空
        /// </summary>
        public const string EVENTNAME_ISNULL = "EVENTNAME_ISNULL";
        /// <summary>
        /// 分类不能为空
        /// </summary>
        public const string TYPEID_ISNULL = "TYPEID_ISNULL";
        /// <summary>
        /// 无效的状态
        /// </summary>
        public const string STATUS_INVALID = "STATUS_INVALID";
        /// <summary>
        /// 已经存在相似的日历事件了
        /// </summary>
        public static string EVENT_REPETITION = "EVENT_REPETITION";
        /// <summary>
        /// 国家不存在
        /// </summary>
        public static string COUNTRY_NOTFOUND = "COUNTRY_NOTFOUND";
        /// <summary>
        /// 省不存在
        /// </summary>
        public static string PROVINCE_NOTFOUND = "PROVINCE_NOTFOUND";
        /// <summary>
        /// 城市不存在
        /// </summary>
        public static string CITY_NOTFOUND = "CITY_NOTFOUND";
        /// <summary>
        /// 请输入有效的名称
        /// </summary>
        public static string NAME_NOTVAILD = "NAME_NOTVAILD";
        /// <summary>
        /// 请上传身份证图片
        /// </summary>
        public static string IDENTITYCARDIMG_NOTVAILD = "IDENTITYCARDIMG_NOTVAILD";
        /// <summary>
        /// 请上传营业证照
        /// </summary>
        public static string SOCIALCREDITCODEIMG_NOTVAILD = "SOCIALCREDITCODEIMG_NOTVAILD";
        /// <summary>
        /// 请输入正确的社会信用代码
        /// </summary>
        public static string SOCIAL_CREDIT_CODE_NOTVAILD = "SOCIAL_CREDIT_CODE_NOTVAILD";
        /// <summary>
        /// 请输入有效的邮箱
        /// </summary>
        public static string CONTACT_EMAIL_NOTVAILD = "CONTACT_EMAIL_NOTVAILD";
        /// <summary>
        /// 请输入有效的手机号
        /// </summary>
        public static string CONTACT_PHONE_NOTVAILD = "CONTACT_PHONE_NOTVAILD";
        /// <summary>
        /// 请输入有效的微信号
        /// </summary>
        public static string CONTACT_WECHAT_NOTVAILD = "CONTACT_WECHAT_NOTVAILD";
        /// <summary>
        /// 请输入有效的地址
        /// </summary>
        public static string ADDRESS_NOTVAILD = "ADDRESS_NOTVAILD";
        /// <summary>
        /// 请选择入驻类型
        /// </summary>
        public static string SNSTYPE_NOTVAILD = "SNSTYPE_NOTVAILD";
        /// <summary>
        /// 请输入有效的省
        /// </summary>
        public static string PROVINCE_NOTVAILD = "PROVINCE_NOTVAILD";
        /// <summary>
        /// 请输入有效的公众号名称
        /// </summary>
        public static string DISPLAYNAME_NOTVAILD = "DISPLAYNAME_NOTVAILD";
        /// <summary>
        /// 请上传一个logo
        /// </summary>
        public static string LOGO_NOTVAILD = "LOGO_NOTVAILD";
        /// <summary>
        /// 请输入有效的国家
        /// </summary>
        public static string COUNTRY_NOTVAILD = "COUNTRY_NOTVAILD";

        /// <summary>
        /// 不能超过31个字符
        /// </summary>
        public static string TITLE_NOTVAILD = "TITLE_NOTVAILD";
        /// <summary>
        /// 标题不能少于10个汉字
        /// </summary>
        public static string TITLE_TOOSHORT = "TITLE_TOOSHORT";

        /// <summary>
        /// 请输入正文
        /// </summary>
        public static string CONTENT_NOTVAILD = "CONTENT_NOTVAILD";

        /// <summary>
        /// 请选择一个分类
        /// </summary>
        public static string CHANNEL_NOTVAILD = "CHANNEL_NOTVAILD";

        /// <summary>
        /// 请上传1张封面图片
        /// </summary>
        public static string ISAUTOCOVERIMGURL_NOTVAILD = "ISAUTOCOVERIMGURL_NOTVAILD";

        /// <summary>
        /// 请上传封面图片
        /// </summary>
        public static string COVERIMGURL_NOTVAILD = "COVERIMGURL_NOTVAILD";

        /// <summary>
        /// 币种参数不正确
        /// </summary>
        public static string COIN_NOTVAILD = "COIN_NOTVAILD";
        /// <summary>
        /// 交易所参数不正确
        /// </summary>
        public static string EXCHANGE_NOTVAILD = "EXCHANGE_NOTVAILD";

        /// <summary>
        /// 入驻信息已经填写过
        /// </summary>
        public static string SETTLED_EXISTS = "SETTLED_EXISTS";
        /// <summary>
        /// 请选择一个虚拟币
        /// </summary>
        public static string COINNAME_NOTSELECT = "COINNAME_NOTSELECT";
        /// <summary>
        /// 请选择一个交易所
        /// </summary>
        public static string EXCHANGE_NOTSELECT = "EXCHANGE_NOTSELECT";
        /// <summary>
        /// 交易所名称不能超过20个字符
        /// </summary>
        public static string EXCHANGENAME_TOO_LONG = "EXCHANGENAME_TOO_LONG";
        /// <summary>
        /// 虚拟币名称不能超过20个字符
        /// </summary>
        public static string COINNAME_TOO_LONG = "COINNAME_TOO_LONG";
        /// <summary>
        /// 请输入企业名称
        /// </summary>
        public static string ENTERPRISENAME_NOTNULL = "ENTERPRISENAME_NOTNULL";
        /// <summary>
        /// 请输入媒体公司名称
        /// </summary>
        public static string MEDIANAME_NOTNULL = "MEDIANAME_NOTNULL";
        /// <summary>
        /// 请输入自媒体名称
        /// </summary>
        public static string SELFMEDIANAME_NOTNULL = "SELFMEDIANAME_NOTNULL";
        /// <summary>
        /// 企业名称不能超过50个字符
        /// </summary>
        public static string ENTERPRISENAME_TOO_LONG = "ENTERPRISENAME_TOO_LONG";
        /// <summary>
        /// 媒体公司名称不能超过50个字符
        /// </summary>
        public static string MEDIANAME_TOO_LONG = "MEDIANAME_TOO_LONG";
        /// <summary>
        /// 自媒体名称不能超过30个字符
        /// </summary>
        public static string SELFMEDIANAME_TOO_LONG = "SELFMEDIANAME_TOO_LONG";
        /// <summary>
        /// 请输入有效的城市名
        /// </summary>
        public static string CITY_NOTVAILD = "CITY_NOTVAILD";
        /// <summary>
        /// 地址不能超过100个字符
        /// </summary>
        public static string ADDRESS_TOO_LONG = "ADDRESS_TOO_LONG";
        /// <summary>
        /// 微信号不能超过20个字符
        /// </summary>
        public static string CONTACT_WECHAT_TOO_LONG = "CONTACT_WECHAT_TOO_LONG";
        /// <summary>
        /// 邮箱不能超过100个字符
        /// </summary>
        public static string CONTACT_EMAIL_TOO_LONG = "CONTACT_EMAIL_TOO_LONG";
        /// <summary>
        /// 补充说明不能超过500个字符
        /// </summary>
        public static string REMARK_TOO_LONG = "REMARK_TOO_LONG";
        /// <summary>
        /// 正文包含敏感信息
        /// </summary>
        public const string CONTENT_CONTAIN_SENSITIVE_INFORMATION = "CONTENT_CONTAIN_SENSITIVE_INFORMATION";

        /// <summary>
        /// 资讯上传封面图片图片在50KB 到5M
        /// </summary>
        public static string COVERIMGURL_NOTVAILDSIZE = "COVERIMGURL_NOTVAILDSIZE";

        /// <summary>
        /// 只能选择未来2~24小时的时间范围
        /// </summary>
        public static string PUBLICTIME_NOTVAILD = "PUBLICTIME_NOTVAILD";

        /// <summary>
        /// 发布时间必须比当前时间晚
        /// </summary>
        public static string PUBLICTIME_NOTVAILD_LESS = "PUBLICTIME_NOTVAILD_LESS";

        /// <summary>
        ///不能超过140个字符
        /// </summary>
        public static string EVENTNAME_NOTVAILD = "EVENTNAME_NOTVAILD";
        /// <summary>
        /// 该帐号已有一个入驻信息
        /// </summary>
        public static string EXISTS_SNSUSERINFO = "EXISTS_SNSUSERINFO";
        /// <summary>
        /// 请输入标题，为了更好的展示，建议输入标题在30个汉字以内
        /// </summary>
        public static string TITLE_ISNULL = "TITLE_ISNULL";

        /// <summary>
        /// 你选择的文件太小了，请编辑后重新上传
        /// </summary>
        public static string IMG_FILEMINSIZE = "IMG_FILEMINSIZE";

        /// <summary>
        /// 上传文件不能超过5MB
        /// </summary>
        public static string IMG_FILEMAXSIZE = "IMG_FILEMAXSIZE";

        /// <summary>
        /// 你选择的文件太大了，请编辑后重新上传
        /// </summary>
        public static string IMG_MAXSIZE = "IMG_MAXSIZE";

        /// <summary>
        /// 你选择的文件太小了，请编辑后重新上传
        /// </summary>
        public static string IMG_MINSIZE = "IMG_MINSIZE";
        /// <summary>
        /// 24小时内的发布次数超限
        /// </summary>
        public static string PUBLIC_24H_COUNT_LIMIT = "PUBLIC_24H_COUNT_LIMIT";
        /// <summary>
        /// 项目方全称/交易所名称/媒体名称/自媒体名称> 入驻已经存在，如果您是运营方，请联系客服微信:feixiaohao16
        /// </summary>
        public static string RepeatProject = "入驻已经存在，如果您是运营方，请联系客服微信:feixiaohao16";
        /// <summary>
        /// 请输入有效的企业名称
        /// </summary>
        public static string BUSINESSNAME_NOTVAILD = "请输入有效的企业名称";//todo: 请输入有效的企业名称
        /// <summary>
        /// 请选择一张币种logo图片
        /// </summary>
        public static string INVALI_SYMBOL_ICON = "请选择一张有效的币种logo图片";
        /// <summary>
        /// 请输入有效的虚拟币简称
        /// </summary>
        public static string INVALI_SYMBOL = "请输入有效的虚拟币简称";
        /// <summary>
        /// 请输入有效的虚拟币全称
        /// </summary>
        public static string INVALI_SYMBOL_NAME = "请输入有效的虚拟币全称";
        /// <summary>
        /// 请输入有效的虚拟币中文名
        /// </summary>
        public static string INVALI_SYMBOL_CNNAME = "请输入有效的虚拟币中文名";
        /// <summary>
        /// 请输入有效的白皮书链接
        /// </summary>
        public static string INVALI_SYMBOL_WHITEPAPERLINK = "请输入有效的白皮书链接";
        /// <summary>
        /// 请输入有效的上架平台
        /// </summary>
        public static string INVALI_SYMBOL_EXCHANGECODES = "请输入有效的上架平台";
        /// <summary>
        /// 请输入有效的智能合约地址
        /// </summary>
        public static string INVALI_SYMBOL_CONTRACTADDRESS = "请输入有效的智能合约地址";
        /// <summary>
        ///  请输入有效的币种简介
        /// </summary>
        public static string INVALI_SYMBOL_DESCRIPTION = "请输入有效的币种简介";
        /// <summary>
        /// 请输入有效的官方网站链接
        /// </summary>
        public static string INVALI_SYMBOL_SITELINK= "请输入有效的官方网站链接";
        /// <summary>
        /// 请输入有效的官方邮箱
        /// </summary>
        public static string INVALI_SYMBOL_CONTACTMAIL= "请输入有效的官方邮箱";
        /// <summary>
        /// 请输入有效的流通量
        /// </summary>
        internal static string INVALI_SYMBOL_CIRCULATINGSUPPLY= "请输入有效的流通量";
        /// <summary>
        /// 该币种已存在首发，不能重复发布
        /// </summary>
        public static string EXITS_SYMBOL= "该币种已存在首发，不能重复发布";
        /// <summary>
        /// 请输入有效的发行总量
        /// </summary>
        public static string INVALI_SYMBOL_TOTALSUPPLY= "请输入有效的发行总量";
        /// <summary>
        /// 币种审核id必须传
        /// </summary>
        public static string INVALI_SYMBOL_ID="币种审核id必须传";
        /// <summary>
        /// 已存在该媒体名称
        /// </summary>
        public static string THE_MEDIA_NAME_ALREADY_EXISTS= "已存在该媒体名称";
        /// <summary>
        /// 系统保留，限制使用
        /// </summary>
        public static string SYSTEM_RESERVATION_RESTRICTED_USE= "系统保留，限制使用";

        /// <summary>
        /// 草稿箱已满
        /// </summary>
        public static string DRAFT_BOX_FULL = "草稿箱已满";
        /// <summary>
        /// T-T 文章中第{0}张图片上传失败，请重新手工上传
        /// </summary>
        public static string FAILED_UPLOAD_NTH_PICTURE= "T-T 文章中第{0}张图片上传失败，请重新手工上传";
    }

}
