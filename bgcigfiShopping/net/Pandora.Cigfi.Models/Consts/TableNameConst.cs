using System;
using System.Collections.Generic;
using System.Text;

namespace Pandora.Cigfi.Models.Consts
{
    /// <summary>
    /// 数据库表名
    /// </summary>
    public class TableNameConst
    {
        #region 虚拟币相关表
        /// <summary>
        /// demo表
        /// </summary>
        public const string DEMO = "demo";
        /// <summary>
        /// 虚拟币基础表
        /// </summary>
        public const string BASE_COIN = "base_coin";


        public const string BASE_COIN_CRAW = "base_coin_socialmedia";

        public const string BASE_COIN_LOCKUP = "base_coin_locklift";


        public const string BASE_COIN_FOUNDERQA = "base_coin_founderqa";

        /// <summary>
        /// 疑似重复表
        /// </summary>
        public const string BASE_COINSYMBOL_REPEATED = "base_coinsymbol_repeated";

        /// <summary>
        /// 虚拟币_ico基础信息
        /// </summary>
        public const string BASE_ICO = "base_ico";

        /// <summary>
        /// 虚拟币_新币
        /// </summary>
        public const string NEW_COIN = "base_exchange_newlisting_coins";
        /// <summary>
        /// 虚拟币_未通过审核基础信息表
        /// </summary>
        public const string BASE_COIN_NOPASSED = "base_coin_nopassed";

        /// <summary>
        /// 新币上市审核表
        /// </summary>
        public const string NEW_COIN_AUDIT = "base_exchange_newlisting_coins_notpassed";

        /// <summary>
        /// 新币上市审核多语言表
        /// </summary>
        public const string NEW_COIN_AUDIT_LANG = "base_exchange_newlisting_coins_notpassed_lang";

        /// <summary>
        /// 虚拟币多语言表
        /// </summary>
        public const string BASE_COIN_LANG = "base_coin_lang";

        /// <summary>
        /// 核心内容要点一级标题主表
        /// </summary>
        public const string BASE_COIN_MAIN_POINT = "base_coin_main_point";

        /// <summary>
        /// 特殊说明
        /// </summary>
        public const string BASE_COIN_EXPLAIN = "base_coin_explain";
        /// <summary>
        /// 虚拟币众筹信息多语言表
        /// </summary>
        public const string BASE_COIN_CROWDFUNDING = "base_coin_crowdfunding";
        /// <summary>
        /// 虚拟币多语言未审核表
        /// </summary>
        public const string BASE_COIN_NOPASSED_LANG = "base_coin_nopassed_lang";
        /// <summary>
        /// 虚拟币标签字典
        /// </summary>
        public const string BASE_COINTAG = "base_cointag";

        /// <summary>
        /// 虚拟币标签字典子分类
        /// </summary>
        public const string BASE_COINTAG_SUBTYPE = "base_cointag_subtype";
        /// <summary>
        /// 虚拟币标签关联表
        /// </summary>
        public const string BASE_COIN_TAG = "base_coin_tag";
        /// <summary>
        /// 虚拟币代币分配关联表
        /// </summary>
        public const string BASE_COIN_RATE = "base_coin_rate";
        /// <summary>
        /// 审计表
        /// </summary>
        public const string Base_Coin_SecurityAudit = "base_coin_securityaudit";
        /// <summary>
        /// 审计详情表
        /// </summary>
        public const string Base_Coin_Audit = "base_coin_audit";
        /// <summary>
        /// 前线情报历史标签表
        /// </summary>
        public const string BASE_COIN_INTELLIGENCE_HIS  = "base_coin_intelligence_historical";
       /// <summary>
       /// 币种前线情报表
       /// </summary>
        public const string BASE_COIN_INTELLIGENCE = "base_coin_intelligence";

