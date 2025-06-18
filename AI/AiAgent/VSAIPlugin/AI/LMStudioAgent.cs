using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VSAIPluginNew.AI
{
    public class LMStudioAgent
    {
        private readonly HttpClient _httpClient;
        private string _modelName;
        private string _systemMessage;
        private const int MaxRetries = 3;
        private const int RetryDelayMs = 1000;
        private const int DefaultPort = 1234;

        public LMStudioAgent(string modelName, string systemMessage = "You are a helpful AI assistant", int? port = null)
        {
            int basePort = port ?? DefaultPort;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri($"http://localhost:{basePort}"),
                Timeout = TimeSpan.FromMinutes(5)
            };
            _modelName = modelName;
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
                    var messages = new List<object>
                    {
                        new { role = "system", content = _systemMessage },
                        new { role = "user", content = promptText }
                    };
                    var requestObject = new
                    {
                        model = _modelName,
                        messages = messages
                    };
                    var content = new StringContent(
                        JsonSerializer.Serialize(requestObject),
                        Encoding.UTF8,
                        "application/json");
                    var response = await _httpClient.PostAsync("/v1/chat/completions", content);
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        throw new HttpRequestException($"HTTP错误: {response.StatusCode}, 内容: {errorContent}");
                    }
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonSerializer.Deserialize<LMStudioResponse>(responseContent);
                    if (responseObject?.Choices == null || responseObject.Choices.Count == 0)
                    {
                        throw new InvalidOperationException("模型响应格式无效");
                    }
                    return responseObject.Choices[0].Message.Content;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount >= MaxRetries)
                    {
                        return $"生成回复时出错: {ex.Message}. 已重试{MaxRetries}次.";
                    }
                    await Task.Delay(RetryDelayMs * retryCount);
                }
            }
        }

        public class LMStudioResponse
        {
            [JsonPropertyName("choices")]
            public List<LMStudioChoice> Choices { get; set; }
        }
        public class LMStudioChoice
        {
            [JsonPropertyName("message")]
            public LMStudioMessage Message { get; set; }
        }
        public class LMStudioMessage
        {
            [JsonPropertyName("role")]
            public string Role { get; set; }
            [JsonPropertyName("content")]
            public string Content { get; set; }
        }
    }
} 