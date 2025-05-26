using System.Net.Http.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace OllamaContext7Api.Services
{
    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly Context7Client _context7Client;
        private readonly ILogger<AIService> _logger;

        private const string OllamaUrl = "http://localhost:11434/api/generate";
        private const string ModelName = "codellama:7b-code"; //"qwen2.5-coder:14b";

        // 模型上下文限制配置
        private const int MaxTokens = 6000; // 为14B模型预留的安全token数量
        private const int MaxContextLength = 4000; // 文档内容最大长度（字符）
        private const int QuestionReservedTokens = 500; // 为问题和提示词预留的token

        public AIService(HttpClient httpClient, Context7Client context7Client, ILogger<AIService> logger)
        {
            _httpClient = httpClient;
            _context7Client = context7Client;
            _logger = logger;

            // 设置HTTP客户端超时
            _httpClient.Timeout = TimeSpan.FromMinutes(5);
        }

        public async Task<string> GetAnswerAsync(string question)
        {
            return await ProcessWithContextAsync(question, "");
        }

        public async Task<string> ProcessWithContextAsync(string question, string context)
        {
            try
            {
                _logger.LogInformation($"处理问题，原始文档长度: {context?.Length ?? 0}");

                // 1. 智能截取相关文档内容
                var optimizedContext = OptimizeContext(question, context);
                _logger.LogInformation($"优化后文档长度: {optimizedContext.Length}");

                // 2. 如果文档仍然太大，进行分块处理
                if (optimizedContext.Length > MaxContextLength)
                {
                    return await ProcessWithChunking(question, optimizedContext);
                }

                // 3. 构建优化的提示词
                var prompt = BuildOptimizedPrompt(question, optimizedContext);

                // 4. 调用Ollama获取答案
                var response = await GetOllamaResponseWithRetry(prompt);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理问题时出错");
                return $"处理问题时出错: {ex.Message}";
            }
        }

        private string OptimizeContext(string question, string context)
        {
            if (string.IsNullOrEmpty(context) || context.Length <= MaxContextLength)
            {
                return context ?? "";
            }

            _logger.LogInformation("开始优化文档内容");

            // 1. 提取问题关键词
            var keywords = ExtractKeywords(question);
            _logger.LogInformation($"提取到关键词: {string.Join(", ", keywords)}");

            // 2. 按段落分割文档
            var paragraphs = SplitIntoParagraphs(context);

            // 3. 计算每个段落的相关性分数
            var scoredParagraphs = paragraphs
                .Select(p => new
                {
                    Content = p,
                    Score = CalculateRelevanceScore(p, keywords, question)
                })
                .Where(p => p.Score > 0)
                .OrderByDescending(p => p.Score)
                .ToList();

            // 4. 选择最相关的段落，直到达到长度限制
            var result = new StringBuilder();
            var currentLength = 0;

            foreach (var paragraph in scoredParagraphs)
            {
                if (currentLength + paragraph.Content.Length > MaxContextLength)
                {
                    // 如果还有空间，尝试截取部分内容
                    var remainingSpace = MaxContextLength - currentLength;
                    if (remainingSpace > 200) // 至少保留200字符才有意义
                    {
                        var truncated = TruncateAtSentence(paragraph.Content, remainingSpace);
                        result.AppendLine(truncated);
                    }
                    break;
                }

                result.AppendLine(paragraph.Content);
                currentLength += paragraph.Content.Length;
            }

            var optimizedContent = result.ToString().Trim();
            _logger.LogInformation($"文档优化完成，原长度: {context.Length}, 优化后: {optimizedContent.Length}");

            return optimizedContent;
        }

        private async Task<string> ProcessWithChunking(string question, string context)
        {
            _logger.LogInformation("使用分块处理策略");

            var chunks = CreateSmartChunks(context, MaxContextLength - 500);
            var responses = new List<string>();

            foreach (var (chunk, index) in chunks.Select((c, i) => (c, i)))
            {
                _logger.LogInformation($"处理分块 {index + 1}/{chunks.Count}");

                var chunkPrompt = BuildChunkPrompt(question, chunk, index + 1, chunks.Count);

                try
                {
                    var response = await GetOllamaResponseWithRetry(chunkPrompt);
                    if (!string.IsNullOrEmpty(response) && !response.Contains("无法") && !response.Contains("没有"))
                    {
                        responses.Add(response);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"处理分块 {index + 1} 失败: {ex.Message}");
                }

                // 避免过快请求
                await Task.Delay(1000);
            }

            // 合并分块响应
            return CombineChunkResponses(question, responses);
        }

        private List<string> ExtractKeywords(string question)
        {
            var keywords = new List<string>();

            // 移除停用词并提取关键词
            var words = Regex.Matches(question.ToLower(), @"\b\w{3,}\b")
                .Cast<Match>()
                .Select(m => m.Value)
                .Where(w => !IsStopWord(w))
                .Distinct()
                .ToList();

            keywords.AddRange(words);

            // 添加技术术语识别
            var techTerms = new[]
            {
                "api", "controller", "service", "model", "dto", "entity", "repository",
                "async", "await", "task", "http", "json", "xml", "database", "sql",
                "authentication", "authorization", "middleware", "dependency", "injection",
                "configuration", "logging", "testing", "exception", "validation"
            };

            keywords.AddRange(techTerms.Where(term => question.ToLower().Contains(term)));

            return keywords.Take(10).ToList(); // 限制关键词数量
        }

        private bool IsStopWord(string word)
        {
            var stopWords = new HashSet<string>
            {
                "the", "a", "an", "and", "or", "but", "in", "on", "at", "to", "for",
                "of", "with", "by", "from", "as", "is", "are", "was", "were", "be",
                "have", "has", "had", "do", "does", "did", "will", "would", "could",
                "should", "may", "might", "can", "how", "what", "when", "where", "why",
                "this", "that", "these", "those", "i", "you", "he", "she", "it", "we", "they"
            };

            return stopWords.Contains(word);
        }

        private List<string> SplitIntoParagraphs(string text)
        {
            return text.Split(new[] { "\n\n", "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Where(p => p.Trim().Length > 50) // 过滤太短的段落
                .Select(p => p.Trim())
                .ToList();
        }

        private double CalculateRelevanceScore(string paragraph, List<string> keywords, string question)
        {
            var score = 0.0;
            var paragraphLower = paragraph.ToLower();
            var questionLower = question.ToLower();

            // 1. 关键词匹配分数
            foreach (var keyword in keywords)
            {
                var keywordCount = Regex.Matches(paragraphLower, $@"\b{Regex.Escape(keyword)}\b").Count;
                score += keywordCount * 2.0;
            }

            // 2. 问题直接匹配分数
            var questionWords = questionLower.Split(' ').Where(w => w.Length > 2);
            foreach (var word in questionWords)
            {
                if (paragraphLower.Contains(word))
                {
                    score += 1.0;
                }
            }

            // 3. 代码相关性分数
            if (paragraph.Contains("```") || paragraph.Contains("class ") || paragraph.Contains("public "))
            {
                score += 3.0;
            }

            // 4. 标题和重要性标记
            if (paragraph.StartsWith("#") || paragraph.Contains("**") || paragraph.Contains("Important"))
            {
                score += 2.0;
            }

            // 5. 长度惩罚（避免选择过长的段落）
            if (paragraph.Length > 1000)
            {
                score *= 0.8;
            }

            return score;
        }

        private string TruncateAtSentence(string text, int maxLength)
        {
            if (text.Length <= maxLength)
                return text;

            var truncated = text.Substring(0, maxLength);
            var lastSentence = truncated.LastIndexOfAny(new[] { '.', '!', '?', '\n' });

            if (lastSentence > maxLength * 0.7) // 如果句子边界在合理位置
            {
                return truncated.Substring(0, lastSentence + 1);
            }

            return truncated + "...";
        }

        private List<string> CreateSmartChunks(string text, int maxChunkSize)
        {
            var chunks = new List<string>();
            var paragraphs = SplitIntoParagraphs(text);

            var currentChunk = new StringBuilder();

            foreach (var paragraph in paragraphs)
            {
                if (currentChunk.Length + paragraph.Length > maxChunkSize && currentChunk.Length > 0)
                {
                    chunks.Add(currentChunk.ToString().Trim());
                    currentChunk.Clear();
                }

                currentChunk.AppendLine(paragraph);
            }

            if (currentChunk.Length > 0)
            {
                chunks.Add(currentChunk.ToString().Trim());
            }

            return chunks;
        }

        private string BuildOptimizedPrompt(string question, string context)
        {
            var sb = new StringBuilder();

            sb.AppendLine("你是一个.NET专家助手。基于以下文档内容回答问题：");
            sb.AppendLine();
            sb.AppendLine("=== 相关文档 ===");
            sb.AppendLine(context);
            sb.AppendLine();
            sb.AppendLine("=== 问题 ===");
            sb.AppendLine(question);
            sb.AppendLine();
            sb.AppendLine("请基于上述文档内容提供准确、简洁的答案。如果文档中没有相关信息，请说明并提供一般性建议。");

            return sb.ToString();
        }

        private string BuildChunkPrompt(string question, string chunk, int chunkIndex, int totalChunks)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"你是.NET专家助手。这是第{chunkIndex}/{totalChunks}部分文档，请基于此部分内容回答问题：");
            sb.AppendLine();
            sb.AppendLine("=== 文档片段 ===");
            sb.AppendLine(chunk);
            sb.AppendLine();
            sb.AppendLine("=== 问题 ===");
            sb.AppendLine(question);
            sb.AppendLine();
            sb.AppendLine("请只基于这部分文档回答相关内容，如果这部分没有相关信息请回答'此部分无相关信息'。");

            return sb.ToString();
        }

        private string CombineChunkResponses(string question, List<string> responses)
        {
            if (!responses.Any())
            {
                return "无法从提供的文档中找到相关信息来回答您的问题。";
            }

            if (responses.Count == 1)
            {
                return responses[0];
            }

            var sb = new StringBuilder();
            sb.AppendLine($"基于文档分析，针对您的问题「{question}」：");
            sb.AppendLine();

            for (int i = 0; i < responses.Count; i++)
            {
                if (!responses[i].Contains("无相关信息"))
                {
                    sb.AppendLine($"**要点 {i + 1}：**");
                    sb.AppendLine(responses[i]);
                    sb.AppendLine();
                }
            }

            return sb.ToString().Trim();
        }

        private async Task<string> GetOllamaResponseWithRetry(string prompt, int maxRetries = 2)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    return await GetOllamaResponse(prompt);
                }
                catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
                {
                    _logger.LogWarning($"Ollama请求超时，重试 {i + 1}/{maxRetries}");
                    if (i == maxRetries - 1)
                    {
                        return "请求超时，请尝试提出更简洁的问题。";
                    }
                    await Task.Delay(2000); // 等待2秒后重试
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Ollama请求失败，重试 {i + 1}/{maxRetries}");
                    if (i == maxRetries - 1)
                    {
                        throw;
                    }
                    await Task.Delay(1000);
                }
            }

            return "请求失败，请稍后重试。";
        }

        private async Task<string> GetOllamaResponse(string prompt)
        {
            var request = new
            {
                model = ModelName,
                prompt = prompt,
                stream = false,
                options = new
                {
                    temperature = 0.3,
                    top_p = 0.9,
                    num_ctx = 8192, // 设置上下文窗口
                    num_predict = 1000 // 限制响应长度
                }
            };

            _logger.LogInformation($"发送Ollama请求，提示词长度: {prompt.Length}");

            var response = await _httpClient.PostAsJsonAsync(OllamaUrl, request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OllamaResponse>();
            var answer = result?.Response ?? "未能获取答案";

            _logger.LogInformation($"收到Ollama响应，长度: {answer.Length}");

            return answer;
        }
    }

    public class OllamaResponse
    {
        public string Response { get; set; } = "";
    }
}