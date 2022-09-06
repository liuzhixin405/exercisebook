using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAoppication
{
    public class ConstDefine
    {
        public const string CacheManagerDefaultKey = "default";
        /// <summary>
        /// CacheManager Reids数据永不过期Key
        /// </summary>
        public const string CacheManagerRedisNeverExpiresKey = "redisNeverExpires";

        /// <summary>
        /// 语言编码
        /// </summary>
        public const string LanguageCode = "language-code";

        /// <summary>
        /// 版本号变量
        /// </summary>
        public const string Version = "Version";
        /// <summary>
        /// 验证token变量
        /// </summary>
        public const string Token = "token";
        /// <summary>
        /// Redis Hash所有缓存token对应客户数据
        /// </summary>
        public const string RedisAllTokenCustomer = "_all_token_customer_";
        /// <summary>
        /// 登录类型：0.网页;1.android;2.iOS
        /// </summary>
        public const string ClientType = "clientType";
        /// <summary>
        /// 商户
        /// </summary>
        public const string BusinessId = "current_businessId";
        /// <summary>
        /// 客户Id
        /// </summary>
        public const string CustomerId = "current_customerId";
        /// <summary>
        /// 客户全仓资产缓存Key
        /// </summary>
        public const string CustomerAllAssetCacheKey = "_asset_customer_all_{0}_";
        /// <summary>
        /// 客户逐仓资产缓存Key
        /// </summary>
        public const string CustomerPositionAssetCacheKey = "_asset_customer_all_{0}_";
        /// <summary>
        /// 并发重试次数
        /// </summary>
        public const int MaxRetryCount = 100;
        /// <summary>
        /// 指数价格主题
        /// </summary>
        public const string Messagebus_IndexPriceChangedTopic = "_Messagebus_IndexPriceChangedTopic_";
        /// <summary>
        /// 发送行情主题
        /// </summary>
        public const string Messagebus_SendMarketDataTopic = "_Messagebus_SendMarketDataTopic_";
        /// <summary>
        /// 开平仓单主题
        /// </summary>
        public const string Messagebus_SendOrderTopic = "_Messagebus_SendOrderTopic_";
        /// <summary>
        /// 撤消单主题
        /// </summary>
        public const string Messagebus_CancelOrderTopic = "_Messagebus_CancelOrderTopic_";
        /// <summary>
        /// 定单状态改变推送主题
        /// </summary>
        public const string Messagebus_OrdersStatusChangedTopic = "_Messagebus_OrdersStatusChangedTopic_";
        /// <summary>
        /// 最新成交单推送主题
        /// </summary>
        public const string Messagebus_OrderFilledTopic = "Messagebus_OrderFilledTopic";
        /// <summary>
        /// 客户资产变化推送主题
        /// </summary>
        public const string Messagebus_AssetsChangedTopic = "Messagebus_AssetsChangedTopic";
        /// <summary>
        /// 实时盘口推送主题
        /// </summary>
        public const string Messagebus_RealTimeHandicapTopic = "Messagebus_RealTimeHandicapTopic";
        /// <summary>
        /// 最新交易推送主题
        /// </summary>
        public const string Messagebus_RecentTradesTopic = "Messagebus_RecentTradesTopic";
        /// <summary>
        /// 推送K线
        /// </summary>
        public const string Messagebus_CandlestickTopic = "Messagebus_CandlestickTopic";
        /// <summary>
        /// 活动单变动通知
        /// </summary>
        public const string Messagebus_ActiveOrderChangedTopic = "Messagebus_ActiveOrderChangedTopic";
        /// <summary>
        /// 用户成交单缓存Key前缀
        /// </summary>
        public const string Cache_OrderFilledPrefix = "_customer_all_orderfilled_";
        /// <summary>
        /// 用户挂单缓存Key前比对
        /// </summary>
        public const string Cache_OrderPrefix = "_customer_all_order_";
        /// <summary>
        /// 用户活动单缓存Key前比对
        /// </summary>
        public const string Cache_ActiveOrderPrefix = "_customer_all_activeorder_";
    }
}
