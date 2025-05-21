using System;
using System.Collections.Generic;
using System.Text;

namespace Pandora.Cigfi.Models.Consts
{
    /// <summary>
    /// 菜单常量
    /// </summary>
    public class MenuKeyConsts
    {

        /// <summary>
        /// demo 
        /// </summary>
        public const string DEMO = "demo";

        public const string REVIEWCOUNT = "ReviewCount";
        #region 商城
        /// <summary>
        ///会员邀请管理
        /// </summary>
        public const string INVITATION = "Invitation";
        /// <summary>
        ///会员邀请管理
        /// </summary>
        public const string REBATE = "rebate";
        #endregion

        #region 配置


        /// <summary>
        /// 配置APP菜单
        /// </summary>
        public const string CONFIG_APP_MENU = "config_app_menu";

        /// <summary>
        /// 关于我们
        /// </summary>
        public const string CONFIGABOUTUS = "config_aboutus";
        /// <summary>
        /// 广告频道
        /// </summary>
        public const string CONFIGAD = "config_ad";

        /// <summary>
        /// 广告文字管理
        /// </summary>
        public const string CONFIG_AD_TEXT = "config_ad_text";
        /// <summary>
        /// 应用
        /// </summary>
        public const string CONFIGAPPLIST = "config_applist";
        /// <summary>
        /// 广告
        /// </summary>
        public const string CONFIGADDETAIL = "config_ad_detail";
        /// <summary>
        /// 语言列表
        /// </summary>
        public const string LANGUAGE = "configlanguage";

        /// <summary>
        /// 国家列表
        /// </summary>
        public const string COUNTRY = "configcountry";

        /// <summary>
        /// 国家多语言管理
        /// </summary>
        public const string COUNTRY_LANG = "configcountrylang";

        /// <summary>
        /// 交易所类法币的代币
        /// </summary>
        public const string EXCHANGESPECIALCOIN = "exchangecoin";

        /// <summary>
        /// 字典配置
        /// </summary>
        public const string CONFIGDICT = "config_dict";

        /// <summary>
        /// 汇率配置
        /// </summary>
        public const string CURRENCYRATE = "currencyrate";
        /// <summary>
        /// 行情白名单
        /// </summary>
        public const string TICKER_WHITELIST = "config_cointicker_whitelist";
        /// <summary>
        /// 内页公告
        /// </summary>
        public const string INSIDE_ANNOUNCE = "cms_announce";

        /// <summary>
        /// 应用信息
        /// </summary>
        public const string CONFIG_APPINFO = "config_appinfo";

        /// <summary>
        /// 友链（网站）导航分类
        /// </summary>
        public const string FRIENDLINK_GROUP = "friendlink_group";

        /// <summary>
        /// 网站导航（友链）
        /// </summary>
        public const string FRIENDLINK = "friendlink";

        #endregion
        
        #region 指数
        /// <summary>
        /// 指数_成分币表
        /// </summary>
        public const string INDEX_COIN = "index_coin";
        /// <summary>
        /// 指数_基础信息多语言表
        /// </summary>
        public const string INDEX_DETAIL_LANG = "index_detail_lang";
        /// <summary>
        /// 指数_基础信息表
        /// </summary>
        public const string INDEX_DETAIL = "index_detail";
        /// <summary>
        /// 指数_行情表
        /// </summary>
        public const string INDEX_TICKER = "index_ticker";
        
        #endregion
        
        #region 虚拟币
        /// <summary>
        /// 虚拟币未审核列表
        /// </summary>
        public const string BASE_COIN_NOPASSED = "base_coin_nopassed";

        /// <summary>
        /// 币理财列表
        /// </summary>
        public const string FUND_INDEX = "fund_index";

        /// <summary>
        /// 虚拟币审核通过列表
        /// </summary>
        public const string BASE_COIN_YESPASSED = "base_coin_yespassed";
        /// <summary>
        /// 新币上市审核
        /// </summary>
        public const string NEWCOIN_AUDIT = "newcoin_audit";

