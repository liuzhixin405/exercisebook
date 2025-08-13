using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ReActAgentNetCore.Core.Interfaces;
using ReActAgentNetCore.Core.Models;

namespace ReActAgentNetCore.Infrastructure.Cache
{
    /// <summary>
    /// Redis缓存服务实现
    /// </summary>
    public class RedisCacheService : ICacheService
    {
        private readonly string _connectionString;
        private readonly string _cachePrefix = "qa_cache:";
        private readonly CacheStrategy _strategy;
        private readonly IQuestionProcessor _questionProcessor;

        public RedisCacheService(string connectionString, CacheStrategy strategy, IQuestionProcessor questionProcessor)
        {
            _connectionString = connectionString;
            _strategy = strategy;
            _questionProcessor = questionProcessor;
        }

        /// <summary>
        /// 查询缓存
        /// </summary>
        public async Task<CacheQueryResult> QueryCacheAsync(string question)
        {
            try
            {
                // 1. 尝试精确匹配
                var exactMatch = await TryExactMatchAsync(question);
                if (exactMatch.Found)
                {
                    return exactMatch;
                }

                // 2. 尝试语义相似度匹配
                if (_strategy.EnableSemanticSearch)
                {
                    var semanticMatch = await TrySemanticMatchAsync(question);
                    if (semanticMatch.Found && semanticMatch.BestSimilarityScore >= _strategy.MinSimilarityThreshold)
                    {
                        return semanticMatch;
                    }
                }

                // 3. 尝试关键词匹配
                if (_strategy.EnableKeywordSearch)
                {
                    var keywordMatch = await TryKeywordMatchAsync(question);
                    if (keywordMatch.Found)
                    {
                        return keywordMatch;
                    }
                }

                return new CacheQueryResult { Found = false };
            }
            catch (Exception ex)
            {
                // 如果Redis不可用，返回未找到
                Console.WriteLine($"缓存查询失败: {ex.Message}");
                return new CacheQueryResult { Found = false };
            }
        }

