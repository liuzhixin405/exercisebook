using Microsoft.AspNetCore.Mvc;
using OllamaContext7Api.Services;
using System.Text;

namespace OllamaContext7Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionController : ControllerBase
    {
        private readonly IAIService _aiService;
        private readonly ILogger<QuestionController> _logger;

        public QuestionController(
            IAIService aiService,
            ILogger<QuestionController> logger)
        {
            _aiService = aiService;
            _logger = logger;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> AskQuestion([FromBody] QuestionRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.Question))
                {
                    return BadRequest("问题不能为空");
                }

                _logger.LogInformation($"收到问题: {request.Question}");

                // 直接使用AI服务处理问题，不使用Context7
                var answer = await _aiService.GetAnswerAsync(request.Question);

                return Ok(new QuestionResponse
                {
                    Question = request.Question,
                    Answer = answer,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"处理问题时出错: {request?.Question}");
                return StatusCode(500, new
                {
                    error = "处理问题时出错",
                    message = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        [HttpPost("ask-stream")]
        public async Task AskQuestionStream([FromBody] QuestionRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.Question))
                {
                    Response.StatusCode = 400;
                    await Response.WriteAsync("问题不能为空");
                    return;
                }

                _logger.LogInformation($"收到流式问题: {request.Question}");

                // 设置SSE响应头
                Response.ContentType = "text/event-stream";
                Response.Headers.Add("Cache-Control", "no-cache");
                Response.Headers.Add("Connection", "keep-alive");
                Response.Headers.Add("Access-Control-Allow-Origin", "*");

                // 发送开始事件
                await SendSseEvent("start", new { question = request.Question });

                // 使用流式AI服务处理问题
                await foreach (var chunk in _aiService.GetAnswerStreamAsync(request.Question,CancellationToken.None))
                {
                    if (!string.IsNullOrEmpty(chunk))
                    {
                        await SendSseEvent("data", new { content = chunk });
                        await Response.Body.FlushAsync();
                    }
                }

                // 发送结束事件
                await SendSseEvent("end", new { timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"流式处理问题时出错: {request?.Question}");
                await SendSseEvent("error", new
                {
                    error = "处理问题时出错",
                    message = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        [HttpGet("health")]
        public async Task<IActionResult> HealthCheck()
        {
            try
            {
                var isHealthy = await _aiService.CheckHealthAsync();
                return Ok(new
                {
                    status = isHealthy ? "healthy" : "unhealthy",
                    timestamp = DateTime.UtcNow,
                    service = "OllamaContext7Api"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "健康检查失败");
                return StatusCode(500, new
                {
                    status = "unhealthy",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        private async Task SendSseEvent(string eventType, object data)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(data);
            var sseData = $"event: {eventType}\ndata: {json}\n\n";
            var bytes = Encoding.UTF8.GetBytes(sseData);
            await Response.Body.WriteAsync(bytes, 0, bytes.Length);
        }
    }

    public class QuestionRequest
    {
        public string Question { get; set; } = "";
    }

    public class QuestionResponse
    {
        public string Question { get; set; } = "";
        public string Answer { get; set; } = "";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}