        /// <summary>
        /// 虚拟币_新币
        /// </summary>
        public const string NEW_COIN = "new_coin";


        /// <summary>
        /// 入驻信息审核
        /// </summary>
        public const string USER_INFO = "user_info";

        /// <summary>
        /// 交易所未审核列表
        /// </summary>
        public const string BASE_EXCHANGE_NOPASSED = "base_exchange_nopassed";

        /// <summary>
        /// 虚拟币标签列表
        /// </summary>
        public const string BASE_COINTAG = "base_cointag";
        /// <summary>
        /// 交易所标签列表
        /// </summary>
        public const string BASE_EXCHANGETAG = "base_exchangetag";

        /// <summary>
        /// 虚拟币列表
        /// </summary>
        public const string BASE_COIN = "base_coin";

        /// <summary>
        /// 交易所列表
        /// </summary>
        public const string BASE_EXCHANGE = "base_exchange";
        /// <summary>
        /// 交易所挖矿
        /// </summary>
        public const string BASE_EXCHANGE_MINING = "base_exchange_mining";
        /// <summary>
        /// 虚拟币的指标信息表
        /// </summary>
        public const string BASE_COIN_INDEX = "base_coin_index";

        /// <summary>
        /// 虚拟币_区块信息表
        /// </summary>
        public const string BASE_COIN_ADDR = "base_coin_addr";
        /// <summary>
        /// 币种热度配置管理
        /// </summary>
        public const string BASE_COIN_ATTENTION_RULE = "base_coin_attention_rule";
        /// <summary>
        /// 代币流通
        /// </summary>
        public const string BASE_COIN_TOKENCIRCULATION = "base_coin_tokencirculation";

        /// <summary>
        /// 交易所_热度规则表
        /// </summary>
        public const string BASE_EXCHANGE_ATTENTION_RULE = "base_exchange_attention_rule";
        /// <summary>
        /// 交易所审核未通过列表
        /// </summary>
        public const string BASE_EXCHANGE_UNNOPASSED = "base_exchange_unnopassed";
        /// <summary>
        /// 交易所钱包地址表
        /// </summary>
        public const string BASE_EXCHANGE_WALLET = "base_exchange_wallet";
        /// <summary>
        /// 交易所资产披露
        /// </summary>
        public const string BASE_EXCHANGE_ASSET = "base_exchange_asset";
        /// <summary>
        /// 交易所推广地址配置
        /// </summary>
        public const string CONFIG_EXCHANGE_TRADEURL = "config_exchange_tradeurl";

        /// <summary>
        /// cmc币种代码与非小号币种代码关联表
        /// </summary>
        public const string CMC_FXH = "cmc_fxh";
        /// <summary>
        /// 币种采集信息待审核表
        /// </summary>
        public const string COIN_INFO_NOPASSED = "coin_info_nopassed";
        /// <summary>
        /// defi 币种资产分类
        /// </summary>
        public const string DEFI_COIN_ASSET_CATEGORY = "defi_coin_asset_category";
        #endregion

        #region 行情
        /// <summary>
        /// 禁止自动匹配交易对行情表的交易对1及交易对2的CoinCode
        /// </summary>
        public const string CONFIG_DISABLE_UPDATECOINCODE = "config_disable_updatecoincode";

        /// <summary>
        /// 行情告警检查结果
        /// </summary>
        public const string MARKETALARMCHECK = "market_alarm_check";
        /// <summary>
        /// 行情告警发送
        /// </summary>
        public const string MARKETALARMSEND = "market_alarm_send";
        /// <summary>
        /// 行情告警参数
        /// </summary>
        public const string MARKETALARMPARAMETER = "market_alarm_parameter";
        /// <summary>
        /// 虚拟币行情列表
        /// </summary>
        public const string MARKET_COIN_TICKER = "market_coin_ticker";
        /// <summary>
        /// 交易对(指数)实时行情
        /// </summary>
        public const string MARKET_TICKER = "market_ticker";


