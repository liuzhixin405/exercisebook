using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Runtime.CompilerServices;

namespace OllamaContext7Api.Services
{
    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AIService> _logger;

        private const string OllamaUrl = "http://localhost:11434/api/generate";
        private const string ModelName = "codellama:7b-code";

        public AIService(HttpClient httpClient, ILogger<AIService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

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

            await foreach (var chunk in GetOllamaStreamAsync(prompt, cancellationToken))
            {
                if (cancellationToken.IsCancellationRequested)
                    yield break;

                yield return chunk;
            }
        }

        public async Task<bool> CheckHealthAsync()
        {
            try
            {
                var request = new
                {
                    model = ModelName,
                    prompt = "Hello",
                    stream = false,
                    options = new { num_predict = 5 }
                };

                var response = await _httpClient.PostAsJsonAsync(OllamaUrl, request);
                return response.IsSuccessStatusCode;
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
                model = ModelName,
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

            var response = await _httpClient.PostAsJsonAsync(OllamaUrl, request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OllamaResponse>();
            var answer = result?.Response ?? "未能获取答案";

            _logger.LogInformation($"收到Ollama响应，长度: {answer.Length}");

            return answer;
        }

        private async IAsyncEnumerable<string> GetOllamaStreamAsync(string prompt,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var request = new
            {
                model = ModelName,
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

            try
            {
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, OllamaUrl)
                {
                    Content = JsonContent.Create(request)
                };

                using var response = await _httpClient.SendAsync(httpRequest,
                    HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                response.EnsureSuccessStatusCode();

                using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                using var reader = new StreamReader(stream);

                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (cancellationToken.IsCancellationRequested)
                        yield break;

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    OllamaStreamResponse streamResponse;
                    try
                    {
                        streamResponse = JsonSerializer.Deserialize<OllamaStreamResponse>(line);
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogWarning(ex, $"解析流响应失败: {line}");
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "流式请求处理失败");
                yield return $"流式处理失败: {ex.Message}";
            }
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
}