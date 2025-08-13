using ReActAgentNetCore.Core.Models;

namespace ReActAgentNetCore.Core.Interfaces
{
    /// <summary>
    /// 缓存服务接口
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// 查询缓存
        /// </summary>
        Task<CacheQueryResult> QueryCacheAsync(string question);

        /// <summary>
        /// 存储到缓存
        /// </summary>
        Task StoreInCacheAsync(string question, string answer, string source = "model");

        /// <summary>
        /// 更新缓存项访问信息
        /// </summary>
        Task UpdateAccessInfoAsync(string questionHash);

        /// <summary>
        /// 清理过期缓存
        /// </summary>
        Task CleanupExpiredCacheAsync();

        /// <summary>
        /// 获取缓存统计信息
        /// </summary>
        Task<Dictionary<string, object>> GetCacheStatsAsync();
    }

    /// <summary>
    /// 问题预处理服务接口
    /// </summary>
    public interface IQuestionProcessor
    {
        /// <summary>
        /// 预处理问题
        /// </summary>
        Task<ProcessedQuestion> ProcessQuestionAsync(string question);

        /// <summary>
        /// 计算问题相似度
        /// </summary>
        Task<double> CalculateSimilarityAsync(string question1, string question2);

        /// <summary>
        /// 提取问题意图
        /// </summary>
        Task<string> ExtractIntentAsync(string question);

        /// <summary>
        /// 提取关键词
        /// </summary>
        Task<List<string>> ExtractKeywordsAsync(string question);
    }

    /// <summary>
    /// 智能路由服务接口
    /// </summary>
    public interface ISmartRouter
    {
        /// <summary>
        /// 决定处理策略
        /// </summary>
        Task<RoutingDecision> DecideRoutingAsync(string question, CacheQueryResult? cacheResult = null);

        /// <summary>
        /// 执行路由决策
        /// </summary>
        Task<string> ExecuteRoutingAsync(RoutingDecision decision);
    }

    /// <summary>
    /// 路由决策
    /// </summary>
    public class RoutingDecision
    {
        public string Strategy { get; set; } = ""; // "use_cache", "call_model", "call_mcp", "hybrid"
        public CacheItem? CacheItem { get; set; }
        public double Confidence { get; set; }
        public string Reason { get; set; } = "";
        public Dictionary<string, object> Parameters { get; set; } = new();
    }
} 