        /// <summary>
        /// 交易对(合约)参数
        /// </summary>
        public const string MARKET_TICKER_EXTENSION = "market_ticker_extension";

        /// <summary>
        /// 交易对上下架
        /// </summary>
        public const string MARKET_TICKER_LIST = "market_ticker2";

        /// <summary>
        /// 交易所_行情数据统计表
        /// </summary>
        public const string MARKET_EXCHANGE = "market_exchange";
        #endregion

        #region 采集配置
        /// <summary>
        /// 作业节点表
        /// </summary>
        public const string CRAWL_JOB_NODE = "crawl_job_node";
        /// <summary>
        /// 作业类型表
        /// </summary>
        public const string CRAWL_JOB_TYPE = "crawl_job_type";
        /// <summary>
        /// 爬虫节点表
        /// </summary>
        public const string CRAWL_SPIDERNODE = "crawl_spidernode";
        /// <summary>
        /// 作业安排表
        /// </summary>
        public const string CRAWL_JOB_CRONTAB = "crawl_job_crontab";

        /// <summary>
        /// 交易对配置表
        /// </summary>
        public const string CRAWL_CONFIG_EXCHANGETICKER = "crawl_config_exchangeticker";

        /// <summary>
        /// 新闻列表配置表
        /// </summary>
        public const string CRAWL_CONFIG_NEWS = "crawl_config_news";
        /// <summary>
        /// 通用配置表
        /// </summary>
        public const string CRAWL_CONFIG_COMMON = "crawl_config_common";

        /// <summary>
        /// 配置用户
        /// </summary>
        public const string CRAWL_CONFIG_USER = "crawl_config_user";

        /// <summary>
        /// 新闻内容配置表
        /// </summary>
        public const string CRAWL_CONFIG_NEWSCONENT = "crawl_config_newsconent";
        /// <summary>
        /// 未配置交易所
        /// </summary>
        public const string UNMATCHED_EXCHANGES = "unmatched_exchanges";

        /// <summary>
        /// 异常配置交易所
        /// </summary>
        public const string CRAWL_CONFIG_EXCHANGETICKER_ERROR = "crawl_config_exchangeticker_error";
        #endregion

        #region 资讯
        /// <summary>
        /// 资讯与虚拟币关联表
        /// </summary>
        public const string MEDIA_NEWS_COIN = "media_news_coin";
        /// <summary>
        /// 资讯频道表
        /// </summary>
        public const string MEDIA_NEWSCHANNEL = "media_newschannel";
        /// <summary>
        /// 资讯表
        /// </summary>
        public const string MEDIA_NEWS = "media_news";
        /// <summary>
        /// 资讯专题表
        /// </summary>
        public const string MEDIA_SUBJECT_NEWS = "media_subject_news";
        /// <summary>
        /// 资讯表_TWITTER
        /// </summary>
        public const string MEDIA_NEWS_TWITTER = "MEDIA_NEWS_TWITTER"; 
        /// <summary>
        /// 资讯标签
        /// </summary>
        public const string MEDIA_TAGS = "media_tags";

        /// <summary>
        /// 官方验证码
        /// </summary>
        public const string  SERVICE_CONTACT = "service_contact";

        /// <summary>
        /// 入驻方资讯表
        /// </summary>
        public const string INPART_MEDIA_NEWS = "inparty_m_news";

        /// <summary>
        /// 资讯表
        /// </summary>
        public const string CONFIG_DISCOVER = "config_discover";
        /// <summary>
        /// 资讯点击记录
        /// </summary>
        public const string MEDIA_NEWS_HITS = "media_news_hits";
        /// <summary>
        /// 资讯推送记录
        /// </summary>
        public const string MEDIA_NEWS_PUSH = "media_news_push";

        /// <summary>
        /// 图片块(图片分类管理)
        /// </summary>
        public const string MEDIA_COVER_GROUP = "media_cover_group";

        /// <summary>
        ///图片块的图片（资讯封面图片）
        /// </summary>
        public const string MEDIA_COVER = "media_cover";
        #endregion