        /// <summary>
        /// 币种的相关币种表
        /// </summary>
        public const string BASE_COIN_RELATE = "base_coin_relate";
        /// <summary>
        /// 虚拟币标签语言表
        /// </summary>
        public const string BASE_COINTAG_LANG = "base_cointag_lang";
        /// <summary>
        /// 虚拟币的指标信息表
        /// </summary>
        public const string BASE_COIN_INDEX = "base_coin_index";

        /// <summary>
        /// 增加币板块二级分类
        /// </summary>
        public const string BASE_COIN_TAG_SUBTYPE = "base_coin_tag_subtype";
        /// <summary>
        /// fxh与cmc币种映射表
        /// </summary>
        public const string CMC_FXH = "config_cmc_fxh_coin_map ";

        /// <summary>
        /// fxh与cmc币种数据同步表
        /// </summary>
        public const string COIN_INFO_NOPASSED = "base_coininfo_nopassed ";

        /// <summary>
        /// fxh与cmc币种数据同步备份表
        /// </summary>
        public const string BASE_COIN_CCMT = "base_coin_ccmt ";

        /// <summary>
        /// 板块币种标签
        /// </summary>
        public const string BASE_TAGS = "base_tags";
        /// <summary>
        /// 虚拟币未审核ICO信息表
        /// </summary>
        public const string BASE_COIN_NOPASSED_ICO = "base_coin_nopassed_ico";
        /// <summary>
        /// 虚拟币_区块信息表
        /// </summary>
        public const string BASE_COIN_ADDR = "base_coin_addr";
        /// <summary>
        /// 币种热度配置管理
        /// </summary>
        public const string BASE_COIN_ATTENTION_RULE = "base_coin_attention_rule";
        /// <summary>
        /// 币种相关事件表
        /// </summary>
        public const string CMS_BIGEVENT = "cms_bigevent";

        /// <summary>
        /// 币圈事件/日历/活动表
        /// </summary>
        public const string CMS_EVENT = "cms_event";


        /// <summary>
        /// 币圈事件/日历/活动表(多语言)
        /// </summary>
        public const string CMS_EVENT_LANG = "cms_event_lang";
        #endregion


        #region 交易所相关表
        //交易所钱包地址表
        public const string BASE_EXCHANGE_WALLET = "base_exchange_wallet";
        /// <summary>
        /// 链上地址标记分组
        /// </summary>
        public const string BASE_EXCHANGE_WALLET_CATEGORY= "base_exchange_wallet_category";
        /// <summary>
        /// 钱包相关的币种(已弃用)
        /// </summary>
        public const string BASE_EXCHANGE_WALLET_COIN = "base_exchange_wallet_coin";

        /// <summary>
        /// 钱包相关的币种
        /// </summary>
        public const string BASE_EXCHANGE_WALLET_COIN_DETAIL = "base_exchange_wallet_coin_detail";
        /// <summary>
        /// 交易所钱包主网关联表
        /// </summary>
        public const string BASE_EXCHANGE_WALLET_MAINNET_RELATION = "base_exchange_wallet_mainnet_relation";

        /// <summary>
        /// 交易所评级得分表
        /// </summary>
        public const string BASE_EXCHANGE_RATING = "base_exchange_rating";

        /// <summary>
        /// 交易所基础表
        /// </summary>
        public const string BASE_EXCHANGE = "base_exchange";

        /// <summary>
        /// 交易所挖矿
        /// </summary>
        public const string BASE_EXCHANGE_MINING = "base_exchange_mining";

        /// <summary>
        /// 交易所ER表
        /// </summary>
        public const string HIS_INDEX_EXCHANGE = "his_index_exchange";

        /// <summary>
        /// 交易所标签字典
        /// </summary>
        public const string BASE_EXCHANGETAG = "base_exchangetag";
        public const string BASE_EXCHANGETAG_LANG = "base_exchangetag_lang";
        /// <summary>
        /// 交易所未审核表
        /// </summary>
        public const string BASE_EXCHANGE_NOPASSED = "base_exchange_nopassed";

        /// <summary>
        /// 交易所投诉表
        /// </summary>
        public const string BASE_EXCHANGE_COMPLAINTS = "base_exchange_complaints";

