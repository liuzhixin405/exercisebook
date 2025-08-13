namespace ReActAgentNetCore.Core.Models
{
    /// <summary>
    /// 缓存项模型
    /// </summary>
    public class CacheItem
    {
        public string Id { get; set; } = "";
        public string Question { get; set; } = "";
        public string Answer { get; set; } = "";
        public string QuestionHash { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime LastAccessed { get; set; }
        public int AccessCount { get; set; }
        public double SimilarityScore { get; set; }
        public string Source { get; set; } = ""; // "model", "mcp", "cache"
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// 问题预处理结果
    /// </summary>
    public class ProcessedQuestion
    {
        public string OriginalQuestion { get; set; } = "";
        public string NormalizedQuestion { get; set; } = "";
        public string QuestionHash { get; set; } = "";
        public List<string> Keywords { get; set; } = new();
        public string Intent { get; set; } = ""; // "create_project", "file_operation", "general_question"
        public Dictionary<string, object> ExtractedInfo { get; set; } = new();
    }

    /// <summary>
    /// 缓存查询结果
    /// </summary>
    public class CacheQueryResult
    {
        public bool Found { get; set; }
        public CacheItem? CacheItem { get; set; }
        public double BestSimilarityScore { get; set; }
        public List<CacheItem> SimilarItems { get; set; } = new();
        public string QueryStrategy { get; set; } = ""; // "exact_match", "semantic_similarity", "keyword_match"
    }

    /// <summary>
    /// 缓存策略配置
    /// </summary>
    public class CacheStrategy
    {
        public double MinSimilarityThreshold { get; set; } = 0.8;
        public int MaxCacheSize { get; set; } = 1000;
        public TimeSpan CacheExpiration { get; set; } = TimeSpan.FromDays(30);
        public bool EnableSemanticSearch { get; set; } = true;
        public bool EnableKeywordSearch { get; set; } = true;
        public int MaxSimilarItems { get; set; } = 5;
    }
} 