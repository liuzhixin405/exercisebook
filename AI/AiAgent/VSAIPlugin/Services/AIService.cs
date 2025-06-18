using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VSAIPluginNew.AI;
using System.Collections.Concurrent;
using VSAIPluginNew.Services;

namespace VSAIPluginNew.Services
{
    public class AIService : IAIService
    {
        // 聊天历史缓存
        private static readonly ConcurrentQueue<ChatMemoryEntry> _chatHistory = new ConcurrentQueue<ChatMemoryEntry>();
        private const int MaxChatHistory = 20;
        private const double SimilarityThreshold = 0.85; // 可调整

        private static readonly string[] ProjectKeywords = new[]
        {
            "代码", "类", "函数", "方法", "接口", "文件", "目录", "controller", "model", "service", "分析", "生成", "实现", "bug", "报错", "异常", "项目", "solution", "namespace"
        };

        private bool IsProjectRelated(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return false;
            return ProjectKeywords.Any(k => query.IndexOf(k, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        // 计算简单相似度（可后续替换为更智能的算法）
        private double CalculateSimilarity(string a, string b)
        {
            if (string.IsNullOrWhiteSpace(a) || string.IsNullOrWhiteSpace(b)) return 0;
            a = a.Trim(); b = b.Trim();
            if (a == b) return 1.0;
            int minLen = Math.Min(a.Length, b.Length);
            int same = 0;
            for (int i = 0; i < minLen; i++)
                if (a[i] == b[i]) same++;
            return (double)same / Math.Max(a.Length, b.Length);
        }

        public async Task<string> GenerateReplyAsync(string prompt, CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                return await MultiAgentManager.Instance.ProcessComplexQueryAsync(prompt);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return $"生成回复时出错: {ex.Message}";
            }
        }

        public async Task<string> ProcessQueryAsync(
            string query, 
            string taskType, 
            Dictionary<string, string>? contextFiles = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                // 0. 判断是否与项目有关
                if (!IsProjectRelated(query))
                {
                    // 直接用AI对话，不查项目
                    return await AgentFactory.ProcessComplexQueryAsync(query, "聊天");
                }

                // 1. 聊天历史缓存检索
                var mostSimilar = _chatHistory
                    .OrderByDescending(e => CalculateSimilarity(e.Question, query))
                    .FirstOrDefault();
                if (mostSimilar != null && CalculateSimilarity(mostSimilar.Question, query) > SimilarityThreshold)
                {
                    // 命中历史缓存，直接返回历史答案
                    return $"[历史缓存命中] {mostSimilar.Answer}";
                }

                // 2. 统计文件内容的总大小
                if (contextFiles != null && contextFiles.Count > 0)
                {
                    int totalSize = 0;
                    foreach (var content in contextFiles.Values)
                    {
                        totalSize += content.Length;
                    }

                    // 如果总大小超过300KB，则可能会导致模型无响应
                    const int MAX_TOTAL_SIZE = 300 * 1024;
                    if (totalSize > MAX_TOTAL_SIZE)
                    {
                        var result = await ProcessLargeContextQueryAsync(query, taskType, contextFiles, MAX_TOTAL_SIZE);
                        // 存入历史
                        _chatHistory.Enqueue(new ChatMemoryEntry { Question = query, Answer = result, Timestamp = DateTime.Now });
                        while (_chatHistory.Count > MaxChatHistory) _chatHistory.TryDequeue(out _);
                        return result;
                    }

                    var result2 = await AgentFactory.ProcessFilesAsync(query, contextFiles);
                    _chatHistory.Enqueue(new ChatMemoryEntry { Question = query, Answer = result2, Timestamp = DateTime.Now });
                    while (_chatHistory.Count > MaxChatHistory) _chatHistory.TryDequeue(out _);
                    return result2;
                }

                // 否则使用普通的复杂查询处理
                var result3 = await AgentFactory.ProcessComplexQueryAsync(query, taskType);
                _chatHistory.Enqueue(new ChatMemoryEntry { Question = query, Answer = result3, Timestamp = DateTime.Now });
                while (_chatHistory.Count > MaxChatHistory) _chatHistory.TryDequeue(out _);
                return result3;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return $"处理查询时出错: {ex.Message}";
            }
        }

        private async Task<string> ProcessLargeContextQueryAsync(
            string query,
            string taskType,
            Dictionary<string, string> contextFiles,
            int maxTotalSize)
        {
            // 1. 选择最相关的文件
            var relevantFiles = new Dictionary<string, string>();
            int currentSize = 0;

            // 2. 计算每个文件的相关性得分
            var scoredFiles = contextFiles
                .Select(f => new
                {
                    Path = f.Key,
                    Content = f.Value,
                    Score = CalculateQueryRelevance(f.Value, query)
                })
                .OrderByDescending(f => f.Score)
                .ToList();

            // 3. 选择最相关的文件，直到达到大小限制
            foreach (var file in scoredFiles)
            {
                if (currentSize + file.Content.Length > maxTotalSize)
                {
                    if (relevantFiles.Count == 0) // 如果还没有添加任何文件，那么至少添加一个文件的部分内容
                    {
                        var truncatedContent = file.Content.Substring(0, maxTotalSize);
                        relevantFiles[file.Path] = truncatedContent;
                    }
                    break;
                }

                relevantFiles[file.Path] = file.Content;
                currentSize += file.Content.Length;
            }

            // 4. 使用筛选后的文件进行处理
            return await AgentFactory.ProcessFilesAsync(query, relevantFiles);
        }

        private double CalculateQueryRelevance(string content, string query)
        {
            if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(query))
                return 0;

            double score = 0;
            content = content.ToLower();
            query = query.ToLower();

            // 1. 直接匹配
            if (content.Contains(query))
                score += 10;

            // 2. 关键词匹配
            var keywords = query.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var keyword in keywords)
            {
                if (keyword.Length < 3) continue;
                if (content.Contains(keyword))
                    score += 1;
            }

            // 3. 变量名或函数名匹配（驼峰命名法或下划线命名法）
            var codePatterns = keywords
                .Where(k => k.Length >= 3 && (char.IsUpper(k[0]) || k.Contains("_")))
                .ToList();

            foreach (var pattern in codePatterns)
            {
                if (content.Contains(pattern))
                    score += 2;
            }

            return score;
        }

        // 原有的ProcessQueryAsync方法现在被标记为过时
        [Obsolete("请使用新的重载方法")]
        public async Task<string> ProcessQueryAsync(string query, string modelName, string templateKey)
        {
            // 创建一个空的字典作为上下文文件
            Dictionary<string, string>? contextFiles = new Dictionary<string, string>();
            return await ProcessQueryAsync(query, modelName, contextFiles);
        }
    }
}