        /// <summary>
        /// 交易所未审核语言表
        /// </summary>
        public const string BASE_EXCHANGE_NOPASSED_LANG = "base_exchange_nopassed_lang";
        /// <summary>
        /// 交易所标签表
        /// </summary>
        public const string BASE_EXCHANGE_TAG = "base_exchange_tag";
        /// <summary>
        /// 交易所语言表
        /// </summary>
        public const string BASE_EXCHANGE_LANG = "base_exchange_lang";
        /// <summary>
        /// 交易所_热度规则表
        /// </summary>
        public const string BASE_EXCHANGE_ATTENTION_RULE = "base_exchange_attention_rule";
        
        public const string BASE_EXCHANGE_ASSET_WALLET = "base_exchange_assetwallet";

        public const string BASE_EXCHANGE_ASSET_LINK = "base_exchange_assetlink";

        #endregion

        #region 配置相关表
        /// <summary>
        /// 活动配置表（项目方或交易所的活动/会议）
        /// </summary>
        public const string CONFIG_EVENT_TYPE_LANG = "config_event_type_lang";
        /// <summary>
        /// 交易所推广地址配置
        /// </summary>
        public const string CONFIG_EXCHANGE_TRADEURL = "config_exchange_tradeurl";
        /// <summary>
        /// 交易所主网协议表
        /// </summary>
        public const string CONFIG_EXCHANGE_WALLET_COIN = "config_exchange_wallet_coin";
        /// <summary>
        /// 交易所主网协议表
        /// </summary>
        public const string Base_MainNet = "base_mainnet";
        ///// <summary>
        ///// 交易所主网协议关系表
        ///// </summary>
        //public const string CONFIG_EXCHANGE_WALLET_MainNet_Relation = "base_exchange_wallet_mainnet_relation";


        /// <summary>
        /// 配置服务，用于暴露各种配置
        /// </summary>
        public const string EXT_CONFIGS = "ext_configs";

        /// <summary>
        /// 语言表
        /// config_language
        /// </summary>
        public const string LANGUAGE = "config_language";

        /// <summary>
        /// 国家表
        /// config_country
        /// </summary>
        public const string COUNTRY = "config_country";

        /// <summary>
        /// 国家多语言表
        /// config_country_lang
        /// </summary>
        public const string COUNTRY_LANG = "config_country_lang";

        /// <summary>
        /// 广告文字表
        /// config_country_lang
        /// </summary>
        public const string CONFIG_AD_TEXT = "config_ad_text";
        /// <summary>
        /// 应用信息表
        /// config_country_lang
        /// </summary>
        public const string CONFIG_APPINFO = "config_appinfo";
        /// <summary>
        /// 汇率表
        /// </summary>
        public const string CONFIG_CURRENCY_RATE = "config_currency_rate";
        /// <summary>
        /// 白名单
        /// </summary>
        public const string CONFIG_COINTICKER_WHITELIST = "config_cointicker_whitelist";
        /// <summary>
        /// 虚拟币类型表
        /// </summary>
        public const string CONFIG_DICT = "config_dict";

        /// <summary>
        /// 禁止自动匹配交易对行情表的交易对1及交易对2的CoinCode
        /// </summary>
        public const string CONFIG_DISABLE_UPDATECOINCODE = "config_disable_updatecoincode";

        /// <summary>
        /// 交易所评级
        /// </summary>
        public const string CONFIG_RATING = "config_rating";
        /// <summary>
        /// 交易所评级指标配置表
        /// </summary>
        public const string CONFIG_RATINGINDEX = "config_ratingindex";
        /// <summary>
        /// 交易所评级指标多语言配置表
        /// </summary>
        public const string CONFIG_RATINGINDEX_LANG = "config_ratingindex_lang";

        /// <summary>
        /// 平台币于法币的对应关系
        /// </summary>
        public const string CONFIG_EXCHANGE_SPECIALCOIN = "config_exchange_specialcoin";
        /// <summary>
        /// 友链（网站）导航分类
        /// </summary>
        public const string SITE_FRIENDLINK_GROUP = "site_friendlink_group";

