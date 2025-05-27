using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Threading.Tasks;
using OllamaContext7Api.Services;

namespace OllamaContext7Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SseController : ControllerBase
    {
        private readonly IAIService _aiService;
        private readonly ILogger<SseController> _logger;

        public SseController(IAIService aiService, ILogger<SseController> logger)
        {
            _aiService = aiService;
            _logger = logger;
        }

        [HttpGet]
        public async Task Get(string question)
        {
            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");

            await foreach (var chunk in _aiService.GetAnswerStreamAsync(question))
            {
                await WriteSseEventAsync(Response.BodyWriter, "message", chunk);
            }
        }

        private async Task WriteSseEventAsync(System.IO.Pipelines.PipeWriter writer, string eventType, string data)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"event: {eventType}");
            stringBuilder.AppendLine($"data: {data}");
            stringBuilder.AppendLine(); // Important: two newlines to terminate the event

            var bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
            await writer.WriteAsync(bytes);
            await writer.FlushAsync();
        }
    }
}
