using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;

namespace OllamaContext7Api.Services
{
    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AIService> _logger;
        private readonly AIServiceOptions _options;

        public AIService(
            HttpClient httpClient, 
            ILogger<AIService> logger,
            IOptions<AIServiceOptions> options)
        {
            _httpClient = httpClient;
            _logger = logger;
            _options = options.Value;

            // 设置HTTP客户端超时（流式响应需要更长时间）
            _httpClient.Timeout = TimeSpan.FromMinutes(10);
        }

        public async Task<string> GetAnswerAsync(string question)
        {
            try
            {
                _logger.LogInformation($"处理问题: {question}");

                var prompt = BuildPrompt(question);
                var response = await GetOllamaResponseAsync(prompt, stream: false);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理问题时出错");
                return $"处理问题时出错: {ex.Message}";
            }
        }

        public async IAsyncEnumerable<string> GetAnswerStreamAsync(string question,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"开始流式处理问题: {question}");

            var prompt = BuildPrompt(question);

            if (_options.Provider == "Ollama")
            {
                await foreach (var chunk in GetOllamaStreamAsyncInternal(prompt, cancellationToken))
                {
                    yield return chunk;
                }
            }
            else
            {
                await foreach (var chunk in GetLMStudioStreamAsyncInternal(prompt, cancellationToken))
                {
                    yield return chunk;
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

        private string BuildPrompt(string question)
        {
            var sb = new StringBuilder();
            sb.AppendLine("你是一个专业的编程助手，请回答以下问题：");
            sb.AppendLine();
            sb.AppendLine($"问题：{question}");
            sb.AppendLine();
            sb.AppendLine("请提供准确、简洁的答案，如果涉及代码请提供示例。");

            return sb.ToString();
        }

        private async Task<string> GetOllamaResponseAsync(string prompt, bool stream = false)
        {
            var request = new
            {
                model = _options.Ollama.Model,
                prompt = prompt,
                stream = stream,
                options = new
                {
                    temperature = 0.3,
                    top_p = 0.9,
                    num_ctx = 4096,
                    num_predict = 1000
                }
            };

            _logger.LogInformation($"发送Ollama请求，提示词长度: {prompt.Length}");

            var response = await _httpClient.PostAsJsonAsync(_options.Ollama.Url, request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OllamaResponse>();
            var answer = result?.Response ?? "未能获取答案";

            _logger.LogInformation($"收到Ollama响应，长度: {answer.Length}");

            return answer;
        }

        private async IAsyncEnumerable<string> GetOllamaStreamAsyncInternal(string prompt,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var request = new
            {
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

            _logger.LogInformation($"发送Ollama流式请求，提示词长度: {prompt.Length}");

            // 使用单独的方法来处理可能抛出异常的操作
            var streamResult = await CreateStreamConnectionAsync(request, cancellationToken);

            if (!streamResult.Success)
            {
                yield return $"流式处理失败: {streamResult.ErrorMessage}";
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
                    _logger.LogWarning($"解析流响应失败: {line}");
                    continue;
                }

                if (streamResponse?.Response != null)
                {
                    yield return streamResponse.Response;
                }

                // 检查是否完成
                if (streamResponse?.Done == true)
                {
                    _logger.LogInformation("流式响应完成");
                    break;
                }
            }
        }

        private async Task<StreamConnectionResult> CreateStreamConnectionAsync(object request, CancellationToken cancellationToken)
        {
            try
            {
                var url = _options.Provider == "Ollama" 
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
                _logger.LogError(ex, "创建流式连接失败");
                return new StreamConnectionResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        private bool TryParseStreamResponse(string json, out OllamaStreamResponse streamResponse)
        {
            streamResponse = null;
            try
            {
                streamResponse = JsonSerializer.Deserialize<OllamaStreamResponse>(json);
                return true;
            }
            catch (JsonException)
            {
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

            _logger.LogInformation($"发送LM Studio流式请求，提示词长度: {prompt.Length}");

            var streamResult = await CreateStreamConnectionAsync(request, cancellationToken);

            if (!streamResult.Success)
            {
                yield return $"流式处理失败: {streamResult.ErrorMessage}";
                yield break;
            }

            using var reader = streamResult.Reader;

            string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (cancellationToken.IsCancellationRequested)
                        yield break;

                    _logger.LogInformation($"接收到的行: {line}");

                    if (string.IsNullOrWhiteSpace(line) || line == "data: [DONE]")
                        continue;

                    var parseSuccess = TryParseLMStudioStreamResponse(line, out var streamResponse);

                    if (!parseSuccess)
                    {
                        _logger.LogWarning($"解析LM Studio流响应失败: {line}");
                        continue;
                    }

                    if (streamResponse?.Choices?.FirstOrDefault()?.Delta?.Content != null)
                    {
                        yield return streamResponse.Choices[0].Delta.Content;
                    }

                    // 检查是否完成
                    if (streamResponse?.Choices?.FirstOrDefault()?.FinishReason != null)
                    {
                        _logger.LogInformation("LM Studio流式响应完成");
                        break;
                    }
                }
        }

        private async IAsyncEnumerable<string> GetErrorStreamAsync(string errorMessage)
        {
            yield return errorMessage;
            await Task.CompletedTask; // 避免编译器警告
        }

        private class StreamConnectionResult
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; } = "";
            public StreamReader? Reader { get; set; }
            public HttpResponseMessage? Response { get; set; }
        }
    }

    public class OllamaResponse
    {
        public string Response { get; set; } = "";
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
}