        /// <summary>
        /// 存储到缓存
        /// </summary>
        public async Task StoreInCacheAsync(string question, string answer, string source = "model")
        {
            try
            {
                var processedQuestion = await _questionProcessor.ProcessQuestionAsync(question);
                var cacheItem = new CacheItem
                {
                    Id = Guid.NewGuid().ToString(),
                    Question = question,
                    Answer = answer,
                    QuestionHash = processedQuestion.QuestionHash,
                    CreatedAt = DateTime.UtcNow,
                    LastAccessed = DateTime.UtcNow,
                    AccessCount = 1,
                    Source = source,
                    Metadata = new Dictionary<string, object>
                    {
                        ["keywords"] = processedQuestion.Keywords,
                        ["intent"] = processedQuestion.Intent,
                        ["normalized_question"] = processedQuestion.NormalizedQuestion
                    }
                };

                var json = JsonSerializer.Serialize(cacheItem);
                var key = $"{_cachePrefix}{processedQuestion.QuestionHash}";
                
                // 使用Redis存储（这里简化实现，实际应该使用Redis客户端）
                await StoreInMemoryCacheAsync(key, json);
                
                // 更新统计信息
                await UpdateCacheStatsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"存储缓存失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 更新缓存项访问信息
        /// </summary>
        public async Task UpdateAccessInfoAsync(string questionHash)
        {
            try
            {
                var key = $"{_cachePrefix}{questionHash}";
                var json = await GetFromMemoryCacheAsync(key);
                
                if (!string.IsNullOrEmpty(json))
                {
                    var cacheItem = JsonSerializer.Deserialize<CacheItem>(json);
                    if (cacheItem != null)
                    {
                        cacheItem.LastAccessed = DateTime.UtcNow;
                        cacheItem.AccessCount++;
                        
                        var updatedJson = JsonSerializer.Serialize(cacheItem);
                        await StoreInMemoryCacheAsync(key, updatedJson);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新缓存访问信息失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 清理过期缓存
        /// </summary>
        public async Task CleanupExpiredCacheAsync()
        {
            try
            {
                var cutoffTime = DateTime.UtcNow.Subtract(_strategy.CacheExpiration);
                var expiredKeys = await GetExpiredKeysAsync(cutoffTime);
                
                foreach (var key in expiredKeys)
                {
                    await RemoveFromMemoryCacheAsync(key);
                }
                
                Console.WriteLine($"清理了 {expiredKeys.Count} 个过期缓存项");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"清理过期缓存失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取缓存统计信息
        /// </summary>
        public async Task<Dictionary<string, object>> GetCacheStatsAsync()
        {
            try
            {
                var stats = await GetCacheStatsFromMemoryAsync();
                return stats;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取缓存统计信息失败: {ex.Message}");
                return new Dictionary<string, object>();
            }
        }

        #region 私有方法

        private async Task<CacheQueryResult> TryExactMatchAsync(string question)
        {
            var processedQuestion = await _questionProcessor.ProcessQuestionAsync(question);
            var key = $"{_cachePrefix}{processedQuestion.QuestionHash}";
            var json = await GetFromMemoryCacheAsync(key);
            
            if (!string.IsNullOrEmpty(json))
            {
                var cacheItem = JsonSerializer.Deserialize<CacheItem>(json);
                if (cacheItem != null)
                {
                    await UpdateAccessInfoAsync(processedQuestion.QuestionHash);
                    return new CacheQueryResult
                    {
                        Found = true,
                        CacheItem = cacheItem,
                        BestSimilarityScore = 1.0,
                        QueryStrategy = "exact_match"
                    };
                }
            }
            
            return new CacheQueryResult { Found = false };
        }

        private async Task<CacheQueryResult> TrySemanticMatchAsync(string question)
        {
            var processedQuestion = await _questionProcessor.ProcessQuestionAsync(question);
            var allKeys = await GetAllCacheKeysAsync();
            var similarItems = new List<CacheItem>();
            var bestScore = 0.0;
            
            foreach (var key in allKeys)
            {
                var json = await GetFromMemoryCacheAsync(key);
                if (!string.IsNullOrEmpty(json))
                {
                    var cacheItem = JsonSerializer.Deserialize<CacheItem>(json);
                    if (cacheItem != null)
                    {
                        var similarity = await _questionProcessor.CalculateSimilarityAsync(
                            processedQuestion.NormalizedQuestion, 
                            cacheItem.Metadata.GetValueOrDefault("normalized_question", "").ToString() ?? "");
                        
                        if (similarity >= _strategy.MinSimilarityThreshold)
                        {
                            similarItems.Add(cacheItem);
                            if (similarity > bestScore)
                            {
                                bestScore = similarity;
                            }
                        }
                    }
                }
            }
            
            if (similarItems.Any())
            {
                var bestItem = similarItems.OrderByDescending(x => 
                    x.Metadata.GetValueOrDefault("similarity_score", 0.0)).First();
                
                await UpdateAccessInfoAsync(bestItem.QuestionHash);
                
                return new CacheQueryResult
                {
                    Found = true,
                    CacheItem = bestItem,
                    BestSimilarityScore = bestScore,
                    SimilarItems = similarItems.Take(_strategy.MaxSimilarItems).ToList(),
                    QueryStrategy = "semantic_similarity"
                };
            }
            
            return new CacheQueryResult { Found = false };
        }

        private async Task<CacheQueryResult> TryKeywordMatchAsync(string question)
        {
            var processedQuestion = await _questionProcessor.ProcessQuestionAsync(question);
            var allKeys = await GetAllCacheKeysAsync();
            var keywordMatches = new List<CacheItem>();
            
            foreach (var key in allKeys)
            {
                var json = await GetFromMemoryCacheAsync(key);
                if (!string.IsNullOrEmpty(json))
                {
                    var cacheItem = JsonSerializer.Deserialize<CacheItem>(json);
                    if (cacheItem != null)
                    {
                        var cacheKeywords = cacheItem.Metadata.GetValueOrDefault("keywords", new List<string>()) as List<string> ?? new List<string>();
                        var commonKeywords = processedQuestion.Keywords.Intersect(cacheKeywords).ToList();
                        
                        if (commonKeywords.Count >= 2) // 至少2个关键词匹配
                        {
                            keywordMatches.Add(cacheItem);
                        }
                    }
                }
            }
            
            if (keywordMatches.Any())
            {
                var bestItem = keywordMatches.OrderByDescending(x => x.AccessCount).First();
                await UpdateAccessInfoAsync(bestItem.QuestionHash);
                
                return new CacheQueryResult
                {
                    Found = true,
                    CacheItem = bestItem,
                    BestSimilarityScore = 0.7, // 关键词匹配的默认相似度
                    SimilarItems = keywordMatches.Take(_strategy.MaxSimilarItems).ToList(),
                    QueryStrategy = "keyword_match"
                };
            }
            
            return new CacheQueryResult { Found = false };
        }

        #endregion

        #region 内存缓存实现（临时，实际应该使用Redis）

        private static readonly Dictionary<string, string> _memoryCache = new();
        private static readonly Dictionary<string, object> _cacheStats = new();

        private Task StoreInMemoryCacheAsync(string key, string value)
        {
            _memoryCache[key] = value;
            return Task.CompletedTask;
        }

        private Task<string> GetFromMemoryCacheAsync(string key)
        {
            _memoryCache.TryGetValue(key, out var value);
            return Task.FromResult(value ?? "");
        }

        private Task RemoveFromMemoryCacheAsync(string key)
        {
            _memoryCache.Remove(key);
            return Task.CompletedTask;
        }

        private Task<List<string>> GetAllCacheKeysAsync()
        {
            return Task.FromResult(_memoryCache.Keys.ToList());
        }

        private Task<List<string>> GetExpiredKeysAsync(DateTime cutoffTime)
        {
            var expiredKeys = new List<string>();
            foreach (var kvp in _memoryCache)
            {
                try
                {
                    var cacheItem = JsonSerializer.Deserialize<CacheItem>(kvp.Value);
                    if (cacheItem?.CreatedAt < cutoffTime)
                    {
                        expiredKeys.Add(kvp.Key);
                    }
                }
                catch
                {
                    // 忽略无效的缓存项
                }
            }
            return Task.FromResult(expiredKeys);
        }

        private Task UpdateCacheStatsAsync()
        {
            _cacheStats["total_items"] = _memoryCache.Count;
            _cacheStats["last_updated"] = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        private Task<Dictionary<string, object>> GetCacheStatsFromMemoryAsync()
        {
            return Task.FromResult(new Dictionary<string, object>(_cacheStats));
        }

        #endregion
    }
} 