        /// <summary>
        /// 友链（网站）
        /// </summary>
        public const string SITE_FRIENDLINK = "site_friendlink";

        /// <summary>
        /// 非小号活动
        /// </summary>
        public const string SITE_PARTRY = "site_partry";

        /// <summary>
        /// seo自定义配置表(主要配置币种及交易所的TDK)
        /// </summary>
        public const string CONFIG_SEO_CUSTOM = "config_seo_custom";
        #endregion
        
        #region 行情管理
        /// <summary>
        /// 交易对(指数)实时行情
        /// </summary>
        public const string MARKET_TICKER = "market_ticker";
        /// <summary>
        /// 交易对(合约)参数
        /// </summary>
        public const string MARKET_TICKER_EXTENSION = "market_ticker_extension";

        /// <summary>
        /// 交易对行情，采集行情直接批量更新的表
        /// </summary>
        public const string MEMORY_MARKET_TICKER = "memory_market_ticker";
        
        /// <summary>
        /// 虚拟币行情表
        /// </summary>
        public const string MARKET_COIN_TICKER = " market_coin_ticker";

        /// <summary>
        /// 交易所_行情数据统计表
        /// </summary>
        public const string MARKET_EXCHANGE = "market_exchange";

        /// <summary>
        /// 行情告警检查结果表
        /// </summary>
        public const string MARKET_ALARM_CHECK = "market_alarm_check";
        /// <summary>
        /// 行情告警发送表
        /// </summary>
        public const string MARKET_ALARM_SEND = "market_alarm_send";
        /// <summary>
        /// 行情告警参数表
        /// </summary>
        public const string MARKET_ALARM_PARAMETER = "market_alarm_parameter";

        #endregion

        #region 指数
        /// <summary>
        /// 交易所排名指标数据
        /// </summary>
        public const string INDEX_EXCHANGE = "index_exchange";
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
        
        #region 系统管理
        /// <summary>
        /// 系统管理员表
        /// </summary>
        public const string SYS_ADMIN = "sys_admin";

        public const string SYS_REVIEW_LOG = "sys_review_log";

        /// <summary>
        /// 管理员操作日志
        /// </summary>
        public const string SYS_ADMINLOG = "sys_adminlog";
        /// <summary>
        /// 管理角色
        /// </summary>
        public const string SYS_ADMINROLES = "sys_adminroles";

        /// <summary>
        /// 系统菜单
        /// </summary>
        public const string SYS_ADMINMENU = "sys_adminmenu";

        /// <summary>
        /// 系统菜单事件
        /// </summary>
        public const string SYS_ADMINMENUEVENT = "sys_adminmenuevent";
        /// <summary>
        /// 事件权限
        /// </summary>
        public const string SYS_TARGETEVENT = "sys_targetevent";

        /// <summary>
        /// 官方验证码
        /// </summary>
        public const string SERVICE_CONTACT = "service_contact";
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
        /// 资讯频道表_多语言表
        /// </summary>
        public const string MEDIA_NEWSCHANNEL_LANG = "media_newschannel_lang";

        /// <summary>
        /// 资讯表
        /// </summary>
        public const string MEDIA_NEWS = "media_news";
        /// <summary>
        /// 资讯专题表
        /// </summary>
        public const string MEDIA_SUBJECT_NEWS = "media_subject_news";
        /// <summary>
        /// 资讯专题关联表
        /// </summary>
        public const string MEDIA_SUBJECT_RELATED = "media_subject_related";

        /// <summary>
        /// 资讯标签冗余表
        /// </summary>
        public const string MEDIA_TAGS_CODE_RALATED = "media_tags_code_related";

        /// <summary>
        /// 资讯标签表
        /// </summary>
        public const string MEDIA_TAGS = "media_tags";