        #region 用户及权限管理
        /// <summary>
        /// 管理角色
        /// </summary>
        public const string ADMINROLE = "adminrole";
        /// <summary>
        /// 修改密码
        /// </summary>
        public const string EDITME = "editme";
        /// <summary>
        /// 管理员
        /// </summary>
        public const string ADMIN = "admins";
        /// <summary>
        /// 后台操作日志
        /// </summary>
        public const string ADMINLOG = "admincplog";

        /// <summary>
        /// 事件管理
        /// </summary>
        public const string EVENTKEY = "eventkey";
        /// <summary>
        /// 管理栏目
        /// </summary>
        public const string ADMINMENU = "adminmenu";

        /// <summary>
        /// 操作日志
        /// </summary>
        public const string OPERATIONLOG = "operationlog";
        /// <summary>
        /// 用户搜索
        /// </summary>
        public const string LOG_USER_SEARCH = "log_user_search";
        /// <summary>
        /// 机器人报价
        /// </summary>
        public const string LOG_ROBOT = "log_robot";
        /// <summary>
        /// 用户点击
        /// </summary>
        public const string LOG_USER_CLICK = "log_user_click";
        /// <summary>
        /// 用户意见
        /// </summary>
        public const string MEMBER_FEEDBACK = "member_feedback";
        /// <summary>
        /// 用户列表
        /// </summary>
        public const string SITE_USER = "site_user";

        /// <summary>
        /// 前台用户列表
        /// </summary>
        public const string T_USER = "t_user";

        /// <summary>
        /// 用户搜索统计 
        /// </summary>
        public const string USERSEARCHCOUNT = "usersearchcount";

        /// <summary>
        /// 用户点击统计 
        /// </summary>
        public const string USERCLICKCOUNT = "userclickcount";

        #endregion

        #region 其他
        /// <summary>
        /// 钱包管理
        /// </summary>
        public const string WALLET = "wallet";

        /// <summary>
        /// 广告点击图表统计 
        /// </summary>
        public const string CLICKCOUNTVIEW = "clickcountview";

        /// <summary>
        /// 24小时币种涨跌幅数据统计 
        /// </summary>
        public const string COINCOUNTVIEW = "coincountview";

        /// <summary>
        /// 币圈百科
        /// </summary>
        public const string BAIKE = "baike";

        /// <summary>
        /// 名词百科
        /// </summary>
        public const string UNON = "unon";

        /// <summary>
        /// 名词标签
        /// </summary>
        public const string UNONTAG = "unontag";
        /// <summary>
        /// 非小号活动
        /// </summary>
        public const string PARTRY = "partry";

        /// <summary>
        /// 关于我们
        /// </summary>
        public const string FAQ = "faq";

        /// <summary>
        /// 币圈事件/日历/活动表
        /// </summary>
        public const string CMSEVENT = "cms_event";

        /// <summary>
        /// 币种相关事件
        /// </summary>
        public const string CMSBIGEVENT = "cms_bigevent";

        /// <summary>
        /// 推广管理
        /// </summary>
        public const string PROMOTION_MANAGEMENT = "promotion_management";

        /// <summary>
        /// 热搜币种列表
        /// </summary>
        public const string HOTCOINLIST = "hotcoinlist";

        /// <summary>
        /// 热搜币种或交易所推广设置 （app搜索下拉联想）列表
        /// </summary>
        public const string HOTCOIN_DROPDOWN_LIST = "hotcoin_dropdown_list";

        /// <summary>
        /// /
        /// </summary>
        public const string d = "";
        /// <summary>
        /// 观点
        /// </summary>
        public const string SNS_ARTICLES = "sns_articles";
        /// <summary>
        /// 用户观点
        /// </summary>
        public const string USER_SNS_ARTICLES = "user_sns_articles";
        /// <summary>
        /// 用户举报
        /// </summary>
        public const string SNS_REPORTDETAILS = "sns_reportdetails";
        /// <summary>
        /// 用户信息
        /// </summary>
        public const string SNS_USERINFO = "community_sns_userinfo";
        /// <summary>
        /// 观点评论
        /// </summary>
        public const string SNS_COMMENTDETAILS = "sns_commentdetails";

