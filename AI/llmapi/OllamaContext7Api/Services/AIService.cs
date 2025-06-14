using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace OllamaContext7Api.Services
{
    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AIService> _logger;
        private readonly AIServiceOptions _options;
        private readonly IFileService _fileService;
        private readonly ConcurrentQueue<ChatMemoryEntry> _chatMemory = new ConcurrentQueue<ChatMemoryEntry>();
        private readonly ConcurrentDictionary<string, List<float>> _fileContentEmbeddings = new ConcurrentDictionary<string, List<float>>();
        private readonly ConcurrentDictionary<string, string> _fileContents = new ConcurrentDictionary<string, string>();
        private const int MAX_CHAT_MEMORY_SIZE = 5;

        public AIService(
            HttpClient httpClient,
            ILogger<AIService> logger,
            IOptions<AIServiceOptions> options,
            IFileService fileService)
        {
            _httpClient = httpClient;
            _logger = logger;
            _options = options.Value;
            _fileService = fileService;

            // 设置HTTP客户端超时（流式响应需要更长时间）
            _httpClient.Timeout = TimeSpan.FromMinutes(10);
        }

        public async IAsyncEnumerable<string> GetAnswerStreamAsync(string question,
            [EnumeratorCancellation] CancellationToken cancellationToken = default, IEnumerable<string> relatedFiles = null, bool isDeepMode = false)
        {
            _logger.LogInformation($"开始处理问题: {question}");

            // 处理相关文件并生成嵌入
            if (relatedFiles != null && relatedFiles.Any())
            {
                foreach (var fileName in relatedFiles)
                {
                    if (!_fileContents.ContainsKey(fileName))
                    {
                        try
                        {
                            var content = await _fileService.ReadFileContentAsync(fileName);
                            _fileContents[fileName] = content;
                            if (_options.EnableMemory && !string.IsNullOrEmpty(_options.EmbeddingModelName))
                            {
                                var fileEmbedding = await VectorOperations.GetEmbeddingAsync(
                                    _httpClient,
                                    content,
                                    _options.Ollama.Url.Replace("/api/generate", "/api/embeddings"),
                                    _options.EmbeddingModelName);
                                _fileContentEmbeddings[fileName] = fileEmbedding;
                                _logger.LogInformation($"文件 '{fileName}' 内容及其嵌入已缓存。");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, $"处理文件 '{fileName}' 失败，将不包含其内容。");
                        }
                    }
                }
            }

            var currentQuestionEmbedding = new List<float>();
            if (_options.EnableMemory && !string.IsNullOrEmpty(_options.EmbeddingModelName))
            {
                try
                {
                    currentQuestionEmbedding = await VectorOperations.GetEmbeddingAsync(
                        _httpClient,
                        question,
                        _options.Ollama.Url.Replace("/api/generate", "/api/embeddings"), // Assuming embeddings endpoint is /api/embeddings
                        _options.EmbeddingModelName);
                    _logger.LogInformation($"问题嵌入生成成功，维度: {currentQuestionEmbedding.Count}");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "生成问题嵌入失败，将不使用记忆功能。");
                    currentQuestionEmbedding = new List<float>(); // Clear embedding if failed
                }
            }

            var retrievedContext = "";
            if (currentQuestionEmbedding.Any()) 
            {
                var contextBuilder = new StringBuilder();

                // Retrieve relevant chat memories
                if (_chatMemory.Any())
                {
                    var relevantMemories = _chatMemory
                        .OrderByDescending(entry => VectorOperations.CosineSimilarity(currentQuestionEmbedding, entry.QuestionEmbedding))
                        .Take(2) // Get top 2 relevant memories
                        .ToList();

                    if (relevantMemories.Any())
                    {
                        _logger.LogInformation($"找到 {relevantMemories.Count} 条相关历史记忆。");
                        foreach (var memory in relevantMemories)
                        {
                            contextBuilder.AppendLine($"历史问题: {memory.Question}");
                            contextBuilder.AppendLine($"历史回答: {memory.Answer}");
                        }
                    }
                }

                // Retrieve relevant file contents (if any)
                if (_options.EnableMemory && !string.IsNullOrEmpty(_options.EmbeddingModelName))
                {
                    if (relatedFiles != null && relatedFiles.Any())
                    {
                        // 如果提供了特定相关文件，则使用所有这些文件的内容
                        foreach (var fileName in relatedFiles)
                        {
                            if (_fileContents.TryGetValue(fileName, out var fileContent))
                            {
                                _logger.LogInformation($"使用指定相关文件内容: {fileName}");
                                contextBuilder.AppendLine($"相关文件: {fileName}");
                                contextBuilder.AppendLine($"文件内容: {fileContent}");
                            }
                        }
                    }
                    else if (_fileContentEmbeddings.Any()) // 如果没有指定相关文件，则回退到对所有缓存文件进行相似度搜索
                    {
                        var relevantFileEntries = _fileContentEmbeddings
                            .Select(kvp => new { FileName = kvp.Key, Embedding = kvp.Value })
                            .OrderByDescending(entry => VectorOperations.CosineSimilarity(currentQuestionEmbedding, entry.Embedding))
                            .Take(1) // 在没有指定相关文件时，保留顶部1个相关文件
                            .ToList();

                        foreach (var fileEntry in relevantFileEntries)
                        {
                            if (_fileContents.TryGetValue(fileEntry.FileName, out var fileContent))
                            {
                                _logger.LogInformation($"找到相关文件内容: {fileEntry.FileName}");
                                contextBuilder.AppendLine($"相关文件: {fileEntry.FileName}");
                                contextBuilder.AppendLine($"文件内容: {fileContent}");
                            }
                        }
                    }
                }

                retrievedContext = contextBuilder.ToString();
                _logger.LogInformation($"Retrieved Context for RAG: {retrievedContext}");
            }

            var prompt = await BuildOptimizedPromptAsync(question, cancellationToken, retrievedContext);

            string fullAnswer = "";
            if (isDeepMode)
            {
                    _logger.LogInformation("深度模式启用：使用LMStudio模型");
                    await foreach (var chunk in GetLMStudioStreamAsyncInternal(prompt, cancellationToken))
                    {
                        fullAnswer += chunk;
                        yield return chunk;
                    }    
            }
            else
            {
                    _logger.LogInformation("非深度模式：使用Ollama模型");
                    await foreach (var chunk in GetOllamaStreamAsyncInternal(prompt, cancellationToken))
                    {
                        fullAnswer += chunk;
                        yield return chunk;
                    }
            }

            // Store in memory cache
            if (_options.EnableMemory && !string.IsNullOrEmpty(_options.EmbeddingModelName) && !string.IsNullOrEmpty(fullAnswer))
            {
                try
                {
                    var answerEmbedding = await VectorOperations.GetEmbeddingAsync(
                        _httpClient,
                        fullAnswer,
                        _options.Ollama.Url.Replace("/api/generate", "/api/embeddings"),
                        _options.EmbeddingModelName);

                    var newMemoryEntry = new ChatMemoryEntry
                    {
                        Question = question,
                        Answer = fullAnswer,
                        QuestionEmbedding = currentQuestionEmbedding,
                        AnswerEmbedding = answerEmbedding,
                        Timestamp = DateTime.UtcNow
                    };

                    _chatMemory.Enqueue(newMemoryEntry);
                    while (_chatMemory.Count > MAX_CHAT_MEMORY_SIZE)
                    {
                        _chatMemory.TryDequeue(out _); // Remove oldest entry if exceeding size
                    }
                    _logger.LogInformation($"新对话已添加到记忆，当前记忆大小: {_chatMemory.Count}");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "生成回答嵌入或存储记忆失败。");
                }
            }
        }

        public async Task<bool> CheckHealthAsync()
        {
            try
            {
                if (_options.Provider == "Ollama")
                {
                    var request = new
                    {
                        model = _options.Ollama.Model,
                        prompt = "Hello",
                        stream = false,
                        options = new { num_predict = 5 }
                    };

                    var response = await _httpClient.PostAsJsonAsync(_options.Ollama.Url, request);
                    return response.IsSuccessStatusCode;
                }
                else
                {
                    var request = new
                    {
                        model = _options.LMStudio.Model,
                        messages = new[] { new { role = "user", content = "Hello" } },
                        stream = false
                    };

                    var response = await _httpClient.PostAsJsonAsync(_options.LMStudio.Url, request);
                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "健康检查失败");
                return false;
            }
        }

        private async Task<string> BuildOptimizedPromptAsync(string prompt, CancellationToken cancellationToken, string retrievedContext)
        {
            var sb = new StringBuilder();
            // 系统指令 - 定义角色和核心要求
            sb.AppendLine("系统指令：你是一个专业的编程助手，专注于.NET开发。");
            sb.AppendLine("你的回答必须遵循以下规则：");
            sb.AppendLine("1. 使用中文输出");
            sb.AppendLine("2. 涉及代码时必须提供完整可运行的C#示例");
            sb.AppendLine("3. 所有技术术语需符合微软官方文档规范");
            sb.AppendLine();

            sb.AppendLine($"当前问题：{prompt}");
            sb.AppendLine();
            sb.AppendLine("请提供准确、简洁的答案，包含以下内容：");
            sb.AppendLine("- 技术实现细节");
            sb.AppendLine("- 代码示例（如有）");
            sb.AppendLine("- 最佳实践建议");

            if (!string.IsNullOrEmpty(retrievedContext))
            {
                sb.AppendLine();
                sb.AppendLine("相关历史记忆：");
                sb.AppendLine(retrievedContext);
            }

            return sb.ToString();
        }

        private async IAsyncEnumerable<string> GetOllamaStreamAsyncInternal(string prompt,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var request = new
            {
                type="ollama",
                model = _options.Ollama.Model,
                prompt = prompt,
                stream = true,
                options = new
                {
                    temperature = 0.3,
                    top_p = 0.9,
                    num_ctx = 4096,
                    num_predict = 1000
                }
            };

            _logger.LogInformation($"发送Ollama请求，提示词长度: {prompt.Length}");

            // 使用单独的方法来处理可能抛出异常的操作
            var streamResult = await CreateStreamConnectionAsync(request, cancellationToken);

            if (!streamResult.Success)
            {
                yield return $"处理失败: {streamResult.ErrorMessage}";
                yield break;
            }

            using var reader = streamResult.Reader;

            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (cancellationToken.IsCancellationRequested)
                    yield break;

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parseSuccess = TryParseStreamResponse(line, out var streamResponse);

                if (!parseSuccess)
                {
                    _logger.LogWarning($"解析响应失败: {line}");
                    continue;
                }

                if (streamResponse?.Response != null)
                {
                    yield return streamResponse.Response;
                }

                // 检查是否完成
                if (streamResponse?.Done == true)
                {
                    _logger.LogInformation("响应完成");
                    break;
                }
            }
        }

        private async Task<StreamConnectionResult> CreateStreamConnectionAsync(object request, CancellationToken cancellationToken)
        {
            try
            {
                var url = ((dynamic)request).type == "ollama"
                    ? _options.Ollama.Url 
                    : _options.LMStudio.Url;
                
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = JsonContent.Create(request)
                };

                var response = await _httpClient.SendAsync(httpRequest,
                    HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                var reader = new StreamReader(stream);

                return new StreamConnectionResult
                {
                    Success = true,
                    Reader = reader,
                    Response = response
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建连接失败");
                return new StreamConnectionResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        private bool TryParseStreamResponse(string line, out OllamaStreamResponse streamResponse)
        {
            streamResponse = null;
            try
            {
                // 处理SSE格式的响应 (可能以"data:"开头)
                var json = line.StartsWith("data:") 
                    ? line.Substring(5).Trim()
                    : line;

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                streamResponse = JsonSerializer.Deserialize<OllamaStreamResponse>(json, options);
                return true;
            }
            catch (JsonException ex)
            {
                _logger.LogWarning($"解析Ollama响应失败: {line}, 错误: {ex.Message}");
                return false;
            }
        }

        private bool TryParseLMStudioStreamResponse(string line, out LMStudioStreamResponse streamResponse)
        {
            streamResponse = null;
            try
            {
                // 处理SSE格式的响应 (可能以"data:"开头)
                var json = line.StartsWith("data:") 
                    ? line.Substring(5).Trim()
                    : line;

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                streamResponse = JsonSerializer.Deserialize<LMStudioStreamResponse>(json, options);
                return true;
            }
            catch (JsonException ex)
            {
                _logger.LogWarning($"解析LM Studio响应失败: {line}, 错误: {ex.Message}");
                return false;
            }
        }

        private async IAsyncEnumerable<string> GetLMStudioStreamAsyncInternal(string prompt,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var request = new
            {
                model = _options.LMStudio.Model,
                messages = new[] { new { role = "user", content = prompt } },
                stream = true,
                temperature = 0.7,
                max_tokens = 1000
            };

            _logger.LogInformation($"发送LM Studio请求，提示词长度: {prompt.Length}");

            var streamResult = await CreateStreamConnectionAsync(request, cancellationToken);

            if (!streamResult.Success)
            {
                yield return $"处理失败: {streamResult.ErrorMessage}";
                yield break;
            }

            using var reader = streamResult.Reader;

            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (cancellationToken.IsCancellationRequested)
                    yield break;

                if (string.IsNullOrWhiteSpace(line) || line == "data: [DONE]")
                    continue;

                var parseSuccess = TryParseLMStudioStreamResponse(line, out var streamResponse);

                if (!parseSuccess)
                {
                    _logger.LogWarning($"解析LM Studio响应失败: {line}");
                    continue;
                }

                if (streamResponse?.Choices?.FirstOrDefault()?.Delta?.Content != null)
                {
                    yield return streamResponse.Choices[0].Delta.Content;
                }

                // 检查是否完成
                if (streamResponse?.Choices?.FirstOrDefault()?.FinishReason != null)
                {
                    _logger.LogInformation("LM Studio响应完成");
                    break;
                }
            }
        }

        public class OllamaStreamResponse
        {
            public string Response { get; set; } = "";
            public bool Done { get; set; }
            public string Model { get; set; } = "";
            public DateTime CreatedAt { get; set; }
        }

        public class LMStudioStreamResponse
        {
            public string Id { get; set; } = "";
            public string Object { get; set; } = "";
            public long Created { get; set; }
            public string Model { get; set; } = "";
            public List<LMStudioChoice> Choices { get; set; } = new();
        }

        public class LMStudioChoice
        {
            public int Index { get; set; }
            public LMStudioDelta Delta { get; set; } = new();
            public object? FinishReason { get; set; }
        }

        public class LMStudioDelta
        {
            public string Content { get; set; } = "";
        }

        public class ChatMemoryEntry
        {
            public string Question { get; set; }
            public string Answer { get; set; }
            public List<float> QuestionEmbedding { get; set; }
            public List<float> AnswerEmbedding { get; set; }
            public DateTime Timestamp { get; set; }
        }

        private class StreamConnectionResult
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; } = "";
            public StreamReader? Reader { get; set; }
            public HttpResponseMessage? Response { get; set; }
        }
    }

    public static class VectorOperations
    {
        public static async Task<List<float>> GetEmbeddingAsync(HttpClient httpClient, string text, string modelUrl, string embeddingModelName)
        {
            var request = new
            {
                model = embeddingModelName,
                prompt = text
            };
            var response = await httpClient.PostAsJsonAsync(modelUrl, request);
            response.EnsureSuccessStatusCode();

            var embeddingResponse = await response.Content.ReadFromJsonAsync<OllamaEmbeddingResponse>();
            return embeddingResponse?.Embedding ?? new List<float>();
        }

        public static double CosineSimilarity(List<float> vector1, List<float> vector2)
        {
            if (vector1 == null || vector2 == null || vector1.Count == 0 || vector1.Count != vector2.Count)
            {
                return 0.0;
            }

            double dotProduct = 0.0;
            double magnitude1 = 0.0;
            double magnitude2 = 0.0;

            for (int i = 0; i < vector1.Count; i++)
            {
                dotProduct += vector1[i] * vector2[i];
                magnitude1 += Math.Pow(vector1[i], 2);
                magnitude2 += Math.Pow(vector2[i], 2);
            }

            magnitude1 = Math.Sqrt(magnitude1);
            magnitude2 = Math.Sqrt(magnitude2);

            if (magnitude1 == 0 || magnitude2 == 0)
            {
                return 0.0;
            }

            return dotProduct / (magnitude1 * magnitude2);
        }
    }

    public class OllamaEmbeddingResponse
    {
        public List<float> Embedding { get; set; } = new List<float>();
    }
}