        /// <summary>
        /// 资讯标签关联表
        /// </summary>
        public const string MEDIA_TAGS_RELATED = "media_tags_related";

        /// <summary>
        /// 名词标签关联表
        /// </summary>
        public const string BAIKE_UNON_TAGS_RELATED = "baike_unon_tags_related";
        /// <summary>
        /// 资讯内容配置表
        /// </summary>
        public const string CRAWL_CONFIG_NEWSCONTENT = "crawl_config_newsconent";

        /// <summary>
        /// 发现表
        /// </summary>
        public const string CONFIG_DISCOVER = "config_discover";
        /// <summary>
        /// 发现表语言
        /// </summary>
        public const string CONFIG_DISCOVER_LANG = "config_discover_lang"; 
        /// <summary>
        /// 发现类型地图
        /// </summary>
        public const string CONFIG_DISCOVER_TYPE_MAP = "config_discover_type_map";
        // <summary>
        /// 资讯推送表
        /// </summary>
        public const string MEDIA_NEWS_PUSH = "msg_news_push";
        /// <summary>
        /// 资讯点击记录
        /// </summary>
        public const string MEDIA_NEWS_HITS = "media_news_hits";
        /// <summary>
        /// 资讯用户
        /// </summary>
        public const string MEDIA_USER = "media_user";
        /// <summary>
        /// 百科_币圈人物_投资机构
        /// </summary>
        public const string BAIKE_ITEM = "baike_item";
        /// <summary>
        /// 百科_币圈人物_投资机构_多语言表
        /// </summary>
        public const string BAIKE_ITEM_LANG = "baike_item_lang";
        /// <summary>
        /// 百科名词表
        /// </summary>
        public const string BAIKE_UNON = "baike_unon";

        /// <summary>
        /// 百科币种表
        /// </summary>
        public const string BAIKE_UNON_COIN = "baike_unon_coin";

        /// <summary>
        /// 名词分类标签表
        /// </summary>
        public const string BAIKE_UNON_TAGS_Related = "baike_unon_tags_related";

        /// <summary>
        /// 名词钱包表
        /// </summary>
        public const string BAIKE_UNON_WALLET = "baike_unon_wallet";

        /// <summary>
        /// 名词交易所表
        /// </summary>
        public const string BAIKE_UNON_EXCHANGECODE = "baike_unon_exchangecode";
        /// <summary>
        /// 币圈人物_投资机构相关的人物_投资机构或是币种，交易所
        /// </summary>
        public const string BAIKE_ITEM_RELATED = "baike_item_related";

        /// <summary>
        /// 图片块管理
        /// </summary>
        public const string MEDIA_COVER_GROUP = "media_cover_group";

        /// <summary>
        /// 图片块下的图片
        /// </summary>
        public const string MEDIA_COVER = "media_cover";
        #endregion

        #region 采集配置
        /// <summary>
        /// 关于我们
        /// </summary>
        public const string CONFIG_ABOUTUS = "config_aboutus";

        /// <summary>
        /// 广告频道
        /// </summary>
        public const string CONFIG_AD = "config_ad";
        /// <summary>
        /// 广告
        /// </summary>
        public const string CONFIG_AD_DETAIL = "config_ad_detail";
        /// <summary>
        /// 广告广告频道关联表
        /// </summary>
        public const string CONFIG_AD_DETAIL_REL = "config_ad_detail_rel";
        /// <summary>
        /// 广告广告频道关联表
        /// </summary>
        public const string CONFIG_AD_DETAIL_LANGUAGE = "config_ad_detail_language";
        /// <summary>
        /// 应用
        /// </summary>
        public const string CONFIG_APPLIST = "config_applist";
        /// <summary>
        /// 广告资讯关联
        /// </summary>
        public const string CONFIG_AD_NEWS = "config_ad_news";
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
        /// 新闻内容配置表
        /// </summary>
        public const string CRAWL_CONFIG_NEWSCONENT = "crawl_config_newsconent";