        /// <summary>
        /// 用户评论
        /// </summary>
        public const string SNS_COMMENTS = "sns_comments";
        /// <summary>
        /// 观点评论举报
        /// </summary>
        public const string SNS_COMMENT_REPORTDETAILS = "sns_comment_reportdetails";
        #endregion

        /// <summary>
        /// 功能反馈跟踪
        /// </summary>
        public const string MODULETRACE= "Moduletrace";
        /// <summary>
        /// 功能反馈建议记录
        /// </summary>
        public const string MODULE_TRACE_ISSUE_OTHER = "MODULE_TRACE_ISSUE_OTHER";
        /// <summary>
        /// 交易所置顶配置
        /// </summary>
        public const string BASE_EXCHANGE_TOP_CONFIG =  "base_exchange_top_config";
        /// <summary>
        /// 用户申诉
        /// </summary>
        public const string SNS_USERINFO_APPEAL = "SNS_USERINFO_APPEAL";
        /// <summary>
        /// 币种分区
        /// </summary>
        public const string BASE_COIN_ZONE = "BASE_COIN_ZONE";

        /// <summary>
        /// DEFI流动性挖矿
        /// </summary>
        public const string DEFI_MINING = "defi_mining";
        /// <summary>
        /// DEFI锚定资产
        /// </summary>
        public const string DEFI_ANCHOR_ASSETS = "defi_anchor_assets";
        /// <summary>
        /// defi 矿池
        /// </summary>
        public const string DEFI_POOL = "DEFI_POOL";
        /// <summary>
        /// 热搜
        /// </summary>
        public const string HOT_SEARCH = "hot_search";
        /// <summary>
        /// 推送热币配置
        /// </summary>
        public const string PUSH_CONFIG_HOTTICKER = "PUSH_CONFIG_HOTTICKER";
        /// <summary>
        /// 上周明星榜
        /// </summary>
        public const string SNS_RAKING_USERINFO = "SNS_RAKING_USERINFO";
        /// <summary>
        /// 推荐用户
        /// </summary>
        public const string BBS_USERINFO_RECOMMEND = "BBS_USERINFO_RECOMMEND";
        /// <summary>
        /// 币种异动配置
        /// </summary>
        public const string MARKET_COIN_UNUSUALACTION_CONFIG = "MARKET_COIN_UNUSUALACTION_CONFIG";
        /// <summary>
        /// 新币动态
        /// </summary>
        public const string NEWCOIN_MINING = "NEWCOIN_MINING";
        /// <summary>
        /// 推送订阅标签
        /// </summary>
        public const string PUSH_TAG_DIC_INDEX = "pushtagdic_index";
        /// <summary>
        /// 推送订阅标签
        /// </summary>
        public const string PUSH_TAG_SUBSCIRBE_INDEX = "pushtag_subscirbe_index";
        /// <summary>
        /// 币种评论
        /// </summary>
        public const string BASE_COIN_COMMENT = "BASE_COIN_COMMENT"; 
        /// <summary>
        /// NFT平台
        /// </summary>
        public const string NFT_PLATFORM = "NFT_PLATFORM";
        /// <summary>
        /// NFT分类
        /// </summary>
        public const string NFT_CATEGORY = "NFT_CATEGORY"; 
        /// <summary>
        /// NFT项目
        /// </summary>
        public const string NFT_PROJECT = "NFT_PROJECT";
        /// <summary>
        /// NFT创作者
        /// </summary>
        public const string NFT_CREATOR = "NFT_CREATOR";
        /// <summary>
        /// k线补采
        /// </summary>
        public const string CRAWKLINEFIX = "CRAWKLINEFIX";
        /// <summary>
        /// 采集错误日志
        /// </summary>
        public const string CRAWLERRORLOG = "crawl_error_log";

        public const string CRAWLERRORLOGTYPE= "crawl_error_log_type";
    }
}
