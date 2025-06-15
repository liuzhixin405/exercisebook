using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OllamaContext7Api.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OllamaContext7Api.Models;

namespace OllamaContext7Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionController : ControllerBase
    {
        private readonly IAIService _aiService;
        private readonly ILogger<QuestionController> _logger;
        private static CancellationTokenSource _cts = new CancellationTokenSource();

        public QuestionController(
            IAIService aiService,
            ILogger<QuestionController> logger)
        {
            _aiService = aiService;
            _logger = logger;
        }

        [HttpPost("ask-stream")]
        public async Task AskQuestionStream([FromBody] QuestionRequest request)
        {
            CancellationToken ct = default;
            try
            {
                if (string.IsNullOrWhiteSpace(request?.Question))
                {
                    Response.StatusCode = 400;
                    await Response.WriteAsync("问题不能为空");
                    return;
                }

                _logger.LogInformation($"收到问题: {request.Question}");

                // Reset the CancellationTokenSource

                ct = _cts.Token;

                // 设置SSE响应头
                Response.ContentType = "text/event-stream";
                Response.Headers.Add("Cache-Control", "no-cache");
                Response.Headers.Add("Connection", "keep-alive");
                Response.Headers.Add("Access-Control-Allow-Origin", "*");

                // 发送开始事件
                await SendSseEvent("start", new { question = request.Question });

                // 使用流式AI服务处理问题
                await foreach (var chunk in _aiService.GetAnswerStreamAsync(request.Question, ct, request.RelatedFiles, request.IsDeepMode))
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
                _logger.LogError(ex, $"处理问题时出错: {request?.Question}");
                if (!ct.IsCancellationRequested)
                {
                    await SendSseEvent("error", new
                    {
                        error = "处理问题时出错",
                        message = ex.Message,
                        timestamp = DateTime.UtcNow
                    });
                }
                else
                {
                    _logger.LogInformation($"处理已取消: {request?.Question}");
                }
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

        [HttpPost("stop-stream")]
        public IActionResult StopStream()
        {
            _cts.Cancel();
            _logger.LogInformation("处理已停止");
            return Ok(new
            {
                status = "stopped",
                timestamp = DateTime.UtcNow
            });
        }

        private async Task SendSseEvent(string eventType, object data)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(data);
            var sseData = $"event: {eventType}\ndata: {json}\n\n";
            var bytes = Encoding.UTF8.GetBytes(sseData);
            await Response.Body.WriteAsync(bytes, 0, bytes.Length);
        }
    }
}
