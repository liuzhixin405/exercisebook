using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VSAIPluginNew.AI
{
    public class OllamaAgent
    {
        private readonly HttpClient _httpClient;
        private string _modelName;
        private string _systemMessage;
        private const int MaxRetries = 3;
        private const int RetryDelayMs = 1000;
        private const int DefaultPort = 11434;

        public OllamaAgent(string modelName, string systemMessage = "You are a helpful AI assistant", int? port = null)
        {
            int basePort = port ?? (AgentFactory.ModelPorts.TryGetValue(modelName, out int modelPort) ? modelPort : DefaultPort);
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri($"http://localhost:{basePort}"),
                Timeout = TimeSpan.FromMinutes(5) // 增加超时时间，处理大型代码库
            };

            // 使用实际的Ollama模型名称
            _modelName = AgentFactory.ModelNames.TryGetValue(modelName, out string? actualName) ? actualName : modelName;
            _systemMessage = systemMessage;
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

        public async Task<string> GenerateReplyAsync(string promptText)
        {
            int retryCount = 0;
            while (true)
            {
                try
                {
                    var messages = new List<Message>
                    {
                        new Message { Role = "system", Content = _systemMessage },
                        new Message { Role = "user", Content = promptText }
                    };

                    var requestObject = new
                    {
                        model = _modelName,
                        messages,
                        stream = false
                    };

                    var content = new StringContent(
                        System.Text.Json.JsonSerializer.Serialize(requestObject),
                        Encoding.UTF8,
                        "application/json");

                    var response = await _httpClient.PostAsync("/api/chat", content);
                    
                    // 检查HTTP响应状态
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        throw new HttpRequestException($"HTTP错误: {response.StatusCode}, 内容: {errorContent}");
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();                    var responseObject = System.Text.Json.JsonSerializer.Deserialize<OllamaResponse>(responseContent);

                    if (responseObject?.Message?.Content == null)
                    {
                        throw new InvalidOperationException("模型响应格式无效");
                    }

                    return responseObject.Message.Content;
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
                var requestObject = new { name = modelName };
                
                var content = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(requestObject),
                    Encoding.UTF8,
                    "application/json");
                
                var response = await _httpClient.PostAsync("/api/show", content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }

    public class OllamaResponse
    {
        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("created_at")]
        public string? CreatedAt { get; set; }

        [JsonPropertyName("message")]
        public Message? Message { get; set; }

        [JsonPropertyName("done")]
        public bool Done { get; set; }

        [JsonPropertyName("total_duration")]
        public long TotalDuration { get; set; }

        [JsonPropertyName("load_duration")]
        public long LoadDuration { get; set; }

        [JsonPropertyName("prompt_eval_duration")]
        public long PromptEvalDuration { get; set; }

        [JsonPropertyName("eval_count")]
        public int EvalCount { get; set; }

        [JsonPropertyName("eval_duration")]
        public long EvalDuration { get; set; }
    }

    public class Message
    {
        [JsonPropertyName("role")]
        public string? Role { get; set; }

        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }
}