        /// <summary>
        /// 异常配置交易所
        /// </summary>
        public const string CRAWL_CONFIG_EXCHANGETICKER_ERROR = "crawl_config_exchangeticker_error";

        #endregion

        #region 钱包相关表
        /// <summary>
        /// 钱包
        /// </summary>
        public const string BASE_WALLET = "base_wallet";
        /// <summary>
        /// 钱包虚拟币关联表
        /// </summary>
        public const string BASE_WALLET_COIN = "base_wallet_coin";
        /// <summary>
        /// 钱包类型表
        /// </summary>
        public const string BASE_WALLET_TYPE = "base_wallet_type";
        /// <summary>
        /// 钱包多语言表
        /// </summary>
        public const string BASE_WALLET_Lang = "base_wallet_lang";
        #endregion

        #region 用户相关表
        /// <summary>
        /// 入驻信息审核表
        /// </summary>
        public const string T_SNSUSERINFO = "usercenter.t_snsuserinfo";
        /// <summary>
        /// 入驻方资讯审核表
        /// </summary>
        public const string T_USER = "usercenter.t_user";
        /// <summary>
        /// 用户搜索表
        /// </summary>
        public const string LOG_USER_SEARCH = "log_user_search";

        /// <summary>
        /// 机器人币种报价表
        /// </summary>
        public const string LOG_ROBOT = "log_robot";

        /// <summary>
        /// 用户点击表
        /// </summary>
        public const string LOG_USER_CLICK = "log_user_click";
        /// <summary>
        /// 用户意见表
        /// </summary>
        public const string MEMBER_FEEDBACK = "usercenter.member_feedback";
        /// <summary>
        /// 用户列表
        /// </summary>
        public const string SITE_USER = "site_user";

        /// <summary>
        /// 用户消息通知列表
        /// </summary>
        public const string T_NOTICE = "usercenter.t_notice";

        /// <summary>
        /// 用户点击统计表
        /// </summary>
        public const string VIEW_USERCLICK = "view_userclick";
        #endregion

        /// <summary>
        /// 24小时币种涨跌数量统计
        /// </summary>
        public const string VIEW_COINCOUNT = "view_coincount";

        /// <summary>
        /// 常见问题
        /// </summary>
        public const string CMS_FAQ = "usercenter.cms_faq";

        /// <summary>
        /// 常见问题类型
        /// </summary>
        public const string CONFIG_FAQTYPE = "usercenter.config_faqtype";

        /// <summary>
        /// 
        /// </summary>
        public const string STAT_COIN_HOT = "stat_coin_hot";

        /// <summary>
        /// 热门搜索统计表_币种及交易所
        /// </summary>
        public const string STAT_HOTSEARCH = "stat_hotsearch";
        /// <summary>
        /// 功能模块问题反馈
        /// </summary>
        public const string MODULE_TRACE= "module_trace";
        /// <summary>
        /// 功能模块问题统计
        /// </summary>
        public const string MODULE_TRACE_ISSUE = "module_trace_issue";
        /// <summary>
        /// 功能模块问题其它问题列表
        /// </summary>
        public const string MODULE_TRACE_ISSUE_OTHER = "module_trace_issue_other";
        /// <summary>
        /// 交易所置顶配置表
        /// </summary>
        public const string BASE_EXCHANGE_TOP_CONFIG = "base_exchange_top_config";
        /// <summary>
        /// defi 币种资产分类表
        /// </summary>
        public const string DEFI_COIN_ASSET_CATEGORY = "defi_coin_asset_category";
        /// <summary>
        /// 用户申诉明细
        /// </summary>
        public const string SNS_USERINFO_APPEAL = "sns_userinfo_appeal";
        /// <summary>
        /// 币种分区
        /// </summary>
        public const string BASE_COIN_ZONE = "base_coin_zone";
        /// <summary>
        /// 币种分区语言
        /// </summary>
        public const string BASE_COIN_ZONE_LANG = "base_coin_zone_lang";
        /// <summary>
        /// 币种分区明细
        /// </summary>
        public const string BASE_COIN_ZONE_DETAIL = "base_coin_zone_detail";
        /// <summary>
        /// defi 矿池
        /// </summary>
        public const string DEFI_POOL = "defi_pool";
        /// <summary>
        /// defi 矿池明细
        /// </summary>
        public const string DEFI_POOL_INFO = "defi_pool_info";
        /// <summary>
        /// 资讯主题
        /// </summary>
        public const string MEDIA_NEWS_TITLE = "media_news_title";
        /// <summary>
        /// 推送热币配置
        /// </summary>
        public const string PUSH_CONFIG_HOTTICKER = "push_config_hotticker";
        /// <summary>
        /// 上周圈子用户明星榜
        /// </summary>
        public const string SNS_RAKING_USERINFO = "sns_raking_userinfo";
        /// <summary>
        /// 推荐列表
        /// </summary>
        public const string BBS_USERINFO_RECOMMEND = "bbs.bbs_userinfo_recommend";
        /// <summary>
        /// 圈子用户
        /// </summary>
        public const string SNS_USERINFO = "bbs.bbs_userinfo";
        /// <summary>
        /// 资讯关联
        /// </summary>

