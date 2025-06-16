using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;

namespace VSAIPluginNew.AI
{
    public class Anthropic_Agent_With_Prompt_Caching
    {
        private readonly HttpClient _httpClient;
        private string _modelName;
        private string _systemMessage;
        private string _apiKey;
        private const int MaxRetries = 3;
        private const int RetryDelayMs = 1000;
        
        // 提示缓存，用于存储已生成的回复
        private static readonly ConcurrentDictionary<string, string> _promptCache = new ConcurrentDictionary<string, string>();
        
        // 缓存控制参数
        private const int MaxCacheSize = 100; // 最大缓存条目数
        private const bool EnableCache = true; // 是否启用缓存

        public Anthropic_Agent_With_Prompt_Caching(string modelName, string systemMessage = "You are a helpful AI assistant", string apiKey = null)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.anthropic.com"),
                Timeout = TimeSpan.FromMinutes(5)
            };
            
            _modelName = modelName;
            _systemMessage = systemMessage;
            _apiKey = apiKey ?? Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY");
            
            // 设置默认请求头
            _httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
        }

        public void UpdateModel(string modelName)
        {
            if (!string.IsNullOrEmpty(modelName))
            {
                _modelName = modelName;
            }
        }
        
        public void UpdateSystemMessage(string systemMessage)
        {
            if (!string.IsNullOrEmpty(systemMessage))
            {
                _systemMessage = systemMessage;
            }
        }
        
        public void UpdateApiKey(string apiKey)
        {
            if (!string.IsNullOrEmpty(apiKey))
            {
                _apiKey = apiKey;
                _httpClient.DefaultRequestHeaders.Remove("x-api-key");
                _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
            }
        }

        private string GenerateCacheKey(string prompt, string model, string systemMessage)
        {
            // 使用提示内容、模型名称和系统消息的组合作为缓存键
            return $"{prompt}_{model}_{systemMessage.GetHashCode()}";
        }

        private bool TryGetFromCache(string prompt, out string cachedResponse)
        {
            if (!EnableCache)
            {
                cachedResponse = null;
                return false;
            }

            string cacheKey = GenerateCacheKey(prompt, _modelName, _systemMessage);
            return _promptCache.TryGetValue(cacheKey, out cachedResponse);
        }

        private void AddToCache(string prompt, string response)
        {
            if (!EnableCache)
            {
                return;
            }

            string cacheKey = GenerateCacheKey(prompt, _modelName, _systemMessage);
            
            // 如果缓存已满，删除一个随机条目
            if (_promptCache.Count >= MaxCacheSize)
            {
                var randomKey = _promptCache.Keys.ElementAt(new Random().Next(0, _promptCache.Count));
                _promptCache.TryRemove(randomKey, out _);
            }
            
            _promptCache[cacheKey] = response;
        }

        public async Task<string> GenerateReplyAsync(string promptText)
        {
            // 先查询缓存
            if (TryGetFromCache(promptText, out string cachedResponse))
            {
                return cachedResponse;
            }

            int retryCount = 0;
            while (true)
            {
                try
                {
                    var requestObject = new
                    {
                        model = _modelName,
                        messages = new[]
                        {
                            new { role = "system", content = _systemMessage },
                            new { role = "user", content = promptText }
                        },
                        max_tokens = 4000
                    };

                    var content = new StringContent(
                        System.Text.Json.JsonSerializer.Serialize(requestObject),
                        Encoding.UTF8,
                        "application/json");

                    var response = await _httpClient.PostAsync("/v1/messages", content);
                    
                    // 检查HTTP响应状态
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        throw new HttpRequestException($"HTTP错误: {response.StatusCode}, 内容: {errorContent}");
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseObject = System.Text.Json.JsonSerializer.Deserialize<AnthropicResponse>(responseContent);

                    if (responseObject?.Content == null || responseObject.Content.Length == 0)
                    {
                        throw new InvalidOperationException("模型响应格式无效");
                    }

                    string result = responseObject.Content[0].Text;
                    
                    // 将结果添加到缓存
                    AddToCache(promptText, result);
                    
                    return result;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    
                    // 如果已达到最大重试次数，抛出异常
                    if (retryCount >= MaxRetries)
                    {
                        return $"生成回复时出错: {ex.Message}. 已重试{MaxRetries}次.";
                    }
                    
                    // 指数退避重试
                    await Task.Delay(RetryDelayMs * retryCount);
                }
            }
        }

        public async Task<string> ProcessFilesAndGenerateReplyAsync(string promptText, Dictionary<string, string> filesContent)
        {
            try
            {
                var contextBuilder = new StringBuilder();
                contextBuilder.AppendLine("我有权访问您Visual Studio解决方案中的以下文件:");
                
                int fileCount = 0;
                int totalChars = 0;
                const int MaxContextSize = 100000; // 防止上下文过大
                
                foreach (var file in filesContent)
                {
                    // 计算添加此文件后的大小
                    int fileSize = file.Key.Length + file.Value.Length + 20; // 20是文件标记的额外字符
                    
                    // 如果添加此文件会使上下文过大，则停止添加
                    if (totalChars + fileSize > MaxContextSize)
                    {
                        contextBuilder.AppendLine($"\n注意: 由于上下文大小限制，跳过了其余{filesContent.Count - fileCount}个文件");
                        break;
                    }
                    
                    contextBuilder.AppendLine($"文件: {file.Key}");
                    contextBuilder.AppendLine("```");
                    contextBuilder.AppendLine(file.Value);
                    contextBuilder.AppendLine("```");
                    
                    fileCount++;
                    totalChars += fileSize;
                }

                contextBuilder.AppendLine("\n");
                contextBuilder.AppendLine(promptText);

                return await GenerateReplyAsync(contextBuilder.ToString());
            }
            catch (Exception ex)
            {
                return $"处理文件时出错: {ex.Message}";
            }
        }
        
        public async Task<bool> IsModelAvailableAsync(string modelName)
        {
            try
            {
                // Anthropic API不提供像Ollama那样的直接模型检查，但可以通过检查环境变量和简单请求来验证
                if (string.IsNullOrEmpty(_apiKey))
                {
                    return false;
                }
                
                // 进行一个小型验证请求
                var requestObject = new
                {
                    model = modelName,
                    messages = new[]
                    {
                        new { role = "user", content = "Hello" }
                    },
                    max_tokens = 1
                };

                var content = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(requestObject),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync("/v1/messages", content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
        
        // 清除缓存的方法
        public static void ClearCache()
        {
            _promptCache.Clear();
        }
    }

    public class AnthropicResponseContent
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }
    }

    public class AnthropicResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("content")]
        public AnthropicResponseContent[] Content { get; set; }
    }
} 