        public const string MEDIA_NEWS_COINMARKET = "media_news_coinmarket";

        /// <summary>
        /// 内页公告
        /// </summary>
        public const string INSIDE_ANNOUNCE = "cms_announce";
        /// <summary>
        /// 币种异动配置
        /// </summary>
        public const string MARKET_COIN_UNUSUALACTION_CONFIG = "market_coin_unusualaction_config";
        /// <summary>
        /// 新币交易所挖矿
        /// </summary>
        public const string NEWCOIN_MINING = "newcoin_mining";
        /// <summary>
        /// 新币发行
        /// </summary>
        public const string NEWCOIN_PUBLISH = "newcoin_publish";
        /// <summary>
        /// 新币待发行
        /// </summary>
        public const string NEWCOIN_TODOPUBLISH = "newcoin_todopublish";
        /// <summary>
        /// 币种常见问题
        /// </summary>
        public const string BASE_COIN_ISSUE = "base_coin_issue";
        /// <summary>
        /// 币种评论
        /// </summary>
        public const string BASE_COIN_COMMENT = "base_coin_comment";
        /// <summary>
        /// 代币流通
        /// </summary>
        public const string BASE_COIN_TOKENCIRCULATION = "base_coin_tokencirculation";
        /// <summary>
        /// 代币流通明细
        /// </summary>
        public const string BASE_COIN_TOKENCIRCULATION_DETAIL = "base_coin_tokencirculation_detail";
        /// <summary>
        /// nft平台
        /// </summary>
        public const string NFT_PLATFORM = "nft_platform";          
        /// <summary>
        /// nft平台扩展字段
        /// </summary>
        public const string NFT_PLATFORM_EXTRA = "nft_platform_extra";     
        /// <summary>
        /// nft分类
        /// </summary>
        public const string NFT_SORTING = "NFT_SORTING";       
        /// <summary>
        /// nft项目
        /// </summary>
        public const string NFT_COLLECTION = "nft_collection";      
        /// <summary>
        /// nft创作者
        /// </summary>
        public const string NFT_CREATOR = "nft_creator";    
        /// <summary>
        /// nft公链
        /// </summary>
        public const string NFT_CHAIN = "nft_chain";
        /// <summary>
        /// nft创作者项目关系
        /// </summary>
        public const string NFT_CREATOR_COLLECTION_RELATION = "nft_creator_collection_relation";
        /// <summary>
        /// nft分类和项目平台关系表
        /// </summary>
        public const string NFT_SORTING_RELATION = "nft_sorting_relation";
        //public static InfluxDBTable InfluxDBTable;

        /// <summary>
        /// 爬虫错误日志
        /// </summary>
        public const string CRAWL_ERROR_LOG = "crawl_error_log";
        public class InfluxDBTable
        {
            public const string KLINE = "kline";
        }
    }
}
